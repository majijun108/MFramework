using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace Lockstep.NetWork
{

    public class TCPChannel : AChannel
    {
        private readonly TcpClient _tcpClient;

        private readonly CircleBuffer recvBuffer = new CircleBuffer();
        private readonly CircleBuffer sendBuffer = new CircleBuffer();
        private PackageParser packageParser;
        private bool isSending = false;
        private TaskCompletionSource<Packet> recvTask;//接受任务
        private readonly Queue<MessageInfo> m_SendQueue = new Queue<MessageInfo>();

        public TCPChannel(NetWorkProxy service,TcpClient client):base(service,ChannelType.Accept)
        {
            this._tcpClient = client;
            packageParser = new PackageParser(this.recvBuffer);

            IPEndPoint ip = (IPEndPoint)this._tcpClient.Client.RemoteEndPoint;
            this.RemoteAddress = ip;

            this.StartRecv();
        }

        public override void Send(byte opcode, object msg,IPEndPoint remote)
        {
            lock (m_SendQueue) 
            {
                m_SendQueue.Enqueue(new MessageInfo() 
                {
                    OpCode = opcode,
                    Msg = msg,
                });

                if (isSending)
                    return;
                this.StartSend();
            }
        }

        public async void StartRecv() 
        {
            while (true)
            {
                if (IsDisposed)
                    return;
                int n = await this.recvBuffer.WriteFromStreamAsync(_tcpClient.GetStream());//从流中写入内存
                if (n == 0) 
                {
                    continue;
                }
                if (recvTask == null) //没用解包任务 不需要解包 继续接收
                    continue;
                bool isOK = this.packageParser.Parse();
                if (isOK) 
                {
                    var task = recvTask;
                    recvTask = null;
                    task.SetResult(this.packageParser.GetPacket());//将结果丢入任务中
                }
            }
        }

        private void PushMsgToBuffer() 
        {
            if (m_SendQueue.Count == 0)
                return;
            lock (m_SendQueue)
            {
                while (m_SendQueue.Count > 0)
                {
                    var sendInfo = m_SendQueue.Dequeue();
                    byte[] msgByte = m_NetProxy.MessagePacker.SerializeToByteArray(sendInfo.OpCode,sendInfo.Msg);//sendInfo.Msg.ToByteArray();
                    if (msgByte.Length + 3 > ushort.MaxValue) //不要超过消息最大长度
                    {
                        continue;
                    }
                    ushort dataLength = (ushort)(3 + msgByte.Length);
                    sendBuffer.Write(BitConverter.GetBytes(dataLength), 0, 2);//写入长度
                    sendBuffer.Write(BitConverter.GetBytes(sendInfo.OpCode), 0, 1);//写入操作
                    sendBuffer.Write(msgByte, 0, msgByte.Length);
                    break;//每次循环写一个
                }
            }
        }

        public async void StartSend() 
        {
            while (true)
            {
                if (this.IsDisposed)
                    return;

                PushMsgToBuffer();

                long length = this.sendBuffer.Length;
                if (length == 0)
                {
                    this.isSending = false;
                    return;
                }

                this.isSending = true;
                await this.sendBuffer.ReadToStreamAsync(_tcpClient.GetStream());
            }
        }

        public override Task<Packet> RecvAsync() 
        {
            if (this.IsDisposed)
                throw new Exception("channel is disposed");
            bool isOK = this.packageParser.Parse();
            if (isOK) 
            {
                var packet = this.packageParser.GetPacket();
                return Task.FromResult(packet);
            }

            recvTask = new TaskCompletionSource<Packet>();
            return recvTask.Task;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
            }
        }
    }
}

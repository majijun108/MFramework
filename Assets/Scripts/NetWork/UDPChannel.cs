using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{

    public class UDPChannel : AChannel
    {
        private readonly UdpClient _udpClient;

        private readonly CircleBuffer recvBuffer = new CircleBuffer();
        private readonly CircleBuffer sendBuffer = new CircleBuffer();
        private PackageParser packageParser;
        private bool isSending = false;
        private TaskCompletionSource<Packet> recvTask;//接受任务
        private readonly Queue<MessageInfo> m_SendQueue = new Queue<MessageInfo>();
        private IPEndPoint m_localIP;

        public UDPChannel(NetWorkProxy service,IPEndPoint localIP):base(service,ChannelType.Accept)
        {
            this._udpClient = new UdpClient(localIP);
            m_localIP = localIP;
            packageParser = new PackageParser(this.recvBuffer);

            this.StartRecvAsync();
        }

        public override void Send(byte opcode, object msg,IPEndPoint remote)
        {
            lock (m_SendQueue)
            {
                m_SendQueue.Enqueue(new MessageInfo()
                {
                    OpCode = opcode,
                    Msg = msg,
                    Remote = remote
                });

                if (isSending)
                    return;
                this.StartSendAsync();
            }
        }

        private IPEndPoint PushMsgToBuffer()
        {
            if (m_SendQueue.Count == 0)
                return null;
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
                    return sendInfo.Remote;//每次循环写一个
                }
                return null;
            }
        }

        private byte[] curSendBuffer = new byte[ushort.MaxValue];
        private Task<int> m_sendTask;
        public async void StartSendAsync()
        {
            try
            {
                while (true)
                {
                    if (this.IsDisposed)
                        return;
                    IPEndPoint remote = PushMsgToBuffer();

                    long length = this.sendBuffer.Length;
                    if (length == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    this.isSending = true;

                    int n = sendBuffer.ReadMsg(curSendBuffer);
                    m_sendTask = _udpClient.SendAsync(curSendBuffer, n, remote);
                    await m_sendTask;
                }
            }
            catch (Exception ex) 
            {
                DebugService.Instance.LogError(ex.ToString());
            }
        }

        private Task<UdpReceiveResult> m_ReceiveTask;
        public async void StartRecvAsync() 
        {
            try
            {
                while (true)
                {
                    if (IsDisposed)
                        return;
                    m_ReceiveTask = _udpClient.ReceiveAsync();
                    var result = await m_ReceiveTask;
                    if (IsDisposed)
                    {
                        _udpClient.Close();
                        _udpClient.Dispose();
                        return;
                    }
                    if (result.Buffer.Length == 0)
                        continue;
                    this.recvBuffer.Write(result.Buffer, 0, result.Buffer.Length);
                    if (recvTask == null)
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
            catch (Exception ex) 
            {
                DebugService.Instance.LogError(ex.ToString());
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
            this.Enable = false;
            //强制发送一条消息 让task关闭
            if (_udpClient != null) 
            {
                m_localIP.Address = IPAddress.Parse(NetHelper.GetLocalIP());
                byte[] data = new byte[1];
                this._udpClient.Send(data, 1, m_localIP);
            }
        }
    }
}

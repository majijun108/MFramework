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
        private NetworkStream _networkStream;

        private readonly CircleBuffer recvBuffer = new CircleBuffer();
        private readonly CircleBuffer sendBuffer = new CircleBuffer();
        private PackageParser packageParser;
        private bool isSending = false;
        private TaskCompletionSource<Packet> recvTask;//接受任务

        public TCPChannel(TCPService service,TcpClient client):base(service,ChannelType.Accept)
        {
            this._tcpClient = client;
            packageParser = new PackageParser(this.recvBuffer);

            IPEndPoint ip = (IPEndPoint)this._tcpClient.Client.RemoteEndPoint;
            this.RemoteAddress = ip;
            _networkStream = client.GetStream();

            this.StartRecv();
        }
        public override void Send(byte opcode,byte[] buffer, int index, int length)
        {
            if (3 + length > ushort.MaxValue)
            {
                throw new Exception("packet to send is to large,size=" + (3 + buffer.Length));
            }
            ushort dataLength = (ushort)(3 + length);
            sendBuffer.Write(BitConverter.GetBytes(dataLength), 0, 2);//写入长度
            sendBuffer.Write(BitConverter.GetBytes(opcode), 0, 1);//写入操作
            sendBuffer.Write(buffer, index, length);
            if (isSending)
                return;
            this.StartSend();
        }

        public override void Send(byte opcode,byte[] buffer)
        {
            this.Send(opcode, buffer, 0, buffer.Length);
        }

        public async void StartRecv() 
        {
            if (IsDisposed)
                return;
            while (true) 
            {
                int n = await this.recvBuffer.WriteFromStreamAsync(_networkStream);//从流中写入内存
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

        public async void StartSend() 
        {
            while (true)
            {
                if (this.IsDisposed)
                    return;
                long length = this.sendBuffer.Length;
                if (length == 0)
                {
                    this.isSending = false;
                    return;
                }

                this.isSending = true;
                await this.sendBuffer.ReadToStreamAsync(_networkStream);
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
    }
}

using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{
    public static class AsyncExtensions
    {
        public static async Task<T> WithCancellation<T>( this Task<T> task, CancellationToken cancellationToken )
        {
            var tcs = new TaskCompletionSource<bool>();
            using( cancellationToken.Register( s => ( (TaskCompletionSource<bool>)s ).TrySetResult( true ), tcs ) )
            {
                if( task != await Task.WhenAny( task, tcs.Task ) )
                {
                    throw new OperationCanceledException( cancellationToken );
                }
            }

            return task.Result;
        }
    }
    public class UDPChannel : AChannel
    {
        private readonly UdpClient _udpClient;

        private readonly CircleBuffer recvBuffer = new CircleBuffer();
        private readonly CircleBuffer sendBuffer = new CircleBuffer();
        private PackageParser packageParser;
        private bool isSending = false;
        private IPEndPoint m_localIP;
        private CancellationTokenSource m_cancel;

        private readonly Queue<MessageInfo> m_SendQueue = new Queue<MessageInfo>();
        private readonly List<MessageInfo> m_ReceiveQueue = new List<MessageInfo>();
        private IMessagePacker m_messagePacker;

        public UDPChannel(NetWorkProxy service,IPEndPoint localIP):base(service,ChannelType.Accept)
        {
            this._udpClient = new UdpClient(localIP);
            m_localIP = localIP;
            packageParser = new PackageParser(this.recvBuffer);
            m_cancel = new CancellationTokenSource();
        }

        public void Init(IMessagePacker msgPacker) 
        {
            m_messagePacker = msgPacker;
        }

        bool isRunning = false;
        public void Start() 
        {
            if (isRunning)
                return;
            isRunning = true;
            Task.Run(StartRecvAsync);
        }

        private async void StartRecvAsync() 
        {
            try
            {
                while (true)
                {
                    var result = await _udpClient.ReceiveAsync().WithCancellation(m_cancel.Token);
                    if (result.Buffer.Length == 0)
                        continue;
                    this.recvBuffer.Write(result.Buffer, 0, result.Buffer.Length);
                    bool isOK = this.packageParser.Parse();
                    if (!isOK)
                        continue;
                    var packet = this.packageParser.GetPacket();
                    var obj = m_messagePacker.DeserializeFrom(packet.OpCode, packet.Bytes, Packet.DataIndex, packet.Size);
                    lock (m_ReceiveQueue)
                    {
                        m_ReceiveQueue.Add(new MessageInfo()
                        {
                            OpCode = packet.OpCode,
                            Msg = obj,
                            Remote = result.RemoteEndPoint
                        });
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugService.Instance.LogWarning(ex.ToString());
            }
        }

        List<MessageInfo> messageInfos = new List<MessageInfo>();
        public void OnUpdate(Action<MessageInfo> cb) 
        {
            if (m_ReceiveQueue.Count <= 0)
                return;
            lock (m_ReceiveQueue) 
            {
                messageInfos.Clear();
                messageInfos.AddRange(m_ReceiveQueue);
                m_ReceiveQueue.Clear();
            }

            foreach (var item in messageInfos)
            {
                cb?.Invoke(item);
            }
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
            }

            if (isSending)
                return;
            isSending = true;
            Task.Run(StartSendAsync);
        }

        private void PushMsgToBuffer(ref MessageInfo sendInfo)
        {
            byte[] msgByte = m_NetProxy.MessagePacker.SerializeToByteArray(sendInfo.OpCode, sendInfo.Msg);//sendInfo.Msg.ToByteArray();
            if (msgByte.Length + 3 > ushort.MaxValue) //不要超过消息最大长度
            {
                return;
            }
            ushort dataLength = (ushort)(3 + msgByte.Length);
            sendBuffer.Write(BitConverter.GetBytes(dataLength), 0, 2);//写入长度
            sendBuffer.Write(BitConverter.GetBytes(sendInfo.OpCode), 0, 1);//写入操作
            sendBuffer.Write(msgByte, 0, msgByte.Length);
        }

        private byte[] curSendBuffer = new byte[ushort.MaxValue];
        public async void StartSendAsync()
        {
            try
            {
                while (true)
                {
                    if (m_SendQueue.Count == 0) 
                    {
                        isSending = false;
                        return;
                    }
                    MessageInfo sendInfo;
                    lock (m_SendQueue) 
                    {
                        sendInfo = m_SendQueue.Dequeue();
                    }
                    PushMsgToBuffer(ref sendInfo);
                    long length = this.sendBuffer.Length;
                    if (length == 0)
                        continue;
                    int n = sendBuffer.ReadMsg(curSendBuffer);
                    await _udpClient.SendAsync(curSendBuffer, n, sendInfo.Remote).WithCancellation(m_cancel.Token);
                }
            }
            catch (Exception ex) 
            {
                DebugService.Instance.LogWarning(ex.ToString());
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Enable = false;
            m_cancel?.Cancel();
            _udpClient?.Close();
            _udpClient?.Dispose();
        }

        public override Task<Packet> RecvAsync()
        {
            throw new NotImplementedException();
        }
    }
}

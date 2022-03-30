using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessageProto = Google.Protobuf.IMessage;
using Google.Protobuf;

namespace Lockstep.NetWork
{
    //服务器和客户端一样
    public class UDPNetProxy : NetWorkProxy
    {
        private UDPChannel m_UdpChannel;
        private readonly Queue<MessageInfo> m_receiveMsgs = new Queue<MessageInfo>();

        public UDPNetProxy(IPEndPoint localIP) 
        {
            m_UdpChannel = new UDPChannel(this,localIP);
            StartReceive();
        }

        public async void StartReceive()
        {
            while (true)
            {
                if (IsDisposed)
                    return;
                Packet packet = await m_UdpChannel.RecvAsync();
                if (IsDisposed)
                    return;
                OnRecv(packet);
            }
        }

        private void OnRecv(Packet packet)
        {
            if (IsDisposed)
                return;
            if (this.MessagePacker == null)
                return;

            var msg = this.MessagePacker.DeserializeFrom(packet.Bytes, Packet.DataIndex, packet.Size);
            lock (m_receiveMsgs)
            {
                //_proxy.OnReceive(this, packet.OpCode, msg);
                m_receiveMsgs.Enqueue(new MessageInfo()
                {
                    OpCode = packet.OpCode,
                    Msg = msg,
                });
            }
        }

        public override void Send(byte opcode, MessageProto msg,IPEndPoint remote)
        {
            if (IsDisposed || m_UdpChannel == null)
                return;
            m_UdpChannel.Send(opcode, msg, remote);
        }

        public override void Update()
        {
            if (IsDisposed)
                return;
            lock (m_receiveMsgs)
            {
                while (m_receiveMsgs.Count > 0)
                {
                    var msgInfo = m_receiveMsgs.Dequeue();
                    this.MessageDispatcher.Dispatch(null, msgInfo.OpCode, msgInfo.Msg);
                }
            }
        }

        public override void Dispose()
        {
            if (m_UdpChannel != null)
            {
                m_UdpChannel.Dispose();
                m_UdpChannel = null;
            }
            base.Dispose();
        }
    }
}
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
//using Google.Protobuf;

namespace Lockstep.NetWork
{
    //服务器和客户端一样
    public class UDPNetProxy : NetWorkProxy,IBroadcast
    {
        private UDPChannel m_UdpChannel;
        private readonly Queue<MessageInfo> m_receiveMsgs = new Queue<MessageInfo>();
        private IPEndPoint m_remote;

        public UDPNetProxy(IPEndPoint localIP) 
        {
            m_UdpChannel = new UDPChannel(this,localIP);
            LocalIPEndPoint = localIP;
            StartReceive();
        }

        public async void StartReceive()
        {
            while (true)
            {
                Packet packet = await m_UdpChannel.RecvAsync();
                if (IsDisposed)
                    return;
                OnRecv(packet);
            }
        }

        public void Connect(string ip, int port) 
        {
            m_remote = NetHelper.GetIPEndPoint(ip, port);
        }

        public void DisConnect() 
        {
            m_remote = null;
        }

        private void OnRecv(Packet packet)
        {
            if (IsDisposed)
                return;
            if (this.MessagePacker == null)
                return;

            var msg = this.MessagePacker.DeserializeFrom(packet.OpCode,packet.Bytes, Packet.DataIndex, packet.Size);
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

        public override void Send(byte opcode, object msg,IPEndPoint remote)
        {
            if (IsDisposed || m_UdpChannel == null)
                return;
            m_UdpChannel.Send(opcode, msg, remote);
        }

        public void Send(byte opcode, object msg)
        {
            if (IsDisposed || m_UdpChannel == null)
                return;
            if (m_remote == null)
                return;
            m_UdpChannel.Send(opcode, msg, m_remote);
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
            base.Dispose();
            if (m_UdpChannel != null)
            {
                m_UdpChannel.Dispose();
                m_UdpChannel = null;
            }
        }

        private IPEndPoint m_lastEndPoint;
        //udp局域网广播
        public void Broadcast(byte opcode, object msg, int remotePort)
        {
            if (m_lastEndPoint == null || m_lastEndPoint.Port != remotePort) 
            {
                m_lastEndPoint = NetHelper.GetIPEndPoint("255.255.255.255", remotePort);
            }
            this.Send(opcode,msg,m_lastEndPoint);
        }

        //向一个端口段 广播
        public void Broadcast(byte opcode, object msg, int remoteMinPort,int remoteMaxPort) 
        {
            for (int i = remoteMinPort; i < remoteMaxPort; i++) 
            {
                Broadcast(opcode, msg, i);
            }
        }
    }
}
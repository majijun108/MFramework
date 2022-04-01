using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{
    public class TCPService : NetWorkProxy
    {
        private TcpListener acceptor;
        private Dictionary<long, Session> m_sessions = new Dictionary<long, Session>();

        public TCPService(IPEndPoint inpoint) 
        {
            acceptor = new TcpListener(inpoint);
            acceptor.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            acceptor.Server.NoDelay = true;
            acceptor.Start();
            this.StartAccept();
        }

        private async Task<AChannel> AcceptChannel()
        {
            TcpClient client = await this.acceptor.AcceptTcpClientAsync();
            TCPChannel chanel = new TCPChannel(this,client);

            return chanel;
        }

        public async void StartAccept()
        {
            while (true)
            {
                if (IsDisposed)
                    return;
                AChannel channel = await AcceptChannel();
                Session session = new Session() { Id = IdGenerater.GenerateId() };
                session.Awake(this, channel);
                m_sessions.Add(session.Id, session);
                session.StartRecv();
            }
        }

        public override void OnReceive(Session session,byte opCode, object msg) 
        {
            if (IsDisposed)
                return;
            if (this.MessagePacker == null)
                return;
            if (!m_sessions.ContainsKey(session.Id))
                return;
            if (this.MessageDispatcher == null)
                return;
            this.MessageDispatcher.Dispatch(session, opCode, msg);
        }

        public override void Send(byte opcode, object msg,IPEndPoint remote)
        {
            if (m_sessions.Count == 0)
                return;
            Session s;
            for (int i = 0; i < m_sessions.Count; i++)
            {
                s = m_sessions[i];
                s.Send(opcode, msg);
            }
        }

        public void Send(byte opcode, object msg, long sessionID) 
        {
            if (!m_sessions.ContainsKey(sessionID))
                return;
            var session = m_sessions[sessionID];
            session.Send(opcode, msg);
        }

        public override void Dispose()
        {
            if (acceptor != null) 
            {
                acceptor.Stop();
                acceptor = null;
            }
            foreach (var item in m_sessions)
            {
                item.Value.Dispose();
            }
            m_sessions.Clear();
            base.Dispose();
        }

        public override void Update()
        {
            if (m_sessions.Count == 0)
                return;
            for (int i = 0; i < m_sessions.Count; i++)
            {
                m_sessions[i].Update();
            }
        }
    }

    public class TCPClient : NetWorkProxy
    {
        private Session m_Client;
        private TcpClient m_TcpClient;
        public TCPClient(IPEndPoint localIP) 
        {
            m_TcpClient = new TcpClient(localIP);
        }

        public void Connect(IPEndPoint remote) 
        {
            m_TcpClient.Connect(remote);
            if (!m_TcpClient.Connected) 
            {
                DebugService.Instance.LogError("tcp client connet failed");
            }
            var channel = new TCPChannel(this, m_TcpClient);
            m_Client = new Session() { Id = IdGenerater.GenerateId() };
            m_Client.Awake(this, channel);
            m_Client.StartRecv();
        }

        public override void OnReceive(Session session, byte opCode, object msg)
        {
            if (IsDisposed)
                return;
            if (this.MessagePacker == null)
                return;
            if (m_Client.Id != session.Id)
                return;
            if (this.MessageDispatcher == null)
                return;
            this.MessageDispatcher.Dispatch(session, opCode, msg);
        }

        public override void Send(byte opcode, object msg,IPEndPoint remote)
        {
            if (m_Client == null)
                return;
            m_Client.Send(opcode, msg);
        }

        public override void Dispose()
        {
            if (m_Client != null)
                m_Client.Dispose();
            m_Client = null;
            base.Dispose();
        }

        public override void Update()
        {
            if (m_Client != null)
                m_Client.Update();
        }
    }
}
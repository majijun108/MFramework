using System;
using System.Collections.Generic;
using System.Net;

namespace Lockstep.NetWork {
    public enum NetworkProtocol
    {
        TCP,
        UDP,
    }
    public class NetworkProxy :NetBase
    {
        private AService _service;

        //消息分发器
        public IMessageDispatcher MessageDispatcher { get; set; }
        //消息打包器
        public IMessagePacker MessagePacker { get; set; }

        private Dictionary<long,Session> _sessions = new Dictionary<long,Session>();

        public void Awake(AService service) 
        {
            _service = service;
        }

        public async void StartAccept() 
        {
            while (true) 
            {
                if(IsDisposed)
                    return;
                AChannel channel = await _service.AcceptChannel();
                Session session = new Session() { Id = IdGenerater.GenerateId()};
                session.Awake(this,channel);
                _sessions.Add(session.Id, session);
                session.StartRecv();
            }
        }

        //创建一个会话
        public Session CreateSession(IPEndPoint point) 
        {
            AChannel aChannel = _service.ConnectChannel(point);
            Session session = new Session() { Id = IdGenerater.GenerateId() };
            session.Awake(this, aChannel);
            _sessions.Add(session.Id, session);

            return session;
        }

        public void Remove(long id) 
        {
            if (_sessions.ContainsKey(id)) 
            {
                var session = _sessions[id];
                _sessions.Remove(id);
                session.Dispose();
            }
        }

        public void Update() 
        {
            if (_service != null) 
            {
                _service.Update();
            }
        }

        public override void Dispose()
        { 
            base.Dispose();
            foreach (var item in _sessions)
            {
                item.Value.Dispose();
            }
            _service.Dispose();
        }
    }

    public class Session : NetBase 
    {
        private AChannel _channel;
        private NetworkProxy _proxy;
        public void Awake(NetworkProxy net, AChannel c) 
        {
            this._proxy = net;
            this._channel = c;
        }

        public async void StartRecv() 
        {
            while (true) 
            {
                if (IsDisposed)
                    return;
                Packet packet = await _channel.RecvAsync();
                if (IsDisposed)
                    return;
                OnRecv(packet);
            }
        }

        public void Send(byte opcode, byte[] data) 
        {
            if (IsDisposed)
                return;
            _channel.Send(opcode, data);
        }

        private void OnRecv(Packet packet) 
        {
            if (this._proxy.MessagePacker == null)
                return;
            var msg = this._proxy.MessagePacker.DeserializeFrom(packet.Bytes,Packet.DataIndex,packet.Size);
            if (this._proxy.MessageDispatcher == null)
                return;
            this._proxy.MessageDispatcher.Dispatch(this, packet.OpCode, msg);
        }

        public override void Dispose()
        {
            base.Dispose();
            this._channel.Dispose();
        }
    }
}

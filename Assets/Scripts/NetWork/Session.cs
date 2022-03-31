using System;
using System.Collections.Generic;
using System.Net;
using MessageProto = Google.Protobuf.IMessage;
using Google.Protobuf;

namespace Lockstep.NetWork
{
    //只有tcp才有会话
    public class Session : NetBase
    {
        private AChannel _channel;
        private NetWorkProxy _proxy;
        private readonly Queue<MessageInfo> m_receiveMsgs = new Queue<MessageInfo>();

        public void Awake(NetWorkProxy net, AChannel c)
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

        private void OnRecv(Packet packet)
        {
            if (IsDisposed)
                return;
            if (_proxy != null)
            {
                if (_proxy.MessagePacker == null)
                    return;

                var msg = _proxy.MessagePacker.DeserializeFrom(packet.OpCode,packet.Bytes, Packet.DataIndex, packet.Size);
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
        }

        public void Update() 
        {
            if (IsDisposed)
                return;
            lock (m_receiveMsgs)
            {
                while (m_receiveMsgs.Count > 0)
                {
                    var msgInfo = m_receiveMsgs.Dequeue();
                    _proxy.OnReceive(this, msgInfo.OpCode, msgInfo.Msg);
                }
            }
        }

        public void Send(byte opcode,object msg)
        {
            if (IsDisposed)
                return;
            _channel.Send(opcode, msg);
        }

        public override void Dispose()
        {
            base.Dispose();
            this._channel.Dispose();
        }
    }
}

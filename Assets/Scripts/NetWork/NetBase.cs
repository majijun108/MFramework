using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{
    public class NetBase : IDisposable
    {
        public long Id;
        public bool IsDisposed = false;
        public virtual void Dispose() { IsDisposed = true; }
    }

    //要发送的信息的结构体
    public struct MessageInfo
    {
        public byte OpCode;
        public object Msg;
        public IPEndPoint Remote;
    }

    public abstract class NetWorkProxy : NetBase
    {
        //消息分发器
        public IMessageDispatcher MessageDispatcher { get; set; }
        //消息打包器
        public IMessagePacker MessagePacker { get; set; }
        public abstract void Send(byte opcode, object msg, IPEndPoint remote = null);
        public virtual void OnReceive(Session session, byte opCode, object msg) { }
        public abstract void Update();
    }

    //一个会话有可能也是一个广播
    public interface IBroadcast
    {
        void Broadcast(byte opcode, object msg,int port);
    }

    public class IdGenerater
    {
        private static long id = 0;

        public static long GenerateId()
        {
            return id++;
        }
    }

    public enum ChannelType
    {
        Connect,
        Accept,
    }

    public abstract class AChannel : NetBase 
    {
        public ChannelType ChannelType { get; }

        protected NetWorkProxy m_NetProxy;

        public IPEndPoint RemoteAddress { get; protected set; }

        private bool m_enable = true;
        public virtual bool Enable { get { return m_enable; } set{
                m_enable = value;} }

        protected AChannel(NetWorkProxy service, ChannelType channelType)
        {
            this.Id = IdGenerater.GenerateId();
            this.ChannelType = channelType;
            this.m_NetProxy = service;
        }

        public abstract void Send(byte opcode, object msg, IPEndPoint remote = null);

        public abstract Task<Packet> RecvAsync();

        /// <summary>
        /// 接收消息
        /// </summary>
        //public abstract Task<Packet> Recv();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}

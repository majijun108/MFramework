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

    public abstract class AService : NetBase 
    {
        public abstract Task<AChannel> AcceptChannel();

        public abstract AChannel ConnectChannel(IPEndPoint ipEndPoint);

        public abstract void Remove(long channelId);

        public abstract void Update();
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

        protected AService service;

        public IPEndPoint RemoteAddress { get; protected set; }

        protected AChannel(AService service, ChannelType channelType)
        {
            this.Id = IdGenerater.GenerateId();
            this.ChannelType = channelType;
            this.service = service;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public abstract void Send(byte opcode, byte[] buffer, int index, int length);

        public abstract void Send(byte opcode, byte[] buffer);

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

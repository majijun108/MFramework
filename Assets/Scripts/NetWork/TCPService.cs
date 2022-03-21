using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{
    public class TCPService : AService
    {
        private TcpListener acceptor;

        public TCPService(IPEndPoint inpoint) 
        {
            acceptor = new TcpListener(inpoint);
            acceptor.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            acceptor.Server.NoDelay = true;
            acceptor.Start();
        }

        public override async Task<AChannel> AcceptChannel()
        {
            TcpClient client = await this.acceptor.AcceptTcpClientAsync();
            TCPChannel chanel = new TCPChannel(this,client);

            return chanel;
        }

        public override AChannel ConnectChannel(IPEndPoint ipEndPoint)
        {
            throw new NotImplementedException();
        }

        public override void Remove(long channelId)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
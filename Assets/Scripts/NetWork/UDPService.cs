using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{
    public class UDPService : AService
    {
        public UDPService() 
        {
            
        }

        public override async Task<AChannel> AcceptChannel()
        {
            throw new Exception("UDP CANNOT ACCEPT");
        }

        public override AChannel ConnectChannel(IPEndPoint ipEndPoint)
        {
            UdpClient client = new UdpClient(ipEndPoint);
            UDPChannel channel = new UDPChannel(this, client);

            return channel;
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
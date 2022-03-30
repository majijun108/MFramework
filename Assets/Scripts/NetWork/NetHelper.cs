using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Lockstep.NetWork
{
    public static class NetHelper
    {
        public static IPEndPoint GetIPEndPoint(IPAddress adress, int port) 
        {
            return new IPEndPoint(adress, port);
        }
        public static IPEndPoint GetIPEndPoint(string ip, int port) 
        {
            return new IPEndPoint(IPAddress.Parse(ip), port);
        }
        public static IPEndPoint GetIPEndPoint(int port)
        {
            return new IPEndPoint(IPAddress.Any, port);
        }

        //获取本地IP
        public static string GetLocalIP()
        {
            string address = string.Empty;
            foreach (var item in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = item.ToString();
                    break;
                }
            }
            return address;
        }
    }
}

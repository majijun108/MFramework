using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

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

        public static int FindAvailablePort(int startPort,int maxPort = -1)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return startPort;
            }

            try
            {
                if (maxPort == -1)
                    maxPort = IPEndPoint.MaxPort;

                //获取本地计算机的网络连接和通信统计数据的信息
                //部分安卓没有该权限
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                //返回本地计算机上的所有Tcp监听程序            
                IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
                //返回本地计算机上的所有UDP监听程序            
                IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
                //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。            
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                List<int> allPorts = new List<int>();
                foreach (IPEndPoint ep in ipsTCP)
                {
                    allPorts.Add(ep.Port);
                }

                foreach (IPEndPoint ep in ipsUDP)
                {
                    allPorts.Add(ep.Port);
                }

                foreach (TcpConnectionInformation conn in tcpConnInfoArray)
                {
                    allPorts.Add(conn.LocalEndPoint.Port);
                }

                while (allPorts.Contains(startPort) && startPort < maxPort)
                {
                    startPort++;
                }

                return startPort;
            }
            catch (Exception e)
            {
                DebugService.Instance.LogError("FindAvailablePort Exception:", startPort.ToString(), e.ToString());
                return startPort;
            }
        }
    }
}

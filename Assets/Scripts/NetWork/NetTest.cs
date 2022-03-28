using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System;
using TMPro;
using System.Linq;
using Google.Protobuf;
using ServerMessage;

class aa : IService
{

}
public class NetTest : MonoBehaviour
{
    public Button cbtn;
    public TMP_Text logText;

    UdpClient server;
    void Start()
    {
        cbtn.onClick.AddListener(OnCbtnClick);

        server = new UdpClient(new IPEndPoint(IPAddress.Any, 7788));
        //receiveServer.Connect(IPAddress.Any, 7788);
        StartRecv();
    }

    async void StartRecv() 
    {
        while (true)
        {
            var steam = await server.ReceiveAsync();
            //Person p = Person.Parser.ParseFrom(steam.Buffer);
            //logText.text = p.Name + "/" + p.Age;
            var sendByte = System.Text.Encoding.UTF8.GetBytes("服务器收到消息");
            await server.SendAsync(sendByte, sendByte.Length,steam.RemoteEndPoint);
        }
    }

    void OnCbtnClick() 
    {
        SendCreate();
    }

    IPEndPoint broadIP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 7788);
    UdpClient client;
    void SendCreate()
    {
        using (UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 6677))) 
        {
            
        }
        //if (client == null)
        //{
        //    client = new UdpClient(new IPEndPoint(IPAddress.Any, 6677));
        //    ClientStartRecv();
        //}
        //Person person = new Person();
        //person.Name = "name";
        //person.Age = 10;
        //byte[] sendbytes = person.ToByteArray();
        //client.Send(sendbytes, sendbytes.Length, broadIP);
    }

    async void ClientStartRecv()
    {
        while (true)
        {
            var steam = await client.ReceiveAsync();
            string msg = System.Text.Encoding.UTF8.GetString(steam.Buffer);
            Debug.LogError("客户端收到消息/"+ msg);
        }
    }

    string GetLocalIP() 
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


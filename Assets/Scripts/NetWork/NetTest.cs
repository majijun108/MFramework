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
using Protobuf;

class aa : IService
{

}
public class NetTest : MonoBehaviour
{
    public Button cbtn;
    public TMP_Text logText;

    UdpClient receiveServer;
    void Start()
    {
        cbtn.onClick.AddListener(OnCbtnClick);

        receiveServer = new UdpClient(new IPEndPoint(IPAddress.Any, 7788));
        //receiveServer.Connect(IPAddress.Any, 7788);
        StartRecv();
    }

    async void StartRecv() 
    {
        while (true) 
        {
            var steam = await receiveServer.ReceiveAsync();
            Person p = Person.Parser.ParseFrom(steam.Buffer);
            logText.text = p.Name + "/" + p.Age;
        }
    }

    void OnCbtnClick() 
    {
        SendCreate();
    }

    IPEndPoint broadIP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 7788);
    void SendCreate()
    {
        string sendstr = "this is test,LOCAL IP->" + GetLocalIP();
        Debug.LogError(sendstr);
        UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        

        Person person = new Person();
        person.Name = "name";
        person.Age = 10;
        byte[] sendbytes = person.ToByteArray();
        client.Send(sendbytes,sendbytes.Length, broadIP);
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


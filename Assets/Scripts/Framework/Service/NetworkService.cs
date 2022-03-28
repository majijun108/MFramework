using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;

/// <summary>
/// 直接面向unity了 如果是服务器 重写这个
/// </summary>
public class NetworkService : BaseService,IMessageDispatcher
{
    const int BROADCAST_PORT = 8912;//监听的端口号
    const string BROADCAST_IP = "255.255.255.255";//广播的地址 全局域网

    private NetworkProxy client;

    public override void DoStart()
    {
        base.DoStart();
        client = new NetworkProxy();
        client.MessagePacker = MessagePacker.Instance;
        client.MessageDispatcher = this;

        client.Awake(new UDPService());
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        client.Dispose();
        client = null;
    }

    private void BroadCast() 
    {

    }


    //客户端的会话
    private Session listenSession;
    void OnEvent_OnEnterHall(object param) 
    {
        if (client != null)
        {
            client.Remove(client.Id);
            client = null;
        }
        listenSession = client.CreateSession(NetHelper.GetIPEndPoint(BROADCAST_PORT));
        listenSession.StartRecv();
    }

    //离开大厅
    void OnEvent_OnLeaveHall(object param) 
    {
        if (client != null) 
        {
            client.Remove(client.Id);
            client = null;
        }
    }


    public void Dispatch(Session session, byte opcode, IMessage message)
    {
        throw new NotImplementedException();
    }
}


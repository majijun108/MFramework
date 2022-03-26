using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;

/// <summary>
/// 直接面向unity了 如果是服务器 重写这个
/// </summary>
public class NetworkService : BaseService,IMessagePacker,IMessageDispatcher
{

    const int BROADCAST_PORT = 8912;//监听的端口号
    const string BROADCAST_IP = "255.255.255.255";//广播的地址 全局域网

    private NetworkProxy udpProxy;

    public override void DoStart()
    {
        base.DoStart();
        udpProxy = new NetworkProxy();
        udpProxy.MessagePacker = this;
        udpProxy.MessageDispatcher = this;

        udpProxy.Awake(new UDPService());
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        udpProxy.Dispose();
        udpProxy=null;
    }


    //进入大厅的时候 开始监听广播 所有房间的
    private Session roomListener;
    void OnEvent_OnEnterHall(object param) 
    {
        if (roomListener != null)
        {
            udpProxy.Remove(roomListener.Id);
            roomListener = null;
        }
        roomListener = udpProxy.CreateSession(NetHelper.GetIPEndPoint(BROADCAST_PORT));
        roomListener.StartRecv();
    }

    //离开大厅
    void OnEvent_OnLeaveHall(object param) 
    {
        if (roomListener != null) 
        {
            udpProxy.Remove(roomListener.Id);
            roomListener = null;
        }
    }


    public IMessage DeserializeFrom(byte[] bytes, int startIndex, int count)
    {
        throw new NotImplementedException();
    }

    public byte[] SerializeToByteArray(IMessage msg)
    {
        throw new NotImplementedException();
    }

    public void Dispatch(Session session, byte opcode, IMessage message)
    {
        throw new NotImplementedException();
    }
}


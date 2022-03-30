using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;

/// <summary>
/// 直接面向unity了 如果是服务器 重写这个
/// </summary>
public class NetworkService : BaseService
{
    const int BROADCAST_PORT = 8912;//监听的端口号
    const string BROADCAST_IP = "255.255.255.255";//广播的地址 全局域网

    private ClientMsgHandler m_clientMsgHandler;

    public override void DoStart()
    {
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }


    void OnEvent_OnEnterHall(object param) 
    {
        
    }

    //离开大厅
    void OnEvent_OnLeaveHall(object param) 
    {
        
    }
}


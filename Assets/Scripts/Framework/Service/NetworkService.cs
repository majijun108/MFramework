using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

/// <summary>
/// 直接面向unity了 如果是服务器 重写这个
/// </summary>
public class NetworkService : BaseGameService
{
    const int BROADCAST_PORT = 8912;//监听的端口号

    private readonly ClientMsgHandler m_clientMsgHandler = new ClientMsgHandler();
    private UDPNetProxy m_broadCast;

    public override void DoStart()
    {
        if (m_broadCast == null)
        {
            m_broadCast = new UDPNetProxy(NetHelper.GetIPEndPoint(BROADCAST_PORT))
            {
                MessageDispatcher = m_clientMsgHandler,
                MessagePacker = MessagePacker.Instance
            };
        }
    }

    public void DoUpdate(float deltaTime) 
    {
        m_broadCast.Update();
    } 

    public override void DoDestroy()
    {
        base.DoDestroy();
    }


    void OnEvent_OnEnterHall(object param)
    {
        C2S_Local myInfo = new C2S_Local()
        {
            ClientIP = NetHelper.GetLocalIP(),
            ClientPort = BROADCAST_PORT,
            PlayerName = "SB"
        };

        m_broadCast.Broadcast((byte)MsgType.C2S_ReqRoomInfo, myInfo, BROADCAST_PORT);
    }

    //离开大厅
    void OnEvent_OnLeaveHall(object param) 
    {
        
    }
}


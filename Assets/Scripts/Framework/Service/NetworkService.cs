using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

/// <summary>
/// 直接面向unity了 如果是服务器 重写这个
/// </summary>
public class NetworkService : BaseSingleService<NetworkService>,INetworkService
{
    public int BROADCAST_PORT = 8962;//监听的端口号
    private UDPNetProxy m_broadCast;
    private RoomManager m_roomManager = new RoomManager();

    public override void DoStart()
    {
        if (m_broadCast == null)
        {
            m_broadCast = new UDPNetProxy(NetHelper.GetIPEndPoint(BROADCAST_PORT))
            {
                MessageDispatcher = ClientMsgHandler.Instance,
                MessagePacker = MessagePacker.Instance
            };
        }
        m_roomManager.Init(this);
        m_roomManager.CreateRoom(BROADCAST_PORT + 1, 2);
    }

    public void DoUpdate(float deltaTime) 
    {
        m_broadCast.Update();
    }

    public override void DoDestroy()
    {
        m_broadCast.Dispose();
        m_broadCast = null;
        base.DoDestroy();
    }


    void OnEvent_OnEnterHall(EEvent type,object param)
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
    void OnEvent_OnLeaveHall(EEvent type,object param) 
    {
        
    }
}


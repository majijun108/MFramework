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

        ClientMsgHandler.Instance.AddListener(MsgType.C2S_ReqRoomInfo, OnReqRoomInfo);
    }

    //玩家请求房间信息
    void OnReqRoomInfo(MsgType type, object param)
    {
        PlayerInfo local = (PlayerInfo)param;
        m_roomManager.BroadcastRoomInfo(local);
    }

    public void DoUpdate(float deltaTime) 
    {
        m_broadCast.Update();
        m_roomManager.Update();
    }

    public bool CreateRoom() 
    {
        var success = m_roomManager.CreateRoom(BROADCAST_PORT + 1,
            m_ConstStateService.RoomMaxCount, m_ConstStateService.PlayerName);
        if (!success)
            return false;

        return true;
    }


    public override void DoDestroy()
    {
        m_broadCast.Dispose();
        m_broadCast = null;
        base.DoDestroy();
        ClientMsgHandler.Instance.RemoveListener(MsgType.C2S_ReqRoomInfo, OnReqRoomInfo);
    }

    void OnEvent_OnEnterHall(EEvent type,object param)
    {
        PlayerInfo myInfo = new PlayerInfo()
        {
            ClientIP = NetHelper.GetLocalIP(),
            ClientPort = BROADCAST_PORT,
            PlayerName = m_ConstStateService.PlayerName
        };

        m_broadCast.Broadcast((byte)MsgType.C2S_ReqRoomInfo, myInfo, BROADCAST_PORT);
    }

    //离开大厅
    void OnEvent_OnLeaveHall(EEvent type,object param) 
    {
        
    }
}


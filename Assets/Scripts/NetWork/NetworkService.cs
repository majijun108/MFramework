using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

/// <summary>
/// 直接面向unity了 如果是服务器 重写这个
/// </summary>
public class NetworkService : BaseSingleService<NetworkService>,INetworkService
{
    public const int BROADCAST_MIN_PORT = 9000;//监听的最小端口号
    public const int BROADCAST_MAX_PORT = 9010;//监听的最大端口号
    private UDPNetProxy m_Client;
    private PlayerInfo m_PlayerInfo;
    private RoomManager m_roomManager = new RoomManager();
    private RoomInfo m_roomInfo;//当前加入的房间信息
    private int m_playerID = -1;

    //获取可用的端口号
    int GetUseablePort() 
    {
        return NetHelper.FindAvailablePort(BROADCAST_MIN_PORT, BROADCAST_MAX_PORT);
    }

    public override void DoStart()
    {
        if (m_Client == null)
        {
            int usePort = GetUseablePort();
            m_Client = new UDPNetProxy(NetHelper.GetIPEndPoint(usePort))
            {
                MessageDispatcher = ClientMsgHandler.Instance,
                MessagePacker = MessagePacker.Instance
            };
            m_PlayerInfo = new PlayerInfo()
            {
                ClientIP = NetHelper.GetLocalIP(),
                ClientPort = usePort,
                PlayerName = m_ConstStateService.PlayerName,
            };
        }
        m_roomManager.Init(this);

        ClientMsgHandler.Instance.AddListener(MsgType.S2C_UpdateRoomInfo, On_S2C_UpdateRoom);
        ClientMsgHandler.Instance.AddListener(MsgType.S2C_ExitRoom, On_S2C_OnExitRoom);
        ClientMsgHandler.Instance.AddListener(MsgType.S2C_CloseRoom, On_S2C_OnClosetRoom);
        ClientMsgHandler.Instance.AddListener(MsgType.S2C_StartGame, On_S2C_StartGame);
    }

    public void DoUpdate(float deltaTime) 
    {
        m_Client.Update();
        m_roomManager.Update();
    }

    //创建房间 并且将自己加入房间
    public void CreateRoomAndEnter() 
    {
        m_PlayerInfo.PlayerName = m_ConstStateService.PlayerName;
        m_roomManager.CreateRoomAndStart(BROADCAST_MAX_PORT + 1,
            m_ConstStateService.RoomMaxCount, m_ConstStateService.PlayerName,BROADCAST_MIN_PORT,BROADCAST_MAX_PORT, m_PlayerInfo);
    }

    //向某一个IP发送一条信息
    public void C2C_SendMsg(MsgType msgType, object msg,IPEndPoint remote) 
    {
        if (m_Client == null)
            return;
        m_Client.Send((byte)msgType, msg, remote);
    }

    //向服务器请求加入房间
    public void C2S_ReqEnterRoom(string serverIP,int serverPort) 
    {
        m_Client.Send((byte)MsgType.C2S_ReqJoinRoom, m_PlayerInfo,
            NetHelper.GetIPEndPoint(serverIP, serverPort));
    }

    //请求退出房间
    public void C2S_ReqExitRoom()
    {
        if (m_roomInfo == null || m_PlayerInfo == null)
            return;
        m_Client.Send((byte)MsgType.C2S_ReqExitRoom, m_PlayerInfo);
    }

    //请求开始游戏
    public void C2S_StartGame()
    {
        m_Client.Send((byte)MsgType.C2S_ReqStartGame, m_PlayerInfo);
    }

    //客户端准备完毕 可以开始战斗
    public void C2S_ClientReady() 
    {

    }



    int GetPlayerID(RoomInfo room)
    {
        if (m_PlayerInfo == null)
            return -1;
        for (int i = 0; i < room.Players.Count; i++)
        {
            var player = room.Players[i];
            if (player.ClientIP == m_PlayerInfo.ClientIP && player.ClientPort == m_PlayerInfo.ClientPort)
                return player.PlayerID;
        }
        return -1;
    }
    //更新房间消息
    public void On_S2C_UpdateRoom(MsgType type,object obj) 
    {
        if (obj == null)
            return;
        RoomInfo room = (RoomInfo)obj;
        if (m_roomInfo != null && m_roomInfo.RoomID == room.RoomID) //更新房间信息
        {
            m_roomInfo = room;
            m_playerID = GetPlayerID(room);
            EventHelper.Instance.Trigger(EEvent.UpdateRoomInfo, m_roomInfo);
            return;
        }
        m_roomInfo = room;
        m_Client.Connect(room.ServerIP,room.ServerPort);
        UIService.Instance.CloseWindow("HallWindowCtrl");
        UIService.Instance.OpenWindow("RoomWindowCtrl");
    }

    //收到退出房间
    public void On_S2C_OnExitRoom(MsgType type, object obj) 
    {
        if (m_roomInfo == null || m_PlayerInfo == null)
            return;
        RoomInfo room = obj as RoomInfo;
        if (room.RoomID != m_roomInfo.RoomID)
            return;

        m_roomInfo = null;
        m_playerID = -1;
        m_Client.DisConnect();
        UIService.Instance.CloseWindow("RoomWindowCtrl");
        UIService.Instance.OpenWindow("HallWindowCtrl");
    }

    public void On_S2C_StartGame(MsgType type, object obj) 
    {
        if (m_roomInfo == null || m_PlayerInfo == null)
            return;
        RoomInfo room = obj as RoomInfo;
        if (room.RoomID != m_roomInfo.RoomID)
            return;
        m_roomInfo = room;
        UIService.Instance.CloseWindow("RoomWindowCtrl");

        LoadingService.Instance.LoadingScene("GameScene", () => 
        {
            C2S_ClientReady();
        });
    }

    public void On_S2C_OnClosetRoom(MsgType type, object obj) 
    {
        if (m_roomInfo == null || m_PlayerInfo == null)
            return;
        RoomInfo room = obj as RoomInfo;
        if (room.RoomID != m_roomInfo.RoomID)
            return;

        m_roomInfo = null;
        m_playerID = -1;
        m_Client.DisConnect();
        UIService.Instance.CloseWindow("RoomWindowCtrl");
        UIService.Instance.OpenWindow("HallWindowCtrl");
    }


    public RoomInfo GetCurrentRoom() 
    {
        return m_roomInfo;
    }

    //是否是主机
    public bool IsMainPlayer(PlayerInfo mainPlayer) 
    {
        if (mainPlayer == null)
            return false;
        return mainPlayer.ClientIP == m_PlayerInfo.ClientIP && mainPlayer.ClientPort == m_PlayerInfo.ClientPort;
    }


    public override void DoDestroy()
    {
        m_Client.Dispose();
        m_Client = null;
        m_roomInfo = null;
        m_PlayerInfo = null;
        m_playerID = -1;
        base.DoDestroy();
        m_roomManager.OnDestroy();
        ClientMsgHandler.Instance.RemoveListener(MsgType.S2C_ExitRoom, On_S2C_OnExitRoom);
        ClientMsgHandler.Instance.RemoveListener(MsgType.S2C_UpdateRoomInfo, On_S2C_UpdateRoom);
        ClientMsgHandler.Instance.RemoveListener(MsgType.S2C_StartGame, On_S2C_StartGame);
    }

    void OnEvent_OnEnterHall(EEvent type,object param)
    {
        m_Client.Broadcast((byte)MsgType.C2S_ReqRoomInfo, m_PlayerInfo,BROADCAST_MIN_PORT,BROADCAST_MAX_PORT);
    }
}


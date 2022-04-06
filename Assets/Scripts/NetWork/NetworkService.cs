﻿using System;
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
    public int BROADCAST_PORT = 8962;//监听的端口号
    private UDPNetProxy m_Client;
    private PlayerInfo m_PlayerInfo;
    private RoomManager m_roomManager = new RoomManager();
    private RoomInfo m_roomInfo;//当前加入的房间信息

    public override void DoStart()
    {
        if (m_Client == null)
        {
            m_Client = new UDPNetProxy(NetHelper.GetIPEndPoint(BROADCAST_PORT))
            {
                MessageDispatcher = ClientMsgHandler.Instance,
                MessagePacker = MessagePacker.Instance
            };
            m_PlayerInfo = new PlayerInfo()
            {
                ClientIP = NetHelper.GetLocalIP(),
                ClientPort = BROADCAST_PORT,
                PlayerName = m_ConstStateService.PlayerName,
            };
        }
        m_roomManager.Init(this);

        ClientMsgHandler.Instance.AddListener(MsgType.S2C_JoinRoom, S2C_OnJoinRoom);
        ClientMsgHandler.Instance.AddListener(MsgType.S2C_ExitRoom, S2C_OnExitRoom);
    }

    public void DoUpdate(float deltaTime) 
    {
        m_Client.Update();
        m_roomManager.Update();
    }

    //创建房间 并且将自己加入房间
    public bool CreateRoom() 
    {
        var roomInfo = m_roomManager.CreateRoom(BROADCAST_PORT + 1,
            m_ConstStateService.RoomMaxCount, m_ConstStateService.PlayerName);
        if (roomInfo==null)
            return false;

        m_roomManager.PlayerEnter(m_PlayerInfo);
        S2C_OnJoinRoom(MsgType.S2C_JoinRoom,roomInfo);
        m_roomManager.BroadcastRoomInfo();
        return true;
    }

    //向服务器请求加入房间
    public void C2S_ReqEnterRoom(RoomInfo room) 
    {
        m_Client.Send((byte)MsgType.C2S_ReqJoinRoom, m_PlayerInfo,
            NetHelper.GetIPEndPoint(room.ServerIP, room.ServerPort));
    }

    //接受服务器加入房间
    public void S2C_OnJoinRoom(MsgType type,object obj) 
    {
        if (obj == null)
            return;
        RoomInfo room = (RoomInfo)obj;
        m_roomInfo = room;
        m_Client.Connect(room.ServerIP,room.ServerPort);
        UIService.Instance.CloseWindow("HallWindowCtrl");
        UIService.Instance.OpenWindow("RoomWindowCtrl");
    }

    //请求退出房间
    public void C2S_ReqExitRoom() 
    {
        if (m_roomInfo == null || m_PlayerInfo == null)
            return;
        m_Client.Send((byte)MsgType.C2S_ReqExitRoom, m_PlayerInfo);
    }

    public void S2C_OnExitRoom(MsgType type, object obj) 
    {
        if (m_roomInfo == null || m_PlayerInfo == null)
            return;
        RoomInfo room = obj as RoomInfo;
        if (room.ServerIP != m_roomInfo.ServerIP || room.ServerPort != m_roomInfo.ServerPort)
            return;
        m_roomInfo = null;
        m_Client.DisConnect();
        UIService.Instance.CloseWindow("RoomWindowCtrl");
        UIService.Instance.OpenWindow("HallWindowCtrl");
    }

    public RoomInfo GetCurrentRoom() 
    {
        return m_roomInfo;
    }

    //是否同时也是服务器
    public bool IsServer() 
    {
        return m_roomManager.IsServer();
    }


    public override void DoDestroy()
    {
        m_Client.Dispose();
        m_Client = null;
        m_roomInfo = null;
        m_PlayerInfo = null;
        base.DoDestroy();
        m_roomManager.OnDestroy();
        ClientMsgHandler.Instance.RemoveListener(MsgType.S2C_ExitRoom, S2C_OnExitRoom);
        ClientMsgHandler.Instance.RemoveListener(MsgType.S2C_JoinRoom, S2C_OnJoinRoom);
    }

    void OnEvent_OnEnterHall(EEvent type,object param)
    {
        m_Client.Broadcast((byte)MsgType.C2S_ReqRoomInfo, m_PlayerInfo, BROADCAST_PORT);
    }

    //离开大厅
    void OnEvent_OnLeaveHall(EEvent type,object param) 
    {
        
    }
}

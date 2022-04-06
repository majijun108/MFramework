using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.NetWork;
using ServerMessage;
using Server;
using System.Net;

public class RoomManager
{

    NetworkService m_Network;
    bool m_IsGaming = false;//�Ƿ�����ս����
    ServerRoom m_Room;
    string m_serverIP;//��ǰ��������IP
    int m_serverPort;//��ǰ�������Ķ˿�
    IPEndPoint m_serverAddress;
    string m_serverName;

    public void Init(NetworkService service)
    {
        m_Network = service;

        ClientMsgHandler.Instance.AddListener(MsgType.C2S_ReqRoomInfo, OnReqRoomInfo);
    }

    //������󷿼���Ϣ
    void OnReqRoomInfo(MsgType type, object param)
    {
        PlayerInfo local = (PlayerInfo)param;
        if (m_Room == null || m_Room.IsDisposed)
            return;
        RoomInfo roomInfo = new RoomInfo()
        {
            ServerIP = m_serverIP,
            ServerPort = m_serverPort,
            RoomName = m_serverName
        };
        m_Network.C2C_SendMsg(MsgType.S2C_RoomInfo, roomInfo,
                NetHelper.GetIPEndPoint(local.ClientIP, local.ClientPort));
    }

    //��������
    public void CreateRoomAndStart(int startPort, int maxCount, string roomName)
    {
        if (m_Room != null && m_Room.IsDisposed == false)
            return;
        m_Room = new ServerRoom(startPort,maxCount, roomName, m_Network.BROADCAST_PORT,ref m_serverIP,ref m_serverPort);
        m_serverAddress = NetHelper.GetIPEndPoint(m_serverIP, m_serverPort);
        m_serverName = roomName;

        PlayerInfo mainPlayer = new PlayerInfo()
        {
            ClientIP = NetHelper.GetLocalIP(),
            ClientPort = m_Network.BROADCAST_PORT,
            PlayerName = roomName
        };
        m_Room.Start(mainPlayer);//���俪ʼ ������������Ϣ
    }

    public IPEndPoint GetServerAddress() 
    {
        if (m_Room != null && m_Room.IsDisposed == false)
            return null;
        return m_serverAddress;
    }

    public void StartGame() 
    {
        
    }

    public void Update()
    {
    }

    public void OnDestroy() 
    {
        ClientMsgHandler.Instance.RemoveListener(MsgType.C2S_ReqRoomInfo, OnReqRoomInfo);
        if (m_Room != null)
        {
            m_Room.Dispose();
            m_Room = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.NetWork;
using Server;
using System.Net;

public class RoomManager
{

    NetworkService m_Network;
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
        if (m_Room == null || m_Room.IsDisposed)
            return;
        m_Room.PushCommand(new ServerCmdInfo()
        {
            type = SERVER_CMD_TYPE.SEND_ROOMINFO,
            obj = param
        });
    }

    //��������
    public void CreateRoomAndStart(int startPort, int maxCount, string roomName,int broadMin,int broadMax,PlayerInfo mainPlayer)
    {
        if (m_Room != null && m_Room.IsDisposed == false)
            return;
        m_Room = new ServerRoom(startPort,maxCount, roomName,broadMin,broadMax,ref m_serverIP,ref m_serverPort);
        m_serverAddress = NetHelper.GetIPEndPoint(m_serverIP, m_serverPort);
        m_serverName = roomName;

        m_Room.Start(mainPlayer);//���俪ʼ ������������Ϣ
    }

    public IPEndPoint GetServerAddress() 
    {
        if (m_Room != null && m_Room.IsDisposed == false)
            return null;
        return m_serverAddress;
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

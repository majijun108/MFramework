using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.NetWork;
using ServerMessage;

public class RoomManager : IMessageDispatcher
{
    class Room
    {
        public UDPNetProxy RoomProxy;
        public RoomInfo Info;
        public Room(UDPNetProxy roomNet, RoomInfo info)
        {
            RoomProxy = roomNet;
            Info = info;
        }
        public void Dispose()
        {
            RoomProxy.Dispose();
            RoomProxy = null;
        }
    }

    NetworkService m_Network;
    public void Init(NetworkService service)
    {
        m_Network = service;

        ClientMsgHandler.Instance.AddListener(MsgType.C2S_ReqRoomInfo, OnReqRoomInfo);
    }

    //������󷿼���Ϣ
    void OnReqRoomInfo(MsgType type, object param)
    {
        PlayerInfo local = (PlayerInfo)param;
        BroadcastRoomInfo(local);
    }

    Room room;
    //��������
    public RoomInfo CreateRoom(int startPort, int maxCount, string roomName)
    {
        if (room != null)
            return null;
        int port = NetHelper.FindAvailablePort(startPort);
        var roomProxy = new UDPNetProxy(NetHelper.GetIPEndPoint(startPort))
        {
            MessagePacker = MessagePacker.Instance,
            MessageDispatcher = this
        };

        RoomInfo roomInfo = new RoomInfo()
        {
            ServerIP = NetHelper.GetLocalIP(),
            ServerPort = port,
            RoomName = roomName,
            MaxCount = maxCount
        };

        room = new Room(roomProxy, roomInfo);

        return roomInfo;
    }

    //�㲥������Ϣ
    public void BroadcastRoomInfo(PlayerInfo other = null)
    {
        if (room == null)
            return;
        if (other == null)
        {
            room.RoomProxy.Broadcast((byte)MsgType.S2C_RoomInfo,
                room.Info, m_Network.BROADCAST_PORT);
        }
        else 
        {
            room.RoomProxy.Send((byte)MsgType.S2C_RoomInfo, room.Info,
                NetHelper.GetIPEndPoint(other.ClientIP, other.ClientPort));
        }
    }

    //��Ҽ��뷿��
    public void PlayerEnter(PlayerInfo info)
    {
        if (info == null || room == null)
            return;
        if (room.Info.Players.Count >= room.Info.MaxCount)
            return;
        room.Info.Players.Add(info);
        //����Լ�������
        //room.Info.Players.Add();
    }

    int GetPlayerIndex(PlayerInfo player) 
    {
        if (room == null)
            return -1;
        for (int i = 0; i < room.Info.Players.Count; i++)
        {
            var p = room.Info.Players[i];
            if (p.ClientIP == player.ClientIP && p.ClientPort == player.ClientPort)
                return i;
        }
        return -1;
    }

    //��ͻ��˷����߳�����
    void SendExitRoom(PlayerInfo player) 
    {
        if (room == null || player == null)
            return;
        int playerIndex = GetPlayerIndex(player);
        if (playerIndex == -1)
            return;
        room.Info.Players.RemoveAt(playerIndex);
        room.RoomProxy.Send((byte)MsgType.S2C_ExitRoom, room.Info,
                NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
    }

    public void CloseRoom() 
    {
        if (room == null)
            return;
        for (int i = room.Info.Players.Count - 1; i >= 0; i--) 
        {
            SendExitRoom(room.Info.Players[i]);
        }
        room.Dispose();
        room = null;
    }

    public bool IsServer() 
    {
        if (room == null)
            return false;
        return true;
    }

    public void Update()
    {
        if (room != null)
        {
            room.RoomProxy.Update();
        }
    }

    public void OnDestroy() 
    {
        CloseRoom();
        ClientMsgHandler.Instance.RemoveListener(MsgType.C2S_ReqRoomInfo, OnReqRoomInfo);
    }

    //�ͻ���������뷿��
    void OnC2S_ReqJoinRoom(PlayerInfo player) 
    {
        if (player == null || room == null)
            return;
        if (room.Info.Players.Count >= room.Info.MaxCount)
            return;
        int index = GetPlayerIndex(player);
        if (index >= 0)
            return;
        room.Info.Players.Add(player);
        room.RoomProxy.Send((byte)MsgType.S2C_JoinRoom, room.Info, 
            NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
    }

    //�ͻ��������˳�����
    void OnC2S_ReqExitRoom(PlayerInfo player)
    {
        if (player == null || room == null)
            return;
        var owner = room.Info.Players[0];
        if (owner.ClientIP == player.ClientIP && owner.ClientPort == player.ClientPort) 
        {
            CloseRoom();
            return;
        }
        SendExitRoom(player);
    }

    //�������յ�����Ϣ
    public void Dispatch(Session session, byte opcode, object message)
    {
        MsgType opType = (MsgType)opcode;
        switch (opType) 
        {
            case MsgType.C2S_ReqJoinRoom://������뷿��
                OnC2S_ReqJoinRoom(message as PlayerInfo);
                break;
            case MsgType.C2S_ReqExitRoom://�����˳�����
                OnC2S_ReqExitRoom(message as PlayerInfo);
                break;
        }
    }
}

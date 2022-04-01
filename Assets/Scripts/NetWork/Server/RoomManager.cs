using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.NetWork;
using ServerMessage;

public class RoomManager:IMessageDispatcher
{
    class Room
    {
        public UDPNetProxy RoomProxy;
        public int PlayerCount;
        public int MaxCount;
        public Room(UDPNetProxy roomNet,int maxCount) 
        {
            RoomProxy = roomNet;
            this.MaxCount = maxCount;
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


    Room room;
    //创建房间
    public bool CreateRoom(int startPort,int maxCount) 
    {
        if (room != null)
            return false;
        int port = NetHelper.FindAvailablePort(startPort);
        var roomProxy = new UDPNetProxy(NetHelper.GetIPEndPoint(startPort)) 
        {
            MessagePacker = MessagePacker.Instance,
            MessageDispatcher = this
        };
        
        room = new Room(roomProxy, maxCount);
        room.PlayerCount = 1;

        C2S_RoomInfo roomInfo = new C2S_RoomInfo()
        {
            ServerIP = NetHelper.GetLocalIP(),
            ServerPort = port,
            RoomName = "test",
            PlayerCount = room.PlayerCount,
            MaxCount = room.MaxCount
        };
        room.RoomProxy.Broadcast((byte)MsgType.S2C_RoomInfo, roomInfo,m_Network.BROADCAST_PORT);

        return true;
    }

    void OnReqRoomInfo(MsgType type, object param)
    {
        if (room == null)
            return;
        C2S_Local local = (C2S_Local)param;
        C2S_RoomInfo roomInfo = new C2S_RoomInfo()
        {
            ServerIP = NetHelper.GetLocalIP(),
            ServerPort = room.RoomProxy.LocalIPEndPoint.Port,
            RoomName = "test",
            PlayerCount = room.PlayerCount,
            MaxCount = room.MaxCount
        };
        room.RoomProxy.Send((byte)MsgType.S2C_RoomInfo, roomInfo,NetHelper.GetIPEndPoint(local.ClientIP,local.ClientPort));
    }

    public void CloseRoom() 
    {
        if (room == null)
            return;
        room.Dispose();
        room = null;
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
    }

    //服务器收到的消息
    public void Dispatch(Session session, byte opcode, object message)
    {
        
    }
}

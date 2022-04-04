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
    }


    Room room;
    //创建房间
    public bool CreateRoom(int startPort, int maxCount, string roomName)
    {
        if (room != null)
            return false;
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

        return true;
    }

    //广播房间信息
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

    //玩家加入房间
    public void PlayerEnter(PlayerInfo info)
    {
        if (info == null || room == null)
            return;
        if (room.Info.Players.Count >= room.Info.MaxCount)
            return;
        room.Info.Players.Add(info);
        //添加自己进房间
        //room.Info.Players.Add();
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

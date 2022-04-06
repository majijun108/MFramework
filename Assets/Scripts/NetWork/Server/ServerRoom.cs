using Lockstep.NetWork;
using ServerMessage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    //服务器房间 跑在另外的线程
    public class ServerRoom:IMessageDispatcher
    {
        private const int FrameRate = 30;

        public bool IsDisposed { get; private set; } = false;
        private UDPNetProxy m_Server;
        private RoomInfo m_roomInfo;
        private int m_broadcastPort;//广播的端口号
        private string m_mainPlayerIP;//主机的IP
        private int m_mainPlayerPort;//主机的端口

        private int m_frameDelta;//每帧的时间
        private DateTime m_startTime;

        public ServerRoom(int startPort, int maxCount, string roomName,int broadPort,ref string serverIP,ref int serverPort) 
        {
            m_broadcastPort = broadPort;
            int port = NetHelper.FindAvailablePort(startPort);
            m_Server = new UDPNetProxy(NetHelper.GetIPEndPoint(startPort))
            {
                MessagePacker = MessagePacker.Instance,
                MessageDispatcher = this
            };

            m_roomInfo = new RoomInfo()
            {
                ServerIP = NetHelper.GetLocalIP(),
                ServerPort = port,
                RoomName = roomName,
                MaxCount = maxCount
            };
            m_frameDelta = 1000 / FrameRate;
            m_startTime = DateTime.Now;
            serverIP = NetHelper.GetLocalIP();
            serverPort = port;
        }

        public void Start(PlayerInfo mainPlayer) 
        {
            m_mainPlayerIP = mainPlayer.ClientIP;
            m_mainPlayerPort = mainPlayer.ClientPort;

            //添加主机信息
            m_roomInfo.Players.Add(mainPlayer);
            m_Server.Send((byte)MsgType.S2C_JoinRoom, m_roomInfo,
                NetHelper.GetIPEndPoint(mainPlayer.ClientIP, mainPlayer.ClientPort));
            BroadcastRoomInfo();

            Run();
        }

        //创建完房间 广播服务器
        private void BroadcastRoomInfo()
        {
            if (m_roomInfo == null)
                return;
            m_Server.Broadcast((byte)MsgType.S2C_RoomInfo, m_roomInfo,
                    m_broadcastPort);
        }

        async void Run() 
        {
            while (true) 
            {
                if (IsDisposed)
                    return;
                Tick();
                await Task.Delay(FrameRate);
            }
        }


        void Tick() 
        {
            m_Server?.Update();
        }

        int GetPlayerIndex(PlayerInfo player)
        {
            if (m_roomInfo == null)
                return -1;
            for (int i = 0; i < m_roomInfo.Players.Count; i++)
            {
                var p = m_roomInfo.Players[i];
                if (p.ClientIP == player.ClientIP && p.ClientPort == player.ClientPort)
                    return i;
            }
            return -1;
        }

        //客户端请求加入房间
        void OnC2S_ReqJoinRoom(PlayerInfo player)
        {
            if (player == null || m_roomInfo == null)
                return;
            if (m_roomInfo.Players.Count >= m_roomInfo.MaxCount)
                return;
            int index = GetPlayerIndex(player);
            if (index >= 0)
                return;
            m_roomInfo.Players.Add(player);
            m_Server.Send((byte)MsgType.S2C_JoinRoom, m_roomInfo,
                NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
        }

        //客户端请求退出房间
        void OnC2S_ReqExitRoom(PlayerInfo player)
        {
            if (player == null || m_roomInfo == null)
                return;
            int index = GetPlayerIndex(player);
            if (index == -1)
                return;

            if (IsMainPlayer(player))
            {
                CloseRoom();
                return;
            }
            m_roomInfo.Players.RemoveAt(index);
            m_Server.Send((byte)MsgType.S2C_ExitRoom, m_roomInfo,
                NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
        }

        //关闭房间
        public void CloseRoom()
        {
            Broadcast(MsgType.S2C_ExitRoom, m_roomInfo);
            this.Dispose();
        }

        //向房间其他人广播
        void Broadcast(MsgType msgType, object msg) 
        {
            if (m_roomInfo == null)
                return;
            for (int i = 0; i < m_roomInfo.Players.Count; i++)
            {
                var player = m_roomInfo.Players[i];
                m_Server.Send((byte)msgType, msg,
                    NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
            }
        }

        //是否是主机
        bool IsMainPlayer(PlayerInfo player) 
        {
            if (player == null)
                return false;
            return player.ClientIP == m_mainPlayerIP && player.ClientPort == m_mainPlayerPort;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            m_Server.Dispose();
            m_Server = null;
            m_roomInfo = null;
        }

        public void Dispatch(Session session, byte opcode, object message)
        {
            MsgType opType = (MsgType)opcode;
            switch (opType)
            {
                case MsgType.C2S_ReqJoinRoom://请求加入房间
                    OnC2S_ReqJoinRoom(message as PlayerInfo);
                    break;
                case MsgType.C2S_ReqExitRoom://请求退出房间
                    OnC2S_ReqExitRoom(message as PlayerInfo);
                    break;
            }
        }
    }
}

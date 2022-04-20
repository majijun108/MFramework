using Lockstep.NetWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public enum SERVER_CMD_TYPE 
    {
        SEND_ROOMINFO,//向玩家发送房间信息
    }
    public struct ServerCmdInfo 
    {
        public SERVER_CMD_TYPE type;
        public object obj;
    }
    //服务器房间 跑在另外的线程
    public class ServerRoom : IMessageDispatcher
    {
        enum RoomState
        {
            WaitingForPlayer,
            WaitingForBattle,
            InBattle,
            Disposed
        }

        private const int FrameDelta = 30;//每帧的间隔 30ms

        private UDPNetProxy m_Server;
        private RoomInfo m_roomInfo;
        private int m_broadcastMinPort;//广播的端口号
        private int m_broadcastMaxPort;//广播的端口号
        private string m_mainPlayerIP;//主机的IP
        private int m_mainPlayerPort;//主机的端口

        private List<PlayerInfo> m_readyPlayer = new List<PlayerInfo>();//已经准备好的客户端

        private Queue<ServerCmdInfo> m_cmdQueue = new Queue<ServerCmdInfo>();
        private RoomState m_roomState;
        private int m_curPlayerID = 0;
        private Dictionary<int,int> m_playerID2Index = new Dictionary<int,int>();//角色ID和数组种索引的对应关系
        private int m_MaxPlayerCount;

        static int roomID = 1;

        private List<Msg_FrameInfo> m_allHistoryFrames = new List<Msg_FrameInfo>();//所有的历史帧

        public bool IsDisposed { get { return m_roomState == RoomState.Disposed; } }

        public ServerRoom(int startPort, int maxCount, string roomName,int broadMin,int broadMax,ref string serverIP,ref int serverPort) 
        {
            m_broadcastMinPort = broadMin;
            m_broadcastMaxPort = broadMax;
            int port = NetHelper.FindAvailablePort(startPort);
            m_Server = new UDPNetProxy(NetHelper.GetIPEndPoint(port))
            {
                MessagePacker = MessagePacker.Instance,
                MessageDispatcher = this
            };

            m_roomInfo = new RoomInfo()
            {
                ServerIP = NetHelper.GetLocalIP(),
                ServerPort = port,
                RoomName = roomName,
                MaxCount = maxCount,
                RoomID = roomID++,
                Players = new List<PlayerInfo>()
            };
            m_MaxPlayerCount = maxCount;
            serverIP = NetHelper.GetLocalIP();
            serverPort = port;

            m_readyPlayer.Clear();
            m_allHistoryFrames.Clear();
            m_playerID2Index.Clear();

            m_roomState = RoomState.WaitingForPlayer;
        }

        public void Start(PlayerInfo mainPlayer) 
        {
            m_mainPlayerIP = mainPlayer.ClientIP;
            m_mainPlayerPort = mainPlayer.ClientPort;

            //添加主机信息
            mainPlayer.PlayerID = m_curPlayerID++;
            m_roomInfo.Players.Add(mainPlayer);
            m_playerID2Index[mainPlayer.PlayerID] = m_roomInfo.Players.Count - 1;

            m_Server.Send((byte)MsgType.S2C_UpdateRoomInfo, m_roomInfo,
                NetHelper.GetIPEndPoint(mainPlayer.ClientIP, mainPlayer.ClientPort));
            if (m_roomInfo.Players.Count == m_roomInfo.MaxCount)
                m_roomState = RoomState.WaitingForBattle;

            BroadcastRoomInfo();

            Run();
        }

        async void Run()
        {
            while (true)
            {
                if (m_roomState == RoomState.Disposed)
                    return;
                Tick();
                await Task.Delay(FrameDelta);
            }
        }

        void Tick()
        {
            if (m_Server == null || m_Server.IsDisposed)
                return;
            ExcuteCmd();
            BattleTick();

            m_Server.Update();
        }


        //创建完房间 广播服务器
        private void BroadcastRoomInfo()
        {
            if (m_roomInfo == null)
                return;
            m_Server.Broadcast((byte)MsgType.S2C_RoomInfo, m_roomInfo,
                    m_broadcastMinPort,m_broadcastMaxPort);
        }


        //向服务器push指令 会在异步调用
        public void PushCommand(ServerCmdInfo info)
        {
            lock (m_cmdQueue)
            {
                m_cmdQueue.Enqueue(info);
            }
        }

        void CmdHandler(ServerCmdInfo cmd) 
        {
            switch (cmd.type)
            {
                case SERVER_CMD_TYPE.SEND_ROOMINFO:
                    PlayerInfo player = cmd.obj as PlayerInfo;
                    if (player != null && m_roomInfo != null)
                    {
                        m_Server.Send((byte)MsgType.S2C_RoomInfo, m_roomInfo
                            , NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
                    }
                    break;
            }
        }

        //解析主线程push过来的指令
        void ExcuteCmd() 
        {
            if (m_cmdQueue.Count <= 0)
                return;
            lock (m_cmdQueue)
            {
                while (m_cmdQueue.Count > 0)
                {
                    var cmd = m_cmdQueue.Dequeue();
                    CmdHandler(cmd);
                }
            }
        }


        int GetPlayerIndex(PlayerInfo player)
        {
            if (m_roomInfo == null)
                return -1;
            //for (int i = 0; i < m_roomInfo.Players.Count; i++)
            //{
            //    var p = m_roomInfo.Players[i];
            //    if (p.ClientIP == player.ClientIP && p.ClientPort == player.ClientPort)
            //        return i;
            //}
            if (m_playerID2Index.ContainsKey(player.PlayerID))
                return m_playerID2Index[player.PlayerID];
            return -1;
        }
        int GetPlayerIndex(int playerid)
        {
            //for (int i = 0; i < m_roomInfo.Players.Count; i++)
            //{
            //    var p = m_roomInfo.Players[i];
            //    if (p.ClientIP == player.ClientIP && p.ClientPort == player.ClientPort)
            //        return i;
            //}
            if (m_playerID2Index.ContainsKey(playerid))
                return m_playerID2Index[playerid];
            return -1;
        }

        bool IsInRoom(PlayerInfo player,out int index) 
        {
            index = -1;
            if (m_roomInfo == null)
                return false;
            if (player == null)
                return false;
            index = GetPlayerIndex(player);
            if (index < 0)
                return false;
            return true;
        }

        //关闭房间
        public void CloseRoom()
        {
            Broadcast(MsgType.S2C_CloseRoom, m_roomInfo);
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
            if (m_roomState == RoomState.Disposed)
                return;
            m_roomState = RoomState.Disposed;
            m_Server.Dispose();
            m_Server = null;
            m_roomInfo = null;
            m_readyPlayer.Clear();
            m_playerID2Index.Clear();
            m_readyPlayer = null;
        }

        #region 开始战斗

        private long m_startTime;
        private int m_Tick;
        private int m_tickSinceGameStart =>
            (int)((LTime.realtimeSinceStartupMS - m_startTime) / FrameDelta);

        void StartBattle() 
        {
            m_roomState = RoomState.InBattle;
            m_startTime = LTime.realtimeSinceStartupMS;
            m_Tick = 0;
        }
        //战斗跑帧
        void BattleTick() 
        {
            if (m_roomState != RoomState.InBattle)
                return;
            while (m_Tick < m_tickSinceGameStart) 
            {
                var frame = GetOrCreateFrame(m_Tick);
                Broadcast(MsgType.S2C_Msg_FrameInfo, frame);
                m_Tick++;
            }
        }
        #endregion



        //客户端请求加入房间
        void OnC2S_ReqJoinRoom(PlayerInfo player)
        {
            if (player == null || m_roomInfo == null)
                return;
            if (m_roomState != RoomState.WaitingForPlayer)
                return;
            int index = GetPlayerIndex(player);
            if (index >= 0)
                return;
            player.PlayerID = m_curPlayerID++;
            m_roomInfo.Players.Add(player);
            m_playerID2Index[player.PlayerID] = m_roomInfo.Players.Count - 1;

            Broadcast(MsgType.S2C_UpdateRoomInfo, m_roomInfo);

            if (m_roomInfo.Players.Count == m_roomInfo.MaxCount)
                m_roomState = RoomState.WaitingForBattle;
        }

        //客户端请求退出房间
        void OnC2S_ReqExitRoom(PlayerInfo player)
        {
            if (!IsInRoom(player,out int index))
                return;

            if (IsMainPlayer(player))
            {
                CloseRoom();
                return;
            }

            m_roomInfo.Players.RemoveAt(index);
            m_playerID2Index.Remove(player.PlayerID);

            m_Server.Send((byte)MsgType.S2C_ExitRoom, m_roomInfo,
                NetHelper.GetIPEndPoint(player.ClientIP, player.ClientPort));
            Broadcast(MsgType.S2C_UpdateRoomInfo, m_roomInfo);

            if (m_roomInfo.Players.Count < m_roomInfo.MaxCount)
                m_roomState = RoomState.WaitingForPlayer;
        }

        //请求开始游戏
        void On_C2S_ReqStartGame(PlayerInfo player) 
        {
            if (!IsInRoom(player, out int index))
                return;
            if (m_roomState != RoomState.WaitingForBattle)
                return;
            Broadcast(MsgType.S2C_StartGame, m_roomInfo);
        }

        //客户端准备完成
        void On_C2S_ClientReady(PlayerInfo player) 
        {
            if (!IsInRoom(player, out int index))
                return;
            for (int i = 0; i < m_readyPlayer.Count; i++)
            {
                var p = m_readyPlayer[i];
                if (p.ClientIP == player.ClientIP && p.ClientPort == player.ClientPort)
                    return;
            }

            m_readyPlayer.Add(player);
            if (m_readyPlayer.Count == m_roomInfo.MaxCount) 
            {
                m_readyPlayer.Clear();
                StartBattle();
            }
        }

        Msg_FrameInfo GetOrCreateFrame(int tick) 
        {
            var frameCount = m_allHistoryFrames.Count;
            if (frameCount <= tick) 
            {
                var count = tick - frameCount + 1;
                for (int i = 0; i < count; i++)
                {
                    m_allHistoryFrames.Add(null);
                }
            }

            if (m_allHistoryFrames[tick] == null) 
            {
                m_allHistoryFrames[tick] = new Msg_FrameInfo() { Tick = tick };
            }

            var frame = m_allHistoryFrames[tick];
            if (frame.Inputs == null) 
            {
                frame.Inputs = new Msg_PlayerInput[m_MaxPlayerCount];
            }
            return frame;
        }

        //收到客户端操作
        void On_C2S_PlayerInput(Msg_PlayerInput input) 
        {
            if (m_roomState != RoomState.InBattle)
                return;
            if (input.Tick < m_Tick)
                return;
            int index = GetPlayerIndex(input.PlayerID);
            if (index < 0)
                return;

            var frame = GetOrCreateFrame(input.Tick);
            frame.Inputs[index] = input;
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
                case MsgType.C2S_ReqStartGame://请求开始游戏
                    On_C2S_ReqStartGame(message as PlayerInfo);
                    break;
                case MsgType.C2S_ClientReady://客户端发送准备完毕
                    On_C2S_ClientReady(message as PlayerInfo);
                    break;
                case MsgType.C2S_PlayerInput://收到客户端操作
                    On_C2S_PlayerInput(message as Msg_PlayerInput);
                    break;
            }
        }
    }
}

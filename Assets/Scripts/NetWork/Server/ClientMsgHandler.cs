using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

public class ClientMsgHandler : IMessageDispatcher
{
    public delegate void GlobalNetMsgHandler(MsgType type, object msg);

    private static ClientMsgHandler m_Instance;
    public static ClientMsgHandler Instance 
    {
        get 
        {
            if(m_Instance == null)
                m_Instance = new ClientMsgHandler();
            return m_Instance;
        }
    }

    public void Dispatch(Session session, byte opcode, object message)
    {
        MsgType opType = (MsgType)opcode;
        switch (opType)
        {
            case MsgType.C2S_ReqRoomInfo:
                C2S_Local msg = message as C2S_Local;
                DebugService.Instance.LogError(msg.ToString());
                break;
        }
    }

    private struct ListenerInfo
    {
        public bool isRegister;//是否是在注册 否则是移除
        public EEvent type;
        public GlobalNetMsgHandler param;
    }

    private Dictionary<int,List<GlobalNetMsgHandler>> m_allNetListener = new Dictionary<int, List<GlobalNetMsgHandler>>();
    private bool isTrigging = false;

    public void RegiserNetHandle(MsgType type,GlobalNetMsgHandler handler) 
    {

    }
}

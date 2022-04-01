using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

public class ClientMsgHandler : BaseEventHandle<MsgType, ClientMsgHandler.GlobalNetMsgHandler>, IMessageDispatcher
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

        if(message != null)
            Trigger(opType, message);
    }
}

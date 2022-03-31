using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

public class ClientMsgHandler : IMessageDispatcher
{
    public void Dispatch(Session session, byte opcode, object message)
    {
        MsgType opType = (MsgType)opcode;
        switch (opType)
        {
            case MsgType.C2S_ReqRoomInfo:
                C2S_Local msg = message as C2S_Local;
                UnityEngine.Debug.LogError(msg.ToString());
                break;
        }
    }
}

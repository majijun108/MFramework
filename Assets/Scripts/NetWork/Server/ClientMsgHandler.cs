using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;

public class ClientMsgHandler : IMessageDispatcher
{
    public void Dispatch(Session session, byte opcode, IMessage message)
    {
        MsgType opType = (MsgType)opcode;
        switch (opType)
        {
            case MsgType.C2S_ReqRoomInfo:
                UnityEngine.Debug.LogError(message.ToString());
                break;
        }
    }
}

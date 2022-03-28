using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;

public class ClientMsgHandler : IMessageDispatcher
{
    public void Dispatch(Session session, byte opcode, IMessage message)
    {
        throw new NotImplementedException();
    }
}

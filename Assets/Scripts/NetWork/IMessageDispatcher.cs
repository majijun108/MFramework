using System;
using System.Collections.Generic;

namespace Lockstep.NetWork
{
    public interface IMessageDispatcher
    {
        void Dispatch(Session session,byte opcode,Google.Protobuf.IMessage message);
    }
}

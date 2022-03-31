using System;
using System.Collections.Generic;

namespace Lockstep.NetWork
{
    public interface IMessagePacker
    {
        object DeserializeFrom(byte opcode, byte[] bytes,int startIndex,int count);
        byte[] SerializeToByteArray(object msg);
    }
}

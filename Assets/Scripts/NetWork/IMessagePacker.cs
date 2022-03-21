using System;
using System.Collections.Generic;

namespace Lockstep.NetWork
{
    public interface IMessagePacker
    {
        Google.Protobuf.IMessage DeserializeFrom(byte[] bytes,int startIndex,int count);
        byte[] SerializeToByteArray(Google.Protobuf.IMessage msg);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Lockstep.NetWork;

//消息解包和打包
public class MessagePacker : IMessagePacker
{
    private static MessagePacker _messagePacker;
    public static MessagePacker Instance 
    {
        get 
        {
            if(_messagePacker == null)
                _messagePacker = new MessagePacker();
            return _messagePacker;
        }
    }
    public IMessage DeserializeFrom(byte[] bytes, int startIndex, int count)
    {
        throw new NotImplementedException();
    }

    public byte[] SerializeToByteArray(IMessage msg)
    {
        throw new NotImplementedException();
    }
}

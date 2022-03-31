using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;

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
    public object DeserializeFrom(byte opcode, byte[] bytes, int startIndex, int count)
    {
        MsgType code = (MsgType)opcode;
        switch (code) 
        {
            case MsgType.C2S_ReqRoomInfo:
                return C2S_Local.Parser.ParseFrom(bytes, startIndex, count);
        }
        return null;
    }

    public byte[] SerializeToByteArray(object msg)
    {
        var newmsg = msg as Google.Protobuf.IMessage;
        return newmsg.ToByteArray();
    }
}

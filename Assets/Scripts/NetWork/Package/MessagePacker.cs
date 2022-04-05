using System;
using System.Collections.Generic;
using Google.Protobuf;
using Lockstep.NetWork;
using ServerMessage;
using MessageProto = Google.Protobuf.IMessage;

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

    //注意解包和拆包是放到另外线程执行的
    public object DeserializeFrom(byte opcode, byte[] bytes, int startIndex, int count)
    {
        MsgType code = (MsgType)opcode;
        switch (code) 
        {
            case MsgType.S2C_RoomInfo:
            case MsgType.S2C_JoinRoom:
            case MsgType.S2C_ExitRoom:
                return RoomInfo.Parser.ParseFrom(bytes, startIndex, count);
            case MsgType.C2S_ReqRoomInfo:
            case MsgType.C2S_ReqJoinRoom:
            case MsgType.C2S_ReqExitRoom:
                return PlayerInfo.Parser.ParseFrom(bytes, startIndex, count);
        }
        return null;
    }

    public byte[] SerializeToByteArray(object msg)
    {
        if (msg == null)
            return new byte[0];
        var newmsg = msg as MessageProto;
        return newmsg.ToByteArray();
    }
}

using System;
using System.Collections.Generic;
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

    //注意解包和拆包是放到另外线程执行的
    public object DeserializeFrom(byte opcode, byte[] bytes, int startIndex, int count)
    {
        MsgType code = (MsgType)opcode;
        switch (code) 
        {
            case MsgType.S2C_RoomInfo:
            case MsgType.S2C_CloseRoom:
            case MsgType.S2C_ExitRoom:
            case MsgType.S2C_StartGame:
            case MsgType.S2C_UpdateRoomInfo:
                return BaseFormater.FromBytes<RoomInfo>(bytes, startIndex, count);
            case MsgType.C2S_ReqRoomInfo:
            case MsgType.C2S_ReqJoinRoom:
            case MsgType.C2S_ReqExitRoom:
            case MsgType.C2S_ReqStartGame:
            case MsgType.C2S_ClientReady:
                return BaseFormater.FromBytes<PlayerInfo>(bytes,startIndex, count);
            case MsgType.S2C_Msg_FrameInfo:
                return BaseFormater.FromBytes<Msg_FrameInfo>(bytes, startIndex, count);
            case MsgType.C2S_PlayerInput:
                return BaseFormater.FromBytes<Msg_PlayerInput>(bytes, startIndex, count);
        }
        return null;
    }

    public byte[] SerializeToByteArray(byte opCode,object msg)
    {
        if (msg == null)
            return new byte[0];
        MsgType code = (MsgType)opCode;
        switch (code)
        {
            case MsgType.S2C_RoomInfo:
            case MsgType.S2C_CloseRoom:
            case MsgType.S2C_ExitRoom:
            case MsgType.S2C_StartGame:
            case MsgType.S2C_UpdateRoomInfo:
                return (msg as RoomInfo).ToBytes();
            case MsgType.C2S_ReqRoomInfo:
            case MsgType.C2S_ReqJoinRoom:
            case MsgType.C2S_ReqExitRoom:
            case MsgType.C2S_ReqStartGame:
            case MsgType.C2S_ClientReady:
                return (msg as PlayerInfo).ToBytes();
            case MsgType.S2C_Msg_FrameInfo:
                return (msg as Msg_FrameInfo).ToBytes();
            case MsgType.C2S_PlayerInput:
                return (msg as Msg_PlayerInput).ToBytes();
        }
        return null;
    }
}

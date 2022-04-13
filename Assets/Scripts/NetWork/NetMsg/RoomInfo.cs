using System;
using System.Collections.Generic;

public class RoomInfo : BaseFormater
{
    public string ServerIP;
    public int ServerPort;
    public string RoomName;
    public int MaxCount;
    public List<PlayerInfo> Players = new List<PlayerInfo>();
    public int RoomID;
    public override void Deserialize(Deserializer reader)
    {
        ServerIP = reader.ReadString();
        ServerPort = reader.ReadInt32();
        RoomName = reader.ReadString();
        MaxCount = reader.ReadInt32();
        Players = reader.ReadList(Players);
        if(Players == null)// 代码中没有空判断的补救 以前用的protobuff
            Players = new List<PlayerInfo>();
        RoomID = reader.ReadInt32();
    }

    public override void Serialize(Serializer writer)
    {
        writer.Write(ServerIP);
        writer.Write(ServerPort);
        writer.Write(RoomName);
        writer.Write(MaxCount);
        writer.Write(Players);
        writer.Write(RoomID);
    }
}

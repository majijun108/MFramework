using System;
using System.Collections.Generic;

public class PlayerInfo : BaseFormater
{
    public int PlayerID;
    public string ClientIP;
    public int ClientPort;
    public string PlayerName;

    public override void Deserialize(Deserializer reader)
    {
        PlayerID = reader.ReadInt32();
        ClientIP = reader.ReadString();
        ClientPort = reader.ReadInt32();
        PlayerName = reader.ReadString();
    }

    public override void Serialize(Serializer writer)
    {
        writer.Write(PlayerID);
        writer.Write(ClientIP);
        writer.Write(ClientPort);
        writer.Write(PlayerName);
    }
}

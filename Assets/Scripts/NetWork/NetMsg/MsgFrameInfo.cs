using Lockstep.Math;
using System;
using System.Collections.Generic;

public class Msg_MultiFrameInfo : BaseFormater
{
    public int StartTick;
    public Msg_FrameInfo[] Frames;

    public override void Deserialize(Deserializer reader)
    {
        StartTick = reader.ReadInt32();
        Frames = reader.ReadArray(Frames);
    }

    public override void Serialize(Serializer writer)
    {
        writer.Write(StartTick);
        writer.Write(Frames);
    }
}

public class Msg_FrameInfo: BaseFormater
{
    public int Tick;
    public Msg_PlayerInput[] Inputs;

    public override void Deserialize(Deserializer reader)
    {
        Tick = reader.ReadInt32();
        Inputs = reader.ReadArray(Inputs);
    }

    public override void Serialize(Serializer writer)
    {
        writer.Write(Tick);
        writer.Write(Inputs);
    }

    public bool Equals(Msg_FrameInfo other) 
    {
        if (other.Tick != Tick)
            return false;
        return false;//TODO
    }
}

public class Msg_PlayerInput : BaseFormater
{
    public int Tick;
    public int PlayerID;
    public int SkillID;
    public LFloat MoveAngle;//移动的角度

    public override void Deserialize(Deserializer reader)
    {
        Tick = reader.ReadInt32();
        PlayerID = reader.ReadInt32();
        SkillID = reader.ReadInt32();
        MoveAngle = reader.ReadLFloat();
    }

    public override void Serialize(Serializer writer)
    {
        writer.Write(Tick);
        writer.Write(PlayerID);
        writer.Write(SkillID);
        writer.Write(MoveAngle);
    }

    public void Reset() 
    {
        Tick=0;
        PlayerID=-1;
        SkillID=-1;
        MoveAngle=LFloat.zero;
    }
}

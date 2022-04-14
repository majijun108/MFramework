using Lockstep.Math;
using System;
using System.Collections.Generic;

public class Msg_FrameInfo : BaseFormater
{
    public int Tick;
    public Msg_PlayerInput[] Inputs;

    public override void Deserialize(Deserializer reader)
    {
        
    }

    public override void Serialize(Serializer writer)
    {
        
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
        
    }

    public override void Serialize(Serializer writer)
    {
        
    }
}

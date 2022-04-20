using System;
using System.Collections.Generic;
using Lockstep.Math;

public class LTransform:BaseComponent
{
    public LVector2 Pos = LVector2.zero;
    public LFloat Y;
    public LFloat Angle;
    public LVector3 Scale;

    //TEST
    LVector2 GetDir(LFloat angle) 
    {
        if(angle < 0)
            return LVector2.zero;
        if (angle >= 45 && angle <= 135)
            return LVector2.up;
        else if (angle > 135 && angle <= 225)
            return LVector2.left;
        else if (angle > 225 && angle < 315)
            return LVector2.down;
        else
            return LVector2.right;
    }

    public override void DoUpdate(LFloat deltaTime)
    {
        base.DoUpdate(deltaTime);
        if (this.baseEntity is PlayerEntity) 
        {
            var player = this.baseEntity as PlayerEntity;
            if (player.Input == null)
                return;
            LFloat angle = player.Input.MoveAngle;
            LVector2 dir = GetDir(angle);
            Pos += dir.normalized * deltaTime;
        }
    }
    //END TEST
}

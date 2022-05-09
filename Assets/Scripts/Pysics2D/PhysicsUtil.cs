using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

public static class PhysicsUtil
{
    public static LRect GetRect(IShape shape) 
    {
        //TODO
        return LRect.CreateRect(LVector2.zero, LVector2.one);
    }
}

using Lockstep.Math;
using System;
using System.Collections.Generic;

public class TransformComponent : IComponent
{
    public LVector2 Position;
    public int Angle;
    public void OnRecycle()
    {
        
    }

    public void OnReuse()
    {
        
    }
}
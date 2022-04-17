using Lockstep.Math;
using System;
using System.Collections.Generic;

//战斗中的系统
public abstract class BaseSystem:BaseGameService
{
    public bool Enable = true;

    protected World m_World;

    public BaseSystem(World world) { m_World = world; }
    public virtual void DoUpdate(LFloat deltaTime) { }
}

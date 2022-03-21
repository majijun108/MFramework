using Lockstep.Math;
using System;
using System.Collections.Generic;

//战斗中的系统
public class BaseSystem:BaseGameService
{
    public bool Enable = true;
    public virtual void DoUpdate(LFloat deltaTime) { }
}

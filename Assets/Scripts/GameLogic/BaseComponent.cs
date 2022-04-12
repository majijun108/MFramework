using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface IComponent { }

public abstract class BaseComponent:IRecyclable,IComponent
{
    public BaseEntity baseEntity { get; private set; }

    public virtual void BindEntity(BaseEntity entity) 
    {
        this.baseEntity = entity;
    }

    public virtual void DoAwake() { }
    public virtual void DoStart() { }
    public virtual void DoUpdate(LFloat deltaTime) { }
    public virtual void DoDestroy() { }

    public virtual void OnReuse(){ }

    public virtual void OnRecycle(){ }
}

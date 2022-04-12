using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface IEntity { }

public abstract class BaseEntity :IRecyclable,IEntity
{
    protected List<BaseComponent> m_allComponents;

    protected void RegisterComponent(BaseComponent component) 
    {
        if(m_allComponents == null)
            m_allComponents = new List<BaseComponent>();
        m_allComponents.Add(component);
        component.BindEntity(this);
    }

    protected void RegisterComponent<T>() where T : BaseComponent,new()
    {
        var comp = ObjectPool.Get<T>();
        RegisterComponent(comp);
    }

    public virtual void DoAwake() 
    {
        if (m_allComponents == null)
            return;
        for (int i = 0; i < m_allComponents.Count; i++)
        {
            m_allComponents[i].DoAwake();
        }
    }

    public virtual void DoStart() 
    {
        if (m_allComponents == null)
            return;
        for (int i = 0; i < m_allComponents.Count; i++)
        {
            m_allComponents[i].DoStart();
        }
    }

    public virtual void DoUpdate(LFloat deltaTime) 
    {
        if (m_allComponents == null)
            return;
        for (int i = 0; i < m_allComponents.Count; i++)
        {
            m_allComponents[i].DoUpdate(deltaTime);
        }
    }

    public virtual void DoDestroy() 
    {
        if (m_allComponents == null)
            return;
        for (int i = 0; i < m_allComponents.Count; i++)
        {
            var comp = m_allComponents[i];
            comp.DoDestroy();
            ObjectPool.Return(comp);
        }
        m_allComponents.Clear();
        m_allComponents = null;
    }


    public void OnReuse()
    {
        
    }

    public void OnRecycle()
    {
        
    }
}

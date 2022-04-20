using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface IEntity 
{
}

public abstract class BaseEntity :IRecyclable,IEntity
{
    protected List<BaseComponent> m_allComponents;
    protected Dictionary<Type,BaseComponent> m_type2Components;

    public LTransform transform;

    protected World m_World { get; set; }

    public int ID { get; private set; }

    protected void RegisterComponent(BaseComponent component) 
    {
        if (m_allComponents == null)
        {
            m_allComponents = new List<BaseComponent>();
            m_type2Components = new Dictionary<Type,BaseComponent>();
        }
        var type = component.GetType();
        if(m_type2Components.ContainsKey(type)) 
        {
            return;
        }
        m_allComponents.Add(component);
        m_type2Components[type] = component;
        component.BindEntity(this);
    }

    protected T RegisterComponent<T>() where T : BaseComponent,new()
    {
        var comp = ObjectPool.Get<T>();
        RegisterComponent(comp);
        return comp;
    }

    public BaseEntity() 
    {
        transform = RegisterComponent<LTransform>();
    }

    public void BindWorld(World world, int id) 
    {
        m_World = world;
        ID = id;
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

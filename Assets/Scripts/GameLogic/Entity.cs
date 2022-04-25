using Lockstep.Math;
using System;
using System.Collections.Generic;

public delegate void EntityComponentChanged(IEntity entity,int index,IComponent component);
public delegate void EntityComponentReplaced(IEntity entity,int index,IComponent preComponent,IComponent newComponent);
public delegate void EntityEnvent(IEntity entity);

public interface IEntity 
{
    event EntityComponentChanged OnComponentAdded;
    event EntityComponentChanged OnComponentRemoved;
    event EntityComponentReplaced OnComponentReplaced;
    event EntityEnvent OnEntityReleased;
    event EntityEnvent OnEntityDestroyed;
    int ID { get; }
    bool IsEnable { get; }

    void Initialize(int id, int totalComponents);

    bool HasComponent(int index);

    void AddComponent(int index,IComponent component);
    void RemoveComponent(int index);
    void ReplaceComponent(int index,IComponent component);
    void RemoveAllComponents();
    void InternalDestroy();
    void Destroy();
}

public class Entity : IRecyclable, IEntity
{

    public event EntityComponentChanged OnComponentAdded;
    public event EntityComponentChanged OnComponentRemoved;
    public event EntityComponentReplaced OnComponentReplaced;
    public event EntityEnvent OnEntityReleased;
    public event EntityEnvent OnEntityDestroyed;

    protected World m_World { get; set; }

    public int ID { get; private set; }

    public bool IsEnable { get; private set; }

    IComponent[] m_components;
    int m_totalComponents;
    IComponent[] m_componentCache;

    public void Initialize(int id, int totalComponents)
    {
        this.ID = id;
        this.m_components = new IComponent[totalComponents];
        m_totalComponents = totalComponents;
        IsEnable = true;
    }

    public bool HasComponent(int index)
    {
        return m_components[index] != null;
    }

    public void OnReuse()
    { 
    }

    public void OnRecycle()
    {  
    }

    public void AddComponent(int index, IComponent component)
    {
        if (!IsEnable)
            return;
        if (HasComponent(index))
            return;
        m_components[index] = component;
        m_componentCache = null;
        if (OnComponentAdded != null) 
        {
            OnComponentAdded(this,index,component);
        }
    }

    public void RemoveComponent(int index)
    {
        if (!IsEnable)
            return;
        if (!HasComponent(index))
            return;
        replaceComponent(index, null);
    }

    public void ReplaceComponent(int index, IComponent component)
    {
        if (!IsEnable)
            return;
        if (HasComponent(index))
        {
            replaceComponent(index, component);
        }
        else if(component != null)
        {
            AddComponent(index, component);
        }
    }

    void replaceComponent(int index, IComponent placeComponent) 
    {
        var preComponent = m_components[index];
        if (placeComponent != preComponent)
        {
            m_components[index] = placeComponent;
            m_componentCache = null;
            if (placeComponent != null)
            {
                OnComponentReplaced?.Invoke(this, index, preComponent, placeComponent);
            }
            else
            {
                OnComponentRemoved?.Invoke(this, index, preComponent);
            }
            ObjectPool.Return(preComponent);
        }
        else 
        {
            OnComponentReplaced?.Invoke(this, index, preComponent, placeComponent);
        }
    }

    public void RemoveAllComponents()
    {
        for (int i = 0; i < m_components.Length; i++)
        {
            if(m_components[i] != null)
                replaceComponent(i,null);
        }
    }

    public void InternalDestroy()
    {
        IsEnable = false;
        RemoveAllComponents();
        OnComponentAdded = null;
        OnComponentRemoved = null;
        OnComponentReplaced = null;
        OnEntityDestroyed = null;
    }

    public void Destroy()
    {
        if (!IsEnable)
            return;
        OnEntityDestroyed?.Invoke(this);
    }
}

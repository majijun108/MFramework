using System;
using System.Collections.Generic;


public interface IGroup 
{
    void HandleEntity(IEntity entity);
    void UpdateEntity(IEntity entity,int index,IComponent preComponent,IComponent newComponent);
    bool ContainsEntity(IEntity entity);
    IEntity[] GetEntities();
}

public class EntityCompareer : IEqualityComparer<IEntity>
{
    public static readonly IEqualityComparer<IEntity> comparer = new EntityCompareer();
    public bool Equals(IEntity x, IEntity y)
    {
        return x == y;
    }

    public int GetHashCode(IEntity obj)
    {
        return obj.ID;
    }
}

public class EntityGroup:IGroup
{
    private IMatcher m_Matcher;
    private HashSet<IEntity> m_entities = new HashSet<IEntity>(EntityCompareer.comparer);
    private IEntity[] m_entitiesCache;

    public EntityGroup(IMatcher matcher) 
    {
        m_Matcher = matcher;
    }

    public bool ContainsEntity(IEntity entity)
    {
        return m_entities.Contains(entity);
    }

    bool addEntity(IEntity entity) 
    {
        if (entity.IsEnable) 
        {
            var added = m_entities.Add(entity);
            if (added) 
            {
                m_entitiesCache = null;
            }
            return added;
        }
        return false;
    }

    bool removeEntity(IEntity entity) 
    {
        var removed = m_entities.Remove(entity);
        if (removed) 
        {
            m_entitiesCache = null;
        }
        return removed;
    }

    public void HandleEntity(IEntity entity)
    {
        if (m_Matcher.Matches(entity))
        {
            addEntity(entity);
        }
        else 
        {
            removeEntity(entity);
        }
    }

    public void UpdateEntity(IEntity entity, int index, IComponent preComponent, IComponent newComponent)
    {
        
    }

    public IEntity[] GetEntities()
    {
        if (m_entitiesCache == null) 
        {
            m_entitiesCache = new IEntity[m_entities.Count];
            m_entities.CopyTo(m_entitiesCache);
        }
        return m_entitiesCache;
    }
}

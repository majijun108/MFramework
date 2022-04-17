using Lockstep.Math;
using System;
using System.Collections;
using System.Collections.Generic;

public class EntityManager : BaseSystem
{
    public int m_currentID = 0;
    private Dictionary<Type, IList> m_type2Entities = new Dictionary<Type,IList>();
    private Dictionary<int,BaseEntity> m_id2Entity = new Dictionary<int, BaseEntity>();

    public EntityManager(World world) : base(world){ }

    private void AddEntity(BaseEntity entity)
    {
        var type = entity.GetType();
        if (m_type2Entities.TryGetValue(type, out IList entities))
        {
            entities.Add(entity);
        }
        else 
        {
            var list = new List<BaseEntity>();
            m_type2Entities[type] = list;
            list.Add(entity);
        }
        m_id2Entity[entity.ID] = entity;
    }

    private void RemoveEntity(BaseEntity entity) 
    {
        var type = entity.GetType();
        if (m_type2Entities.TryGetValue(type, out IList entities)) 
        {
            entities.Remove(entity);
            m_id2Entity.Remove(entity.ID);
        }
    }

    public T CreateEntity<T>(int prefabID,LVector3 initPos) where T :BaseEntity,new()
    {
        var entity = new T();//Activator.CreateInstance(typeof(T), m_World,m_currentID++);
        entity.BindWorld(m_World, m_currentID++);

        entity.DoAwake();
        entity.DoStart();

        AddEntity(entity);
        return entity;
    }
}

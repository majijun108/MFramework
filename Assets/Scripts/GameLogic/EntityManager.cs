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

    private void AddEntity<T>(T entity) where T : BaseEntity
    {
        var type = entity.GetType();
        if (m_type2Entities.TryGetValue(type, out IList entities))
        {
            entities.Add(entity);
        }
        else 
        {
            var list = new List<T>();
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

        //TEST
        var go = UnityEngine.GameObject.Find("CompleteTank");
        var test = go.AddComponent<CSharpTest>();
        test.BaseEntity = entity;

        AddEntity<T>(entity);
        return entity;
    }

    private List<T> GetEntities<T>()
    {
        var t = typeof(T);
        if (m_type2Entities.TryGetValue(t, out var lstObj))
        {
            return lstObj as List<T>;
        }
        else
        {
            var lst = new List<T>();
            m_type2Entities.Add(t, lst);
            return lst;
        }
    }

    public PlayerEntity[] GetPlayers() 
    {
        return GetEntities<PlayerEntity>().ToArray();
    }
}

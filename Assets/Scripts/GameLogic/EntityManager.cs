﻿using Lockstep.Math;
using System;
using System.Collections;
using System.Collections.Generic;

public class EntityManager
{
    public int m_currentID = 0;

    private HashSet<IEntity> m_entities = new HashSet<IEntity>(EntityCompareer.comparer);
    private IEntity[] m_entitiesCache;

    readonly EntityComponentChanged m_entityComponentChanged;
    readonly EntityComponentReplaced m_entityComponentReplaced;
    readonly EntityEnvent m_entityReleased;
    readonly EntityEnvent m_entityDestroyed;

    private Dictionary<IMatcher,IGroup> m_groups = new Dictionary<IMatcher,IGroup>();
    private List<IGroup>[] m_index2Groups;
    private int m_totalComponents;
    private PhysicsWorld m_physicsWorld;

    public EntityManager(int totalComponents, PhysicsWorld physics)
    {
        m_entityComponentChanged = onEntityComponentChanged;
        m_entityComponentReplaced = onEntityComponentReplaced;
        m_entityReleased = onEntityReleased;
        m_entityDestroyed = onEntityDestroyed;

        m_totalComponents = totalComponents;
        m_index2Groups = new List<IGroup>[totalComponents];
        m_physicsWorld = physics;
    }

    public Entity CreateEntity()
    {
        var entity = ObjectPool.Get<Entity>();
        entity.Initialize(m_currentID++, m_totalComponents);
        m_entities.Add(entity);
        m_entitiesCache = null;

        entity.OnComponentAdded += m_entityComponentChanged;
        entity.OnComponentRemoved += m_entityComponentChanged;
        entity.OnComponentReplaced += m_entityComponentReplaced;
        entity.OnEntityReleased += m_entityReleased;
        entity.OnEntityDestroyed += m_entityDestroyed;

        return entity;
    }

    public IGroup GetGroup(IMatcher matcher) 
    {
        IGroup group;
        if (!m_groups.TryGetValue(matcher, out group)) 
        {
            group = new EntityGroup(matcher);
            var entities = GetEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                group.HandleEntity(entities[i]);
            }

            m_groups.Add(matcher,group);

            for (int i = 0; i < matcher.Indices.Length; i++)
            {
                var index = matcher.Indices[i];
                if (m_index2Groups[index] == null) 
                {
                    m_index2Groups[index] = new List<IGroup>();
                }
                m_index2Groups[index].Add(group);
            }
        }
        return group;
    }


    IEntity[] GetEntities() 
    {
        if (m_entitiesCache == null) 
        {
            m_entitiesCache = new Entity[m_entities.Count];
            m_entities.CopyTo(m_entitiesCache);
        }
        return m_entitiesCache;
    }

    void onEntityComponentChanged(IEntity entity, int index, IComponent component) 
    {
        var groups = m_index2Groups[index];
        if (groups == null)
            return;
        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].HandleEntity(entity);
        }
    }

    void onEntityComponentReplaced(IEntity entity, int index, IComponent preComponent, IComponent placeComponent) 
    {
        var groups = m_index2Groups[index];
        if (groups == null)
            return;
        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].UpdateEntity(entity,index,preComponent,placeComponent);
        }
    }

    void onEntityReleased(IEntity entity) 
    {

    }

    void onEntityDestroyed(IEntity entity) 
    {
        var removed = m_entities.Remove(entity);
        if (!removed)
            return;

        m_entitiesCache = null;
        entity.InternalDestroy();
        ObjectPool.Return(entity);
    }

    public T GetEntityComponent<T>(IEntity entity) where T : IComponent 
    {
        return entity.GetComponent<T>(ComponentRegister.GetComponentIndex<T>());
    }

    public T AddComponent<T>(IEntity entity) where T : IComponent,new()
    {
        var component = ObjectPool.Get<T>();
        var index = ComponentRegister.GetComponentIndex<T>();
        entity.AddComponent(index, component);
        return component;
    }
}

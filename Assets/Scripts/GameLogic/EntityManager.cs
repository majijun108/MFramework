using Lockstep.Math;
using System;
using System.Collections;
using System.Collections.Generic;

public class EntityManager
{
    public int m_currentID = 0;

    private HashSet<IEntity> m_entities = new HashSet<IEntity>(EntityCompareer.comparer);

    readonly EntityComponentChanged m_entityComponentChanged;
    readonly EntityComponentReplaced m_entityComponentReplaced;
    readonly EntityEnvent m_entityReleased;
    readonly EntityEnvent m_entityDestroyed;

    private Dictionary<IMatcher,IGroup> m_groups = new Dictionary<IMatcher,IGroup>();
    private List<IGroup>[] m_index2Groups;
    private int m_totalComponents;

    public EntityManager(int totalComponents)
    {
        m_entityComponentChanged = onEntityComponentChanged;
        m_entityComponentReplaced = onEntityComponentReplaced;
        m_entityReleased = onEntityReleased;
        m_entityDestroyed = onEntityDestroyed;

        m_totalComponents = totalComponents;
        m_index2Groups = new List<IGroup>[totalComponents];
    }

    public Entity CreateEntity()
    {
        var entity = ObjectPool.Get<Entity>();
        entity.Initialize(m_currentID++, m_totalComponents);
        m_entities.Add(entity);

        entity.OnComponentAdded += m_entityComponentChanged;
        entity.OnComponentRemoved += m_entityComponentChanged;
        entity.OnComponentReplaced += m_entityComponentReplaced;
        entity.OnEntityReleased += m_entityReleased;
        entity.OnEntityDestroyed += m_entityDestroyed;

        return entity;
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

        entity.InternalDestroy();
        ObjectPool.Return(entity);
    }
}

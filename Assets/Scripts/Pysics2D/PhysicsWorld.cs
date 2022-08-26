using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

public class PhysicsWorld
{
    private BoundsQuadTree m_boundsQuadTree;
    private Dictionary<int,ColliderProxy> m_id2Collider = new Dictionary<int, ColliderProxy>();
    private LogicWorld _mLogicWorld;

    public PhysicsWorld(LogicWorld logicWorld,LFloat initSize,LVector2 initPos, LFloat minNodeSize, LFloat loosenessVal) 
    {
        _mLogicWorld = logicWorld;
        m_boundsQuadTree = new BoundsQuadTree(initSize, initPos, minNodeSize, loosenessVal);
        m_id2Collider.Clear();
    }

    public bool IsValide(IEntity entity) 
    {
        return entity != null && entity.HasComponent(ComponentRegister.PhysicsIndex)
            && entity.HasComponent(ComponentRegister.TransformIndex);
    }

    public void UpdateObj(IEntity entity) 
    {
        if (!IsValide(entity)) 
        {
            DebugService.Instance.LogError("unvalide entity for phsics");
            return;
        }
        int entityID = entity.ID;
        if (!m_id2Collider.ContainsKey(entityID)) 
        {
            AddObj(entity);
            return;
        }

        PhysicsComponent physics = _mLogicWorld.EntityMgr.GetEntityComponent<PhysicsComponent>(entity);
        TransformComponent trans = _mLogicWorld.EntityMgr.GetEntityComponent<TransformComponent>(entity);
        LRect bound = PhysicsUtil.GetRect(physics.Shape, trans.Position, trans.Angle);

        ColliderProxy proxy = m_id2Collider[entityID];
        proxy.Bounds = bound;
        proxy.PhysicsBody = physics;
        proxy.Entity = entity;
        proxy.Transform = trans;
        m_boundsQuadTree.UpdateObj(proxy,bound);
    }

    public void AddObj(IEntity entity) 
    {
        if (!IsValide(entity))
        {
            DebugService.Instance.LogError("unvalide entity for phsics");
            return;
        }
        int entityID = entity.ID;
        if (m_id2Collider.ContainsKey(entityID)) 
        {
            UpdateObj(entity);
            return;
        }

        PhysicsComponent physics = _mLogicWorld.EntityMgr.GetEntityComponent<PhysicsComponent>(entity);
        TransformComponent trans = _mLogicWorld.EntityMgr.GetEntityComponent<TransformComponent>(entity);
        LRect bound = PhysicsUtil.GetRect(physics.Shape, trans.Position, trans.Angle);

        ColliderProxy proxy = ObjectPool.Get<ColliderProxy>();
        proxy.Bounds = bound;
        proxy.PhysicsBody = physics;
        proxy.Entity = entity;
        proxy.Transform = trans;

        m_id2Collider[entityID] = proxy;
        m_boundsQuadTree.Add(proxy,bound);
    }

    public void RemoveObj(int entityID) 
    {
        if (m_id2Collider.TryGetValue(entityID,out ColliderProxy proxy)) 
        {
            m_boundsQuadTree.Remove(proxy);
            m_id2Collider.Remove(entityID);
            ObjectPool.Return(proxy);
        }
    }

    public void DoDestroy() 
    {
        foreach (var item in m_id2Collider)
        {
            RemoveObj(item.Key);
        }
        m_id2Collider.Clear();
    }

    private List<ColliderProxy> m_collidingList = new List<ColliderProxy>();
    public void GetColliding(IShape shape,LVector2 pos,int angle) 
    {
        m_collidingList.Clear();
        LRect checkBounds = PhysicsUtil.GetRect(shape, pos, angle);
        m_boundsQuadTree.GetColliding(m_collidingList, checkBounds);
        for (int i = 0; i < m_collidingList.Count; i++)
        {

        }
    }
}

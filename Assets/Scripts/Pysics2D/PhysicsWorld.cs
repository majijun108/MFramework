using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

public class PhysicsWorld
{
    private BoundsQuadTree m_boundsQuadTree;
    private Dictionary<int,ColliderProxy> m_id2Collider = new Dictionary<int, ColliderProxy>();

    public PhysicsWorld(LFloat initSize,LVector2 initPos, LFloat minNodeSize, LFloat loosenessVal) 
    {
        m_boundsQuadTree = new BoundsQuadTree(initSize, initPos, minNodeSize, loosenessVal);
        m_id2Collider.Clear();
    }

    public void UpdateObj(int entityID,ref LRect bound) 
    {
        if (!m_id2Collider.ContainsKey(entityID)) 
        {
            AddObj(entityID,ref bound);
            return;
        }
        ColliderProxy proxy = m_id2Collider[entityID];
        m_boundsQuadTree.UpdateObj(proxy,bound);
    }

    public void AddObj(int entityID, ref LRect bound) 
    {
        if (m_id2Collider.ContainsKey(entityID)) 
        {
            UpdateObj(entityID,ref bound);
            return;
        }
        ColliderProxy proxy = ObjectPool.Get<ColliderProxy>();
        //TODO
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
}

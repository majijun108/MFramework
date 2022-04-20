using Lockstep.Math;
using System;
using System.Collections.Generic;

public class PlayerSystem : BaseSystem
{
    private EntityManager m_entityManager;

    public PlayerSystem(World world) : base(world)
    {
    }

    public override void DoStart()
    {
        base.DoStart();
        m_entityManager = m_World.GetSystem<EntityManager>();
    }

    public override void Tick(LFloat deltaTime)
    {
        base.Tick(deltaTime);
        foreach (var item in m_entityManager.GetPlayers())
        {
            item.DoUpdate(deltaTime);
        }
    }
}

using Lockstep.Math;
using System;
using System.Collections.Generic;

public class MoveSystem : IInitializeSystem, IExecuteSystem
{
    World m_world;
    IGroup m_entityGroup;

    public MoveSystem(World world) 
    {
        m_world = world;
    }

    public void Initialize()
    {
        m_entityGroup = m_world.EntityMgr.GetGroup(new EntityMatcher(typeof(PositionComponent), typeof(SpeedComponent)));
    }

    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        LFloat deltaTime = m_world.Billboard.FrameDeltaTime;
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var moveSpeed = m_world.EntityMgr.GetEntityComponent<SpeedComponent>(entity);
            var position = m_world.EntityMgr.GetEntityComponent<PositionComponent>(entity);

            position.Position += moveSpeed.MoveSpeed * deltaTime;
        }
    }
}

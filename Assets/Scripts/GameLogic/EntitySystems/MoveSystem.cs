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
        m_entityGroup = m_world.EntityMgr.GetGroup(new EntityMatcher(typeof(TransformComponent), typeof(PhysicsComponent)));
    }

    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        LFloat deltaTime = m_world.Billboard.FrameDeltaTime;
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var transform = m_world.EntityMgr.GetEntityComponent<TransformComponent>(entity);
            var physics = m_world.EntityMgr.GetEntityComponent<PhysicsComponent>(entity);

            physics.DeltaPosition = physics.Velocity * m_world.Billboard.FrameDeltaTime;
            //TODO 测试
            transform.Position += physics.DeltaPosition;
            transform.Angle = physics.Angle;
        }
    }
}

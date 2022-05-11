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
        m_entityGroup = m_world.EntityMgr.GetGroup(new EntityMatcher(typeof(TransformComponent), typeof(MoveComponent)));
    }

    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        LFloat deltaTime = m_world.Billboard.FrameDeltaTime;
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var transform = m_world.EntityMgr.GetEntityComponent<TransformComponent>(entity);
            var moveCom = m_world.EntityMgr.GetEntityComponent<MoveComponent>(entity);

            moveCom.DeltaPosition = moveCom.Velocity * m_world.Billboard.FrameDeltaTime;
            //TODO 测试
            transform.Position += moveCom.DeltaPosition;
            transform.Angle = moveCom.Angle;
        }
    }
}

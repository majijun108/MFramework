using System;
using System.Collections.Generic;

//物理系统 最后一个更新的系统 计算后把真实位置赋值给transform
public class PhysicsSystem : IInitializeSystem, IExecuteSystem
{
    private World m_world;
    private IGroup m_entityGroup;

    public PhysicsSystem(World world) 
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
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var transform = m_world.EntityMgr.GetEntityComponent<TransformComponent>(entity);
            var physics = m_world.EntityMgr.GetEntityComponent<PhysicsComponent>(entity);

            if (physics.DeltaPosition.x > 0 || physics.DeltaPosition.y > 0 || physics.Angle > 0) 
            {
                UpdatePhysics(entity, physics, transform);
            }
        }
    }

    public void UpdatePhysics(IEntity entity,PhysicsComponent phsics,TransformComponent trans) 
    {

    }
}

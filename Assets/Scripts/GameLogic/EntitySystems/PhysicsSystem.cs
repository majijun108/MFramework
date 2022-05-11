using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
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
        //物理检测只做角色相关的 TODO 改为移动组件相关的
        m_entityGroup = m_world.EntityMgr.GetGroup(new EntityMatcher(typeof(TransformComponent), typeof(PhysicsComponent),typeof(MoveComponent)));
    }
    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var transform = m_world.EntityMgr.GetEntityComponent<TransformComponent>(entity);
            var physics = m_world.EntityMgr.GetEntityComponent<PhysicsComponent>(entity);
            var move = m_world.EntityMgr.GetEntityComponent<MoveComponent>(entity);

            if (move.DeltaPosition.x > 0 || move.DeltaPosition.y > 0 || move.Angle > 0) 
            {
                UpdatePhysics(entity, physics, transform, move);
            }
        }
    }

    private void UpdatePhysics(IEntity entity,PhysicsComponent phsics,TransformComponent trans,MoveComponent move) 
    {

    }

    void GetTargetPos(LVector2 targetPos,int targetAngle,IShape shape) 
    {
        LRect rect = PhysicsUtil.GetRect(shape, targetPos, targetAngle);
    }
}

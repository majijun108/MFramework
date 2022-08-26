using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using System;
using System.Collections.Generic;

//物理系统 最后一个更新的系统 计算后把真实位置赋值给transform
public class PhysicsSystem : IInitializeSystem, IExecuteSystem
{
    private LogicWorld _mLogicWorld;
    private IGroup m_entityGroup;

    public PhysicsSystem(LogicWorld logicWorld) 
    {
        _mLogicWorld = logicWorld;
    }

    public void Initialize()
    {
        //物理检测只做角色相关的 TODO 改为移动组件相关的
        m_entityGroup = _mLogicWorld.EntityMgr.GetGroup(new EntityMatcher(typeof(TransformComponent), typeof(PhysicsComponent),typeof(MoveComponent)));
    }
    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var transform = _mLogicWorld.EntityMgr.GetEntityComponent<TransformComponent>(entity);
            var physics = _mLogicWorld.EntityMgr.GetEntityComponent<PhysicsComponent>(entity);
            var move = _mLogicWorld.EntityMgr.GetEntityComponent<MoveComponent>(entity);

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

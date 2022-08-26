using Lockstep.Math;
using System;
using System.Collections.Generic;

public class MoveSystem : IInitializeSystem, IExecuteSystem
{
    LogicWorld _mLogicWorld;
    IGroup m_entityGroup;

    public MoveSystem(LogicWorld logicWorld) 
    {
        _mLogicWorld = logicWorld;
    }

    public void Initialize()
    {
        m_entityGroup = _mLogicWorld.EntityMgr.GetGroup(new EntityMatcher(typeof(TransformComponent), typeof(MoveComponent)));
    }

    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        LFloat deltaTime = _mLogicWorld.Billboard.FrameDeltaTime;
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var transform = _mLogicWorld.EntityMgr.GetEntityComponent<TransformComponent>(entity);
            var moveCom = _mLogicWorld.EntityMgr.GetEntityComponent<MoveComponent>(entity);

            moveCom.DeltaPosition = moveCom.Velocity * _mLogicWorld.Billboard.FrameDeltaTime;
            //TODO 测试
            transform.Position += moveCom.DeltaPosition;
            transform.Angle = moveCom.Angle;
        }
    }
}

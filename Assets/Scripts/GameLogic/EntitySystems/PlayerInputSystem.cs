using Lockstep.Math;
using System;
using System.Collections.Generic;

public class PlayerInputSystem : IInitializeSystem, IExecuteSystem
{
    LogicWorld _mLogicWorld;
    IGroup m_entityGroup;
    public PlayerInputSystem(LogicWorld logicWorld) 
    {
        _mLogicWorld = logicWorld;
    }
    public void Initialize()
    {
        m_entityGroup = _mLogicWorld.EntityMgr.GetGroup(new EntityMatcher(typeof(PlayerComponent), typeof(MoveComponent)));
    }

    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var playerCom = _mLogicWorld.EntityMgr.GetEntityComponent<PlayerComponent>(entity);
            var input = _mLogicWorld.Billboard.GetPlayerInput(playerCom.PlayerID);
            if (input != null)
            {
                var moveCom = _mLogicWorld.EntityMgr.GetEntityComponent<MoveComponent>(entity);
                moveCom.Angle = input.MoveAngle;
                if (input.MoveAngle == 0)
                {
                    moveCom.Velocity = LVector2.zero;
                }
                else
                {
                    moveCom.Velocity = PhysicsUtil.GetRotateDir(input.MoveAngle, LVector2.right).normalized * 5;
                }
            }
        }
    }
}

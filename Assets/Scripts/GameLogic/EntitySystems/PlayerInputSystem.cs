using Lockstep.Math;
using System;
using System.Collections.Generic;

public class PlayerInputSystem : IInitializeSystem, IExecuteSystem
{
    World m_world;
    IGroup m_entityGroup;
    public PlayerInputSystem(World world) 
    {
        m_world = world;
    }
    public void Initialize()
    {
        m_entityGroup = m_world.EntityMgr.GetGroup(new EntityMatcher(typeof(PlayerComponent), typeof(MoveComponent)));
    }

    public void Execute()
    {
        var entities = m_entityGroup.GetEntities();
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var playerCom = m_world.EntityMgr.GetEntityComponent<PlayerComponent>(entity);
            var input = m_world.Billboard.GetPlayerInput(playerCom.PlayerID);
            if (input != null)
            {
                var moveCom = m_world.EntityMgr.GetEntityComponent<MoveComponent>(entity);
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

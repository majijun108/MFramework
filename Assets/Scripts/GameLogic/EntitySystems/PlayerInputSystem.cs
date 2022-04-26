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
        m_entityGroup = m_world.EntityMgr.GetGroup(new EntityMatcher(typeof(PlayerComponent), typeof(SpeedComponent)));
    }

    LVector2 GetSpeedByAngle(LFloat angle) 
    {
        if (angle == 0)
            return LVector2.zero;
        if (angle > 45 && angle <= 135)
        {
            return LVector2.up;
        }
        else if (angle > 135 && angle <= 225)
        {
            return LVector2.left;
        }
        else if (angle > 225 && angle < 315)
        {
            return LVector2.down;
        }
        else 
        {
            return LVector2.right;
        }
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
                var moveSpeed = m_world.EntityMgr.GetEntityComponent<SpeedComponent>(entity);
                moveSpeed.MoveSpeed = GetSpeedByAngle(input.MoveAngle).normalized * 5;
            }
        }
    }
}

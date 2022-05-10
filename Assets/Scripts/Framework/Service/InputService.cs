using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Math;

public class InputService:BaseSingleService<InputService>,IInputService,IUpdate
{
    public Msg_PlayerInput m_playerInput = new Msg_PlayerInput();

    private bool m_isEnable = false;

    public override void DoAwake(IServiceContainer services)
    {
        base.DoAwake(services);
    }

    public void EnableInput(bool enable) 
    {
        m_isEnable = enable;
    }

    public void DoUpdate(float deltaTime) 
    {
        if (!m_isEnable)
            return;
        m_playerInput.Reset();
        m_playerInput.PlayerID = NetworkService.Instance.LocalPlayerID;

        LVector2 dir = LVector2.zero;
        if (Input.GetKey(KeyCode.W))
            dir.y += LFloat.one;
        if (Input.GetKey(KeyCode.S))
            dir.y -= LFloat.one;
        if (Input.GetKey(KeyCode.D))
            dir.x += LFloat.one;
        if (Input.GetKey(KeyCode.A))
            dir.x -= LFloat.one;

        if(dir != LVector2.zero)
            m_playerInput.MoveAngle = PhysicsUtil.GetRotateAngle(dir, LVector2.right);
    }

    public Msg_PlayerInput GetInput()
    {
        return m_playerInput;
    }
}

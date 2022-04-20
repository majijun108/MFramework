using System;
using System.Collections.Generic;
using UnityEngine;

public class InputService:BaseSingleService<InputService>,IInputService,IUpdate
{
    public Msg_PlayerInput m_playerInput = new Msg_PlayerInput();

    public override void DoAwake(IServiceContainer services)
    {
        base.DoAwake(services);
    }

    public void DoUpdate(float deltaTime) 
    {
        m_playerInput.Reset();
        m_playerInput.PlayerID = NetworkService.Instance.LocalPlayerID;
        if (Input.GetKey(KeyCode.W)) 
        {
            m_playerInput.MoveAngle = new Lockstep.Math.LFloat(90);
        }else if(Input.GetKey(KeyCode.S))
            m_playerInput.MoveAngle = new Lockstep.Math.LFloat(270);
        else if(Input.GetKey(KeyCode.D))
            m_playerInput.MoveAngle = new Lockstep.Math.LFloat(0);
        else if( Input.GetKey(KeyCode.A))
            m_playerInput.MoveAngle = new Lockstep.Math.LFloat(180);
    }

    public Msg_PlayerInput GetInput()
    {
        if(m_playerInput.MoveAngle < 0)
            return null;
        return m_playerInput;
    }
}

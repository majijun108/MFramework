using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface Billboard 
{
    Msg_PlayerInput GetPlayerInput(int playerId);
    LFloat FrameDeltaTime { get; }

    void Reset();
    void SetFrameInfo(Msg_FrameInfo frameInfo);
    void SetFrameDeltaTime(LFloat frameDeltaTime);
}

public class WorldBillboard : Billboard
{
    private Dictionary<int, Msg_PlayerInput> curInput = new Dictionary<int, Msg_PlayerInput>();
    public LFloat FrameDeltaTime { get; private set; }


    public void SetFrameInfo(Msg_FrameInfo frameInfo)
    {
        curInput.Clear();
        for (int i = 0; i < frameInfo.Inputs.Length; i++)
        {
            var input = frameInfo.Inputs[i];
            if (input != null)
                curInput[input.PlayerID] = input;
        }
    }

    public Msg_PlayerInput GetPlayerInput(int playerId)
    {
        if(curInput.ContainsKey(playerId))
            return curInput[playerId];
        return null;
    }

    public void Reset()
    {
        curInput.Clear();
    }

    public void SetFrameDeltaTime(LFloat frameDeltaTime)
    {
        FrameDeltaTime = frameDeltaTime;
    }
}

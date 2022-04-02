using Lockstep.Math;
using System;
using System.Collections.Generic;


public class CommonStateService : BaseService, ICommonStateService
{
    public int Tick => throw new NotImplementedException();

    public LFloat DeltaTime => throw new NotImplementedException();

    public LFloat TimeSinceGameStart => throw new NotImplementedException();

    public bool IsPause { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void SetDeltaTime(LFloat val)
    {
        throw new NotImplementedException();
    }

    public void SetTick(int val)
    {
        throw new NotImplementedException();
    }

    public void SetTimeSinceGameStart(LFloat val)
    {
        throw new NotImplementedException();
    }
}

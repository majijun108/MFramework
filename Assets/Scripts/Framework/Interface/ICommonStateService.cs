using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface ICommonStateService:IService
{
    int Tick { get; }
    LFloat DeltaTime { get; }
    LFloat TimeSinceGameStart { get; }
    bool IsPause { get; set; }

    void SetTick(int val);
    void SetDeltaTime(LFloat val);
    void SetTimeSinceGameStart(LFloat val);
}

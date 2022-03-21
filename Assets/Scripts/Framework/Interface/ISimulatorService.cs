using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface ISimulatorService : IService
{
    void RunVideo();
    void JumpTo(int tick);
}

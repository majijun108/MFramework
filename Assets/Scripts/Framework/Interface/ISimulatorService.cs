using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface ISimulatorService : IService
{
    void RunVideo();
    void JumpTo(int tick);

    void StartGame(string mapName,List<PlayerInfo> players);
}

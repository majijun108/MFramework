using System;
using System.Collections.Generic;

public class SimulatorService : BaseGameService, ISimulatorService
{
    private IServiceContainer m_serviceContainer;

    public void JumpTo(int tick)
    {
        
    }

    public void RunVideo()
    {
        
    }

    public override void DoAwake(IServiceContainer services)
    {
        base.DoAwake(services);
        m_serviceContainer = services;
    }

    public void StartGame() 
    {

    }
}

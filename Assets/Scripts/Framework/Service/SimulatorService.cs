using System;
using System.Collections.Generic;

public class SimulatorService : BaseGameService, ISimulatorService,ILoadingHandle
{
    private IServiceContainer m_serviceContainer;
    private World m_mainWorld;
    private bool m_isGaming = false;
    private List<PlayerInfo> m_curPlayers;

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

    public void StartGame(string mapName, List<PlayerInfo> players)
    {
        if (m_isGaming)
            return;
        m_isGaming = true;
        m_curPlayers = players;
        LoadingService.Instance.LoadingScene("GameScene",this);
    }

    public void OnLoadSuccess()
    {
        NetworkService.Instance.C2S_ClientReady();
    }

    public void CreatWorld()
    {
        m_mainWorld = new World();
        m_mainWorld.DoAwake(m_ServiceContainer);
        m_mainWorld.DoStart();
    }

    //预加载
    public void Preload()
    {
        
    }

    public void CreatePlayer()
    {
        m_mainWorld?.CreatePlayers(m_curPlayers);
    }
}

using System;
using System.Collections.Generic;

public class SimulatorService : BaseGameService, ISimulatorService,ILoadingHandle,IUpdate
{
    private IServiceContainer m_serviceContainer;
    private LogicWorld _mMainLogicWorld;
    private bool m_isGaming = false;
    private RoomInfo m_room;

    public const int UPDATE_DELTA_TIME = 30;

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

    public override void DoStart()
    {
        base.DoStart();
        ClientMsgHandler.Instance.AddListener(MsgType.S2C_Msg_FrameInfo, OnRecvFramInfo);
    }

    //收到服务器帧
    void OnRecvFramInfo(MsgType type, object obj) 
    {
        _mMainLogicWorld?.PushServerFrame(obj as Msg_FrameInfo);
    }


    public void StartGame(string sceneName,RoomInfo roomInfo)
    {
        if (m_isGaming)
            return;
        m_isGaming = true;
        m_room = roomInfo;
        LoadingService.Instance.LoadingScene(sceneName,this);
    }

    public void OnLoadSuccess()
    {
        NetworkService.Instance.C2S_ClientReady();
    }

    public void CreatLogicWorld()
    {
        _mMainLogicWorld = LogicWorld.CreateWorld(m_room);
    }

    //预加载
    public void Preload()
    {
        
    }

    public void CreateViewWorld()
    {
        
    }

    public override void DoDestroy()
    {
        base.DoDestroy();
        ClientMsgHandler.Instance.RemoveListener(MsgType.S2C_Msg_FrameInfo, OnRecvFramInfo);
    }

    public void DoUpdate(float deltaTime)
    {
        _mMainLogicWorld?.DoUpdate(deltaTime);
    }
}

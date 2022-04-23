using Lockstep.Math;
using System;
using System.Collections.Generic;

public class World
{
    public enum WORLD_STATE 
    {
        INIT,
        WAITING_FOR_FRAME,
        RUNNING,
        PAUSE,
        DESTROYED
    }

    private List<BaseSystem> m_BattleSystems = new List<BaseSystem>();
    private Dictionary<System.Type, BaseSystem> m_type2System = new Dictionary<Type, BaseSystem>();
    
    protected EntityManager m_entityMgr;
    protected bool m_hasCreatePlayer = false;
    public int Tick { get; private set; }

    private long m_gameStartTimestampMs = -1;
    private IFrameBuffer m_FrameBuffer;
    private int m_updateDeltaTime;
    private LFloat m_updateFloatDeltaTime;

    private int m_maxPredictCount = 0;//最大预测帧数 TODO
    private int m_maxPursueMsPerFrame = 20;//每个update追帧的最大时间
    private int m_tickSinceGameStart => (int)((LTime.realtimeSinceStartupMS - m_gameStartTimestampMs) / m_updateDeltaTime);
    private int m_inputTargetTick => m_tickSinceGameStart + 1;//发送操作最大帧数
    private int m_TargetTick => m_tickSinceGameStart;//目标帧

    public WORLD_STATE State { get; private set; } = WORLD_STATE.INIT;

    public World(int updateTime) 
    {
        m_updateDeltaTime = updateTime;
        m_updateFloatDeltaTime = new LFloat(true,updateTime);
        m_FrameBuffer = new FrameBuffer(this, 2000);
    }

    public void RegisterSystem(BaseSystem system)
    {
        var type = system.GetType();
        if (m_type2System.ContainsKey(type))
            return;
        m_BattleSystems.Add(system);
        m_type2System.Add(type, system);
    }

    public T GetSystem<T>() where T : BaseSystem 
    {
        var tyep = typeof(T);
        if (m_type2System.TryGetValue(tyep, out var system)) 
        {
            return (T)system;
        }
        return null;
    }

    void RegisterSystems() 
    {
        RegisterSystem(new EntityManager(this));
        RegisterSystem(new PlayerSystem(this));
    }

    public void DoAwake(IServiceContainer services) 
    {
        RegisterSystems();

        for (int i = 0; i < m_BattleSystems.Count; i++) 
        {
            m_BattleSystems[i].DoAwake(services);
        }

        m_entityMgr = GetSystem<EntityManager>();
    }

    public void DoStart() 
    {
        for (int i = 0; i < m_BattleSystems.Count; i++)
        {
            m_BattleSystems[i].DoStart();
        }
    }

    public void CreatePlayers(List<PlayerInfo> players) 
    {
        if (m_hasCreatePlayer)
            return;
        for (int i = 0; i < players.Count; i++) 
        {
            m_entityMgr.CreateEntity<PlayerEntity>(0, LVector3.zero);
        }
        State = WORLD_STATE.WAITING_FOR_FRAME;
    }

    public void PushServerFrame(Msg_FrameInfo frame) 
    {
        m_FrameBuffer.PushServerFrame(frame);
        if (State == WORLD_STATE.WAITING_FOR_FRAME) 
        {
            State = WORLD_STATE.RUNNING;
        }
    }

    //跑逻辑帧
    void Step(LFloat delta) 
    {
        for (int i = 0; i < m_BattleSystems.Count; i++)
        {
            m_BattleSystems[i].Tick(delta);
        }
    }

    private Dictionary<int,Msg_PlayerInput> curInput = new Dictionary<int, Msg_PlayerInput> ();
    //注入操作
    void ProcessInput(Msg_FrameInfo frame) 
    {
        curInput.Clear();
        for (int i = 0; i < frame.Inputs.Length; i++) 
        {
            var input = frame.Inputs[i];
            if(input != null)
                curInput[input.PlayerID] = input;
        }
        foreach (var item in m_entityMgr.GetPlayers())
        {
            item.Input = curInput.ContainsKey(item.PlayerID) ? curInput[item.PlayerID] : null;
        }
    }

    void Simulate(Msg_FrameInfo frame, bool needGenSnap = false) 
    {
        ProcessInput(frame);
        Step(m_updateFloatDeltaTime);
        Tick++;
    }


    private int m_inputTick = 0;
    public void DoUpdate(float delta) 
    {
        if (State != WORLD_STATE.RUNNING)
            return;
        if (m_gameStartTimestampMs == -1)
            m_gameStartTimestampMs = LTime.realtimeSinceStartupMS;

        m_FrameBuffer.FrameCheck(delta);
        while (m_inputTick <= m_inputTargetTick) 
        {
            SendInputs(m_inputTick++);
        }
        DoNormalUpdate();
    }

    void DoNormalUpdate() 
    {
        if ((Tick - m_FrameBuffer.MaxContinueServerTick) > m_maxPredictCount)
            return;
        var deadline = LTime.realtimeSinceStartupMS + m_maxPursueMsPerFrame;
        //DebugService.Instance.LogError(m_FrameBuffer.CurtTickInServer.ToString() +"-----------------------------------"+LTime.realtimeSinceStartupMS);
        //追帧
        while (Tick < m_FrameBuffer.CurtTickInServer) 
        {
            var sFrame = m_FrameBuffer.GetServerFrame(Tick);
            if (sFrame == null)
            {
                OnPursuingFrame();
                return;
            }
            m_FrameBuffer.PushLocalFrame(sFrame);
            Simulate(sFrame);
            if (LTime.realtimeSinceStartupMS > deadline)
            {
                OnPursuingFrame();
                return;
            }
        }

        if (m_FrameBuffer.IsNeedRollback) //回滚 TODO
        {

        }

        //预测 TODO
    }

    //追帧中的操作
    void OnPursuingFrame() 
    {

    }


    void SendInputs(int tick) 
    {
        var input = InputService.Instance.GetInput();
        if (input == null)
            return;
        input.Tick = tick;
        NetworkService.Instance.C2S_PlayerInput(input);
    }

    public void DoDestroy() 
    {
        State = WORLD_STATE.DESTROYED;
    }
}

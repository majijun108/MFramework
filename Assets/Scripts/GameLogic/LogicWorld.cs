using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using System;
using System.Collections.Generic;

public class LogicWorld
{
    public enum WORLD_STATE 
    {
        INIT,
        WAITING_FOR_FRAME,
        RUNNING,
        PAUSE,
        DESTROYED
    }
    
    protected EntityManager m_entityMgr;
    public EntityManager EntityMgr { get { return m_entityMgr; } }
    private WorldBillboard m_billboard;
    public WorldBillboard Billboard { get { return m_billboard; } }

    private PhysicsWorld m_physicsWorld;
    public PhysicsWorld Physics { get { return m_physicsWorld; } }

    private Systems m_systems;


    protected bool m_hasCreatePlayer = false;
    public int Tick { get; private set; }

    private long m_gameStartTimestampMs = -1;
    private IFrameBuffer m_FrameBuffer;
    private int m_updateDeltaTime;
    private LFloat m_updateFloatDeltaTime;

    private int m_maxPredictCount = 0;//最大预测帧数 TODO
    private int m_maxPursueMsPerFrame = 20;//每个update追帧的最大时间
    private int m_tickSinceGameStart => (int)((LTime.realtimeSinceStartupMS - m_gameStartTimestampMs) / m_updateDeltaTime);
    private int m_inputTargetTick => m_tickSinceGameStart + 5;//发送操作最大帧数
    private int m_TargetTick => m_tickSinceGameStart;//目标帧

    public WORLD_STATE State { get; private set; } = WORLD_STATE.INIT;

    public LogicWorld(int updateTime) 
    {
        m_updateDeltaTime = updateTime;
        m_updateFloatDeltaTime = new LFloat(true,updateTime);
        m_FrameBuffer = new FrameBuffer(this, 2000);

        m_physicsWorld = new PhysicsWorld(this,MapService.Instance.MapInitSize,MapService.Instance.MapInitPos,2.ToLFloat(),LFloat.one);
        m_entityMgr = new EntityManager(this,ComponentRegister.ComponentCount);
        m_billboard = new WorldBillboard();

        m_systems = new Systems();
    }

    public static LogicWorld CreateWorld(RoomInfo room)
    {
        LogicWorld logicWorld = new LogicWorld(room.UpdateTime);
        logicWorld.RegisterSystems();
        logicWorld.Init();
        logicWorld.CreatePlayers(room.Players);
        return logicWorld;
    }

    void RegisterSystems() 
    {
        m_systems.Add(new PlayerInputSystem(this));


        m_systems.Add(new MoveSystem(this));
        m_systems.Add(new PhysicsSystem(this));//物理系统必须最后一个更新
    }

    void Init() 
    {
        m_systems.Initialize();
    }

    void CreatePlayers(List<PlayerInfo> players) 
    {
        if (m_hasCreatePlayer)
            return;
        for (int i = 0; i < players.Count; i++) 
        {
            var playerInfo = players[i];
            var entity = EntityMgr.CreateEntity();
            var player = EntityMgr.AddComponent<PlayerComponent>(entity);
            player.PlayerID = playerInfo.PlayerID;

            EntityMgr.AddComponent<TransformComponent>(entity);
            EntityMgr.AddComponent<MoveComponent>(entity);
            var physics = EntityMgr.AddComponent<PhysicsComponent>(entity);
            physics.Shape = new COBB() { Size = LVector2.one * 2,Angle = 0};
            Physics.AddObj(entity);
            
            GameViewService.Instance.CreatView(entity, "CompleteTank",player.PlayerID == NetworkService.Instance.LocalPlayerID);
        }
        State = WORLD_STATE.WAITING_FOR_FRAME;
    }
    
    
    
    public void PushServerFrame(Msg_FrameInfo frame) 
    {
        m_FrameBuffer.PushServerFrame(frame);
        if (State == WORLD_STATE.WAITING_FOR_FRAME) 
        {
            State = WORLD_STATE.RUNNING;
            InputService.Instance.EnableInput(true);
        }
    }

    //跑逻辑帧
    void Step(LFloat delta) 
    {
        m_systems.Execute();
        m_systems.Cleanup();
    }

    void Simulate(Msg_FrameInfo frame, bool needGenSnap = false) 
    {
        Billboard.Reset();
        Billboard.SetFrameDeltaTime(m_updateFloatDeltaTime);
        Billboard.SetFrameInfo(frame);

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
        //NetworkService.Instance.C2S_PlayerInput(input);
    }

    public void DoDestroy() 
    {
        State = WORLD_STATE.DESTROYED;
        m_systems.TearDown();
        Physics.DoDestroy();
    }
}

using System;
using System.Collections.Generic;


public interface ISystem { }

public interface IInitializeSystem :ISystem
{
    void Initialize();
}

public interface IExecuteSystem :ISystem
{
    void Execute();
}

public interface ICleanupSystem : ISystem 
{
    void Cleanup();
}

public interface ITearDownSystem : ISystem 
{
    void TearDown();
}


public class Systems
{
    protected List<IInitializeSystem> m_initializeSystems;
    protected List<IExecuteSystem> m_executeSystems;
    protected List<ICleanupSystem> m_cleanupSystems;
    protected List<ITearDownSystem> m_tearDownSystems;

    public Systems() 
    {
        m_initializeSystems = new List<IInitializeSystem>();
        m_executeSystems = new List<IExecuteSystem>();
        m_cleanupSystems = new List<ICleanupSystem>();
        m_tearDownSystems = new List<ITearDownSystem>();
    }

    public void Add(ISystem system) 
    {
        var initSystem = system as IInitializeSystem;
        if(initSystem != null)
            m_initializeSystems.Add(initSystem);

        var excuteSystem = system as IExecuteSystem;
        if(excuteSystem != null)
            m_executeSystems.Add(excuteSystem);

        var cleanupSystem = system as ICleanupSystem;
        if(cleanupSystem != null)
            m_cleanupSystems.Add(cleanupSystem);

        var teardownSystem = system as ITearDownSystem;
        if(teardownSystem != null)
            m_tearDownSystems.Add(teardownSystem);
    }

    public void Initialize() 
    {
        for (int i = 0; i < m_initializeSystems.Count; i++)
        {
            m_initializeSystems[i].Initialize();
        }
    }

    public void Execute() 
    {
        for (int i = 0; i < m_executeSystems.Count; i++)
        {
            m_executeSystems[i].Execute();
        }
    }

    public void Cleanup() 
    {
        for (int i = 0; i < m_cleanupSystems.Count; i++)
        {
            m_cleanupSystems[i].Cleanup();
        }
    }

    public void TearDown() 
    {
        for (int i = 0; i < m_tearDownSystems.Count; i++)
        {
            m_tearDownSystems[i].TearDown();
        }
    }
}

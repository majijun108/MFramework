using Lockstep.Math;
using System;
using System.Collections.Generic;

public class World
{
    private List<BaseSystem> m_BattleSystems = new List<BaseSystem>();
    private Dictionary<System.Type, BaseSystem> m_type2System = new Dictionary<Type, BaseSystem>();
    
    protected EntityManager m_entityMgr;
    protected bool m_hasCreatePlayer = false;

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
    }


    public void DoUpdate(LFloat delta) 
    {
        for (int i = 0; i < m_BattleSystems.Count; i++)
        {
            m_BattleSystems[i].DoUpdate(delta);
        }
    }
}

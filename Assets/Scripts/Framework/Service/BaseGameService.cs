using System;
using System.Collections.Generic;

/// <summary>
/// game的情况下对service的扩展 gameplay用到的service
/// </summary>
public abstract class BaseGameService : BaseService
{
    public override void InitReference(IServiceContainer container)
    {
        base.InitReference(container);
    }
}

//需要单例化的service
public abstract class BaseSingleService<T> : BaseService where T : class
{
    private static T m_Instance;
    public static T Instance 
    {
        get 
        {
            if (m_Instance == null)
                DebugService.Instance.LogError("cannot get instance before awake");
            return m_Instance;  
        }
    }

    public override void DoAwake(IServiceContainer services)
    {
        base.DoAwake(services);
        m_Instance = this as T;
    }
}
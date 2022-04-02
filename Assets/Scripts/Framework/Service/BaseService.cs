using System;
using System.Collections.Generic;

public abstract class BaseService : IService, ILifeCycle
{
    public virtual void DoAwake(IServiceContainer services)
    {
    }

    public virtual void DoStart()
    {
    }

    public virtual void DoDestroy()
    {
    }

    public virtual void OnApplicationQuit()
    {
    }

    protected IServiceContainer m_ServiceContainer;

    protected ICommonStateService m_CommonStateService;
    protected IConstStateService m_ConstStateService;
    //禁止单例 所有service通过桥接传入
    public virtual void InitReference(IServiceContainer container) 
    {
        m_ServiceContainer = container;

        //通用的service直接引用
        m_CommonStateService = GetService<CommonStateService>();
        m_ConstStateService = GetService<ConstStateService>();
    }

    protected T GetService<T>() where T : IService 
    {
        return m_ServiceContainer.GetService<T>();
    }

}
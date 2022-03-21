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

    protected IServiceContainer _serviceContainer;

    protected ICommonStateService _commonStateService;
    protected IConstStateService _constStateService;
    //禁止单例 所有service通过桥接传入
    public virtual void InitReference(IServiceContainer container) 
    {
        _serviceContainer = container;

        //通用的service直接引用
        _commonStateService = GetService<ICommonStateService>();
        _constStateService = GetService<IConstStateService>();
    }

    protected T GetService<T>() where T : IService 
    {
        return _serviceContainer.GetService<T>();
    }

}
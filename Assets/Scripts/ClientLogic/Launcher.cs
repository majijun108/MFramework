using System;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.IO;

public class Launcher : ILifeCycle
{
    public static Launcher Instance { get; private set; }
    private BaseServiceContainer _serviceContainer;
    private ManagerContainer _managerContainer;
    private NetworkService m_NetworkService;

    public void DoAwake(IServiceContainer services)
    {
        if(Instance != null) return;
        Instance = this;

        _managerContainer = new ManagerContainer();
        _serviceContainer = (BaseServiceContainer)services;

        var svs = _serviceContainer.GetAllServices();
        foreach (var item in svs)
        {
            if (item is BaseService) 
            {
                _managerContainer.RegisterManager(item as BaseService);
            }
        }
    }

    public void DoStart()
    {
        foreach (var item in _managerContainer.GetAllServices())
        {
            item.InitReference(_serviceContainer);
        }

        //只有baseservice继承过来的才统一注册事件
        var eventService = _serviceContainer.GetService<EventRegisterService>();
        foreach (var item in _managerContainer.GetAllServices())
        {
            eventService.RegisterEvent(item);
        }

        foreach (var item in _managerContainer.GetAllServices())
        {
            item.DoAwake(_serviceContainer);
        }
        _DoAwake();

        foreach (var item in _managerContainer.GetAllServices())
        {
            item.DoStart();
        }
        _DoStart();
    }

    void _DoAwake() 
    {
        m_NetworkService = _serviceContainer.GetService<NetworkService>();
    }

    void _DoStart() 
    {
        var uiservice = _serviceContainer.GetService<UIService>();
        uiservice.OpenWindow("HallWindowCtrl");
    }

    public void DoUpdate(float deltaTime) 
    {
        m_NetworkService.DoUpdate(deltaTime);
    }

    public void DoDestroy()
    {
        if(Instance == null) return;
        foreach (var item in _managerContainer.GetAllServices())
        {
            item.DoDestroy();
        }
        Instance = null;
    }

    public void OnApplicationQuit()
    {
        DoDestroy();
    }
}

using System;
using System.Collections.Generic;

public enum UILayer 
{
    BACKGROUND = 0,
    NORMAL,
    NOTICE,
    FORWARD,
    Max
}

public enum UIState
{
    INIT = 0,
    LOADING,
    DESTROYED,//大于这个的 说明有实体
    LOADED,
    SHOWING,
    HIDING,
    Max
}

public interface IUIService:IService
{
    void OpenWindow(string winName, object openParams = null);
    void CloseWindow(string winName);
}

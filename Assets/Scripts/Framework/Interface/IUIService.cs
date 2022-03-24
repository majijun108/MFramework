using System;
using System.Collections.Generic;

public enum UILayer 
{
    BACKGROUND,
    NORMAL,
    NOTICE,
    FORWARD,
}

public enum UIState
{
    INIT,
    LOADING,
    LOADED,
    SHOWING,
    HIDING,
    DESTROYED
}

public interface IUIService:IService
{
    void OpenWindow(string winName, object openParams = null);
    void CloseWindow(string winName);
}

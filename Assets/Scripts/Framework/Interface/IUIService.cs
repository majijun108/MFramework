using System;
using System.Collections.Generic;

public enum UILayer 
{
    Background,
    Normal,
    Notice,
    Forward,
}

public interface IUIService:IService
{
    void OpenWindow(string winName, UILayer layer = UILayer.Normal);
    void CloseWindow(string winName);
}

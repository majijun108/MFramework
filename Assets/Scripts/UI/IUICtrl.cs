using System;
using System.Collections.Generic;


public interface IUICtrl
{
    UIState State { get;}
    UILayer Layer { get;}
    string ViewType { get; }
    void OnCreate();
    void OnShow();
    void OnHide();
    void OnDestroy();
}

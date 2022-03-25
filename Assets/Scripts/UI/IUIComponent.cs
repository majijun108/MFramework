using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInstance 
{
    void InitTransform(object trans);
    Transform GetInstance();
}

public interface IUIComponent
{
    BaseUICtrl Ctrl { get; }
    void Awake(BaseUICtrl ctrl);
    void OnShow();
    void OnHide();
    void OnDestroy();
}
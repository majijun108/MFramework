using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInstanceProvider:IUIComponent
{
    public BaseUICtrl Ctrl { get; private set; }

    public bool IsInstance => false;

    public void Awake(BaseUICtrl ctrl)
    {
        this.Ctrl = ctrl;
    }

    public void OnDestroy()
    {
        
    }

    public void OnHide()
    {
        
    }

    public void OnShow()
    {
        
    }

    public void SetSprite() 
    {

    }
}

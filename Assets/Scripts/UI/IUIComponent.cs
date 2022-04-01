using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInstance 
{
    void InitTransform(object trans);
    Transform GetInstance();
}

public abstract class IUIComponent
{
    protected BaseUICtrl Ctrl { get; set; }
    public virtual void Awake(BaseUICtrl ctrl) { this.Ctrl = ctrl; }
    public virtual void OnShow() { }
    public virtual void OnHide() { }
    public virtual void OnDestroy() { }
}

public abstract class IViewComponent 
{
    protected BaseUIView View { get; set; }
    public virtual void Awake(BaseUIView view) { this.View = view; }
    public virtual void OnShow() { }
    public virtual void OnHide() { }
    public virtual void OnDestroy() { }
}
using System;
using System.Collections.Generic;

public abstract class AUICtrl : IUICtrl
{

    public string ViewType => throw new NotImplementedException();

    public UIState State { get; private set; } = UIState.DESTROYED;
    public UILayer Layer { get; set; }

    protected IUIView View;

    private List<AUICtrl> m_subCtrl;

    private object m_openParams;
    public object OpenParams 
    {
        get { return m_openParams; }
        set { m_openParams = value; }
    }

    public void InitView(IUIView view) 
    {
        this.View = view;
        this.View.InitTransform();
        OnCreate();
        this.State = UIState.LOADED;
    }

    public void Show() 
    {
        this.State = UIState.SHOWING;
        if (View != null)
        {
            this.View.Show();
        }
        OnShow();
    }

    public void Hide() 
    {
        this.State = UIState.HIDING;
        if(View != null)
            this.View.Hide();
        OnHide();
    }

    public void Destroy() 
    {

    }

    public abstract void OnCreate();
    public abstract void OnShow();

    public abstract void OnHide();
    public abstract void OnDestroy();
}

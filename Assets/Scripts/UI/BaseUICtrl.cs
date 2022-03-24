using System;
using System.Collections.Generic;

public abstract class BaseUICtrl
{
    public UIState State { get; set; } = UIState.INIT;
    public UILayer Layer { get; set; }
    public string Name { get; set; }

    public IUIView View { get; private set; }

    private List<BaseUICtrl> m_subCtrl;

    public void InitView(IUIView view) 
    {
        this.View = view;
        OnCreate();
    }

    public void Show(object openParam) 
    {
        if (View != null)
        {
            this.View.Show();
        }
        OnShow(openParam);
    }

    public void Hide() 
    {
        if(View != null)
            this.View.Hide();
        OnHide();
    }

    public void Destroy() 
    {
        if (View != null)
            this.View.Destroy();
        this.View = null;
        OnDestroy();
    }
    public abstract Type GetViewType();
    public abstract string GetViewName();
    public abstract void OnCreate();
    public abstract void OnShow(object openParam);

    public abstract void OnHide();
    public abstract void OnDestroy();
}

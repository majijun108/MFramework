using System;
using System.Collections.Generic;

public abstract class BaseUICtrl
{
    public UIState State { get; set; } = UIState.INIT;
    public UILayer Layer { get; set; }
    public string Name { get; set; }

    public IUIView View { get; private set; }

    private Dictionary<string,BaseUICtrl> m_SubCtrl = new Dictionary<string, BaseUICtrl>();
    private Dictionary<string,IUIComponent> m_Components = new Dictionary<string,IUIComponent>();

    private UIInstanceProvider m_InstanceProvider;
    public BaseUICtrl() 
    {
        m_InstanceProvider = GetOrAddComponent<UIInstanceProvider>();
    }

    public void InitView(IUIView view) 
    {
        this.View = view;
        OnCreate();
    }

    protected T GetView<T>() where T : class, IUIView
    {
        return View as T;
    }

    public T GetOrAddComponent<T>(object obj = null) where T :IUIComponent,new()
    {
        string name = typeof(T).Name;
        if (m_Components.ContainsKey(name)) 
        {
            return (T)m_Components[name];
        }

        var instance = new T();
        instance.Awake(this);
        if (instance is IInstance) 
        {
            (instance as IInstance).InitTransform(obj);
        }
        m_Components[name] = instance;
        return instance;
    }

    public void Show(object openParam) 
    {
        foreach (var item in m_SubCtrl)
        {
            item.Value.Show(openParam);
        }
        if (View != null)
        {
            this.View.Show();
        }
        OnShow(openParam);

        foreach (var item in m_Components)
        {
            item.Value.OnShow();
        }
    }

    protected void ShowSubView(string subName,object openParam = null) 
    {
        if (this.View == null)
            return;
        if (m_SubCtrl.ContainsKey(subName)) 
        {
            m_SubCtrl[subName].Show(openParam);
            return;
        }

        var parent = this.View.GetSubViewRoot(subName);
        UIService.Instance.LoadSubView(subName, parent, this, openParam);
    }

    public void SubViewLoaded(BaseUICtrl ctrl,object openParams = null) 
    {
        m_SubCtrl[ctrl.Name] = ctrl;
        OnSubViewLoaded(ctrl.Name, openParams);
    }

    public void Hide()
    {
        foreach (var item in m_SubCtrl)
        {
            item.Value.Hide();
        }
        if (View != null)
            this.View.Hide();
        OnHide();

        foreach (var item in m_Components)
        {
            item.Value.OnHide();
        }
    }

    private List<IUIComponent> m_RemoveList = new List<IUIComponent>();
    public void Destroy()
    {
        foreach (var item in m_SubCtrl)
        {
            item.Value.Destroy();
        }
        m_SubCtrl.Clear();
        if (View != null)
            this.View.Destroy();
        this.View = null;
        OnDestroy();
        
        m_RemoveList.Clear();
        foreach (var item in m_Components)
        {
            item.Value.OnDestroy();
            if (item.Value is IInstance) 
            {
                m_RemoveList.Add(item.Value);
            }
        }
        foreach (var item in m_RemoveList)
        {
            m_Components.Remove(item.GetType().Name);
        }
        m_RemoveList.Clear();
    }

    public abstract Type GetViewType();
    public abstract string GetViewName();
    public abstract void OnCreate();
    public abstract void OnShow(object openParam);
    public virtual void OnSubViewLoaded(string subName, object openParams = null) { }

    public abstract void OnHide();
    public abstract void OnDestroy();
}

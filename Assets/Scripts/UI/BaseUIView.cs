using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIView : IUIView
{
    protected Transform m_RootTransform;

    private Dictionary<string, IViewComponent> m_Components = new Dictionary<string, IViewComponent>();

    private List<IViewComponent> m_RemoveList = new List<IViewComponent>();
    public virtual void Destroy()
    {
        if(m_RootTransform != null)
            UIService.Instance.ReleaseUIInstance(m_RootTransform.gameObject);
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
        OnDestroy();
    }

    public virtual Transform GetSubViewRoot(string subName) { return m_RootTransform; }

    public T GetOrAddComponent<T>(object obj = null) where T : IViewComponent, new()
    {
        string name = typeof(T).Name;
        if (m_Components.ContainsKey(name))
        {
            return (T)m_Components[name];
        }

        var instance = new T();
        instance.Awake(this);
        m_Components[name] = instance;
        return instance;
    }

    public virtual void Hide()
    {
        if (this is ICanvas canvas)
        {
            canvas.SetCanvasActive(false);
            return;
        }
        UIUtil.SetActive(m_RootTransform, false);

        foreach (var item in m_Components)
        {
            item.Value.OnHide();
        }
        OnHide();
    }

    public virtual void InitTransform(Transform root) 
    {
        m_RootTransform = root;
        OnCreate();
    }

    public virtual void Show()
    {
        if (this is ICanvas canvas) 
        {
            canvas.SetCanvasActive(true);
        }
        else
            UIUtil.SetActive(m_RootTransform,true);

        if (m_RootTransform != null) 
        {
            m_RootTransform.SetAsLastSibling();
        }

        foreach (var item in m_Components)
        {
            item.Value.OnShow();
        }
        OnShow();
    }

    public abstract void OnCreate();
    public abstract void OnShow();
    public abstract void OnHide();
    public abstract void OnDestroy();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public interface IUIEventRegister
{
    void Register(UnityEngine.Object obj, System.Delegate cb = null);
}

public class UIBtnClickRegister : IUIEventRegister
{
    public void Register(UnityEngine.Object obj, Delegate cb = null)
    {
        if (obj == null)
            return;
        var btn = obj as Button;
        btn.onClick.RemoveAllListeners();
        if (cb != null) 
        {
            btn.onClick.AddListener(cb as UnityEngine.Events.UnityAction);
        }
    }
}

public class UIInputValueChangeRegister : IUIEventRegister
{
    public void Register(UnityEngine.Object obj, Delegate cb = null)
    {
        if (obj == null)
            return;
        var btn = obj as TMPro.TMP_InputField;
        btn.onValueChanged.RemoveAllListeners();
        if (cb != null)
        {
            btn.onValueChanged.AddListener(cb as UnityEngine.Events.UnityAction<string>);
        }
    }
}

public abstract class BaseUIEvent
{
    public UnityEngine.Object MainObj { get; set; }

    protected IUIEventRegister m_register;
    public abstract IUIEventRegister MRegister { get; }
    public abstract void InitComponent(Transform root, string path);
    public void Register(Delegate cb = null) 
    {
        MRegister.Register(MainObj, cb);
    }
}

public class ButtonEvent<T> : BaseUIEvent where T : IUIEventRegister, new()
{
    public override IUIEventRegister MRegister { get 
        { if (m_register == null) m_register = new T();return m_register; } 
    }

    public override void InitComponent(Transform root,string path)
    {
        Transform trans = UIUtil.GetTransform(root, path);
        if (trans == null)
            return;
        MainObj = trans.GetComponent<Button>();
    }
}

public class InputEvent<T> : BaseUIEvent where T : IUIEventRegister, new()
{
    public override IUIEventRegister MRegister
    {
        get
        { if (m_register == null) m_register = new T(); return m_register; }
    }

    public override void InitComponent(Transform root, string path)
    {
        Transform trans = UIUtil.GetTransform(root, path);
        if (trans == null)
            return;
        MainObj = trans.GetComponent<TMPro.TMP_InputField>();
    }
}

public class UIClickComponent : IViewComponent
{
    private Transform m_Transform;
    public void Init(Transform root) 
    {
        m_Transform = root;
    }

    private Dictionary<string, BaseUIEvent> m_ClickEvents = 
        new Dictionary<string, BaseUIEvent>();
    private Dictionary<string,System.Delegate> m_EventBuffers = 
        new Dictionary<string,System.Delegate>();

    public void RegisterUIEvent<T>(string path,System.Delegate cb = null) where T : BaseUIEvent, new()
    {
        if (m_ClickEvents.ContainsKey(path)) 
        {
            var info = m_ClickEvents[path];
            info.Register(cb);
            return;
        }

        var register = new T();
        register.InitComponent(m_Transform, path);
        m_ClickEvents[path] = register;
        register.Register(cb);
    }

    //注册自定义的事件buff 比如item的点击 在这里注册了之后 需要的时候获取缓存就可以了
    public void RegisterUIEventBuffer(string bufferName, System.Delegate cb) 
    {
        m_EventBuffers[bufferName] = cb;
    }

    public T GetUIEventBuffer<T>(string bufferName) where T:System.Delegate
    {
        if (!m_EventBuffers.ContainsKey(bufferName))
            return null;
        return m_EventBuffers[bufferName] as T;
    }

    public override void OnHide()
    {
        base.OnHide();
        foreach (var item in m_ClickEvents)
        {
            item.Value.Register(null);
        }
        m_ClickEvents.Clear();
        m_EventBuffers.Clear();
    }
}

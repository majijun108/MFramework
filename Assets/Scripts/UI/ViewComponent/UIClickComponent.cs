using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClickComponent : IViewComponent
{
    private Transform m_Transform;
    public void Init(Transform root) 
    {
        m_Transform = root;
    }

    private Dictionary<string, ClickInfo> m_ClickEvents = 
        new Dictionary<string, ClickInfo>();
    private Dictionary<string,System.Delegate> m_EventBuffers = 
        new Dictionary<string,System.Delegate>();

    public void RegisterUIEvent<T>(string path,System.Delegate cb = null)
    {
        if (m_ClickEvents.ContainsKey(path)) 
        {
            var info = m_ClickEvents[path];
            Register(info.TypeName, info.Obj, cb);
            return;
        }

        string typeName = typeof(T).Name;
        object obj = GetComponent(path, typeName);
        if (obj == null)
            return;

        m_ClickEvents[path] = new ClickInfo()
        {
            Obj = obj,
            TypeName = typeName,
        };
        Register(typeName, obj, cb);
    }

    object GetComponent(string path, string typeName) 
    {
        Transform trans = UIUtil.GetTransform(m_Transform, path);
        if (trans == null)
            return null;
        switch (typeName) 
        {
            case "Button":
                return trans.GetComponent<Button>();
        }
        return null;
    }

    void Register(string name,object obj,System.Delegate cb = null) 
    {
        switch (name) 
        {
            case "Button":
                Button btn = (Button)obj;
                btn.onClick.RemoveAllListeners();
                var callback = cb as UnityEngine.Events.UnityAction;
                if (callback != null)
                    btn.onClick.AddListener(callback);
                break;
        }
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
            Register(item.Value.TypeName, item.Value.Obj);
        }
        m_ClickEvents.Clear();
        m_EventBuffers.Clear();
    }

    struct ClickInfo
    {
        public object Obj;
        public string TypeName;
    }
}

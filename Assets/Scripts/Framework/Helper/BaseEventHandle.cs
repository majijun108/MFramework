using System;
using System.Collections.Generic;

public abstract class BaseEventHandle<Tetype, Handle> where Tetype : Enum where Handle : System.Delegate
{
    private struct ListenerInfo
    {
        public bool isRegister;//是否是在注册 否则是移除
        public Tetype type;
        public Handle param;
    }
    private struct MsgInfo
    {
        public Tetype type;
        public object param;
    }

    private bool IsTrigging = false;

    private Dictionary<int, List<Handle>> allListener = new Dictionary<int, List<Handle>>();
    private Queue<ListenerInfo> allPendingListener = new Queue<ListenerInfo>();
    private Queue<MsgInfo> allPendingInfo = new Queue<MsgInfo>();

    public void AddListener(Tetype eventType, Handle handler)
    {
        if (IsTrigging)
        {
            allPendingListener.Enqueue(new ListenerInfo() { isRegister = true, type = eventType, param = handler });
            return;
        }
        int intType = Convert.ToInt32(eventType);
        if (allListener.TryGetValue(intType, out var list))
        {
            list.Add(handler);
        }
        else
        {
            var lst = new List<Handle>();
            lst.Add(handler);
            allListener.Add(intType, lst);
        }
    }

    public void RemoveListener(Tetype eventType, Handle handle)
    {
        if (IsTrigging)
        {
            allPendingListener.Enqueue(new ListenerInfo() { isRegister = false, type = eventType, param = handle });
            return;
        }

        var intType = Convert.ToInt32(eventType);
        if (allListener.TryGetValue(intType, out var list))
        {
            if (list.Remove(handle))
            {
                if (list.Count == 0)
                    allListener.Remove(intType);
            }
        }
    }

    public void Trigger(Tetype type, object param = null)
    {
        if (IsTrigging)
        {
            allPendingInfo.Enqueue(new MsgInfo() { type = type, param = param });
            return;
        }

        int idType = Convert.ToInt32(type);
        if (allListener.TryGetValue((int)idType, out var list))
        {
            IsTrigging = true;
            foreach (var item in list)
            {
                item?.DynamicInvoke(type, param);
            }
            IsTrigging = false;
        }

        while (allPendingListener.Count > 0)
        {
            var msgInfo = allPendingListener.Dequeue();
            if (msgInfo.isRegister)
            {
                AddListener(msgInfo.type, msgInfo.param);
            }
            else
            {
                RemoveListener(msgInfo.type, msgInfo.param);
            }
        }

        while (allPendingInfo.Count > 0)
        {
            var msgInfo = allPendingInfo.Dequeue();
            Trigger(msgInfo.type, msgInfo.param);
        }
    }
}

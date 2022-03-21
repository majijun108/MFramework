using System;
using System.Collections.Generic;
using System.Reflection;

public class EventHelper
{
    public delegate void GlobalEventHandler(object param);

    public static T CreateDelegateFromeMethodInfo<T>(System.Object instance, MethodInfo method) where T :Delegate
    {
        return Delegate.CreateDelegate(typeof(T), instance, method) as T;
    }

    public struct ListenerInfo 
    {
        public bool isRegister;//是否是在注册 否则是移除
        public EEvent type;
        public GlobalEventHandler param;
    }
    public struct MsgInfo 
    {
        public EEvent type;
        public object param;
    }

    private static bool IsTrigging = false;

    private static Dictionary<int,List<GlobalEventHandler>> allListener = new Dictionary<int,List<GlobalEventHandler>>();
    private static Queue<ListenerInfo> allPendingListener = new Queue<ListenerInfo>();
    private static Queue<MsgInfo> allPendingInfo = new Queue<MsgInfo>();

    public static void AddListener(EEvent eventType, GlobalEventHandler handler) 
    {
        if (IsTrigging) 
        {
            allPendingListener.Enqueue(new ListenerInfo() { isRegister = true,type = eventType,param = handler});
            return;
        }
        var intType = (int)eventType;
        if (allListener.TryGetValue(intType, out var list))
        {
            list.Add(handler);
        }
        else 
        {
            var lst = new List<GlobalEventHandler>();
            lst.Add(handler);
            allListener.Add(intType, lst);
        }
    }

    public static void RemoveListener(EEvent eventType, GlobalEventHandler handle) 
    {
        if (IsTrigging) 
        {
            allPendingListener.Enqueue(new ListenerInfo() { isRegister = false,type = eventType,param = handle});
            return;
        }

        var intType = (int)eventType;
        if (allListener.TryGetValue(intType, out var list)) 
        {
            if (list.Remove(handle)) 
            {
                if(list.Count == 0)
                    allListener.Remove(intType);
            }
        }
    }

    public static void Trigger(EEvent type, object param = null) 
    {
        if (IsTrigging) 
        {
            allPendingInfo.Enqueue(new MsgInfo() { type = type, param = param });
            return;
        }

        int idType = (int)type;
        if (allListener.TryGetValue((int)idType, out var list)) 
        {
            IsTrigging = true;
            foreach (var item in list)
            {
                item?.Invoke(param);
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

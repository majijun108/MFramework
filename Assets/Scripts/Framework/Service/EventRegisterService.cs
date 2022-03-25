using System;
using System.Collections.Generic;
using System.Reflection;

public class EventRegisterService : IEventRegisterService
{
    public void RegisterEvent(object obj)
    {
        RegisterEvent<EEvent, EventHelper.GlobalEventHandler>("OnEvent_", "OnEvent_".Length,
            EventHelper.AddListener, obj);
    }

    public void UnregisterEvent(object obj)
    {
        RegisterEvent<EEvent, EventHelper.GlobalEventHandler>("OnEvent_", "OnEvent_".Length,
            EventHelper.RemoveListener, obj);
    }

    public void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen, Action<TEnum, TDelegate> callback, object obj)
        where TDelegate:Delegate
        where TEnum:struct
    {
        if (callback == null)
            return;

        var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var method in methods) 
        {
            var methodName = method.Name;
            if (methodName.StartsWith(prefix)) 
            {
                var eventName = methodName.Substring(ignorePrefixLen);
                if (Enum.TryParse(eventName, out TEnum etype)) 
                {
                    var handle = EventHelper.CreateDelegateFromMethodInfo<TDelegate>(obj, method);
                    callback(etype, handle);
                }
            }
        }
    }
}

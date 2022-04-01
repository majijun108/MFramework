using System;
using System.Collections.Generic;
using System.Reflection;


public class EventHelper:BaseEventHandle<EEvent,EventHelper.GlobalEventHandler>
{
    public delegate void GlobalEventHandler(EEvent type,object param);

    public static T CreateDelegateFromMethodInfo<T>(System.Object instance, MethodInfo method) where T :Delegate
    {
        return Delegate.CreateDelegate(typeof(T), instance, method) as T;
    }

    private static EventHelper m_instance;
    public static EventHelper Instance 
    {
        get 
        {
            if(m_instance == null)
                m_instance = new EventHelper();
            return m_instance;
        }
    }
}

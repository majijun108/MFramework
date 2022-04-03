using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventComponent : IUIComponent
{
    private Dictionary<byte,ClientMsgHandler.GlobalNetMsgHandler> m_NetHandlers = new Dictionary<byte,ClientMsgHandler.GlobalNetMsgHandler>();
    private Dictionary<byte,EventHelper.GlobalEventHandler> m_EventHandlers = new Dictionary<byte,EventHelper.GlobalEventHandler>();

    public void RegisterEventHandler(EEvent type, EventHelper.GlobalEventHandler handler) 
    {
        byte key = (byte)type;
        if (m_EventHandlers.ContainsKey(key))
            return;
        m_EventHandlers[key] = handler;
        EventHelper.Instance.AddListener(type, handler);
    }

    public void RegisterNetHandler(MsgType type, ClientMsgHandler.GlobalNetMsgHandler handler) 
    {
        byte key = (byte)type;
        if (m_NetHandlers.ContainsKey(key))
            return;
        m_NetHandlers[key] = handler;
        ClientMsgHandler.Instance.AddListener(type, handler);
    }

    public override void OnHide()
    {
        base.OnHide();
        if (m_NetHandlers.Count > 0) 
        {
            foreach (var item in m_NetHandlers)
            {
                ClientMsgHandler.Instance.RemoveListener((MsgType)item.Key, item.Value);
            }
            m_NetHandlers.Clear();
        }
        if (m_EventHandlers.Count > 0) 
        {
            foreach (var item in m_EventHandlers)
            {
                EventHelper.Instance.RemoveListener((EEvent)item.Key, item.Value);
            }
            m_EventHandlers.Clear();
        }
    }
}

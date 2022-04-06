using ServerMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallWindowCtrl : BaseUICtrl
{

    private int m_selectIndex = -1;
    public override string GetViewName()
    {
        return "HallWindow";
    }

    public override Type GetViewType()
    {
        return typeof(HallWindow);
    }

    public override void OnCreate()
    {
        
    }

    public override void OnDestroy()
    {
        
    }

    private List<RoomInfo> m_AllRooms = new List<RoomInfo>();
    private HallWindow m_Window;

    int GetRoomIndex(string serverIP) 
    {
        if (m_AllRooms == null||m_AllRooms.Count == 0)
            return -1;
        for (int i = 0; i < m_AllRooms.Count; i++)
        {
            var room = m_AllRooms[i];
            if (room.ServerIP == serverIP)
                return i;
        }
        return -1;
    }

    void OnGetRoomInfo(MsgType type, object param)
    {
        RoomInfo roomInfo = (RoomInfo)param;
        if (roomInfo == null)
            return;
        int index = GetRoomIndex(roomInfo.ServerIP);
        if (index < 0)
        {
            m_AllRooms.Add(roomInfo);
        }
        else 
        {
            m_AllRooms[index] = roomInfo;
        }
        GetView<HallWindow>().RefreshScroll(m_AllRooms);
    }

    public override void OnShow(object openParam)
    {
        m_Window = GetView<HallWindow>();
        this.RegisterNetHandler(MsgType.S2C_RoomInfo, OnGetRoomInfo);
        EventHelper.Instance.Trigger(EEvent.OnEnterHall);

        m_Window.RegisterUIEvent<Button>("Btns/CreateBtn", new UnityEngine.Events.UnityAction(OnCreateClick));
        m_Window.RegisterUIEvent<Button>("Btns/EnterBtn", new UnityEngine.Events.UnityAction(OnJoinClick));
        m_Window.RegisterUIEventBuffer("OnItemClick", new Action<int>(OnItemClick));
    }

    public override void OnHide()
    {
        EventHelper.Instance.Trigger(EEvent.OnLeaveHall);
    }

    void OnItemClick(int index) 
    {
        m_selectIndex = index;
    }

    public void OnCreateClick() 
    {
        NetworkService.Instance.CreateRoom();
    }

    public void OnJoinClick() 
    {
        if (m_selectIndex < 0 || m_selectIndex >= m_AllRooms.Count)
            return;
        var data = m_AllRooms[m_selectIndex];
        NetworkService.Instance.C2S_ReqEnterRoom(data);
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallWindowCtrl : BaseUICtrl
{
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

    private int m_selectIndex = -1;
    private string m_currentName;

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

        m_Window.RegisterUIEvent<ButtonEvent<UIBtnClickRegister>>("Btns/CreateBtn", new UnityEngine.Events.UnityAction(OnCreateClick));
        m_Window.RegisterUIEvent<ButtonEvent<UIBtnClickRegister>>("Btns/EnterBtn", new UnityEngine.Events.UnityAction(OnJoinClick));
        m_Window.RegisterUIEventBuffer("OnItemClick", new Action<int>(OnItemClick));

        m_Window.RegisterUIEvent<InputEvent<UIInputValueChangeRegister>>("InputName", new UnityEngine.Events.UnityAction<string>(OnInputValueChange));

        m_currentName = ConstStateService.Instance.PlayerName;
        m_Window.SetInputName(m_currentName);
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
        NetworkService.Instance.CreateRoomAndEnter();
    }

    public void OnJoinClick() 
    {
        if (m_selectIndex < 0 || m_selectIndex >= m_AllRooms.Count)
            return;
        var data = m_AllRooms[m_selectIndex];
        NetworkService.Instance.C2S_ReqEnterRoom(data.ServerIP,data.ServerPort);
    }
    public void OnInputValueChange(string value) 
    {
        m_currentName = value;
        if (!string.IsNullOrEmpty(m_currentName)) 
        {
            ConstStateService.Instance.PlayerName = m_currentName;
        }
    }
}

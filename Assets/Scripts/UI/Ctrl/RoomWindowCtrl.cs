using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerMessage;
using UnityEngine.UI;

public class RoomWindowCtrl : BaseUICtrl
{
    public override string GetViewName()
    {
        return "RoomWindow";
    }

    public override Type GetViewType()
    {
        return typeof(RoomWindow);
    }

    public override void OnCreate()
    {
        
    }

    public override void OnDestroy()
    {
        
    }

    public override void OnHide()
    {
        
    }

    private ServerMessage.RoomInfo roomInfo;
    private List<PlayerInfo> players = new List<PlayerInfo>();
    private RoomWindow m_Window;

    public override void OnShow(object openParam)
    {
        m_Window = GetView<RoomWindow>();
        roomInfo = NetworkService.Instance.GetCurrentRoom();
        if (roomInfo == null)
            return;

        m_Window.RegisterUIEvent<Button>("Btns/ExitBtn", 
            new UnityEngine.Events.UnityAction(OnExitBtnClick));
        m_Window.RegisterUIEvent<Button>("Btns/StartBtn",
            new UnityEngine.Events.UnityAction(OnStartBtnClick));

        UpdateView();
    }

    void UpdateView() 
    {
        players.Clear();
        foreach (var item in roomInfo.Players)
        {
            players.Add(item);
        }
        m_Window.RefreshRoomScroll(players);
        m_Window.SetStartBtnActive(NetworkService.Instance.IsServer());
    }


    void OnExitBtnClick() 
    {
        NetworkService.Instance.C2S_ReqExitRoom();
    }

    void OnStartBtnClick() 
    {
        NetworkService.Instance.StartGame();
    }
}

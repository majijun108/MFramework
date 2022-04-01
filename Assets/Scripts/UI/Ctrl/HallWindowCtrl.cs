using ServerMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void OnHide()
    {
        EventHelper.Instance.Trigger(EEvent.OnLeaveHall);
    }

    void OnGetRoomInfo(MsgType type, object param)
    {
        C2S_RoomInfo roomInfo = (C2S_RoomInfo)param;
        GetView<HallWindow>().RefreshScroll(new List<ServerMessage.C2S_RoomInfo> {
        roomInfo});
    }

    public override void OnShow(object openParam)
    {
        ClientMsgHandler.Instance.AddListener(MsgType.S2C_RoomInfo, OnGetRoomInfo);
        EventHelper.Instance.Trigger(EEvent.OnEnterHall);
    }
}

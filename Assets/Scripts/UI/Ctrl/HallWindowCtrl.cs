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

    public override void OnShow(object openParam)
    {
        EventHelper.Instance.Trigger(EEvent.OnEnterHall);
        GetView<HallWindow>().RefreshScroll(new List<ServerMessage.C2S_RoomInfo> { 
        new ServerMessage.C2S_RoomInfo(){
            RoomName = "ROOM_NAME",
            MaxCount = 10,
            PlayerCount = 1
        } });
    }
}

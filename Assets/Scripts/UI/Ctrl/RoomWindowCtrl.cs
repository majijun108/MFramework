using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private ServerMessage.C2S_RoomInfo roomInfo;

    public override void OnShow(object openParam)
    {
        roomInfo = (ServerMessage.C2S_RoomInfo)openParam;
        if (roomInfo == null)
            return;
        
    }
}

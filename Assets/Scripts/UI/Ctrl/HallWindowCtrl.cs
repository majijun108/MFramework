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
        EventHelper.Trigger(EEvent.OnLeaveHall);
    }

    public override void OnShow(object openParam)
    {
        EventHelper.Trigger(EEvent.OnEnterHall);
    }
}

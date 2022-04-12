using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWindowCtrl : BaseUICtrl
{
    private LoadingWindow m_view;
    public override string GetViewName()
    {
        return "LoadingWindow";
    }

    public override Type GetViewType()
    {
        return typeof(LoadingWindow);
    }

    public override UILayer Layer { get; set; } = UILayer.FORWARD;

    public override void OnHide()
    {
        m_view = null;
    }

    public override void OnShow(object openParam)
    {
        m_view = GetView<LoadingWindow>();
        this.RegisterEventHandler(EEvent.LoadingSceneState, OnLoadingStateChange);
        m_view.ShowText("");
    }

    private LoadingState m_loadState;
    void OnLoadingStateChange(EEvent type, object param) 
    {
        m_loadState = param as LoadingState;
        switch (m_loadState.state) 
        {
            case LoadingState.STATE_TYPE.SCENE_PERCENT:
                string content = ((int)(m_loadState.percent * 100.0f)).ToString();
                m_view.ShowText(content);
                break;
            case LoadingState.STATE_TYPE.MAP_INFO:
                m_view.ShowText("加载地图数据中...");
                break;
            case LoadingState.STATE_TYPE.CREATE_PLAYER:
                m_view.ShowText("创建角色中...");
                break;
        }
    }

}

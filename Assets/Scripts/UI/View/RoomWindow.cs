using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerMessage;
using TMPro;

public class RoomWindow : BaseUIView,ICanvas
{
    private Canvas canvas;

    private Transform m_RoomItem;
    private UIPoolComponent m_PoolComponent;
    private Transform m_StartBtn;

    public override void OnCreate()
    {
        canvas = m_RootTransform.GetComponent<Canvas>();
        m_RoomItem = UIUtil.GetTransform(m_RootTransform, "RoomItem");
        UIUtil.SetActive(m_RoomItem, false);

        m_PoolComponent = GetOrAddComponent<UIPoolComponent>();
        Transform root = UIUtil.GetTransform(m_RootTransform, "Scroll View/Viewport/Content");
        m_PoolComponent.RegisterArchetype(m_RoomItem, root);
        m_StartBtn = UIUtil.GetTransform(m_RootTransform, "Btns/StartBtn");
    }

    List<PlayerInfo> players;
    public void RefreshRoomScroll(List<PlayerInfo> players) 
    {
        this.players = players;
        m_PoolComponent.Refresh(players.Count, UpdateItem);
        this.players = null;
    }

    void UpdateItem(Transform trans, int index) 
    {
        if (players == null || index >= players.Count)
            return;
        PlayerInfo player = players[index];
        var nameText = UIUtil.GetTransform(trans, "Name").GetComponent<TMP_Text>();
        UIUtil.SetText(nameText, player.PlayerName);
    }

    public void SetStartBtnActive(bool isServer) 
    {
        UIUtil.SetActive(m_StartBtn, isServer);
    }

    public override void OnShow()
    {

    }

    public override void OnDestroy()
    {
        
    }

    public override void OnHide()
    {
        
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public void SetCanvasActive(bool active)
    {
        if (canvas != null)
            UIUtil.SetActive(canvas, active);
    }
}

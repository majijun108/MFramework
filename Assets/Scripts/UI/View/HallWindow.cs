using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerMessage;
using TMPro;
using System;

public class HallWindow : BaseUIView, ICanvas
{
    private Canvas canvas;

    Transform m_roomItem;
    UIPoolComponent m_ItemPool;
    TMPro.TMP_InputField m_InputField;

    public override void OnCreate()
    {
        canvas = m_RootTransform.GetComponent<Canvas>();

        m_roomItem = UIUtil.GetTransform(m_RootTransform,"RoomItem");
        UIUtil.SetActive(m_roomItem, false);
        m_InputField = UIUtil.GetTransform(m_RootTransform, "InputName").GetComponent<TMP_InputField>();

        m_ItemPool = GetOrAddComponent<UIPoolComponent>(this);
        Transform parent = UIUtil.GetTransform(m_RootTransform, "RoomScroll/Viewport/Content");
        m_ItemPool.RegisterArchetype(m_roomItem, parent);
    }

    public override void OnShow()
    {
        
    }

    private List<RoomInfo> m_roomList;
    private int m_SelectIndex = -1;
    public void RefreshScroll(List<RoomInfo> dataList) 
    {
        m_roomList = dataList;
        m_ItemPool.Refresh(dataList.Count, UpdateItem);
        m_roomList = null;
    }

    public void SetInputName(string name) 
    {
        m_InputField.text = name;
    }


    void UpdateItem(Transform trans,int index) 
    {
        if (m_roomList == null || index >= m_roomList.Count)
            return;
        RoomInfo info = m_roomList[index];
        var nameText = UIUtil.GetTransform(trans,"Name").GetComponent<TMP_Text>();
        UIUtil.SetText(nameText, info.RoomName);
        var countText = UIUtil.GetTransform(trans, "Count").GetComponent<TMP_Text>();
        UIUtil.SetText(countText,UIUtil.StringConcat(info.Players.Count.ToString(),
            "/",info.MaxCount.ToString()));

        UpdateSelect(trans, index);
        Button btn = trans.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => 
        {
            OnItemClick(trans, index);
        });
    }

    private Transform m_SelectTrans;
    void UpdateSelect(Transform trans, int index) 
    {
        var newSelect = UIUtil.GetTransform(m_SelectTrans, "Select");
        UIUtil.SetActive(newSelect, index == m_SelectIndex);
        if (m_SelectIndex == index)
            m_SelectTrans = trans;
    }

    void OnItemClick(Transform trans, int index) 
    {
        if (m_SelectIndex == index)
            return;
        if (m_SelectTrans)
        {
            var select = UIUtil.GetTransform(m_SelectTrans, "Select");
            UIUtil.SetActive(select, false);
        }
        m_SelectIndex = index;
        UpdateSelect(trans, index);

        var cb = GetUIEventBuffer<Action<int>>("OnItemClick");
        if (cb != null) 
        {
            cb.Invoke(index);
        }
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public void SetCanvasActive(bool active)
    {
        if(canvas != null)
            UIUtil.SetActive(canvas, active);
    }

    public override void OnHide()
    {
        m_SelectIndex = -1;
    }

    public override void OnDestroy()
    {
        
    }
}

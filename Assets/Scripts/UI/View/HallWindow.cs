using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ServerMessage;
using TMPro;

public class HallWindow : BaseUIView, ICanvas
{
    private Canvas canvas;

    Transform m_roomItem;
    UIPoolComponent m_ItemPool;

    public override void InitTransform(Transform root)
    {
        base.InitTransform(root);
        canvas = root.GetComponent<Canvas>();

        UIUtil.GetTransform(m_RootTransform,"Btns/CreateBtn").GetComponent<Button>().onClick.AddListener(OnCreateClick);
        UIUtil.GetTransform(m_RootTransform,"Btns/EnterBtn").GetComponent<Button>().onClick.AddListener(OnCreateClick);
        m_roomItem = UIUtil.GetTransform(m_RootTransform,"RoomItem");
        UIUtil.SetActive(m_roomItem, false);

        m_ItemPool = GetOrAddComponent<UIPoolComponent>(this);
        m_ItemPool.RegisterArchetype(m_roomItem);
    }

    public override void Show()
    {
        base.Show();
    }

    private List<C2S_RoomInfo> m_roomList;
    public void RefreshScroll(List<C2S_RoomInfo> dataList) 
    {
        m_roomList = dataList;
        m_ItemPool.Refresh(dataList.Count, UpdateItem);
        m_roomList = null;
    }

    void UpdateItem(Transform trans,int index) 
    {
        if (m_roomList == null || index >= m_roomList.Count)
            return;
        C2S_RoomInfo info = m_roomList[index];
        var nameText = UIUtil.GetTransform(trans,"Name").GetComponent<TMP_Text>();
        nameText.text = info.RoomName;
        var countText = UIUtil.GetTransform(trans, "Count").GetComponent<TMP_Text>();
        countText.text = UIUtil.StringConcat(info.PlayerCount.ToString(),"/",info.MaxCount.ToString());
    }

    void OnCreateClick() 
    {

    }

    void OnEnterClick() 
    {

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
}

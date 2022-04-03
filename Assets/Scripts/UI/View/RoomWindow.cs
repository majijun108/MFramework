using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomWindow : BaseUIView
{

    private Transform m_RoomItem;
    private UIPoolComponent m_PoolComponent;

    public override void OnCreate()
    {
        UIUtil.GetTransform(m_RootTransform, "Btns/ExitBtn").GetComponent<Button>().onClick.AddListener(OnExitClick);
        UIUtil.GetTransform(m_RootTransform, "Btns/StartBtn").GetComponent<Button>().onClick.AddListener(OnStartClick);
        m_RoomItem = UIUtil.GetTransform(m_RootTransform, "RoomItem");
        UIUtil.SetActive(m_RoomItem, false);

        m_PoolComponent = GetOrAddComponent<UIPoolComponent>();
        Transform root = UIUtil.GetTransform(m_RootTransform, "Scroll View/Viewport/Content");
        m_PoolComponent.RegisterArchetype(m_RoomItem, root);
    }

    void OnExitClick() 
    {

    }

    void OnStartClick() 
    {

    }

    public void RefreshRoomScroll() 
    {

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
}

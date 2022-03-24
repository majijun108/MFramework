using System;
using System.Collections.Generic;
using UnityEngine;


public class UIService : BaseService, IUIService
{
    public static UIService Instance;
    ResourceService _resourceService;
    Transform _uiRoot;

    private Dictionary<string, AUICtrl> m_AllUIView = new Dictionary<string, AUICtrl>();
    private Stack<AUICtrl> m_UIStack = new Stack<AUICtrl>();

    public override void DoStart()
    {
        if(Instance != null)
            Instance = this;

        base.DoStart();
        _resourceService = GetService<ResourceService>();
        _DoStart();
    }

    void _DoStart() 
    {
        _uiRoot = GameObject.Find("Canvas").transform;
        UnityEngine.Object.DontDestroyOnLoad(_uiRoot);
    }

    void LoadView(IUICtrl window) 
    {
        string winName = window.ViewType;
        
        _resourceService.LoadGameObjectAsync(winName, (go) =>
        {
            if (!m_AllUIView.ContainsKey(winName)) 
            {
                _resourceService.ReleaseAsset(go);
                return;
            }
            OnViewLoaded(go, winName);
        });
    }

    void OnViewLoaded(GameObject go,string winName) 
    {
        go.name = winName;
        go.transform.SetParent(_uiRoot);
        go.transform.localScale = Vector3.one;
        RectTransform rect = go.transform as RectTransform;
        rect.anchorMin = Vector3.zero;
        rect.anchorMax = Vector3.one;
        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.zero;

        AUICtrl ctrl = m_AllUIView[winName];
        System.Type type = System.Type.GetType(winName);
        var uibase = go.AddComponent(type) as IUIView;
        ctrl.InitView(uibase);
    }

    public void OpenWindow(string ctrlName,object openParams = null)
    {
        if (m_AllUIView.ContainsKey(ctrlName)) 
        {
            var windowCache = m_AllUIView[ctrlName];
            windowCache.Show();
            return;
        }

        System.Type type = System.Type.GetType(ctrlName);
        AUICtrl ctrl = Activator.CreateInstance(type) as AUICtrl;
        m_AllUIView.Add(ctrlName,ctrl);
        LoadView(ctrl);
    }

    public void CloseWindow(string winName)
    {
        if (m_AllUIView.ContainsKey(winName))
        {
            var windowCache = m_AllUIView[winName];
            windowCache.Hide();
        }
    }
}

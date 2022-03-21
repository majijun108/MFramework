using System;
using System.Collections.Generic;
using UnityEngine;


public class UIService : BaseService, IUIService
{
    ResourceService _resourceService;
    Transform _uiRoot;
    public override void DoStart()
    {
        base.DoStart();
        _resourceService = GetService<ResourceService>();
        _DoStart();
    }

    void _DoStart() 
    {
        _uiRoot = GameObject.Find("Canvas").transform;
        UnityEngine.Object.DontDestroyOnLoad(_uiRoot);
    }

    public void OpenWindow(string winName, UILayer layer = UILayer.Normal)
    {

        string path = ResourceHelper.GetUIPath(winName);
        var go = Resources.Load<GameObject>(path);
        GameObject ui = GameObject.Instantiate(go);
        ui.transform.SetParent(_uiRoot);
        ui.transform.localScale = Vector3.one;
        RectTransform rect = ui.transform as RectTransform;
        rect.anchorMin = Vector3.zero;
        rect.anchorMax = Vector3.one;
        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.zero;
    }

    public void CloseWindow(string winName)
    {
    }
}

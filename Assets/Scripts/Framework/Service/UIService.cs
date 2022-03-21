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
        _resourceService.LoadGameObjectAsync(winName, (go) => 
        {
            go.name = winName;
            go.transform.SetParent(_uiRoot);
            go.transform.localScale = Vector3.one;
            RectTransform rect = go.transform as RectTransform;
            rect.anchorMin = Vector3.zero;
            rect.anchorMax = Vector3.one;
            rect.offsetMin = Vector3.zero;
            rect.offsetMax = Vector3.zero;

            //_resourceService.ReleaseAsset(go);
        });
    }

    public void CloseWindow(string winName)
    {
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;


public class UIService : BaseSingleService<UIService>//, IUIService
{
    
    ResourceService _resourceService;
    Transform _uiRoot;
    Transform[] m_LayerRoots = new Transform[(int)UILayer.Max];

    private Dictionary<string, BaseUICtrl> m_UICtrls = new Dictionary<string, BaseUICtrl>();
    private Dictionary<string,UIInfo> m_loadingInfos = new Dictionary<string, UIInfo>();
    private UILayerStack m_LayerStack;

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

        int max = (int)UILayer.Max;
        for (int i = 0; i < max; i++) 
        {
            GameObject go = new GameObject(((UILayer)i).ToString());
            m_LayerRoots[i] = go.transform;
        }

        m_LayerStack = new UILayerStack(this);
    }

    public override void DoDestroy()
    {
        base.DoDestroy();
    }

    BaseUICtrl GetCtrl(string ctrl) 
    {
        if(m_UICtrls.ContainsKey(ctrl))
            return m_UICtrls[ctrl];
        var actrl = Activator.CreateInstance(System.Type.GetType(ctrl)) as BaseUICtrl;
        if(actrl == null)
            throw new Exception("has not ctrl type:" +ctrl);
        actrl.Name = ctrl;
        m_UICtrls[(ctrl)] = actrl;
        return actrl;
    }

    Transform GetUIRoot(UILayer layer) 
    {
        int layerIndex = (int)layer;
        if (layerIndex >= (int)UILayer.Max)
            return _uiRoot;
        return m_LayerRoots[layerIndex];
    }

    public void OpenWindow(string ctrlName,object openParams = null)
    {
        BaseUICtrl ctrl = GetCtrl(ctrlName);
        if (ctrl == null)
            return;

        if (ctrl.State == UIState.LOADING)
            return;

        if (ctrl.State == UIState.INIT || ctrl.State == UIState.DESTROYED) 
        {
            var myParent = GetUIRoot(ctrl.Layer);
            ctrl.State = UIState.LOADING;
            LoadView(ctrl,openParams, myParent);
            return;
        }

        ShowUI(ctrl,openParams);
        ctrl.State = UIState.SHOWING;
    }

    public void LoadSubView(string ctrlName,Transform parent,BaseUICtrl parentCtrl ,object openParams = null) 
    {
        BaseUICtrl ctrl = GetCtrl(ctrlName);
        if (ctrl.State > UIState.LOADING)
            return;
        LoadView(ctrl,openParams,parent,parentCtrl);
    }

    void LoadView(BaseUICtrl ctrl, object openParams = null, Transform parent = null,BaseUICtrl parentCtrl = null)
    {
        string winName = ctrl.GetViewName();
        if (winName == null)
            throw new Exception("ctrl has not view name");

        string ctrlName = ctrl.Name;
        m_loadingInfos.Add(ctrlName, new UIInfo()
        {
            CtrlName = ctrl.Name,
            Params = openParams,
            Parent = parent,
            ParentCtrl = parentCtrl
        });
        _resourceService.LoadGameObjectAsync(winName, (go) =>
        {
            OnViewLoaded(go, ctrlName);
        });
    }

    bool CheckAvailable(GameObject go, string ctrlName) 
    {
        if (!m_loadingInfos.ContainsKey(ctrlName)) 
        {
            ReleaseUIInstance(go);
            return false;
        }
        var uiInfo = m_loadingInfos[ctrlName];
        BaseUICtrl ctrl = GetCtrl(uiInfo.CtrlName);
        if (uiInfo.Parent == null || (uiInfo.ParentCtrl != null && uiInfo.ParentCtrl.State > UIState.LOADING)) 
        {
            ctrl.State = UIState.INIT;
            ReleaseUIInstance(go);
            return false;
        }

        if (ctrl.State > UIState.LOADING) 
        {
            ReleaseUIInstance(go);
            return false;
        }

        return true;
    }

    void OnViewLoaded(GameObject go, string ctrlName)
    {
        if (!CheckAvailable(go, ctrlName))
        {
            if(m_loadingInfos.ContainsKey(ctrlName))
                m_loadingInfos.Remove(ctrlName);
            return;
        }

        var uiInfo = m_loadingInfos[ctrlName];
        m_loadingInfos.Remove(ctrlName);
        BaseUICtrl ctrl = GetCtrl(uiInfo.CtrlName);

        ctrl.State = UIState.LOADED;
        InitView(ctrl, go.transform,uiInfo.Parent);

        ctrl.State = UIState.SHOWING;
        ShowUI(ctrl, uiInfo.Params);

        if (uiInfo.ParentCtrl != null) 
        {
            uiInfo.ParentCtrl.SubViewLoaded(ctrl,uiInfo.Params);
        }
    }

    void InitView(BaseUICtrl ctrl,Transform root,Transform parent) 
    {
        System.Type type = ctrl.GetViewType();
        var view = Activator.CreateInstance(ctrl.GetViewType()) as IUIView;
        if (view == null)
            throw new Exception("view is null type,ctrlname:"+ctrl.Name);

        root.name = ctrl.GetViewName();
        root.transform.SetParent(_uiRoot);
        root.transform.localScale = Vector3.one;
        RectTransform rect = root.transform as RectTransform;
        rect.anchorMin = Vector3.zero;
        rect.anchorMax = Vector3.one;
        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.zero;

        view.InitTransform(root);
        ctrl.InitView(view);
    }

    void ShowUI(BaseUICtrl ctrl, object openParams = null) 
    {
        ctrl.Show(openParams);
        if(ctrl.Layer == UILayer.NORMAL)
            m_LayerStack.PushUI(ctrl);
    }

    public void CloseWindow(string winName)
    {
        var ctrl = GetCtrl(winName);
        if (ctrl == null)
            return;
        if (ctrl.State != UIState.SHOWING)
            return;
        ctrl.Hide();
        if (ctrl.Layer == UILayer.NORMAL) 
        {
            m_LayerStack.PopUI(ctrl);
        }
    }

    public void CancelLoad(string ctrlName) 
    {
        if (!m_loadingInfos.ContainsKey(ctrlName))
            return;
        m_loadingInfos.Remove(ctrlName);
    }

    public void ReleaseUIInstance(GameObject go)
    {
        _resourceService.ReleaseAsset(go);
    }

    struct UIInfo 
    {
        public string CtrlName;
        public object Params;
        public Transform Parent;
        public BaseUICtrl ParentCtrl;
    }
}

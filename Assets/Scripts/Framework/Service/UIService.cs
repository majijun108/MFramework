using System;
using System.Collections.Generic;
using UnityEngine;


public class UIService : BaseService//, IUIService
{
    public static UIService Instance;
    ResourceService _resourceService;
    Transform _uiRoot;

    private Dictionary<string, BaseUICtrl> m_UICtrls = new Dictionary<string, BaseUICtrl>();
    private Stack<BaseUICtrl> m_UIStack = new Stack<BaseUICtrl>();
    private List<UIInfo> m_loadingInfos = new List<UIInfo>();

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
        return _uiRoot;
    }


    public void OpenWindow(string ctrlName,object openParams = null,Transform parent = null)
    {
        BaseUICtrl ctrl = GetCtrl(ctrlName);
        if (ctrl == null)
            return;

        if (ctrl.State == UIState.LOADING)
            return;

        if (ctrl.State == UIState.INIT || ctrl.State == UIState.DESTROYED) 
        {
            var myParent = parent;
            if(myParent == null)
                myParent = GetUIRoot(ctrl.Layer);
            ctrl.State = UIState.LOADING;
            LoadView(ctrl,openParams, myParent);
            return;
        }

        ShowUI(ctrl,openParams);
        ctrl.State = UIState.SHOWING;
    }
    void LoadView(BaseUICtrl ctrl, object openParams = null, Transform parent = null)
    {
        string winName = ctrl.GetViewName();
        if (winName == null)
            throw new Exception("ctrl has not view name");

        int id = m_loadingInfos.Count;
        m_loadingInfos.Add(new UIInfo()
        {
            ID = id,
            CtrlName = ctrl.Name,
            Params = openParams,
            Parent = parent
        });
        _resourceService.LoadGameObjectAsync(winName, (go) =>
        {
            OnViewLoaded(go, id);
        });
    }

    void ReleaseUIInstance(GameObject go) 
    {
        _resourceService.ReleaseAsset(go);
    }

    void OnViewLoaded(GameObject go, int index)
    {
        if (index > m_loadingInfos.Count - 1)//可能是切换场景被清除了
        {
            ReleaseUIInstance(go);
            return;
        }

        var uiInfo = m_loadingInfos[index];
        m_loadingInfos.RemoveAt(index);

        BaseUICtrl ctrl = GetCtrl(uiInfo.CtrlName);//父物体已经不存在
        if (uiInfo.Parent == null) 
        {
            ctrl.State = UIState.INIT;
            ReleaseUIInstance(go);
            return;
        }

        ctrl.State = UIState.LOADED;
        InitView(ctrl, go.transform,uiInfo.Parent);

        ctrl.State = UIState.SHOWING;
        ShowUI(ctrl, uiInfo.Params);
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
    }

    public void CloseWindow(string winName)
    {
        var ctrl = GetCtrl(winName);
        if (ctrl == null)
            return;
        if (ctrl.State != UIState.SHOWING)
            return;
        ctrl.Hide();
    }

    struct UIInfo 
    {
        public int ID;
        public string CtrlName;
        public object Params;
        public Transform Parent;
    }
}

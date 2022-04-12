using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadingState
{
    public enum STATE_TYPE
    {
        SCENE_PERCENT,//场景加载进度
        MAP_INFO,//加载地图信息
        //CREATE_ENTITY,//创建场景 静态物体
        CREATE_PLAYER,//创建角色
        ALL_READY,//全部加载完成
    }
    public STATE_TYPE state;
    public float percent;
}

public class LoadingService:BaseSingleService<LoadingService>
{
    enum LOADING_STATE 
    {
        INIT,
        LOADING,
        LOADED
    }

    private LOADING_STATE m_state = LOADING_STATE.INIT;
    private AsyncOperationHandle m_loadingHandle;
    private Action m_onLoadingSuccess;
    private string m_loadingSceneName;
    public string SceneName { get; private set; }

    public void LoadingScene(string sceneName,Action onSuccess) 
    {
        if (m_state != LOADING_STATE.LOADED)
            return;
        ResourceService.Instance.ReleaseScene(m_loadingHandle);

        m_loadingSceneName = sceneName;
        m_onLoadingSuccess = onSuccess;
        m_state = LOADING_STATE.LOADING;
        m_loadingHandle = ResourceService.Instance.LoadSceneAsync(sceneName,UnityEngine.SceneManagement.LoadSceneMode.Single);

        CoroutineHelper.StartCoroutine(LoadingProcess());
    }

    private LoadingState m_loadingState = new LoadingState();
    //一个完整的加载场景流程
    IEnumerator LoadingProcess() 
    {
        //显示UI
        yield return null;
        while (!m_loadingHandle.IsDone) 
        {
            m_loadingState.state = LoadingState.STATE_TYPE.SCENE_PERCENT;
            m_loadingState.percent = m_loadingHandle.PercentComplete;
            EventHelper.Instance.Trigger(EEvent.LoadingSceneState, m_loadingState);
            yield return null;
        }

        //加载地图数据
        m_loadingState.state = LoadingState.STATE_TYPE.MAP_INFO;
        EventHelper.Instance.Trigger(EEvent.LoadingSceneState, m_loadingState);
        yield return new WaitForSeconds(0.1f);

        //加载角色
        m_loadingState.state = LoadingState.STATE_TYPE.CREATE_PLAYER;
        EventHelper.Instance.Trigger(EEvent.LoadingSceneState, m_loadingState);
        yield return new WaitForSeconds(0.1f);

        //打扫战场
        m_onLoadingSuccess?.Invoke();
        m_onLoadingSuccess = null;
        SceneName = m_loadingSceneName;
        m_loadingSceneName = null;
        m_state = LOADING_STATE.LOADED;

        m_loadingState.state = LoadingState.STATE_TYPE.ALL_READY;
        EventHelper.Instance.Trigger(EEvent.LoadingSceneState, m_loadingState);
        //关闭UI
    }
}

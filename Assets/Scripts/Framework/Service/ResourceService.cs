using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lockstep.Logging;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ResourceService:BaseSingleService<ResourceService>,IResourceService
{

    private Dictionary<int,AsyncOperationHandle> m_loadedAssets = new Dictionary<int,AsyncOperationHandle>();

    public void LoadAssetAsync<T>(string path, Action<T> callback) where T :UnityEngine.Object
    {
        Addressables.LoadAssetAsync<T>(path).Completed += (hand) => 
        {
            if (hand.Status != AsyncOperationStatus.Succeeded) 
            {
                Lockstep.Logging.Debug.LogError("load asset failed,asset:" + path);
                return;
            }
            m_loadedAssets[hand.Result.GetInstanceID()] = hand;
            callback(hand.Result);
        };
    }

    public void LoadGameObjectAsync(string path, Action<GameObject> callback)
    {
        Addressables.InstantiateAsync(path).Completed += (handle) =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Lockstep.Logging.Debug.LogError("load asset failed,asset:" + path);
                return;
            }
            m_loadedAssets[handle.Result.GetInstanceID()] = handle;
            callback(handle.Result);
        };
    }

    //加载场景
    public void LoadSceneAsync(string sceneName,UnityEngine.SceneManagement.LoadSceneMode mode) 
    {
        var hadle = Addressables.LoadSceneAsync(sceneName, mode);
    }

    public void ReleaseAsset(UnityEngine.Object obj)
    {
        var insId = obj.GetInstanceID();
        if (m_loadedAssets.ContainsKey(insId)) 
        {
            var handle = m_loadedAssets[insId];
            Addressables.Release(handle);
            m_loadedAssets.Remove(insId);
        }
    }
}

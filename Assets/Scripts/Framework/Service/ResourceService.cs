using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceService:IResourceService
{

    private Dictionary<int,AsyncOperationHandle> loadingAssets = new Dictionary<int,AsyncOperationHandle>();

    public void LoadAssetAsync<T>(string path, Action<T> callback) where T :UnityEngine.Object
    {
        var handle = Addressables.LoadAssetAsync<T>(path);
        handle.Completed += (hand) => 
        {
            callback(hand.Result);
        };
    }

    public void LoadGameObjectAsync(string path, Action<GameObject> callback)
    {
        throw new NotImplementedException();
    }

    public void ReleaseAsset(UnityEngine.Object obj)
    {
        throw new NotImplementedException();
    }

    public void ReleaseGameObject(GameObject go)
    {
        throw new NotImplementedException();
    }
}

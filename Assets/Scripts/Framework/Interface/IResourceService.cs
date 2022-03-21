using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;


public interface IResourceService:IService
{
    void LoadAssetAsync<T>(string path,Action<T> callback) where T :UnityEngine.Object;
    void LoadGameObjectAsync(string path,Action<GameObject> callback);
    void ReleaseAsset(UnityEngine.Object obj);
    void ReleaseGameObject(GameObject go);
}

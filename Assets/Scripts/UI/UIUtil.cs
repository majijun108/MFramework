using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIUtil
{
    public static void SetActive(Canvas canvas,bool active) 
    {
        if (canvas == null)
            return;
        canvas.enabled = active;
        GraphicRaycaster caster = canvas.GetComponent<GraphicRaycaster>();
        if (caster) 
        {
            caster.enabled = active;
        }
    }

    public static void SetActive(Transform trans, bool active) 
    {
        if (!trans)
            return;
        trans.gameObject.SetActive(active);
    }

    public static Transform Instantiate(Transform trans,Transform parent = null)
    {
        if (trans == null) 
        {
            DebugService.Instance.LogError("instantiate a null gameobject");
            return null;
        }
        var newgo =  GameObject.Instantiate(trans);
        if (parent != null) 
        {
            newgo.SetParent(parent);
            newgo.transform.localPosition = Vector3.zero;
            newgo.transform.localScale = Vector3.one;
        }
        return newgo;
    }

    public static Transform GetTransform(Transform trans,string path) 
    {
        Queue<Transform> findQueue = new Queue<Transform>();
        findQueue.Enqueue(trans);
        Transform item;
        while (findQueue.Count > 0) 
        {
            item = findQueue.Dequeue();
            var result = item.Find(path);
            if (result != null)
                return result;
            if (item.childCount == 0)
                continue;
            for (int i = 0; i < item.childCount; i++)
            {
                findQueue.Enqueue(item.GetChild(i));
            }
        }
        return null;
    }

    static StringBuilder stringBuilder = new StringBuilder();
    public static string StringConcat(params string[] str) 
    {
        stringBuilder.Clear();
        for (int i = 0; i < str.Length; i++) 
        {
            stringBuilder.Append(str[i]);
        }
        return stringBuilder.ToString();
    }
}

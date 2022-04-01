using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPoolComponent : IViewComponent
{
    private Transform m_Parent;
    private Transform m_Prefab;
    private Queue<Transform> m_UnusePool = new Queue<Transform>();
    private Queue<Transform> m_UsingPool = new Queue<Transform>();

    public void RegisterArchetype(Transform mtype,Transform parent)
    {
        m_Prefab = mtype;
        m_Parent = parent;
        UIUtil.SetActive(mtype, false);
    }

    Transform Get() 
    {
        if (m_UnusePool.Count > 0)
        {
            var mtrans = m_UnusePool.Dequeue();
            UIUtil.SetActive(mtrans, true);
            return mtrans;
        }
        var trans = UIUtil.Instantiate(m_Prefab,m_Parent);
        UIUtil.SetActive(trans, true);
        return trans;
    }

    void Return(Transform trans) 
    {
        if (trans == null)
            return;
        if (m_UnusePool.Contains(trans))
            return;
        m_UnusePool.Enqueue(trans);
        UIUtil.SetActive(trans, false);
    }

    void Clear() 
    {
        while (m_UsingPool.Count > 0)
        {
            Return(m_UsingPool.Dequeue());
        }
    }

    public void Refresh(int count,Action<Transform,int> cb) 
    {
        Clear();
        for (int i = 0; i < count; i++)
        {
            Transform item = Get();
            cb(item,i);
        }
    }
}

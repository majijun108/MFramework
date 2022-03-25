using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIView : IUIView
{
    protected Transform m_RootTransform;

    public void Destroy()
    {
        if(m_RootTransform != null)
            UIService.Instance.ReleaseUIInstance(m_RootTransform.gameObject);
    }

    public virtual Transform GetSubViewRoot(string subName) { return m_RootTransform; }

    public void Hide()
    {
        if (this is ICanvas canvas)
        {
            canvas.SetCanvasActive(false);
            return;
        }
        UIUtil.SetActive(m_RootTransform, false);
    }

    public virtual void InitTransform(Transform root) 
    {
        m_RootTransform = root;
    }

    public void Show()
    {
        if (this is ICanvas canvas) 
        {
            canvas.SetCanvasActive(true);
        }
        else
            UIUtil.SetActive(m_RootTransform,true);

        if (m_RootTransform != null) 
        {
            m_RootTransform.SetAsLastSibling();
        }
    }
}

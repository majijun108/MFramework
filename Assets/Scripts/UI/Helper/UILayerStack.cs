using System;
using System.Collections.Generic;
using UnityEngine;

public class UILayerStack
{
    private UIService m_UIService;
    private Stack<BaseUICtrl> m_UIStack = new Stack<BaseUICtrl>();

    public UILayerStack(UIService service) 
    {
        m_UIService = service;
    }

    bool IsAvailable(BaseUICtrl ctrl) 
    {
        if (ctrl.Layer != UILayer.NORMAL) 
        {
            return false;
        }
        return true;
    }

    public void PushUI(BaseUICtrl ctrl) 
    {
        if (!IsAvailable(ctrl))
            return;
        if (m_UIStack.Contains(ctrl)) 
        {
            ReturnToCtrl(ctrl);
            return;
        }
        m_UIStack.Push(ctrl);
    }

    //返回到某一个UI
    public void ReturnToCtrl(BaseUICtrl ctrl) 
    {
        while (true) 
        {
            if (m_UIStack.Count == 0)
                break;
            BaseUICtrl top = m_UIStack.Peek();
            if (top == ctrl)
                break;
            m_UIService.CloseWindow(ctrl.Name);
        }
    }

    public void PopUI(BaseUICtrl ctrl) 
    {
        if (m_UIStack.Count == 0)
            return;
        if (ctrl != m_UIStack.Peek())
            return;
        m_UIStack.Pop();
    }

    public BaseUICtrl GetTop() 
    {
        if (m_UIStack.Count == 0)
            return null;
        return m_UIStack.Peek();
    }
}

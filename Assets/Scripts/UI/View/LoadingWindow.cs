using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingWindow : BaseUIView
{
    TMPro.TMP_Text m_Content;

    public override void OnCreate()
    {
        m_Content = UIUtil.GetTransform(m_RootTransform,"Content").GetComponent<TMPro.TMP_Text>();
    }

    public void ShowText(string text) 
    {
        m_Content.text = text;
    }

    public override void OnDestroy()
    {
        
    }
}

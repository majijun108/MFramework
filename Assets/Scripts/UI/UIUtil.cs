using System;
using System.Collections.Generic;
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
}

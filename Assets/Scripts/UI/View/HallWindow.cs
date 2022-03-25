using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallWindow : BaseUIView, ICanvas
{
    private Canvas canvas;

    public override void InitTransform(Transform root)
    {
        base.InitTransform(root);
        canvas = root.GetComponent<Canvas>();
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }

    public void SetCanvasActive(bool active)
    {
        if(canvas != null)
            UIUtil.SetActive(canvas, active);
    }
}

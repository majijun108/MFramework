using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIView
{
    void InitTransform(Transform root);
    Transform GetSubViewRoot(string subName);
    void Show();
    void Hide();
    void Destroy();
}

public interface ICanvas 
{
    Canvas GetCanvas();
    void SetCanvasActive(bool active);
}

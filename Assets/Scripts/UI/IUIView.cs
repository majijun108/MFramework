using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIView
{
    void InitTransform(Transform root);
    void ChangeOrder();
    void Show();
    void Hide();
    void Destroy();
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CSharpTest : MonoBehaviour
{
    public delegate void handle(int num);

    private List<handle> myhand = new List<handle>();
    void Start()
    {
        var methodinfo = this.GetType().GetMethod("MyMethod",BindingFlags.NonPublic | BindingFlags.Public |
            BindingFlags.Instance | BindingFlags.DeclaredOnly);
        myhand.Add(Delegate.CreateDelegate(typeof(handle), this, methodinfo) as handle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            myhand[0].Invoke(10);
        }
        if (Input.GetKeyDown(KeyCode.S)) 
        {
            if (myhand.Remove(MyMethod)) 
            {
                Debug.LogError("ÒÆ³ý³É¹¦");
            }
            if (myhand.Count > 0) 
            {
                myhand[0].Invoke(10);
            }
        }
    }

    void MyMethod(int num) 
    {
        Debug.LogError("123");
    }
}

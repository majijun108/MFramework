using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CSharpTest : MonoBehaviour
{
    public BaseEntity BaseEntity { get; set; }

    private void Update()
    {
        if (BaseEntity != null) 
        {
            var player = BaseEntity as PlayerEntity;
            if (player == null)
                return;
            Vector3 pos = new Vector3(BaseEntity.transform.Pos.x.ToFloat(), 0, BaseEntity.transform.Pos.y.ToFloat());
            transform.position = pos;
        }
    }
}

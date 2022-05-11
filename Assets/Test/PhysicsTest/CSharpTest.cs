using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Lockstep.Math;

public class CSharpTest : MonoBehaviour
{
    //public BaseEntity BaseEntity { get; set; }

    //private void Update()
    //{
    //    if (BaseEntity != null) 
    //    {
    //        var player = BaseEntity as PlayerEntity;
    //        if (player == null)
    //            return;
    //        Vector3 pos = new Vector3(BaseEntity.transform.Pos.x.ToFloat(), 0, BaseEntity.transform.Pos.y.ToFloat());
    //        transform.position = Vector3.Lerp(transform.position,pos,0.3f);
    //    }
    //}
    private void Awake()
    {
        //Lockstep.Math.LFloat re = Lockstep.Math.LVector2.Cross(LVector2.right,LVector2.one);
        //LFloat re = PhysicsUtil.GetRotateAngle(LVector2.right,LVector2.one);
        //Debug.LogError(re.ToString());
    }
}

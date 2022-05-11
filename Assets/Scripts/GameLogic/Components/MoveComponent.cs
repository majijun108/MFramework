using Lockstep.Math;
using System;
using System.Collections.Generic;

//移动组件 可以移动的物体才会有 移动是先修改移动组件的值 然后做物理检测修正 最后真正修改位置
public class MoveComponent : IComponent
{
    public LVector2 Velocity;//移动速度
    public int Angle = 0;//移动角度 角色的朝向 0就是不改变朝向
    public LVector2 DeltaPosition;//预移动的距离

    public void OnRecycle()
    {
        
    }

    public void OnReuse()
    {
        
    }
}

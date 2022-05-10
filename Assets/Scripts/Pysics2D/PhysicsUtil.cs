using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

public static class PhysicsUtil
{
    public static LRect GetRect(IShape shape,LVector2 pos,LFloat angle) 
    {
        switch ((ShapeType)shape.Type) 
        {
            case ShapeType.CIRCLE:
                var circle = (CCircle)shape;
                return LRect.CreateRect(pos, new LVector2(circle.Radius, circle.Radius));
            case ShapeType.OBB:
                var obb = (COBB)shape;
                LMatrix33 rotate = LMatrix33.identity;
                angle += obb.Angle;
                GetRotateTranslateMatrix(pos,angle,ref rotate);
                return GetMultiVertexRect(obb.Size,ref rotate);
            case ShapeType.AABB:
                var aabb = (CAABB)shape;
                return LRect.CreateRect(pos, aabb.Size / 2);
            case ShapeType.POLYGON:
                var polygon = (CPolygon)shape;
                LMatrix33 rotate1 = LMatrix33.identity;
                angle += polygon.Angle;
                GetRotateTranslateMatrix(pos,angle,ref rotate1);
                return GetMultiVertexRect(polygon.VertexCount,ref polygon.Vertices,ref rotate1);
        }
        
        return default(LRect);
    }

    static void GetRotateTranslateMatrix(LVector2 pos, LFloat angle,ref LMatrix33 matrix)
    {
        //求旋转矩阵 先旋转在平移
        var rad = LMath.Deg2Rad * angle;
        matrix.SetColumn(0, new LVector3(LMath.Cos(rad), LMath.Sin(rad), LFloat.zero));
        matrix.SetColumn(1, new LVector3(-LMath.Sin(rad), LMath.Cos(rad), LFloat.zero));
        matrix.SetColumn(2, new LVector3(true, pos._x, pos._y, 1000));
    }

    //获取旋转后的方向
    public static LVector2 GetRotateDir(LFloat angle,LVector2 originDir) 
    {
        LMatrix33 matrix = new LMatrix33();
        var rad = LMath.Deg2Rad * angle;
        matrix.SetColumn(0, new LVector3(LMath.Cos(rad), LMath.Sin(rad), LFloat.zero));
        matrix.SetColumn(1, new LVector3(-LMath.Sin(rad), LMath.Cos(rad), LFloat.zero));
        matrix.SetColumn(2, new LVector3(true, 0, 0, 1000));
        LVector2 dir = matrix * originDir.ToHomogeneous;
        return dir;
    }

    //获取旋转角度 逆时针
    public static LFloat GetRotateAngle(LVector2 dir, LVector2 origin)
    {
        dir = dir.normalized;
        origin = origin.normalized;
        LFloat cross = LVector2.Cross(origin, dir);
        if (cross > 0) 
        {
            LFloat angle = LMath.Acos(LVector2.Dot(origin, dir)) * LMath.Rad2Deg;
            return angle;
        }
        LFloat angle1 = LMath.Acos(LVector2.Dot(origin, dir)) * LMath.Rad2Deg;
        return 360 - angle1;
    }

    static LRect GetMultiVertexRect(LVector2 size,ref LMatrix33 matrix) 
    {
        LVector2[] vertices = new LVector2[4];
        LVector2 half = size / 2;
        vertices[0] = -half;
        vertices[1] = new LVector2(-half._x, half._y);
        vertices[2] = half;
        vertices[3] = new LVector2(half._x, -half._y);

        return GetMultiVertexRect(4, ref vertices, ref matrix);
    }

    static LRect GetMultiVertexRect(int count,ref LVector2[] vertices, ref LMatrix33 matrix)
    {
        if(count == 0)
            return default(LRect);
        LVector2 min = matrix * vertices[0].ToHomogeneous;
        LVector2 max = matrix * vertices[0].ToHomogeneous;
        for (int i = 1; i < count; i++) 
        {
            var vertex = matrix * vertices[i].ToHomogeneous;
            if (vertex.x < min.x)
                min.x = vertex.x;
            if(vertex.y < min.y)
                min.y = vertex.y;
            if(vertex.x > max.x)
                max.x = vertex.x;
            if(vertex.y > max.y)
                max.y = vertex.y;
        }

        LRect rect = new LRect();
        rect.min = min;
        rect.max = max;
        return rect;
    }
}

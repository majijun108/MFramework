using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

public static class PhysicsUtil
{
    //角度和方向改为查表 TODO
    static List<LVector2> angle2Dir = new List<LVector2>() 
    {

    };
    public enum ECollisionPair 
    {
        OBB_OBB = 0x00,
        OBB_POLYGON = 0x01,
        OBB_CIRCLE = 0x02,
        OBB_AABB = 0x03,

        POLYGON_POLYGON = 0x11,
        POLYGON_CIRCLE = 0x12,
        POLYGON_AABB = 0x13,

        CIRCLE_CIRCLE = 0x22,
        CIRCLE_AABB = 0x23,

        AABB_AABB = 0x33,
    }
    public delegate bool CheckCollisionFunc(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2);
    static Dictionary<int, CheckCollisionFunc> checkCollisionFuncs =
        new Dictionary<int, CheckCollisionFunc>()
        {
            [(int)ECollisionPair.OBB_OBB] = Check_OBB_OBB,
            [(int)ECollisionPair.OBB_POLYGON] = Check_OBB_POLYGON,
            [(int)ECollisionPair.OBB_CIRCLE] = Check_OBB_CIRCLE,
            [(int)ECollisionPair.OBB_AABB] = Check_OBB_AABB,
            [(int)ECollisionPair.POLYGON_POLYGON] = Check_POLYGON_POLYGON,
            [(int)ECollisionPair.POLYGON_CIRCLE] = Check_POLYGON_CIRCLE,
            [(int)ECollisionPair.POLYGON_AABB] = Check_POLYGON_AABB,
            [(int)ECollisionPair.CIRCLE_CIRCLE] = Check_CIRCLE_CIRCLE,
            [(int)ECollisionPair.CIRCLE_AABB] = Check_CIRCLE_AABB,
            [(int)ECollisionPair.AABB_AABB] = Check_AABB_AABB,
        };

    static bool Check_OBB_OBB(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }

    static bool Check_OBB_POLYGON(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_OBB_CIRCLE(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_OBB_AABB(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_POLYGON_POLYGON(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_POLYGON_CIRCLE(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_POLYGON_AABB(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_CIRCLE_CIRCLE(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_CIRCLE_AABB(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }
    static bool Check_AABB_AABB(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        return false;
    }

    //不同形状的碰撞检测
    public static bool CheckCollision(IShape shape1, LVector2 pos1, int angle1,
        IShape shape2, LVector2 pos2, int angle2)
    {
        if ((int)shape1.Type > (int)shape2.Type) 
        {
            return CheckCollision(shape2,pos2,angle2,shape1,pos1,angle1);
        }
        int id = (int)shape1.Type << 4 | (int)shape2.Type;
        if (!checkCollisionFuncs.ContainsKey(id))
            return false;

        var func = checkCollisionFuncs[id];
        return func(shape1,pos1,angle1,shape2,pos2,angle2);
    }

    public static LRect GetRect(IShape shape,LVector2 pos,int angle) 
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

    static void GetRotateTranslateMatrix(LVector2 pos, int angle,ref LMatrix33 matrix)
    {
        //求旋转矩阵 先旋转在平移
        var rad = LMath.DegRad(angle); //LMath.Deg2Rad * angle;
        matrix.SetColumn(0, new LVector3(LMath.Cos(rad), LMath.Sin(rad), LFloat.zero));
        matrix.SetColumn(1, new LVector3(-LMath.Sin(rad), LMath.Cos(rad), LFloat.zero));
        matrix.SetColumn(2, new LVector3(true, pos._x, pos._y, 1000));
    }

    //获取旋转后的方向
    public static LVector2 GetRotateDir(int angle,LVector2 originDir) 
    {
        LMatrix33 matrix = new LMatrix33();
        var rad = LMath.Pi * (angle.ToLFloat() / 180.ToLFloat());//直接乘以弧度 精度损失特别大
        matrix.SetColumn(0, new LVector3(LMath.Cos(rad), LMath.Sin(rad), LFloat.zero));
        matrix.SetColumn(1, new LVector3(-LMath.Sin(rad), LMath.Cos(rad), LFloat.zero));
        matrix.SetColumn(2, new LVector3(true, 0, 0, 1000));
        LVector2 dir = matrix * originDir.ToHomogeneous;
        return dir;
    }

    //获取旋转角度 逆时针
    public static int GetRotateAngle(LVector2 dir, LVector2 origin)
    {
        dir = dir.normalized;
        origin = origin.normalized;
        LFloat cross = LVector2.Cross(origin, dir);
        if (cross > 0) 
        {
            LFloat angle = LMath.Acos(LVector2.Dot(origin, dir)) * LMath.Rad2Deg;
            return angle.ToInt();
        }
        LFloat angle1 = LMath.Acos(LVector2.Dot(origin, dir)) * LMath.Rad2Deg;
        return 360 - angle1.ToInt();
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

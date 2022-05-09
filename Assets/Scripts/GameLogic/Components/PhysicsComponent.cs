using System;
using System.Collections.Generic;
using Lockstep.Math;

public enum ShapeType 
{
    OBB,
    POLYGON,
    CIRCLE,
    AABB
}

public interface IShape 
{
    ShapeType Type { get; }
}
public struct COBB : IShape
{
    public ShapeType Type => ShapeType.OBB;
    public LVector2 Size;
    public LFloat Angle;//旋转角度 0-360
}

public struct CAABB : IShape
{
    public ShapeType Type => ShapeType.AABB;
    public LVector2 Size;
}

public struct CCircle : IShape
{
    public ShapeType Type => ShapeType.CIRCLE;
    public LFloat Radius;
}

public struct CPolygon : IShape
{
    public ShapeType Type => ShapeType.POLYGON;
    public int VertexCount;
    public LFloat Angle;//旋转角度 0-360
    public LVector2[] Vertices;
}

public class PhysicsComponent : IComponent
{
    public IShape Shape;

    public void OnRecycle()
    {
        
    }

    public void OnReuse()
    {
        
    }
}
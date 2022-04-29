using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJKTest
{
    //检测是否碰撞
    public class GJKCollider
    {

        private Simplex m_simplex = new Simplex();
        /// 浮点数误差。
        public float epsilon = 0.00001f;

        public bool CheckCollider(Shape shapeA, Shape shapeB)
        {
            m_simplex.Clear();

            Vector2 dir = findFirstDir(shapeA, shapeB);
            m_simplex.Add(support(shapeA,shapeB,dir));

            dir = -dir;
            while (true) 
            {
                Vector2 p = support(shapeA,shapeB,dir);
                if (Vector2.Dot(p, dir) < epsilon)
                    return false;
                m_simplex.Add(p);
                if (m_simplex.Contains(Vector2.zero))
                    return true;
                dir = m_simplex.FindNextDir();
            }
        }

        Vector2 findFirstDir(Shape a,Shape b) 
        {
            Vector2 dir = a[0] - b[0];
            if (dir.sqrMagnitude < epsilon) 
            {
                dir = a[1] - b[0];
            }
            return dir;
        }

        Vector2 support(Shape shapeA, Shape shapeB, Vector2 dir) 
        {
            Vector2 fastA = shapeA.FindFastestPointInDir(dir);
            Vector2 fastB = shapeB.FindFastestPointInDir(-dir);
            return fastA - fastB;
        }

        class Simplex
        {
            private List<Vector2> points = new List<Vector2>();


            public void Clear()
            {
                points.Clear();
            }

            public void Add(Vector2 point)
            {
                points.Add(point);
            }

            public bool Contains(Vector2 point)
            {
                return GJKUtil.Cotains(points, point);
            }

            public Vector2 FindNextDir()
            {
                if (points.Count == 2)
                {
                    Vector2 point = GJKUtil.GetOriginFootPoint(points[0], points[1]);
                    return Vector2.zero - point;
                }
                else if (points.Count == 3)
                {
                    Vector2 point1 = GJKUtil.GetOriginFootPoint(points[2], points[0]);
                    Vector2 point2 = GJKUtil.GetOriginFootPoint(points[2], points[1]);
                    if (point1.sqrMagnitude < point2.sqrMagnitude)
                    {
                        points.RemoveAt(1);
                        return Vector2.zero - point1;
                    }
                    else
                    {
                        points.RemoveAt(0);
                        return Vector2.zero - point2;
                    }
                }
                else
                    return Vector2.zero;
            }
        }
    }

    //求多边形之间的距离
    public class GJKDistancce 
    {
        private Simplex m_simplex = new Simplex();

        public Vector2 FromA { get; private set; }
        public Vector2 FromB { get; private set; }

        public bool CheckCollider(Shape a, Shape b) 
        {
            m_simplex.Clear();
            
            Vector2 dir = findFirstDir(a,b);
            SupportPoint support = new SupportPoint();
            GJKUtil.Support(a, b, dir, ref support);
            m_simplex.Add(support);
            GJKUtil.Support(a,b,-dir,ref support);
            m_simplex.Add(support);

            dir = -GJKUtil.FindClosestToOrigin(m_simplex[0].Point,m_simplex[1].Point);
            bool isCollider = false;
            while (true) 
            {
                if (dir.sqrMagnitude < GJKUtil.epsilon) 
                {
                    isCollider = true;
                    break;
                }
                GJKUtil.Support(a,b,dir,ref support);
                if (GJKUtil.SqrDistance(support.Point, m_simplex[0].Point) < GJKUtil.epsilon ||
                    GJKUtil.SqrDistance(support.Point, m_simplex[1].Point) < GJKUtil.epsilon) 
                {
                    isCollider = false;
                    break;
                }
                m_simplex.Add(support);
                if (m_simplex.Contains(Vector2.zero)) 
                {
                    isCollider = true;
                    break;
                }
                dir = m_simplex.FindNextDir(0);
            }

            if (!isCollider) 
            {
                ComputerClosePoint();
            }
            return isCollider;
        }

        //计算距离最近的两个点
        void ComputerClosePoint() 
        {
            SupportPoint a = m_simplex[0];
            SupportPoint b = m_simplex[1];

            Vector2 ab = b.Point - a.Point;
            float dot = Vector2.Dot(a.Point, ab);
            float t = -dot / ab.sqrMagnitude;
            t = Mathf.Clamp01(t);

            FromA = a.FromeA + (b.FromeA - a.FromeA) * t;
            FromB = a.FromeB + (b.FromeB - a.FromeB) * t;
        }

        Vector2 findFirstDir(Shape a, Shape b)
        {
            Vector2 dir = a[0] - b[0];
            if (dir.sqrMagnitude < GJKUtil.epsilon)
            {
                dir = a[1] - b[0];
            }
            return dir;
        }
    }

    //单纯形
    public class Simplex 
    {
        private List<SupportPoint> points = new List<SupportPoint>();

        public SupportPoint this[int index] 
        {
            get 
            {
                return points[index];
            }
        }

        public void Clear() 
        {
            points.Clear();
        }

        public void Add(SupportPoint point) 
        {
            points.Add(point);
        }

        public bool Contains(Vector2 point) 
        {
            return GJKUtil.Cotains(points, point);
        }

        public Vector2 FindNextDir() 
        {
            if (points.Count == 2)
            {
                Vector2 point = GJKUtil.GetOriginFootPoint(points[0].Point, points[1].Point);
                return Vector2.zero - point;
            }
            else if (points.Count == 3) 
            {
                Vector2 point1 = GJKUtil.GetOriginFootPoint(points[2].Point, points[0].Point);
                Vector2 point2 = GJKUtil.GetOriginFootPoint(points[2].Point, points[1].Point);
                if (point1.sqrMagnitude < point2.sqrMagnitude)
                {
                    points.RemoveAt(1);
                    return Vector2.zero - point1;
                }
                else 
                {
                    points.RemoveAt(0);
                    return Vector2.zero - point2;
                }
            }
            else
                return Vector2.zero;
        }

        public Vector2 FindNextDir(int _)
        {
            if (points.Count == 2)
            {
                Vector2 dir = GJKUtil.FindClosestToOrigin(points[0].Point, points[1].Point);
                return -dir;
            }
            else if (points.Count == 3) 
            {
                Vector2 dir20 = GJKUtil.FindClosestToOrigin(points[2].Point, points[0].Point);
                Vector2 dir21 = GJKUtil.FindClosestToOrigin(points[2].Point, points[1].Point);
                if (dir20.sqrMagnitude < dir21.sqrMagnitude)
                {
                    points.RemoveAt(1);
                    return -dir20;
                }
                else 
                {
                    points.RemoveAt(0);
                    return -dir21;
                }
            }else
                return Vector2.zero;
        }
    }

    //public interface IShape 
    //{
    //    Vector2 GetPoint(int index);
    //    int PointCount { get; }
    //    void AddPoint(Vector2 p);
    //    Vector2 FindFastestPointInDir(Vector2 dir);
    //}

    //public abstract class BaseShape : IShape 
    //{

    //}

    //public class Polygon 
    //{

    //}

    public class Shape
    {
        private List<Vector2> points = new List<Vector2>();
        public Vector2 position;

        public Vector2 this[int index] 
        { 
            get 
            {
                return points[index] + position;
            } 
        }

        public int Count { get { return points.Count; } }

        public void AddPoint(Vector2 p) 
        {
            this.points.Add(p);
        }

        //寻找这个方向上的最远点
        public Vector2 FindFastestPointInDir(Vector2 dir) 
        {
            float maxDis = float.MinValue;
            int findIndex = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var distance = Vector2.Dot(this[i], dir);
                if (distance > maxDis) 
                {
                    maxDis = distance;
                    findIndex = i;
                }
            }
            return this[findIndex];
        }
    }
}

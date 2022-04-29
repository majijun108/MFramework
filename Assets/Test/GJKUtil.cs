using System;
using System.Collections.Generic;
using UnityEngine;

namespace GJKTest
{
    public struct SupportPoint 
    {
        public Vector2 Point;
        public Vector2 FromeA;
        public Vector2 FromeB;
    }

    public static class GJKUtil
    {
        public static float epsilon = 0.00001f;
        static int Cross(Vector2 a, Vector2 b, Vector2 c) 
        {
            Vector2 ab = b - a;
            Vector2 ac = c - a;
            float cross = ab.x * ac.y - ab.y * ac.x;
            if(cross == 0)return 0;
            if (cross > 0) return 1;
            return -1;
        }

        //多边形是否包含一个点
        public static bool Cotains(List<Vector2> shape, Vector2 point) 
        {
            if(shape.Count < 3)
                return false;
            float slider = Cross(shape[0], shape[1], shape[2]);
            int n = shape.Count;
            for (int i = 0; i < n; i++)
            {
                int nextIndex = (i + 1) % n;
                float cross = Cross(shape[i],shape[nextIndex],point);
                if (cross == 0)
                    return true;
                if (cross != slider)
                    return false;
            }

            return true;
        }

        public static bool Cotains(List<SupportPoint> shape, Vector2 point)
        {
            if (shape.Count < 3)
                return false;
            float slider = Cross(shape[0].Point, shape[1].Point, shape[2].Point);
            int n = shape.Count;
            for (int i = 0; i < n; i++)
            {
                int nextIndex = (i + 1) % n;
                float cross = Cross(shape[i].Point, shape[nextIndex].Point, point);
                if (cross == 0)
                    return true;
                if (cross != slider)
                    return false;
            }

            return true;
        }

        //求原点到直线ab的垂足
        public static Vector2 GetOriginFootPoint(Vector2 a, Vector2 b) 
        {
            Vector2 ab = b - a;
            Vector2 ao = Vector2.zero - a;
            float dot = Vector2.Dot(ab, ao) / Vector2.SqrMagnitude(ab);
            var res = a + ab * dot;
            return res;
        }

        //求闵可夫斯基差集的最远点
        public static void Support(Shape a, Shape b, Vector2 dir,ref SupportPoint point) 
        {
            Vector2 fastestA = a.FindFastestPointInDir(dir);
            Vector2 fastestB = b.FindFastestPointInDir(dir);
            point.Point = fastestA - fastestB;
            point.FromeA = fastestA;
            point.FromeB = fastestB;
        }

        //寻找线段上距离原点最近的点
        public static Vector2 FindClosestToOrigin(Vector2 a, Vector2 b) 
        {
            Vector2 ab = b - a;
            float sqrDis = ab.sqrMagnitude;
            if (sqrDis < epsilon)
                return a;

            Vector2 ao = Vector2.zero - a;
            var dot = Vector2.Dot(ab, ao);
            if (dot < 0) 
            {
                return a;
            }
            float disNormal = dot / sqrDis;
            if (disNormal > 1)
                return b;
            return a + disNormal * ab;
        }
    }
}

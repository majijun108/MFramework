using System;
using System.Collections.Generic;
using UnityEngine;

namespace GJKTest
{
    public static class GJKUtil
    {

        static int Cross(Vector2 a, Vector2 b, Vector2 c) 
        {
            Vector2 ab = b - a;
            Vector2 ac = c - a;
            float cross = ab.x * ac.y - ab.y * ac.x;
            if(cross == 0)return 0;
            if (cross > 0) return 1;
            return -1;
        }

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

        //求原点到直线ab的垂足
        public static Vector2 GetOriginFootPoint(Vector2 a, Vector2 b) 
        {
            Vector2 ab = b - a;
            Vector2 ao = Vector2.zero - a;
            float dot = Vector2.Dot(ab, ao) / Vector2.SqrMagnitude(ab);
            var res = a + ab * dot;
            return res;
        }
    }
}

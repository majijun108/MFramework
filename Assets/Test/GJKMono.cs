using System;
using System.Collections.Generic;
using UnityEngine;

namespace GJKTest
{
    public class GJKMono:MonoBehaviour
    {

        Shape m_shaper;

        private void Start()
        {
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
            m_shaper = new Shape();
            for (int i = 0; i < poly.points.Length; i++)
            {
                m_shaper.AddPoint(poly.points[i]);
            }
        }

        private void Update()
        {
            
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}

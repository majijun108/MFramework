using System;
using System.Collections.Generic;
using UnityEngine;

namespace GJKTest
{
    public class GJKMono:MonoBehaviour
    {

        public GJKMono other;
        public bool isMain = false;

        public Shape m_shaper;
        MeshFilter m_meshFilter;
        Material m_material;
        GJKCollider m_collider = new GJKCollider();

        private void Start()
        {
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
            m_shaper = new Shape();
            Vector3[] vertices = new Vector3[poly.points.Length];
            for (int i = 0; i < poly.points.Length; i++)
            {
                m_shaper.AddPoint(poly.points[i]);
                vertices[i] = poly.points[i];
            }
            m_meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            int trianglesCount = poly.points.Length - 2;
            int[] triangles = new int[trianglesCount * 3];
            mesh.vertices = vertices;
            for (int i = 0; i < trianglesCount; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
            mesh.triangles = triangles;
            m_meshFilter.mesh = mesh;

            m_material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            m_shaper.position = transform.position;
            if (!isMain)
                return;

            if (m_collider.CheckCollider(m_shaper, other.m_shaper))
            {
                m_material.SetColor("_Color", Color.red);
            }
            else 
            {
                m_material.SetColor("_Color", Color.green);
            }
        }

    }
}

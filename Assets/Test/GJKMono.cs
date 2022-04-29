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
        //GJKCollider m_collider = new GJKCollider();
        GJKDistancce m_collider = new GJKDistancce();

        private void Start()
        {
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
            m_shaper = new Shape();
            m_shaper.position = transform.position;

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

        Vector2 distance = new Vector2(100.0f,100.0f);
        private void Update()
        {
            if (!isMain)
                return;

            int x = 0;
            int y = 0;
            if (Input.GetKey(KeyCode.W))
            {
                y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                y -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                x += 1;
            }

            Vector2 MoveDir = new Vector3(x, y) * Time.deltaTime * 2;
            float t = Vector2.Dot(MoveDir, distance) / distance.sqrMagnitude;
            if (t > 1)
            {
                t = 1.0f / t;
                MoveDir = MoveDir * t;
                isMain = false;

                transform.position += new Vector3(MoveDir.x, MoveDir.y, 0);
                m_shaper.position += MoveDir;
            }
            else
            {
                transform.position += new Vector3(MoveDir.x, MoveDir.y, 0);
                m_shaper.position += MoveDir;
                if (!m_collider.CheckCollider(m_shaper, other.m_shaper))
                {
                    distance = m_collider.FromB - m_collider.FromA;
                    GJKUtil.DebugDraw(new List<Vector2>() { m_collider.FromA, m_collider.FromB }, Color.green);
                }
            }
            //if (m_collider.CheckCollider(m_shaper, other.m_shaper))
            //{
            //    m_material.SetColor("_Color", Color.red);
            //}
            //else 
            //{
            //    m_material.SetColor("_Color", Color.green);
            //    GJKUtil.DebugDraw(new List<Vector2>() { m_collider.FromA, m_collider.FromB }, Color.green);
            //}
        }

    }
}

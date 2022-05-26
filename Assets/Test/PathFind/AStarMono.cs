using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

public class AStarMono : MonoBehaviour
{
    public AStarMap map;
    public AStarMechine mechine;
    public List<Vector2> path = null;
    Stopwatch sw;

    void Start()
    {
        map = GetComponent<AStarMap>();
        mechine = new AStarMechine();
        sw = new Stopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            var mousePos = Input.mousePosition;
            //Debug.LogError(mousePos);
            var worldpos = Camera.main.ScreenToWorldPoint(mousePos);
            //Debug.LogError(worldpos);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool hasFind = mechine.PathFind(this.transform.position, worldpos, map);
            sw.Stop();
            UnityEngine.Debug.Log(string.Format("total: {0}", sw.ElapsedTicks));
            if (hasFind) 
            {
                path = mechine.GetPath();
                //map.DebugDrawPath(path);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var mousePos = Input.mousePosition;
            //Debug.LogError(mousePos);
            var worldpos = Camera.main.ScreenToWorldPoint(mousePos);
            var node = map.GetNodeByPos(worldpos);
            if (node == null)
                return;
            //Debug.LogError(worldpos);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool hasFind = map.CanDirectlyTo(0,0, (int)node.Index.x, (int)node.Index.y);
            sw.Stop();
            UnityEngine.Debug.Log(string.Format("total: {0}", sw.ElapsedTicks));
            if (hasFind)
            {
                path = new List<Vector2>()
                {
                    Vector2.zero,
                    new Vector2(node.Index.x,node.Index.y)
                };
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        if(map != null)
            map.DebugDraw();
        if (path != null)
        {
            map.DebugDrawPath(path);
        }
    }
}

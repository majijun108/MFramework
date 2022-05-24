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
        map = new AStarMap(this.transform.position);
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
            var t = DateTime.Now;
            bool hasFind = mechine.PathFind(this.transform.position, worldpos, map);
            UnityEngine.Debug.Log(string.Format("total: {0} ms", (DateTime.Now - t).TotalMilliseconds));
            if (hasFind) 
            {
                path = mechine.GetPath();
                //map.DebugDrawPath(path);
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

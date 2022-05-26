using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapNode 
{
    public Vector2 Index;
    public Vector2 Pos;
    public bool MoveAble = true;
    public Color Color = Color.white;
    public Vector2 Center 
    {
        get 
        {
            return Pos + AStarMap.NodeSize * 0.5f;
        }
    }

    public int GetIndex() 
    {
        return (int)(Index.y * AStarMap.MapSize.x + Index.x);
    }
}

public class AStarMap:MonoBehaviour
{
    public Vector2 LeftBottomPos = Vector2.zero;
    public static Vector2 NodeSize = Vector2.one;
    static int IndexWidth = 40;
    public static Vector2 MapSize = new Vector2(IndexWidth, IndexWidth);
    public MapNode[,] Maps = new MapNode[IndexWidth, IndexWidth];

    public List<MapNode> mapNodes = new List<MapNode>();
    public int[,] notMove = new int[IndexWidth, IndexWidth];

    public void Awake()
    {
        float xOff = -MapSize.x * NodeSize.x * 0.5f;
        float yOff = -MapSize.y * NodeSize.y * 0.5f;
        LeftBottomPos = transform.position + new Vector3(xOff,yOff,0);
        transform.position = LeftBottomPos;
        InitNotMove();

        for (int h = 0; h < MapSize.y; h++)
        {
            for (int w = 0; w < MapSize.x; w++)
            {
                var node = new MapNode();
                node.Index = new Vector2(w, h);
                float posX = LeftBottomPos.x + w * NodeSize.x;
                float posY = LeftBottomPos.y + h * NodeSize.y;
                node.Pos = new Vector2(posX, posY);
                node.MoveAble = CanMove(w, h);
                if (!node.MoveAble)
                {
                    node.Color = Color.red;
                }
                Maps[w, h] = node;
            }
        }
    }

    void InitNotMove() 
    {
        var notMoveParent = transform.Find("NotMove");
        if (!notMoveParent)
            return;
        int count = notMoveParent.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = notMoveParent.GetChild(i);
            Vector2 index = GetNodeIndexByPos(child.transform.position);
            if (index.x < 0 || index.x >= MapSize.x || index.y < 0 || index.y >= MapSize.y)
            {
                continue;
            }
            notMove[(int)index.x, (int)index.y] = 1;
        }
    }

    Vector2 GetNodeIndexByPos(Vector2 pos) 
    {
        float wOff = pos.x - LeftBottomPos.x;
        float hOff = pos.y - LeftBottomPos.y;
        int indexx = (int)(wOff / NodeSize.x);
        int indexy = (int)(hOff / NodeSize.y);
        return new Vector2(indexx, indexy);
    }

    public MapNode GetNodeByPos(Vector2 pos) 
    {
        float wOff = pos.x - LeftBottomPos.x;
        float hOff = pos.y - LeftBottomPos.y;
        int indexx = (int)(wOff / NodeSize.x);
        int indexy = (int)(hOff / NodeSize.y);
        return GetNode(indexx, indexy);
    }

    public bool CanMove(int x, int y) 
    {
        return notMove[x, y] == 0;
    }

    public MapNode GetMoveableNode(int x, int y)
    {
        var node = GetNode(x, y);
        if (node == null)
            return null;
        if(node.MoveAble)
            return node;
        return null;
    }

    public MapNode GetNode(int x, int y) 
    {
        if (x < 0 || x >= MapSize.x || y < 0 || y >= MapSize.y)
        {
            return null;
        }
        var node = Maps[x, y];
        return node;
    }

    private List<MapNode> mapNeighbor = new List<MapNode>();
    public List<MapNode> GetUseableNeighbors(int x, int y) 
    {
        mapNeighbor.Clear();
        for (int i = -1; i <= 1; i++) 
        {
            for (int j = -1; j <= 1; j++) 
            {
                if (i == 0 && j == 0)
                    continue;
                //if (i == 1 && j == 1)
                //    continue;
                //if (i == 1 && j == -1)
                //    continue;
                //if (i == -1 && j == 1)
                //    continue;
                //if (i == -1 && j == -1)
                //    continue;
                var node = GetMoveableNode(i + x, j + y);
                if(node != null)
                    mapNeighbor.Add(node);
            }
        }
        return mapNeighbor;
    }

    static float Eplise = 0.00001f;
    //是否可以直接到达
    public bool CanDirectlyTo(int x1,int y1,int x2,int y2) 
    {
        Vector2 from = new Vector2(x1+0.5f,y1+0.5f);
        Vector2 to = new Vector2(x2+0.5f,y2+0.5f);

        Vector2 dir = (to - from).normalized;
        Vector2 start = from;
        int startX = (int)start.x;
        int startY = (int)start.y;
        float speedX = 1.0f/Mathf.Abs(dir.x);
        float speedY = 1.0f/Mathf.Abs(dir.y);
        int count = 0;
        while (startX != x2 && startY != y2 &&count++ < 1000) 
        {
            float tx = (dir.y > 0 ? (startY + 1.0f) - start.y : start.y - startY) * speedY;
            if (dir.y == 0) tx = 10.0f;
            float ty = (dir.x > 0 ? (startX + 1.0f) - start.x : start.x - startX) * speedX;
            if(dir.y == 0) ty = 10.0f;
            float t = Mathf.Min(tx, ty);

            start = start + t * dir;
            startX = (int)start.x;
            startY = (int)start.y;
            if (GetNode(startX, startY).MoveAble == false)
                return false;
        }

        return true;
    }

    void DrawNode(MapNode node) 
    {
        Debug.DrawLine(node.Pos, node.Pos + new Vector2(0, NodeSize.y),node.Color);
        Debug.DrawLine(node.Pos + new Vector2(0, NodeSize.y), node.Pos + NodeSize, node.Color);
        Debug.DrawLine(node.Pos + NodeSize, node.Pos + new Vector2(NodeSize.x, 0), node.Color);
        Debug.DrawLine(node.Pos + new Vector2(NodeSize.x, 0), node.Pos, node.Color);
    }

    public void DebugDraw() 
    {
        for (int x = 0; x < MapSize.x; x++) 
        {
            for (int y = 0; y < MapSize.y; y++) 
            {
                var mapNode = Maps[x, y];
                DrawNode(mapNode);
            }
        }
    }

    public void DebugDrawPath(List<Vector2> path) 
    {
        for (int i = 0; i < path.Count - 1; i++) 
        {
            int nextIndex = (i + 1)%path.Count;
            var from = GetNode((int)path[i].x, (int)path[i].y);
            var to = GetNode((int)path[nextIndex].x, (int)path[nextIndex].y);
            Debug.DrawLine(from.Center, to.Center, Color.green);
        }
    }
}

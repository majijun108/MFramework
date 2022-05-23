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

public class AStarMap
{
    public static Vector2 LeftBottomPos = Vector2.zero;
    public static Vector2 NodeSize = Vector2.one;
    public static Vector2 MapSize = new Vector2(40, 40);
    public MapNode[,] Maps = new MapNode[40,40];

    public List<MapNode> mapNodes = new List<MapNode>();
    public int[,] notMove = new int[40, 40];

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

    public AStarMap()
    {
        notMove[1, 2] = 1;
        notMove[1, 3] = 1;
        notMove[1, 4] = 1;
        notMove[1, 5] = 1;


        notMove[5, 2] = 1;
        notMove[5, 3] = 1;
        notMove[5, 4] = 1;
        notMove[5, 5] = 1;

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
                if (i == 1 && j == 1)
                    continue;
                if (i == 1 && j == -1)
                    continue;
                if (i == -1 && j == 1)
                    continue;
                if (i == -1 && j == -1)
                    continue;
                var node = GetMoveableNode(i + x, j + y);
                if(node != null)
                    mapNeighbor.Add(node);
            }
        }
        return mapNeighbor;
    }

    void DrawNode(MapNode node) 
    {
        Debug.DrawLine(node.Pos, node.Pos + new Vector2(0, MapSize.y),node.Color);
        Debug.DrawLine(node.Pos + new Vector2(0, MapSize.y), node.Pos + MapSize, node.Color);
        Debug.DrawLine(node.Pos + MapSize, node.Pos + new Vector2(MapSize.x, 0), node.Color);
        Debug.DrawLine(node.Pos + new Vector2(MapSize.x, 0), node.Pos, node.Color);
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

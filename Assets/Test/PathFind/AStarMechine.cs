using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarNode 
{
    public int Index;

    public float F;
    public float G;
    public float H;
    public int indexX;
    public int indexY;

    public AStarNode LinkNext;
    public AStarNode Parent;
}

public class AStarMechine
{

    AStarMap map;
    MapNode target;
    private Dictionary<int, AStarNode> openDic = new Dictionary<int, AStarNode>();
    private Dictionary<int,AStarNode> closeDic = new Dictionary<int, AStarNode>();
    private AStarNode linkHead;

    public AStarNode FindePath;
    public int MaxDepth = 100;

    public AStarMechine() 
    {

    }

    AStarNode CreateAStarNode(MapNode node) 
    {
        int index = node.GetIndex();
        var starNode = new AStarNode() { Index = index,indexX = (int)node.Index.x, indexY = (int)node.Index.y};
        starNode.H = (target.Index - node.Index).sqrMagnitude;
        starNode.G = 0;
        starNode.F = starNode.G + starNode.H;

        return starNode;
    }

    public bool PathFind(Vector2 fromPos,Vector2 pos,AStarMap map) 
    {
        this.map = map;
        MapNode node = map.GetNodeByPos(pos);
        MapNode from = map.GetNodeByPos(fromPos);
        if (node == null || from == null || node.MoveAble == false || from.MoveAble == false)
            return false;

        openDic.Clear();
        closeDic.Clear();
        linkHead = null;
        FindePath = null;
        target = node;

        AStarNode astarNode = CreateAStarNode(from);
        AddToOpen(astarNode);

        for (int i=0;i <= MaxDepth;i++) 
        {
            var current = GetCheapestNode();
            if (current == null) 
            {
                return false;
            }
            if (current.Index == target.GetIndex())
            {
                FindePath = current;
                return true;
            }

            var list = this.map.GetUseableNeighbors(current.indexX, current.indexY);
            foreach (var item in list)
            {
                int index = item.GetIndex();
                if (openDic.ContainsKey(index))
                {
                    var neighbor = openDic[index];
                    float G = current.G + new Vector2(neighbor.indexX - current.indexX, neighbor.indexY - current.indexY).sqrMagnitude;
                    float H = neighbor.H;
                    float F = G + H;
                    if (F < neighbor.F) 
                    {
                        neighbor.F = F;
                        neighbor.G = G;
                        neighbor.H = H;
                        neighbor.Parent = current;
                    }
                }
                else 
                {
                    AStarNode newNode = CreateAStarNode(item);
                    newNode.G = current.G + new Vector2(newNode.indexX - current.indexX, newNode.indexY - current.indexY).sqrMagnitude;
                    newNode.F = newNode.G + newNode.H;
                    newNode.Parent = current;
                    if (item.GetIndex() == target.GetIndex()) 
                    {
                        FindePath = newNode;
                        return true;
                    }

                    AddToOpen(newNode);
                }
            }
            RemoveFromeOpen(current);
            AddToClose(current);
        }

        Debug.LogError("ovverride");
        return false;
    }

    public void AddToOpen(AStarNode node) 
    {
        if (openDic.ContainsKey(node.Index)) 
        {
            Debug.LogError("重复添加" + node.Index);
            return;
        }
        if (linkHead == null) 
        {
            linkHead = node;
            openDic.Add(node.Index, node);
            return;
        }

        openDic.Add(node.Index, node);
        var current = linkHead;
        AStarNode pre = null;
        while (current != null) 
        {
            if (node.F > current.F) 
            {
                if (current.LinkNext == null) 
                {
                    current.LinkNext = node;
                    return;
                }
                pre = current;
                current = current.LinkNext;
                continue;
            }

            if (pre == null) 
            {
                linkHead = node;
                node.LinkNext = current;
                return;
            }

            pre.LinkNext = node;
            node.LinkNext = current;
            return;
        }
    }

    public void RemoveFromeOpen(AStarNode node) 
    {
        if (!openDic.ContainsKey(node.Index))
        {
            Debug.LogError("删除不存在的openlist" + node.Index);
            return;
        }

        if (linkHead != null && linkHead.Index == node.Index) 
        {
            linkHead = null;
            return;
        }

        var current = linkHead;
        bool hasFind = false;
        while (current != null) 
        {
            if (current.LinkNext.Index == node.Index) 
            {
                hasFind = true;
                break;
            }
            current = current.LinkNext;
        }

        if (hasFind) 
        {
            current.LinkNext = node.LinkNext;
        }
    }

    public void AddToClose(AStarNode node) 
    {
        if (closeDic.ContainsKey(node.Index)) 
        {
            Debug.LogError("重复添加closelist" + node.Index);
            return;
        }
        closeDic.Add(node.Index,node);
    }

    public AStarNode GetCheapestNode() 
    {
        return linkHead;
    }

    public List<Vector2> GetPath() 
    {
        List<Vector2> path = new List<Vector2>();
        var current = FindePath;
        while (current != null) 
        {
            path.Add(new Vector2(current.indexX, current.indexY));
            current = current.Parent;
        }
        return path;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarNode:OrderLinkItem<AStarNode>
{
    public int Index;

    public float F;
    public float G;
    public float H;
    public int indexX;
    public int indexY;

    //public AStarNode LinkNext;
    public AStarNode Parent;

    public AStarNode Next { get; set; }
}

public class AStarMechine
{
    AStarMap map;
    MapNode target;
    private Dictionary<int, AStarNode> openDic = new Dictionary<int, AStarNode>();
    private Dictionary<int,AStarNode> closeDic = new Dictionary<int, AStarNode>();
    //private AStarNode linkHead;
    private PriorityQueue<AStarNode> openQueue;//优先队列
    private OrderLinkList<AStarNode> linkQueue;//有序链表

    public AStarNode FindePath;
    public int MaxDepth = 1000;
    private bool useLinkQueue = true;//是否使用链表 测试

    public AStarMechine() 
    {
        openQueue = new PriorityQueue<AStarNode>((a, b) =>{return a.F < b.F;});
        linkQueue = new OrderLinkList<AStarNode>((a, b) => { return a.F < b.F; });
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

        Setup();
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

            RemoveFromeOpen(current);
            var list = this.map.GetUseableNeighbors(current.indexX, current.indexY);
            foreach (var item in list)
            {
                int index = item.GetIndex();
                if (closeDic.ContainsKey(index))
                    continue;
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
                    //Debug.LogError("AddToOpen" + newNode.indexX + "/" + newNode.indexY);
                }
            }
            AddToClose(current);
            //Debug.LogError("RemoveFromOpen" + current.indexX + "/" + current.indexY);
        }

        Debug.LogError("ovverride");
        return false;
    }

    void Setup() 
    {
        openDic.Clear();
        closeDic.Clear();
        //linkHead = null;
        openQueue.Clear();
        linkQueue.Clear();
        FindePath = null;
    }

    public void AddToOpen(AStarNode node)
    {
        if (openDic.ContainsKey(node.Index))
        {
            Debug.LogError("重复添加" + node.Index);
            return;
        }
        openDic.Add(node.Index, node);
        if(useLinkQueue)
            linkQueue.Enqueue(node);
        else
            openQueue.Enqueue(node);
    }

    public void RemoveFromeOpen(AStarNode node)
    {
        if (!openDic.ContainsKey(node.Index))
        {
            Debug.LogError("删除不存在的openlist" + node.Index);
            return;
        }

        openDic.Remove(node.Index);
        if (useLinkQueue)
            linkQueue.Dequeue();
        else
            openQueue.Dequeue();
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
        if (!useLinkQueue)
        {
            if (openQueue.IsEmpty)
                return null;
            return openQueue.Peek();
        }
        else 
        {
            return linkQueue.Peek();
        }
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

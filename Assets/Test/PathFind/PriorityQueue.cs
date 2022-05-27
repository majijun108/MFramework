using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//优先队列
public class PriorityQueue<T>
{
    private T[] m_data;
    private int m_size;
    private Func<T, T,bool> m_compareFunc;

    public PriorityQueue(Func<T, T, bool> comparen) 
    {
        m_data = new T[32];
        m_size = 0;
        m_compareFunc = comparen;
    }

    public bool IsEmpty { get 
        {
            return m_size <= 0;
        } }

    void Resize() 
    {
        int size = m_data.Length * 2;
        T[] newData = new T[size];
        Array.Copy(m_data,newData, m_data.Length);
        m_data = newData;
    }

    void Exchange(int from, int to) 
    {
        if (from < 1 || from > m_size || to < 1 || to > m_size) 
        {
            throw new Exception("exchange error,over data length");
        }

        var temp = m_data[to - 1];
        m_data[to - 1] = m_data[from - 1];
        m_data[from - 1] = temp;
    }

    bool Compare(int from, int to) 
    {
        return m_compareFunc(m_data[--from],m_data[--to]);
    }

    //向上调整队列
    void UpAdjust() 
    {
        int valuePos = m_size;
        int insertPos = m_size >> 1;
        while (insertPos > 0) 
        {
            if (Compare(valuePos,insertPos))
            {
                Exchange(valuePos, insertPos);
                valuePos = insertPos;
                insertPos = valuePos >> 1;
            }
            else 
            {
                break;
            }
        }
    }

    //向下调整队列
    void DownAdjust() 
    {
        int valuePos = 1;
        int insertPosLeft = valuePos << 1;
        int insertPosRight = insertPosLeft + 1;
        while (insertPosLeft <= m_size) 
        {
            int inserPos = insertPosLeft;
            if (insertPosRight <= m_size && !Compare(insertPosLeft,insertPosRight)) 
            {
                inserPos = insertPosRight;
            }

            if (!Compare(valuePos, inserPos))
            {
                Exchange(valuePos, inserPos);
                valuePos = inserPos;
                insertPosLeft = valuePos << 1;
                insertPosRight = insertPosLeft + 1;
            }
            else
                break;
        }
    }

    public void Enqueue(T item) 
    {
        if (m_size >= m_data.Length)
            Resize();
        m_data[m_size++] = item;
        UpAdjust();
    }

    public T Dequeue() 
    {
        if (m_size <= 0) 
        {
            throw new System.Exception("queue is empty");
        }

        T head = m_data[0];
        m_data[0] = m_data[--m_size];
        DownAdjust();
        return head;
    }

    public T Peek() 
    {
        if (m_size <= 0)
        {
            throw new System.Exception("queue is empty");
        }
        return m_data[0];
    }

    public void Clear() 
    {
        m_size = 0;
        Array.Clear(m_data,0, m_size);
    }

    public void DebugQueue() 
    {
        if (m_size <= 0)
            return;
        string s = "";
        for (int i = 0; i < m_size; i++)
        {
            s = s + "->" + m_data[i].ToString();
        }
        Debug.LogError(s);
    }
}


public interface OrderLinkItem<T> 
{
    public T Next { get; set; }
}
//有序链表
public class OrderLinkList<T> where T : class,OrderLinkItem<T>
{
    private T m_head;
    private int m_count;
    private Func<T, T, bool> m_compare;

    public OrderLinkList(Func<T, T, bool> cfunc) 
    {
        m_compare = cfunc;
        m_head = null;
    }

    public void Enqueue(T t) 
    {
        if (m_head == null)
        {
            m_head = t;
            return;
        }
        T cur = m_head;
        T pre = null;
        while (cur != null) 
        {
            if (m_compare(cur, t)) 
            {
                if (cur.Next == null)
                {
                    cur.Next = t;
                    break;
                }
                pre = cur;
                cur = cur.Next;
                continue;
            }

            if (pre == null) 
            {
                t.Next = cur;
                m_head = t;
                break;
            }

            pre.Next = t;
            t.Next = cur;
            break;
        }
        m_count++;
    }

    public T Peek() 
    {
        return m_head;
    }

    public T Dequeue() 
    {
        if (m_count == 0)
            return null;
        T t = m_head;
        m_head = m_head.Next;
        m_count--;
        return t;
    }

    public void Clear() 
    {
        m_count = 0;
        m_head = null;
    }
}

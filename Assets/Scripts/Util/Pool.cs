using System;
using System.Collections.Generic;
using Lockstep.Util;

public interface IRecyclable
{
    void OnReuse();
    void OnRecycle();
}

public class BaseRecyclable : IRecyclable
{
    public virtual void OnReuse() { }
    public virtual void OnRecycle() { }
    public override string ToString()
    {
        return JsonUtil.ToJson(this);
    }
}

public class ObjectPool
{
    private static Dictionary<Type, IPool> m_allPool = new Dictionary<Type, IPool>();
    public static void Return<T>(T val) where T : IRecyclable, new()
    {
        var type = typeof(T);
        if (!m_allPool.ContainsKey(type)) 
        {
            return;
        }
        var pool = m_allPool[type] as Pool<T>;
        pool.Return(val);
    }

    public static void Return(object obj) 
    {
        if (!(obj is IRecyclable))
            return;
        var type = obj.GetType();
        if (!m_allPool.ContainsKey(type))
        {
            return;
        }
        var pool = m_allPool[type];
        pool.Return(obj);
    }

    public static T Get<T>() where T : IRecyclable, new()
    {
        var type = typeof(T);
        if (!m_allPool.ContainsKey(type))
        {
            m_allPool[type] = new Pool<T>();
        }
        var pool = m_allPool[type] as Pool<T>;
        return pool.Get();
    }

    public static void Clear() 
    {
        m_allPool.Clear();
    }
}

public interface IPool 
{
    object Get();
    void Return(object val);
}

public class Pool<T> : IPool where T : IRecyclable, new()
{
    private Stack<T> pool = new Stack<T>();

    public T Get()
    {
        T ret;
        if (pool.Count == 0)
        {
            ret = new T();
        }
        else
        {
            ret = pool.Pop();
        }
        ret.OnReuse();
        return ret;
    }

    public void Return(T val)
    {
        if (val == null) return;
        val.OnRecycle();
        pool.Push(val);
    }

    public void Return(object obj)
    {
        Return((T)obj);
    }

    object IPool.Get()
    {
        var value = Get();
        return value;
    }
}
using System;
using System.Collections.Generic;

public class ComponentRegister
{
    private static List<System.Type> m_types = new List<System.Type>()
    {
        typeof(PlayerComponent),
        typeof(TransformComponent),
        typeof(PhysicsComponent),
    };
    private static Dictionary<System.Type, int> m_TypeLookup = new Dictionary<System.Type, int>();

    static bool m_hasInit = false;
    static void EnsureTypeLookup() 
    {
        if (m_hasInit)
            return;
        m_hasInit = true;
        m_TypeLookup.Clear();
        for (int i = 0; i < m_types.Count; i++)
        {
            m_TypeLookup[m_types[i]] = i;
        }
    }

    public static int GetComponentIndex(System.Type type)
    {
        EnsureTypeLookup();
        if (m_TypeLookup.ContainsKey(type)) 
        {
            return m_TypeLookup[type];
        }
        return -1;
    }

    public static int GetComponentIndex<T>() where T : IComponent 
    {
        return GetComponentIndex(typeof(T));
    }

    public static int ComponentCount => m_types.Count;
}

using System;
using System.Collections.Generic;


public interface IMatcher 
{
    bool Matches(IEntity entity);
}

public class EntityMatcher:IMatcher
{
    private readonly List<int> m_comsIndices;
    public EntityMatcher(params System.Type[] coms) 
    {
        m_comsIndices = new List<int>(coms.Length);
        for (int i = 0; i < coms.Length; i++) 
        {
            var index = ComponentRegister.GetComponentIndex(coms[i]);
            m_comsIndices.Add(index);
        }
    }

    public bool Matches(IEntity entity) 
    {
        for (int i = 0; i < m_comsIndices.Count; i++)
        {
            if (!entity.HasComponent(m_comsIndices[i]))
                return false;
        }
        return true;
    }
}

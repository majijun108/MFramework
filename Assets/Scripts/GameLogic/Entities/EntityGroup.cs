using System;
using System.Collections.Generic;


public interface IGroup 
{
    void HandleEntity(IEntity entity);
    void UpdateEntity(IEntity entity);
    bool ContainsEntity(IEntity entity);
}

public class EntityGroup:IGroup
{

    private IMatcher m_Matcher;

    public EntityGroup(IMatcher matcher) 
    {
        m_Matcher = matcher;
    }

    public bool ContainsEntity(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public void HandleEntity(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public void UpdateEntity(IEntity entity)
    {
        throw new NotImplementedException();
    }
}

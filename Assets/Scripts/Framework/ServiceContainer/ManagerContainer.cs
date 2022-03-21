using System;
using System.Collections.Generic;

public class ManagerContainer
{
    private Dictionary<string,BaseService> _name2Mgr = new Dictionary<string, BaseService>();
    public List<BaseService> AllMgrs = new List<BaseService>();

    public void RegisterManager(BaseService service) 
    {
        var name = service.GetType().Name;
        if (_name2Mgr.ContainsKey(name))
            return;
        _name2Mgr.Add(name, service);
        AllMgrs.Add(service);
    }

    public BaseService[] GetAllServices()
    {
        return AllMgrs.ToArray();
    }

    public T GetManager<T>() where T : BaseService
    {
        if (_name2Mgr.TryGetValue(typeof(T).Name, out var value)) 
        {
            return value as T;
        }
        return null;
    }
}

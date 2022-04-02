using System;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseServiceContainer : IServiceContainer
{
    protected Dictionary<Type,IService> _allServices = new Dictionary<Type,IService>();

    public virtual void RegisterService(IService service, bool overwrite = true) 
    {
        //var interfaceTypes = service.GetType().FindInterfaces((type, criteria) => type.GetInterfaces().Any(t => t == typeof(IService)),
        //    service).ToArray();
        //foreach (var interfaceType in interfaceTypes)
        //{
            
        //}
        var interfaceType = service.GetType();
        if (!_allServices.ContainsKey(interfaceType))
        {
            _allServices.Add(interfaceType, service);
        }
        else if (overwrite)
            _allServices[interfaceType] = service;
    }

    public IService[] GetAllServices()
    {
        return _allServices.Values.ToArray();
    }

    public T GetService<T>() where T : IService
    {
        var key = typeof(T);
        if(!_allServices.ContainsKey(key))
            return default(T);
        else
            return (T)_allServices[key];
    }
}

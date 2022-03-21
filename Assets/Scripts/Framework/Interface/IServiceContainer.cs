using System;
using System.Collections.Generic;

public interface IServiceContainer
{
    T GetService<T>() where T : IService;
    IService[] GetAllServices();
}

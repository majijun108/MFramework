using System;
using System.Collections.Generic;

/// <summary>
/// 事件系统
/// </summary>
public interface IEventRegisterService:IService
{
    void RegisterEvent(object obj);
    void UnregisterEvent(object obj);
    void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
        Action<TEnum, TDelegate> callback, object obj)
        where TDelegate : Delegate
        where TEnum : struct;
}

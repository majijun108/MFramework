using System;
using System.Collections.Generic;

public interface IDebugService:IService
{
    void Log(params string[] message);
    void LogWarning(params string[] message);
    void LogError(params string[] message);
}

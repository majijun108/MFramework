using System;
using System.Collections.Generic;

public interface IDebugService:IService
{
    void Log(string message);
    void LogWarning(string message);
    void LogError(string message);
}

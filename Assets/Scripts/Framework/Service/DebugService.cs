using System;
using System.Collections.Generic;
using UnityEngine;


public class DebugService : BaseSingleService<DebugService>, IDebugService
{
    public void Log(string message)
    {
        Debug.Log(message);
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }

    public void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
}

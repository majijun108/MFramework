using System;
using System.Collections.Generic;
using UnityEngine;


public class DebugService : BaseSingleService<DebugService>, IDebugService
{
    public void Log(params string[] message)
    {
        Debug.Log(UIUtil.StringConcat(message));
    }

    public void LogError(params string[] message)
    {
        Debug.LogError(UIUtil.StringConcat(message));
    }

    public void LogError(Exception e)
    {
        Debug.LogError(e.ToString());
    }

    public void LogWarning(params string[] message)
    {
        Debug.LogWarning(UIUtil.StringConcat(message));
    }
}

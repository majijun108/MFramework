using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface IConstStateService : IService
{
    /// <summary>
    /// 是否正在追帧
    /// </summary>
    bool IsPursueFrame { get; set; }
}

using Lockstep.Math;
using System;
using System.Collections.Generic;

public interface IConstStateService : IService
{
    /// <summary>
    /// 是否正在追帧
    /// </summary>
    bool IsPursueFrame { get; set; }
    string PlayerName { get; set; }//玩家名字
    int RoomMaxCount { get; set; }//房间最大人数
}

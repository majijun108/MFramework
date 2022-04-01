using System;
using System.Collections.Generic;


public class ConstStateService : BaseService,IConstStateService
{
    public bool IsPursueFrame { get; set; }
    public string PlayerName { get; set; } = "test";
    public int RoomMaxCount { get; set; } = 2;
}

using System;
using System.Collections.Generic;


public class ConstStateService : BaseSingleService<ConstStateService>,IConstStateService
{
    public bool IsPursueFrame { get; set; }
    public string PlayerName { get; set; } = "test_name";
    public int RoomMaxCount { get; set; } = 2;
}

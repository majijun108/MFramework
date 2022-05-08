using System;
using System.Collections.Generic;
using Lockstep.Math;

public interface IMapService
{
    LFloat MapInitSize { get; }
    LVector2 MapInitPos { get; }
}

using Lockstep.Math;
using System;
using System.Collections.Generic;

public class MapService : BaseSingleService<MapService>, IMapService
{
    public LFloat MapInitSize => new LFloat(2000);

    public LVector2 MapInitPos => LVector2.zero;
}

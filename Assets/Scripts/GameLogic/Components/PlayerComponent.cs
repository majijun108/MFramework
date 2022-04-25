using System;
using System.Collections.Generic;

public class PlayerComponent : IComponent
{
    public int PlayerID = -1;

    public void OnRecycle()
    {
    }

    public void OnReuse()
    {
    }
}

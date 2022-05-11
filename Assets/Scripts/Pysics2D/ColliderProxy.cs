using Lockstep.UnsafeCollision2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ColliderProxy : IRecyclable
{
    public LRect Bounds;
    public void OnRecycle()
    {
        
    }

    public void OnReuse()
    {
        
    }
}

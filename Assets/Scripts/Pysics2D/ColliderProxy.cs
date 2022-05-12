using Lockstep.UnsafeCollision2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ColliderProxy : IRecyclable
{
    public LRect Bounds;
    public IEntity Entity;
    public PhysicsComponent PhysicsBody;
    public TransformComponent Transform;

    public void OnRecycle()
    {
        
    }

    public void OnReuse()
    {
        
    }
}

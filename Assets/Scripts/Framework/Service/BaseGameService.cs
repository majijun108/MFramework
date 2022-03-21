using System;
using System.Collections.Generic;

/// <summary>
/// game的情况下对service的扩展 gameplay用到的service
/// </summary>
public abstract class BaseGameService : BaseService
{
    public override void InitReference(IServiceContainer container)
    {
        base.InitReference(container);
    }
}
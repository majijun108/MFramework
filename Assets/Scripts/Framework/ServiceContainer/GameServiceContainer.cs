using System;
using System.Collections.Generic;
using System.Linq;

public class GameServiceContainer : BaseServiceContainer
{
    public GameServiceContainer() 
    {
        RegisterService(new CommonStateService());
        RegisterService(new ConstStateService());
        RegisterService(new EventRegisterService());
    }
}

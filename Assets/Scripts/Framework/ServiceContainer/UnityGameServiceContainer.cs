using System;
using System.Collections.Generic;

//unity客户端用的 会多有view层相关的service
public class UnityGameServiceContainer:GameServiceContainer
{
    public UnityGameServiceContainer() :base()
    {
        RegisterService(new NetworkService());
        RegisterService(new UIService());
        RegisterService(new ResourceService());
        RegisterService(new DebugService());
        RegisterService(new LoadingService());
    }
}

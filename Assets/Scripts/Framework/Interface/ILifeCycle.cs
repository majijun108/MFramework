using System.Collections;
using System.Collections.Generic;

public interface ILifeCycle
{
    void DoAwake(IServiceContainer services);
    void DoStart();
    void DoDestroy();
    void OnApplicationQuit();
}

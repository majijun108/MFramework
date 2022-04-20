using System.Collections;
using System.Collections.Generic;

public interface ILifeCycle
{
    void DoAwake(IServiceContainer services);
    void DoStart();
    void DoDestroy();
    void OnApplicationQuit();
}

//��Ϸ��Ⱦ��update ��֡�õ�tick
public interface IUpdate 
{
    void DoUpdate(float deltaTime);
}

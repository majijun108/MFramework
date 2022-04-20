using System.Collections;
using System.Collections.Generic;

public interface ILifeCycle
{
    void DoAwake(IServiceContainer services);
    void DoStart();
    void DoDestroy();
    void OnApplicationQuit();
}

//游戏渲染的update 跑帧用的tick
public interface IUpdate 
{
    void DoUpdate(float deltaTime);
}

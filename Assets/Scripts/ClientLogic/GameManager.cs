using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager:MonoBehaviour
{
    public Launcher launcher = new Launcher();

    private BaseServiceContainer _serviceContainer;

    private void Awake()
    {
        _serviceContainer = new UnityGameServiceContainer();

        launcher.DoAwake(_serviceContainer);
    }

    private void Start()
    {
        launcher.DoStart();
    }

    private void Update()
    {
        launcher.DoUpdate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        launcher.DoDestroy();
    }

    private void OnApplicationQuit()
    {
        launcher.OnApplicationQuit();
    }
}

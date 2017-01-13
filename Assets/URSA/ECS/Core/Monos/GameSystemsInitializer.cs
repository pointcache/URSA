using UnityEngine;
using System;
using System.Collections.Generic;

public class GameSystemsInitializer : MonoBehaviour
{

    GameObject loaders, systems;

    void OnEnable()
    {
        loaders = transform.Find("Loaders").gameObject;
        if (!loaders)
        {
            Debug.LogError("GameSystems: No Initializers gameobject found");
            return;
        }

        systems = transform.Find("Systems").gameObject;
        if (!systems)
        {
            Debug.LogError("GameSystems: No Systems gameobject found");
            return;
        }

        InitializerSystem.Initialization += Init;
        InitializerSystem.PostInitialization += PostInit;
    }

    void OnDisable()
    {
        InitializerSystem.Initialization -= Init;
        InitializerSystem.PostInitialization -= PostInit;
    }

    void Init()
    {
        loaders.SetActive(true);
    }

    void PostInit()
    {
        systems.SetActive(true);
    }

}

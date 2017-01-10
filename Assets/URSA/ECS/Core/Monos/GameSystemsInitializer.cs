using UnityEngine;
using System;
using System.Collections.Generic;

public class GameSystemsInitializer : MonoBehaviour
{

    GameObject configs, initializers, systems;

    void OnEnable()
    {
        configs = transform.Find("Configs").gameObject;
        if (!configs)
        {
            Debug.LogError("GameSystems: No Configs gameobject found");
            return;
        }

        initializers = transform.Find("Initializers").gameObject;
        if (!initializers)
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

        InitializerSystem.PreInitalization += PreInit;
        InitializerSystem.Initialization += Init;
        InitializerSystem.PostInitialization += PostInit;
    }

    void OnDisable()
    {
        InitializerSystem.PreInitalization -= PreInit;
        InitializerSystem.Initialization -= Init;
        InitializerSystem.PostInitialization -= PostInit;
    }

    void PreInit()
    {
        configs.SetActive(true);
    }

    void Init()
    {
        initializers.SetActive(true);
    }

    void PostInit()
    {
        systems.SetActive(true);
    }

}

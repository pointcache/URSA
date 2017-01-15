using UnityEngine;
using System;
using System.Collections.Generic;

public class LocalSystemsInitializer : MonoBehaviour
{

    GameObject loaders, systems;

    void Awake()
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
    }

    void OnLoadersEnabled()
    {
        loaders.SetActive(true);
    }

    void OnSystemsEnabled()
    {
        systems.SetActive(true);
    }

}

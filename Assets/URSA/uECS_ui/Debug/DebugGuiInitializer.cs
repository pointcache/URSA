using UnityEngine;
using System;
using System.Collections.Generic;

public class DebugGuiInitializer : MonoBehaviour {
    
    private void OnEnable()
    {
        InitializerSystem.UiInitialization += Init;
    }

    void Init()
    {

    }

    private void Update()
    {
        if (!InitializerSystem.FullyInitialized)
            return;
        var input = Pool<InputConfig>.First;

        if (Input.GetKeyDown(input.debugMode))
        {
            var ch = transform.GetChild(0);
            ch.gameObject.SetActive(!ch.gameObject.activeSelf);
        }
    }
}

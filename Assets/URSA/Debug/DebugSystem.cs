using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA;

public class DebugSystem : MonoBehaviour {

    public string debugUiPrefab = "URSA/Debug/DebugUi";
    GameObject current;
    private void OnEnable() {
        Pool<InternalConfig>.First.DebugGUI.OnChanged += SetDebugUi;
        UrsaConsole.RegisterCommand("debug.enableDebugUi", EnableDebugUi);
        UrsaConsole.RegisterCommandWithParameters("test.test1", TestparamsMethod);
    }

    public void SetDebugUi(bool val) {
        if (!current) {
            current = Helpers.Spawn(debugUiPrefab);
        }
        current.SetActive(val);
    }

    void EnableDebugUi() {
        SetDebugUi(true);
    }

    void TestparamsMethod(string[] parameters) {
        int a = Convert.ToInt16(parameters[0]);
        int b = Convert.ToInt16(parameters[1]);
        UrsaConsole.WriteLine((a + b).ToString());
    }
}

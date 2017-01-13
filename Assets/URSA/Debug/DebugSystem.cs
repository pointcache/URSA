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
        URSA.Console.RegisterCommand("debug.enableDebugUi", this.EnableDebugUi);
    }

    public void SetDebugUi(bool val) {
        if (!current) {
            current = Helpers.Spawn(debugUiPrefab);
        }
        current.SetActive(val);
    }

    void EnableDebugUi() {

    }
}

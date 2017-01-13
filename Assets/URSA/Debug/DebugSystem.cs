﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA;

public class DebugSystem : MonoBehaviour {

    public static rVar<string> selectedName = new rVar<string>("selected");

    public static bool debugMode;
    const string debugUiPrefab = "URSA/Debug/DebugUi";
    const string debugOverlayPrefab = "URSA/Debug/DebugMode";

    GameObject debugUi, overlay;
    private void OnEnable() {
        Pool<InternalConfig>.First.DebugGUI.OnChanged += SetDebugUi;
        UrsaConsole.RegisterCommand("debug.enable", EnableDebug);
        UrsaConsole.RegisterCommand("debug.disable", DisableDebug);
    }

    private void Update() {
        var input = Pool<InputConfig>.First;
        var inter = Pool<InternalConfig>.First;
        if (debugMode) {
            if (Input.GetKeyDown(input.debugUi)) {
                inter.DebugGUI.Value = !inter.DebugGUI;
            }
        }
    }

    public void SetDebugUi(bool val) {
        
        debugUi.SetActive(val);
    }

    void EnableDebug() {
        var inter = Pool<InternalConfig>.First;
        if (!debugUi) {
            debugUi = Helpers.Spawn(debugUiPrefab);
        }
        if (!overlay) {
            overlay = Helpers.Spawn(debugOverlayPrefab);
        }
        debugMode = true;
        inter.DebugGUI.Value = false;
    }

    void DisableDebug() {
        var inter = Pool<InternalConfig>.First;
        inter.DebugGUI.Value = false;
        debugMode = false;
        Destroy(debugUi);
        Destroy(overlay);
    }
}
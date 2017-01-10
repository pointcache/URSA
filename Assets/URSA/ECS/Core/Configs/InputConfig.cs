using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class InputConfig : ConfigBase {

    [ConfigVar("input.debugModeKey", "key used to enable debug mode")]
    public r_KeyCode debugMode = new r_KeyCode(KeyCode.F1);

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

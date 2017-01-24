using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

[Config("Input")]
public class InputConfig : ConfigBase {

    [ConsoleVar("input.debugModeKey", "key used to enable debug mode")]
    public r_KeyCode debugUi = new r_KeyCode(KeyCode.F1);

    
    public r_KeyCode Fire = new r_KeyCode(KeyCode.Mouse0);
    public r_KeyCode AltFire = new r_KeyCode(KeyCode.Mouse1);
    public r_KeyCode SpecialMove = new r_KeyCode(KeyCode.LeftShift);

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

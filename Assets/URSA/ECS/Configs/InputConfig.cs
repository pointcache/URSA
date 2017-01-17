using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

[Config("Input")]
public class InputConfig : ConfigBase {

    [ConsoleVar("input.debugModeKey", "key used to enable debug mode")]
    public r_KeyCode debugUi = new r_KeyCode(KeyCode.F1);

    public r_KeyCode paddle1_up = new r_KeyCode(KeyCode.W);

    public r_KeyCode paddle1_down = new r_KeyCode(KeyCode.S);

    public r_KeyCode paddle2_up = new r_KeyCode(KeyCode.UpArrow);

    public r_KeyCode paddle2_down = new r_KeyCode(KeyCode.DownArrow);

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

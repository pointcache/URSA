using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

[Config("Internal", "Edit at your own risk, these are not intended to be changed, but have fun breaking the game :) ")]
public class InternalConfig : ConfigBase
{
    const string inter = "internal.";

    [ConsoleVar(inter+ "updateAllowed", "will the simulation systems update")]
    public r_bool UpdateAllowed = new r_bool();
    [ConsoleVar(inter+ "debugGui", "show debug gui")]
    public r_bool DebugGUI = new r_bool();
    public r_string player_prefab = new r_string( "player");
    public r_string ui_prefab = new r_string( "ui");

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
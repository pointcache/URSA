using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

[Config("Internal", "Edit at your own risk, these are not intended to be changed, but have fun breaking the game :) ")]
public class InternalConfig : ConfigBase
{
    const string inter = "internal.";
    const string debug = "debug.";

    [ConsoleVar(inter+ "updateAllowed", "will the simulation systems update")]
    public r_bool UpdateAllowed = new r_bool();
    [ConsoleVar(debug + "debugGui", "show debug gui")]
    public r_bool DebugGUI = new r_bool();
    [ConsoleVar(debug + "realtimeLogging", "Enable realtime logging, each log message will be written to disk, this is expensive but allows to catch crash bugs.")]
    public r_bool rtLogging = new r_bool();
    public GameObject player_prefab;
    public GameObject gameHud;
    public GameObject gameOverHud;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
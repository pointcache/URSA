using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using URSA;

[Config("Graphics")]
public class GraphicsConfig : ConfigBase
{
    [ConsoleVar("gfx.targetframerate")]
    public r_int TargetFramerate = new r_int(300);







    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
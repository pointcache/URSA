using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using URSA;

public class GraphicsConfig : ConfigBase
{
    [ConfigVar("gfx.targetframerate")]
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
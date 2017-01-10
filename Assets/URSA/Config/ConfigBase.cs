using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA;

public class ConfigBase : ComponentBase
{

    public override void OnEnable()
    {
        base.OnEnable();
        ConfigSystem.RegisterConfig(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        ConfigSystem.UnregisterConfig(this);
    }
}

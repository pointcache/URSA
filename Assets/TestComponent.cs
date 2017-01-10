using UnityEngine;
using System;
using System.Collections.Generic;

public class TestComponent : ComponentBase {
    [Header("----------------------------------")]
    public Data data;
    [Serializable]
    public class Data : ComponentData
    {
        public CompRef testReference = new CompRef();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

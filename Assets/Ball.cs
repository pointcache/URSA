using UnityEngine;
using System;
using System.Collections.Generic;

public class Ball : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

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

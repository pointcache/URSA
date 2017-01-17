using UnityEngine;
using System;
using System.Collections.Generic;

public class Rotation : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

    }

    public float currentRotation;

    public override void OnEnable()
    {
        Entity.GetEntityComponent<Rotation>();
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

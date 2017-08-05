using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class ColorComponent06 : ECSComponent {

    public Color color;

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

    }

    public override void OnEnable() {
        base.OnEnable();

        Entity.GetUnityComponent<MeshRenderer>().material.color = color;
    }
}

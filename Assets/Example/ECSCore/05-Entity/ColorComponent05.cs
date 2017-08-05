using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class ColorComponent05 : ECSComponent {

    public Color color
    {
        set {
            GetComponent<MeshRenderer>().material.color = value;
        }
    }



    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

    }

    public override void OnEnable() {
        base.OnEnable();

        
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class Bag : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

    }

    public int Capacity = 10;
    public string Name = "Small Bag";
    

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

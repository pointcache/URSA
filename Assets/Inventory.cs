using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventory : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public CompRef weapon1;
        public CompRef weapon2;
        public CompRef weapon3;
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

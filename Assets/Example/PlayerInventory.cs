using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInventory : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public CompRef Weapon = new CompRef();
        public CompRef Armor = new CompRef();

        public CompRef Bag = new CompRef();
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

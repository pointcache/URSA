using UnityEngine;
using System;
using System.Collections.Generic;

public class Weapon : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public r_float durability = new r_float(100f);
    }

    public float damage;
    public float impulse;
    public float range;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class Weapon : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public r_string name = new r_string("name");
        public r_int damage = new r_int(100);
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

using UnityEngine;
using System;
using System.Collections.Generic;

public class TestComponent : ComponentBase {
    [Header("--------------------------------------------------------------------")]
    public Data serializedData;
    [Serializable]
    public class Data : SerializedData {
        public CompRef testReference = new CompRef();
        public r_float testFloat = new r_float(100f);

    }

    public r_float testFloat2 = new r_float(200f);
    public r_float testFloat3 = new r_float(3100f);
    public r_float testFloat4 = new r_float(1400f);
    public r_bool testbool = new r_bool(true);
    [SerializeField]
    private r_bool Ptestbool = new r_bool(true);


    public float asdasd = 88888f;
    public int kxzjckzxc = 999;
    public Color color = Color.red;
    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnDisable() {
        base.OnDisable();
    }

    public override SerializedData GetData() {
        return serializedData;
    }

    public override void SetData(object _data) {
        serializedData = _data as Data;
    }
}

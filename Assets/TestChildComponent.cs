using UnityEngine;
using System.Collections.Generic;

public class TestChildComponent : TestComponent {

    public Data data;
    [System.Serializable]
    public class Data : SerializedData
    {
        public float somefloat;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        data.somefloat = Random.Range(0f, 10f);
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

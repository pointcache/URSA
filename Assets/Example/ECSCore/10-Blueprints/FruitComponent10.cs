using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class FruitComponent10 : ECSComponent {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public float size;
        public float growthRate;
        public float minGrowthRate = 0.1f;
        public float maxGrowthRate = 1f;
    }

    protected override void Initialize() {
        base.Initialize();

        data.growthRate = UnityEngine.Random.Range(data.minGrowthRate, data.maxGrowthRate);
    }

    protected override void OnDeserialized() {
        base.OnDeserialized();

        Entity.transform.localScale = Vector3.one * data.size;
    }
}

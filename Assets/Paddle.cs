using UnityEngine;
using System;
using System.Collections.Generic;

public class Paddle : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public Player player;
        public float min, max;
    }

    public enum Player {
        one, two

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

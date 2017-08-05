using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class ExampleComponent08 : ECSComponent {

    public float somefloat = 10f;

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

        public string serializedString = "somestring";

        public float serializedFloat = 20f;

    }
}

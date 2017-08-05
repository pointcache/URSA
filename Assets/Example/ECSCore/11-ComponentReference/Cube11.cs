using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class Cube11 : ECSComponent {

	

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public CompRef sphere;
    }
}

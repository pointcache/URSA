using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class CompWithSerializedData07 : ECSComponent {

    

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

    }
}

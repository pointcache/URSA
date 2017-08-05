using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class ExampleConfig : ConfigBase {

    public int GlobalInt = 100;
    public string SomeString;
    public Dictionary<int, string> dictWillBeSerializedAsWell = new Dictionary<int, string>();
}

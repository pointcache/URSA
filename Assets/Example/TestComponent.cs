using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;

public class TestComponent : ComponentBase {

    public Data data;
	public class Data : SerializedData {
        public int SomeInt;
    }
}

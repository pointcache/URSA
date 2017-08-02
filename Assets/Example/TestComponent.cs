using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;
using URSA.Serialization;

public class TestComponent : ECSComponent {

    public Data data = new Data();
    [System.Serializable]
	public class Data : SerializedData {
        public int SomeInt;
        public int SomeInt2;
        public int SomeInt3;
        public int SomeInt4;

        public CompRef componentReference = new CompRef();
        public CompRef SomeOtherComponent = new CompRef();

    }
}


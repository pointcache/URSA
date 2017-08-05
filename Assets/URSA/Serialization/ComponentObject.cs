namespace URSA.Serialization
{

    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ComponentObject 
    {
        public int component_ID;
        public bool initialized;
        public bool enabled;
        public SerializedData data;
    }
}
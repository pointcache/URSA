namespace URSA.Save
{

    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ComponentObject 
    {
        public string entity_ID;
        public string component_ID;

        public ComponentData data;
    }
}
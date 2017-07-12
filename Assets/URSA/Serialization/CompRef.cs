namespace URSA.Serialization {


    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using FullSerializer;

    [Serializable, fsObject(Processor = typeof(CompRefSerializationProcessor))]
    public class CompRef {

        [HideInInspector]
        public string entity_ID;
        [HideInInspector]
        public string component_ID;
        [HideInInspector]
        public bool isNull;
        [HideInInspector]
        public string entityName;

        public ComponentBase target;

        public static implicit operator ComponentBase(CompRef var) {
            return var.target;
        }
    }

    public class CompRef<T> : CompRef where T : ComponentBase {

        public static implicit operator T(CompRef<T> var) {
            return var.target as T;
        }
    }
}
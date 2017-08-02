namespace URSA {


    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using FullSerializer;
    using URSA.Serialization;

    [Serializable, fsObject(Processor = typeof(CompRefSerializationProcessor))]
    public class CompRef {

        [HideInInspector]
        public int entity_ID;
        [HideInInspector]
        public int component_ID;
        [HideInInspector]
        public bool isNull;
        [HideInInspector]
        public string entityName;

        public ECSComponent component;

        public static implicit operator ECSComponent(CompRef var) {
            return var.component;
        }
    }

    public class CompRef<T> : CompRef where T : ECSComponent {

        public static implicit operator T(CompRef<T> var) {
            return var.component as T;
        }
    }
}
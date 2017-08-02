namespace URSA.Serialization {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using FullSerializer;
    using URSA;

    public class CompRefSerializationProcessor : fsObjectProcessor {

        public static bool blueprint;

        public static List<CompRef> refs;

        public override void OnBeforeSerialize(Type storageType, object instance) {

            CompRef cref = instance as CompRef;
            ECSComponent comp = cref.component;
            if (comp != null) {
                cref.isNull = false;
                if (blueprint)
                    cref.entity_ID = comp.Entity.blueprintID;
                else
                    cref.entity_ID = comp.Entity.ID;

                cref.component_ID = comp.componentID;
                cref.entityName = comp.Entity.name;
            }
            else {
                cref.isNull = true;

            }
            cref.component = null;
        }

        public override void OnAfterDeserialize(Type storageType, object instance) {
            base.OnAfterDeserialize(storageType, instance);

            CompRefSerializationProcessor.refs.Add(instance as CompRef);
        }
    }

}
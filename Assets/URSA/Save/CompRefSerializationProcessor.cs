using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;

public class CompRefSerializationProcessor : fsObjectProcessor {

    public override void OnBeforeSerialize(Type storageType, object instance) {

        CompRef cref = instance as CompRef;
        ComponentBase comp = cref.Value;
        if (comp != null) {
            cref.isNull = false;
            cref.entity_ID = comp.Entity.ID;
            cref.component_ID = comp.ID;
            cref.entityName = comp.Entity.name;
        } else
            cref.isNull = true;
        cref.setValueDirectly(null);
    }
}

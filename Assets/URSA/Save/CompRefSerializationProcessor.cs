using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;

public class CompRefSerializationProcessor : fsObjectProcessor {


    //public override bool CanProcess(Type type) {
    //    return type == typeof(CompRef); // process only ints
    //}

    //public override void OnAfterDeserialize(Type storageType, object instance) {
    //    SaveSystem.injectionList.Add(instance as CompRef);
    //}

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

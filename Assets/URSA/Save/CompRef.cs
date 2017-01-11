using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;

[Serializable, fsObject(Processor = typeof(CompRefSerializationProcessor))]
public class CompRef : rVar<ComponentBase> {
    [HideInInspector]
    public string entity_ID;
    [HideInInspector]
    public string component_ID;
    [HideInInspector]
    public bool isNull;
	public CompRef() : base() { }
    public CompRef(ComponentBase initialValue) : base(initialValue) { }
    public static implicit operator ComponentBase(CompRef var)
    {
        return var.Value;
    }   
}
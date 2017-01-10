using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;

[Serializable, fsObject(Processor = typeof(CompRefSerializationProcessor))]
public class CompRef : rVar<ComponentBase> {

    public string entity_ID;
    public string component_ID;
	public CompRef() : base() { }
    public CompRef(ComponentBase initialValue) : base(initialValue) { }
    public static implicit operator ComponentBase(CompRef var)
    {
        return var.Value;
    }   
}
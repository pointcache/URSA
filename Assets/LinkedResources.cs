using UnityEngine;
using System;
using System.Collections.Generic;

public class LinkedResources : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {

    }

    public GameObject gfx3d;
    public GameObject gfxInventoryIcon;
    public GameObject prefabWorldDrop;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}

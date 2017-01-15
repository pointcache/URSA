#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Used to id objects, MAKE ABSOLUTELY SURE THAT THIS COMPONENT IS ATTACHED TO THE ROOT OF THE PREFAB OR OBJECT
/// </summary>
[DisallowMultipleComponent]
public class PrefabType : MonoBehaviour {

    public ObjType type;

    public enum ObjType {
        entity,
        @static,
        light,
        volume,
        fx,
        npc
    }

}

#endif
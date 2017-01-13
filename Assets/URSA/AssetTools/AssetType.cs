#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Used to id objects, MAKE ABSOLUTELY SURE THAT THIS COMPONENT IS ATTACHED TO THE ROOT OF THE PREFAB OR OBJECT
/// </summary>
[DisallowMultipleComponent]
public class AssetType : MonoBehaviour {

    public ObjType type;

    public enum ObjType {
        dynamic,
        @static,
        light,
        volume,
        fx,
        enemy,
        npc
    }
}

#endif
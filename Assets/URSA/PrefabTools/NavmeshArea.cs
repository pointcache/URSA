#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

//Marks asset for this area on parse, all objects by default are marked as non walkable
public class NavmeshArea : MonoBehaviour {

    public bool ApplyToChildren;
    public Area area;
    public enum Area {
        walkable = 0,
        notWalkable = 1,
        jump = 2
    }
}

#endif
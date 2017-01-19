#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

public class NavmeshIgnore : MonoBehaviour {

    /* simply acts as a flag for assets parser to unset navigation static for this object 
     add it on top level it will recursively change the children*/
}

#endif
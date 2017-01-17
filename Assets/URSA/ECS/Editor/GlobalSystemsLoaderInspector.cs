using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GlobalSystemsLoader), true)]
public class GlobalSystemsLoaderInspector : Editor {

    public override void OnInspectorGUI() {
        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/System");
        base.OnInspectorGUI();
    }
}

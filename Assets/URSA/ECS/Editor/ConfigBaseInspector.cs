using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ConfigBase), true)]
public class ConfigBaseInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/ConfigBaseLabel", "https://github.com/pointcache/URSA/wiki/Component");
        base.OnInspectorGUI();
    }
}

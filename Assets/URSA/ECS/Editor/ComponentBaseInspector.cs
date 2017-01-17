using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ComponentBase), true)]
public class ComponentBaseInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/ComponentBaseLabel", "https://github.com/pointcache/URSA/wiki/Component");
        base.OnInspectorGUI();
    }
}

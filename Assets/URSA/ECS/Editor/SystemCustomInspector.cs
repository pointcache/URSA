using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(SystemBase), true)]
public class SystemCustomInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/SystemLabel", "https://github.com/pointcache/URSA/wiki/System");
        base.OnInspectorGUI();
    }
}

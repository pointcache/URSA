using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Entity), true)]
public class EntityCustomInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/EntityLabel", "https://github.com/pointcache/URSA/wiki/Entity");
        base.OnInspectorGUI();
    }
}

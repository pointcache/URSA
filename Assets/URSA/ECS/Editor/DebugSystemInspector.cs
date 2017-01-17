using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugSystem), true)]
public class DebugSystemInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/Debug");
        base.OnInspectorGUI();
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(URSA.Console), true)]
public class ConsoleInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/Console");
        base.OnInspectorGUI();
    }
}

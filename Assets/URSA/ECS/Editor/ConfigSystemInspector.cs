using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(URSA.ConfigSystem), true)]
public class ConfigSystemInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/Configs");
        base.OnInspectorGUI();
    }
}

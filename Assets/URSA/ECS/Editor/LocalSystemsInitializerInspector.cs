using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocalSystemsInitializer), true)]
public class LocalSystemsInitializerInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/Local-Systems");
        base.OnInspectorGUI();
    }
}

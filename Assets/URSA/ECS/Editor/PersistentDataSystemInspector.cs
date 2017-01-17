using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PersistentDataSystem), true)]
public class PersistentDataSystemInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/PersistentData");
        base.OnInspectorGUI();
    }
}

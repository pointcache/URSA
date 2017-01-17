using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Database), true)]
public class DatabaseInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/Database");
        base.OnInspectorGUI();
    }
}

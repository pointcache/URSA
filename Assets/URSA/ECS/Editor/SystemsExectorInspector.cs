using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SystemsExecutor), true)]
public class SystemsExectorInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/SystemsExecutor");
        base.OnInspectorGUI();
    }
}

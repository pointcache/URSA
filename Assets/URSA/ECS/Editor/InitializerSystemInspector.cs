using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InitializerSystem), true)]
public class InitializerSystemInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/InitializerSystem");
        base.OnInspectorGUI();
    }
}

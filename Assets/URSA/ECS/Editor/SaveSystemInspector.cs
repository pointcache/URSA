using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveSystem), true)]
public class SaveSystemInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/SaveSystem");
        base.OnInspectorGUI();
    }
}

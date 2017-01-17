using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentPoolSystem), true)]
public class PoolInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/ComponentPool");
        base.OnInspectorGUI();
    }
}

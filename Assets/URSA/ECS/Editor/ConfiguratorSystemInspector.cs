using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConfiguratorSystem), true)]
public class ConfiguratorSystemInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/Configurator-System");
        base.OnInspectorGUI();
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InitializationEventsReceiver), true)]
public class InitializationEventReceiverInspector : Editor {

    public override void OnInspectorGUI() {

        URSAInspectorHelper.DrawLabel("URSA/Img/HelpLabel", "https://github.com/pointcache/URSA/wiki/InitializationEventsReceiver");
        base.OnInspectorGUI();
    }
}

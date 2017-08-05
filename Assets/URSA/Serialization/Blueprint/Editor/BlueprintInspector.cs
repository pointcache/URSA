namespace URSA.Serialization.Blueprints.Editor {

    using UnityEngine;
    using UnityEditor;
    using URSA.Utility;

    [CustomEditor(typeof(BlueprintLoader))]
    public class BlueprintInspector : Editor {

        public static string LastPath = "/Resources/";
        public override void OnInspectorGUI() {
            BlueprintLoader t = target as BlueprintLoader;

            if (t.Blueprint == null) {
                GUIStyle style = GUI.skin.GetStyle("Label");
                Color c = style.normal.textColor;
                style.normal.textColor = Color.red;

                GUILayout.Label("THE BLUEPRINT IS NULL", style);
                style.normal.textColor = c;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as")) {
                save_as();
            }
            if (GUILayout.Button("Save")) {
                if (t.Blueprint == null)
                    save_as();
                else {
                    string path = AssetDatabase.GetAssetPath(t.Blueprint);
                    var bp = t.Save();
                    if (bp == null)
                        return;
                    SerializationHelper.Serialize(bp, path, true);
                    AssetDatabase.Refresh();
                }

            }
            if (GUILayout.Button("Load")) {
                t.Load();
            }
            if (GUILayout.Button("Clear")) {
                if(t.Blueprint == null) {
                    Debug.LogError("Clearing without blueprint will result in data loss, can't do that.");
                }
                else
                    t.transform.DestroyChildren();
            }
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        void save_as() {
            BlueprintLoader t = target as BlueprintLoader;

            string path = EditorUtility.SaveFilePanel("Create new Blueprint", LastPath, t.name, "txt");
            LastPath = path;
            if (path == "")
                return;
            var bp = t.Save();
            SerializationHelper.Serialize(bp, path, true);
            AssetDatabase.Refresh();
            string assetpath = path.Remove(0, path.IndexOf("Assets/"));
            t.Blueprint = AssetDatabase.LoadAssetAtPath(assetpath, typeof(TextAsset)) as TextAsset;
            //t.Blueprint = Resources.Load(path.ClearPathToResources().RemoveExtension()) as TextAsset;
        }
    }

}
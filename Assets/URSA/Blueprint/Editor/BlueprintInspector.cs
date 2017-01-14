using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using URSA.Save;

[CustomEditor(typeof(BlueprintLoader))]
public class BlueprintInspector : Editor {



    public static string LastPath = "/Resources/";
    public override void OnInspectorGUI() {
        BlueprintLoader t = target as BlueprintLoader;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save as")) {
            save_as();
        }
        if (GUILayout.Button("Save")) {
            if (t.blueprint == null)
                save_as();
            else {
                string path = AssetDatabase.GetAssetPath(t.blueprint);
                var bp = t.Save();
                SerializationHelper.Serialize(bp, path, true);
                AssetDatabase.Refresh();
            }

        }
        if (GUILayout.Button("Load")) {
            if (t.blueprint == null)
                return;
            else {
                t.transform.DestroyChildren();
                SaveSystem.LoadBlueprint(t.blueprint.text, t.transform);
            }
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
        t.blueprint = Resources.Load(path.ClearPathToResources().RemoveExtension()) as TextAsset;
    }
}

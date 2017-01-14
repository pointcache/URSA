using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Blueprint))]
public class BlueprintInspector : Editor {



    public static string LastPath = "/Resources/";
    public override void OnInspectorGUI() {
        Blueprint t = target as Blueprint;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save as")) {
            save_as();
        }
        if (GUILayout.Button("Save")) {
            if (t.dataFile == null)
                save_as();
            else {
                string path = AssetDatabase.GetAssetPath(t.dataFile);
                var bp = t.Save();
                SerializationHelper.Serialize(bp, path, true);
                AssetDatabase.Refresh();
            }

        }
        if (GUILayout.Button("Load")) {

        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }

    void save_as() {
        Blueprint t = target as Blueprint;

        string path = EditorUtility.SaveFilePanel("Create new Blueprint", LastPath, "blueprint", "txt");
        LastPath = path;
        if (path == "")
            return;
        var bp = t.Save();
        SerializationHelper.Serialize(bp, path, true);
        AssetDatabase.Refresh();
        t.dataFile = Resources.Load(path.ClearPathToResources().RemoveExtension()) as TextAsset;
    }
}

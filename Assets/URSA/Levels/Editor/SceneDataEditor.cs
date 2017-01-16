using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
[CustomEditor(typeof(SceneData))]
public class SceneDataEditor : Editor
{
    UnityEngine.Object targetscene;

    public override void OnInspectorGUI() {

        SceneData t = target as SceneData;

        GUILayout.BeginHorizontal();
        targetscene = EditorGUILayout.ObjectField("RelinkScene", targetscene, typeof(SceneAsset), false);
        if (GUILayout.Button("Relink")) {
            if(targetscene == null) {
                return;
            }
            LevelsSystemEditor.RelinkDataToScene(t, targetscene as SceneAsset);
            targetscene = null;
        }
        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}




using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using pointcache.utility;

[CustomEditor(typeof(HierarchyVisualizer))]
public class HierarchyVisualizerEditor : Editor {

    public override void OnInspectorGUI()
    {
        HierarchyVisualizer t = target as HierarchyVisualizer;
        if (GUILayout.Button("On"))
        {
            t.On();
        }

        if (GUILayout.Button("Off"))
        {
            t.Off();
        }
        if (GUILayout.Button("Toggle Mesh"))
        {
            t.ToggleMesh();
        }
    }
}

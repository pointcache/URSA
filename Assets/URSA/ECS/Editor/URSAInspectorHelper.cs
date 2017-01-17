using UnityEngine;
using System;
using System.Collections.Generic;

public static class URSAInspectorHelper  {

    public static void DrawLabel(string img, string web) {
        //GUILayout.Label((Texture2D)Resources.Load("URSA/Img/ComponentBaseLabel", typeof(Texture2D)));
        GUIStyle style = new GUIStyle();
        Color defcol = GUI.backgroundColor;
        GUI.backgroundColor = Color.clear;
        if (GUILayout.Button((Texture2D)Resources.Load(img, typeof(Texture2D)), style ,
            GUILayout.Height(URSAConstants.editor_gui_label_height), GUILayout.Width(URSAConstants.editor_gui_label_width), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false))) {
             Application.OpenURL(web);

        }
        GUI.backgroundColor = defcol;
    }
}

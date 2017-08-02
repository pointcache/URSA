#if UNITY_EDITOR


using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using URSA.Utility;

[CustomPropertyDrawer(typeof(NotEditableStringAttribute))]
public class NotEditableStringDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {

        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
        GUIStyle style = GUI.skin.GetStyle("Button");
        float origWidth = style.fixedWidth;
        style.fixedWidth = 40;
        Rect offset = position;
        offset.x += position.width - 40f;
        if (GUI.Button(offset, "copy", style)) {
            GUIUtility.systemCopyBuffer = prop.stringValue;
        }
        style.fixedWidth = origWidth;
    }

    void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label) {
        position.width -= 40f;
        EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));



    }
}

[CustomPropertyDrawer(typeof(NotEditableIntAttribute))]
public class NotEditableIntDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {

        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
        GUIStyle style = GUI.skin.GetStyle("Button");
        float origWidth = style.fixedWidth;
        style.fixedWidth = 40;
        Rect offset = position;
        offset.x += position.width - 40f;
        if (GUI.Button(offset, "copy", style)) {
            GUIUtility.systemCopyBuffer = prop.intValue.ToString();
        }
        style.fixedWidth = origWidth;

    }

    void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label) {
        position.width -= 40f;
        EditorGUI.LabelField(position, label, new GUIContent(prop.intValue.ToString()));
    }

}

#endif
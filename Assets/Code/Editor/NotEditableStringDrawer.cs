using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(NotEditableStringAttribute))]
public class NotEditableStringDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)    {

        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
    }
 
    void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)    {
        EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
    }
}
namespace URSA.Utility.NotEditableString {
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
            offset.x += 80f;
            if (GUI.Button(offset, "copy", style)) {
                GUIUtility.systemCopyBuffer = prop.stringValue;
            }
            style.fixedWidth = origWidth;
        }

        void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label) {
            EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));



        }
    }

}
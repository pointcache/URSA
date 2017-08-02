namespace URSA.Serialization.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;

    [UnityEditor.CustomPropertyDrawer(typeof(CompRef))]
    public class CompRefDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            var targetSerializedProperty = property.FindPropertyRelative("component");
            UnityEditor.EditorGUI.PropertyField(position, targetSerializedProperty, label, includeChildren: true);
            EditorGUI.EndChangeCheck();
        }
    }
}
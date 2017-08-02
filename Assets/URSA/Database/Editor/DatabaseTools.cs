namespace URSA.Database.Editor   {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;
    using URSA.ECS;
    using URSA.Internal;

    public class DatabaseTools : EditorWindow {

        // Add menu item named "My Window" to the Window menu
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_DATABASE + "/DatabaseTools")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(DatabaseTools));

        }

        int id_search = 0;
        int scene_search = 0;
        string  name_search = "name";

        void OnGUI() {
            GUILayout.Label("Search entity by id");
            GUILayout.BeginHorizontal();
            id_search = EditorGUILayout.IntField(id_search);
            if (GUILayout.Button("Paste")) {
                id_search = Convert.ToInt16(GUIUtility.systemCopyBuffer);
            }
            if (GUILayout.Button("Find")) {
                var pref = EntityDatabase.GetPrefab(id_search);
                Selection.activeObject = pref;
            }
            GUILayout.EndHorizontal();


            GUILayout.Label("Search entities in scene by id");
            GUILayout.BeginHorizontal();
            scene_search = EditorGUILayout.IntField(scene_search);
            if (GUILayout.Button("Paste")) {
                scene_search = Convert.ToInt16(GUIUtility.systemCopyBuffer);
            }
            if (GUILayout.Button("Find")) {
                List<GameObject> selection = new List<GameObject>();
                var entities = GameObject.FindObjectsOfType<Entity>();
                foreach (var e in entities) {
                    if (e.entityID == scene_search) {
                        selection.Add(e.gameObject);
                    }
                }
                Selection.objects = selection.ToArray();

            }
            GUILayout.EndHorizontal();


            GUILayout.Label("Regex search by name");
            GUILayout.BeginHorizontal();
            name_search = GUILayout.TextArea(name_search);
            if (GUILayout.Button("Paste")) {
                name_search = GUIUtility.systemCopyBuffer;
            }
            if (GUILayout.Button("Find")) {
                List<GameObject> selection = new List<GameObject>();
                var entities = Resources.LoadAll<Entity>(URSASettings.Current.DatabaseRootFolder + "/");

                var names = entities.Select(x => x.name).ToArray();

                for (int i = 0; i < names.Length; i++) {
                    if (System.Text.RegularExpressions.Regex.IsMatch(names[i], name_search, System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
                        selection.Add(entities[i].gameObject);
                    }
                }
                Selection.objects = selection.ToArray();

            }
            GUILayout.EndHorizontal();
        }
    }

}
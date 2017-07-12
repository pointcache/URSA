namespace URSA.Serialization.Editor {
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;
    using URSA.ECS;

    public class DatabaseTools : EditorWindow {

        // Add menu item named "My Window" to the Window menu
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_DATABASE + "/DatabaseTools")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(DatabaseTools));

        }

        string id_search = "paste id";
        string scene_search = "paste id";
        string name_search = "paste name";

        void OnGUI() {
            GUILayout.Label("Search entity by id");
            GUILayout.BeginHorizontal();
            id_search = GUILayout.TextArea(id_search);
            if (GUILayout.Button("Paste")) {
                id_search = GUIUtility.systemCopyBuffer;
            }
            if (GUILayout.Button("Find")) {
                var pref = EntityDatabase.GetPrefab(id_search);
                Selection.activeObject = pref;
            }
            GUILayout.EndHorizontal();


            GUILayout.Label("Search entities in scene by id");
            GUILayout.BeginHorizontal();
            scene_search = GUILayout.TextArea(scene_search);
            if (GUILayout.Button("Paste")) {
                scene_search = GUIUtility.systemCopyBuffer;
            }
            if (GUILayout.Button("Find")) {
                List<GameObject> selection = new List<GameObject>();
                var entities = GameObject.FindObjectsOfType<Entity>();
                foreach (var e in entities) {
                    if (e.database_ID == scene_search) {
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
                var entities = Resources.LoadAll<Entity>(URSASettings.current.DatabaseRootFolder + "/");

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
namespace URSA.Utility {
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using URSA.ECS;
    using URSA.SceneManagement.Editor;
    using System.Collections.Generic;
    using URSA.Internal;
    using URSA.EntityDatabase;

    public static class URSAEditorTools {


        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + "/UpdateEverything", priority = 1)]
        public static void UpdateEverything() {
            var settings = URSASettings.current;

            if (settings.RebuildDatabase)
                Database.RebuildWithoutReloadOfTheScene();
            if (settings.CollectSceneData)
                ScenesSystem.CollectLevelsData();
            if (settings.SaveAndReloadScene) {

                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                var scene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
            }
        }

        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + "/AddSystems", priority = 2)]
        public static void AddSystems() {
            var settings = URSASettings.current;
            var gl = GameObject.Find(URSAConstants.SYSTEMS_GLOBAL_NAME) as GameObject;
            if (!gl) {
                if (settings.CustomGlobalSystemsPrefab) {
                    gl = Helpers.SpawnEditor(settings.CustomGlobalSystemsPrefab);
                }
                else if (settings.GlobalSystemsTemplate)
                    gl = Helpers.SpawnEditor(settings.GlobalSystemsTemplate);
                else
                    gl = Helpers.SpawnEditor("URSA/" + URSAConstants.SYSTEMS_GLOBAL_NAME);
                Debug.Log(URSAConstants.SYSTEMS_GLOBAL_NAME + " spawned");
            }
            else
                Debug.LogError(URSAConstants.SYSTEMS_GLOBAL_NAME + " already exists ");
            var sc = GameObject.Find(URSAConstants.SYSTEMS_LOCAL_NAME) as GameObject;
            if (!sc) {
                if (settings.CustomLocalSystemsPrefab) {
                    sc = Helpers.SpawnEditor(settings.CustomLocalSystemsPrefab);
                }
                else if (settings.LocalSystemsTemplate)
                    sc = Helpers.SpawnEditor(settings.LocalSystemsTemplate);
                else
                    sc = Helpers.SpawnEditor("URSA/" + URSAConstants.SYSTEMS_LOCAL_NAME);

                Debug.Log(URSAConstants.SYSTEMS_LOCAL_NAME + " spawned");
            }
            else
                Debug.LogError(URSAConstants.SYSTEMS_LOCAL_NAME + " already exists ");
            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();
        }

        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_TOOLS + "/Select Missing Scripts")]
        static void SelectMissing(MenuCommand command) {
            Transform[] ts = GameObject.FindObjectsOfType<Transform>();
            List<GameObject> selection = new List<GameObject>();
            foreach (Transform t in ts) {
                Component[] cs = t.gameObject.GetComponents<Component>();
                foreach (Component c in cs) {
                    if (c == null) {
                        selection.Add(t.gameObject);
                    }
                }
            }
            Selection.objects = selection.ToArray();
        }
    }
#endif 
}
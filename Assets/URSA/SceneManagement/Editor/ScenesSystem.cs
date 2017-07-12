namespace URSA.SceneManagement.Editor {

    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine.SceneManagement;
    using UnityEngine.Events;
    using UnityEditor.SceneManagement;
    using System.Linq;
    using URSA.ECS;
    using URSA.Internal;
    using URSA.Utility;

    public static class ScenesSystem     {

        public static event Action<SceneAsset, SceneData> OnSceneDataCreated = delegate { };
        public static event Action<SceneData> OnSceneDataCollected = delegate { };

        private const string SaveSystem_MENU = "Assets/SaveSystem/";
        private const string LEVELDATA_NEW = "New Data";


        [MenuItem(SaveSystem_MENU + LEVELDATA_NEW)]
        public static void CreateLevelData() {
            SceneAsset scene = Selection.activeObject as SceneAsset;
            if (!scene)
                return;

            var assets = AssetDatabase.FindAssets(scene.name + "_data");
            if (assets.Length > 0) {
                Debug.LogError("Data file for this scene already exists, please use the existing one or recreate it.");
                return;
            }
            makeData(scene);
            Debug.Log("Created level data for scene :" + scene.name);
        }

        static SceneData makeData(SceneAsset asset) {

            SceneData data = (SceneData)CreateAsset<SceneData>(asset.name + "_data");
            processData(data, asset);
            return data;
        }

        static void processData(SceneData data, SceneAsset asset) {
            var dataasset = AssetDatabase.GetAssetPath(data);
            AssetDatabase.RenameAsset(dataasset, asset.name + "_data");
            data.name = asset.name;
            data.scene = asset.name;
            data.levelname = asset.name;
            data.NiceName = asset.name;
            OnSceneDataCreated(asset, data);
            CollectLevelsData();
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        public static void RelinkDataToScene(SceneData data, SceneAsset asset) {
            processData(data, asset);
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static ScriptableObject CreateAsset<T>(string filename) where T : ScriptableObject {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "") {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + filename + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }


        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_LEVELS + "/CollectSceneData")]
        public static void CollectLevelsData() {
            SceneDataCollector collector = Resources.Load(URSAConstants.PATH_ADDITIONAL_DATA + "/SceneDataCollector") as SceneDataCollector;
            collector.scenes.Clear();

            HashSet<string> set = new HashSet<string>();

            var scenedatas = AssetDatabase.FindAssets("t:SceneData");
            foreach (var s in scenedatas) {
                SceneData scenedata = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(SceneData)) as SceneData;
                if (scenedata.ID == "" || scenedata.ID == null) {
                    scenedata.ID = Helpers.GetUniqueID(set);
                }
                set.Add(scenedata.ID);
                collector.scenes.Add(scenedata);
                string path = AssetDatabase.GetAssetPath(scenedata);
                path = Directory.GetParent(path).ToString();
                scenedata.scenePath = path.Replace("Assets/", "") + "/" + scenedata.scene;
                scenedata.sceneManagerPath = "Assets/" + scenedata.scenePath + ".unity";
                string absolutepath = Application.dataPath + "/" + scenedata.scenePath + ".unity";
                var scene = File.Exists(absolutepath);

                if (!scene) {
                    Debug.LogError("Target scene was not found.", scenedata);
                }

                Scene editorScene = EditorSceneManager.GetActiveScene();
                if (scenedata.scenePath == editorScene.path.Replace("Assets/", "").RemoveExtension()) {
                    OnSceneDataCollected(scenedata);
                }

                EditorUtility.SetDirty(scenedata);
            }

            EditorUtility.SetDirty(collector);
            AssetDatabase.SaveAssets();

        }
    }

}
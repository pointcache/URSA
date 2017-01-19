using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEditor.SceneManagement;
using System.Linq;

public static class LevelsSystemEditor {

    public static event Action<SceneAsset, SceneData> OnSceneDataCreated = delegate { };
    public static event Action<SceneData> OnSceneDataCollected = delegate { };

    private const string SaveSystem_MENU = "Assets/SaveSystem/";
    private const string LEVELDATA_NEW = "New Data";


    [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_LEVELS + "/OrganizeLevel")]
    public static void LevelOrganizer() {

        var settings = Helpers.FindScriptableObject<PrefabToolsSettings>();
        GameObject npc = null;
        GameObject entity = null;
        GameObject light = null;
        GameObject fx = null;
        GameObject volume = null;
        GameObject @static = null;

        var entities = GameObject.Find(settings.lv_entities_root) as GameObject;
        if (entities) {

            npc = GameObject.Find(settings.lv_npc) as GameObject;
            if (!npc) {
                Debug.LogError(settings.lv_npc + " not found ");
            }

            entity = GameObject.Find(settings.lv_entity) as GameObject;
            if (!entity) {
                Debug.LogError(settings.lv_entity + " not found ");
            }

        } else {
            Debug.LogError(settings.lv_entities_root + " not found ");
            return;
        }

        var stat = GameObject.Find(settings.lv_static_root) as GameObject;
        if (stat) {
            light = GameObject.Find(settings.lv_lights) as GameObject;
            if (!light) {
                Debug.LogError(settings.lv_lights + " not found ");
            }
            fx = GameObject.Find(settings.lv_fx) as GameObject;
            if (!fx) {
                Debug.LogError(settings.lv_fx + " not found ");
            }
            volume = GameObject.Find(settings.lv_volumes) as GameObject;
            if (!volume) {
                Debug.LogError(settings.lv_volumes + " not found ");
            }
            @static = GameObject.Find(settings.lv_static) as GameObject;
            if (!@static) {
                Debug.LogError(settings.lv_static + " not found ");
            }
        } else {
            Debug.LogError(settings.lv_static_root + " not found ");
            return;
        }


        var all = GameObject.FindObjectsOfType<PrefabType>();

        foreach (var obj in all) {
            if (obj.transform.parent != null && (obj.transform.parent.GetComponentInParents<Entity>() || obj.transform.parent.GetComponentInParents<BlueprintLoader>()) || obj.OrganizerIgnore) {
                continue;
            }


            switch (obj.type) { 
                case PrefabType.ObjType.entity:
                    obj.transform.SetParent(entity.transform);
                    break;
                case PrefabType.ObjType.@static:
                    obj.transform.SetParent(@static.transform);
                    break;
                case PrefabType.ObjType.light:
                    obj.transform.SetParent(light.transform);
                    break;
                case PrefabType.ObjType.volume:
                    obj.transform.SetParent(volume.transform);
                    break;
                case PrefabType.ObjType.fx:
                    obj.transform.SetParent(fx.transform);
                    break;
                case PrefabType.ObjType.npc:
                    obj.transform.SetParent(npc.transform);
                    break;
                default:
                    break;
            }
        }

        Debug.Log(all.Length + " objects organized.");

        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
    }

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
        } else if (Path.GetExtension(path) != "") {
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
            if(scenedata.ID == "" || scenedata.ID == null) {
                scenedata.ID = Database.get_unique_id(set);
            }
            set.Add(scenedata.ID);
            collector.scenes.Add(scenedata);
            string path = AssetDatabase.GetAssetPath(scenedata);
            path = Directory.GetParent(path).ToString();
            scenedata.scenePath = path.Replace("Assets/", "") + "/" + scenedata.scene ;
            string absolutepath = Application.dataPath + "/" + scenedata.scenePath + ".unity";
            var scene = File.Exists(absolutepath);
 
            if(!scene) {
                URSA.Log.Error("Target scene was not found.", scenedata);
            }

            Scene editorScene = EditorSceneManager.GetActiveScene();
            if(scenedata.scenePath == editorScene.path.Replace("Assets/", "").RemoveExtension()) {
                OnSceneDataCollected(scenedata);
            }

            EditorUtility.SetDirty(scenedata);
        }

        EditorUtility.SetDirty(collector);
        AssetDatabase.SaveAssets();

    }
}

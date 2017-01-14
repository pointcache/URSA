using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEditor.SceneManagement;

public static class LevelsSystemEditor {


    private const string SaveSystem_MENU = "Assets/SaveSystem/";
    private const string LEVELDATA_NEW = "New Data";


    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_LEVELS + "/OrganizeLevel")]
    public static void LevelOrganizer() {

        var settings = Helpers.FindScriptableObject<AssetToolsSettings>();
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

        } else
            Debug.LogError(settings.lv_entities_root + " not found ");

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
        } else
            Debug.LogError(settings.lv_static_root + " not found ");


        var all = GameObject.FindObjectsOfType<AssetType>();

        foreach (var obj in all) {
            if (obj.transform.parent != null && (obj.transform.parent.GetComponentInParents<Entity>() || obj.transform.parent.GetComponentInParents<BlueprintLoader>())) {
                continue;
            }


            switch (obj.type) { 
                case AssetType.ObjType.entity:
                    obj.transform.SetParent(entity.transform);
                    break;
                case AssetType.ObjType.@static:
                    obj.transform.SetParent(@static.transform);
                    break;
                case AssetType.ObjType.light:
                    obj.transform.SetParent(light.transform);
                    break;
                case AssetType.ObjType.volume:
                    obj.transform.SetParent(volume.transform);
                    break;
                case AssetType.ObjType.fx:
                    obj.transform.SetParent(fx.transform);
                    break;
                case AssetType.ObjType.npc:
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


    static SceneData makeData(SceneAsset scene) {

        SceneData level = (SceneData)CreateAsset<SceneData>(scene.name + "_data");
        level.scene = scene.name;
        level.levelname = scene.name;
        level.NiceName = scene.name;
        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        return level;
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


    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_LEVELS + "/CollectSceneData")]
    public static void CollectLevelsData() {
        SceneDataCollector collector;
        var assets = AssetDatabase.FindAssets("t:SceneDataCollector");
        if (assets.Length == 0) {
            Debug.LogError("ScenesData file was not found, creating");
            collector = (SceneDataCollector)CreateAsset<SceneDataCollector>("SceneDataCollector");
        } else {
            collector = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(SceneDataCollector)) as SceneDataCollector;
        }
        var scenedatas = AssetDatabase.FindAssets("t:SceneData");

        collector.scenes.Clear();

        foreach (var scenedata in scenedatas) {
            SceneData data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scenedata), typeof(SceneData)) as SceneData;
            collector.scenes.Add(data);
        }
        EditorUtility.SetDirty(collector);
        AssetDatabase.SaveAssets();
        Debug.Log("success");
    }
}

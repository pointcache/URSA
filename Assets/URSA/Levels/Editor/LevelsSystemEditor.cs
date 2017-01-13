using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class LevelsSystemEditor
{

    
    private const string SaveSystem_MENU = "Assets/SaveSystem/";
    private const string LEVELDATA_NEW = "New Data";



    [MenuItem(SaveSystem_MENU + LEVELDATA_NEW)]
    public static void CreateLevelData()
    {
        SceneAsset scene = Selection.activeObject as SceneAsset;
        if (!scene)
            return;

        var assets = AssetDatabase.FindAssets(scene.name + "_data");
        if (assets.Length > 0)
        {
            Debug.LogError("Data file for this scene already exists, please use the existing one or recreate it.");
            return;
        }
        SceneData data = makeData(scene);

        Debug.Log("Created level data for scene :" + scene.name);
    }


    static SceneData makeData(SceneAsset scene)
    {

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
    public static ScriptableObject CreateAsset<T>(string filename) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
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


    [MenuItem(URSAConstants.MENUITEM_ROOT +  URSAConstants.MENUITEM_LEVELS + "/CollectData")]
    public static void CollectLevelsData()
    {
        SceneDataCollector collector;
        var assets = AssetDatabase.FindAssets("t:SceneDataCollector");
        if (assets.Length == 0)
        {
            Debug.LogError("ScenesData file was not found, creating");
            collector = (SceneDataCollector)CreateAsset<SceneDataCollector>("SceneDataCollector");
        }
        else
        {
            collector = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(SceneDataCollector)) as SceneDataCollector;
        }
        var scenedatas = AssetDatabase.FindAssets("t:SceneData");

        collector.scenes.Clear();

        foreach (var scenedata in scenedatas)
        {
            SceneData data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scenedata), typeof(SceneData)) as SceneData;
            collector.scenes.Add(data);
        }
        EditorUtility.SetDirty(collector);
        AssetDatabase.SaveAssets();
        Debug.Log("success");
    }
}

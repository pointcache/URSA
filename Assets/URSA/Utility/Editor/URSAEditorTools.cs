#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class URSAEditorTools {

    [MenuItem("Edit/Run _F5")] // shortcut key F5 to Play (and exit playmode also)
    static void PlayGame() {
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    [MenuItem(URSAConstants.MENUITEM_ROOT + "/UpdateEverything", priority = 1)]
    public static void UpdateEverything() {
        Database.RebuildWithoutReloadOfTheScene();
        AssetsTools.ParseResources();
        LevelsSystemEditor.CollectLevelsData();
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
        LevelsSystemEditor.LevelOrganizer();
    }

    [MenuItem("GameObject/CreateParent")]
    public static void CreateParent() {
        var go = Selection.activeGameObject;
        if (go) {
            var parent = new GameObject(go.name + "_root");
            Undo.RegisterCreatedObjectUndo(parent, "Created parent");
            parent.transform.SetParent(go.transform.parent);
            var rect = go.GetComponent<RectTransform>();

            if (rect) {
                var goparentrect = go.transform.parent.GetComponent<RectTransform>();
                var parentCanvas = go.transform.parent.GetComponent<Canvas>();
                var parentRect = parent.GetComponent<RectTransform>();
                if (!parentRect)
                    parentRect = parent.AddComponent<RectTransform>();
                if (!parentCanvas)
                    parentRect.position = goparentrect.transform.position;
                else
                    parentRect.position = parentCanvas.transform.position;

                parentRect.anchorMax = new Vector2(1f, 1f);
                parentRect.anchorMin = Vector2.zero;
                parentRect.offsetMax = Vector2.zero;
                parentRect.offsetMin = Vector2.zero;
            } else {
                parent.transform.position = go.transform.position;
                parent.transform.rotation = go.transform.rotation;
                go.transform.SetParent(parent.transform);
            }
            Undo.SetTransformParent(go.transform, parent.transform, "reparent");
        }
    }

    [MenuItem(URSAConstants.MENUITEM_ROOT + "/AddSystems", priority = 2)]
    public static void AddSystems() {
        LevelStructureConstructor();


        var gl = GameObject.Find("GlobalSystems") as GameObject;
        if (gl) {
            Debug.LogError("GlobalSystems already exists ");
            return;
        }
        var settings = Helpers.FindScriptableObject<URSASettings>();
        if (settings.CustomGlobalSystemsPrefab) {
            gl = GameObject.Instantiate(settings.CustomGlobalSystemsPrefab);
        } else
            gl = Helpers.SpawnEditor("URSA/GlobalSystems");
        Debug.Log("Global systems spawned");

        var sc = GameObject.Find("SceneSystems") as GameObject;
        if (sc) {
            Debug.LogError("SceneSystems already exists ");
            return;
        }
        if (settings.CustomSceneSystemsPrefab) {
            sc = GameObject.Instantiate(settings.CustomSceneSystemsPrefab);
        } else
            sc = Helpers.SpawnEditor("URSA/SceneSystems");
        Debug.Log("Scene systems spawned");



        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
    }

    public static void LevelStructureConstructor() {

        var settings = Helpers.FindScriptableObject<AssetToolsSettings>();

        var stat = GameObject.Find(settings.lv_static_root) as GameObject;
        if (stat) {
            Debug.LogError(settings.lv_static_root + " already exists ");
        } else
            stat = new GameObject(settings.lv_static_root);
        {


            var lights = new GameObject(settings.lv_lights);
            lights.transform.SetParent(stat.transform);

            var fx = new GameObject(settings.lv_fx);
            fx.transform.SetParent(stat.transform);

            var volumes = new GameObject(settings.lv_volumes);
            volumes.transform.SetParent(stat.transform);

            var @static = new GameObject(settings.lv_static);
            @static.transform.SetParent(stat.transform);

        }

        var entities = GameObject.Find(settings.lv_entities_root) as GameObject;
        if (entities) {
            Debug.LogError(settings.lv_entities_root + " object already exists ");
        } else
            entities = new GameObject(settings.lv_entities_root);

        {
            var npcs = new GameObject(settings.lv_npc);
            npcs.transform.SetParent(entities.transform);

            var other = new GameObject(settings.lv_entity);
            other.transform.SetParent(entities.transform);
        }


        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
    }

   
}
#endif
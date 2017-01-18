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
    [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + "/UpdateEverything", priority = 1)]
    public static void UpdateEverything() {
        var settings = URSASettings.current;

        if (settings.RebuildDatabase)
            Database.RebuildWithoutReloadOfTheScene();
        if (settings.ParsePrefabs)
            PrefabTools.ParseResources();
        if (settings.CollectSceneData)
            LevelsSystemEditor.CollectLevelsData();
        if (settings.OrganizeLevel)
            LevelsSystemEditor.LevelOrganizer();
        if (settings.SaveAndReloadScene) {

            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();
            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
        }
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
    [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + "/AddSystems", priority = 2)]
    public static void AddSystems() {
        var settings = URSASettings.current;
        var gl = GameObject.Find(URSAConstants.SYSTEMS_GLOBAL_NAME) as GameObject;
        if (!gl) {
            if (settings.CustomGlobalSystemsPrefab) {
                gl = Helpers.SpawnEditor(settings.CustomGlobalSystemsPrefab);
            } else if (settings.GlobalSystemsTemplate)
                gl = Helpers.SpawnEditor(settings.GlobalSystemsTemplate);
            else
                gl = Helpers.SpawnEditor("URSA/" + URSAConstants.SYSTEMS_GLOBAL_NAME);
            Debug.Log(URSAConstants.SYSTEMS_GLOBAL_NAME + " spawned");
        } else
            Debug.LogError(URSAConstants.SYSTEMS_GLOBAL_NAME + " already exists ");
        var sc = GameObject.Find(URSAConstants.SYSTEMS_LOCAL_NAME) as GameObject;
        if (!sc) {
            if (settings.CustomLocalSystemsPrefab) {
                sc = Helpers.SpawnEditor(settings.CustomLocalSystemsPrefab);
            } else if(settings.LocalSystemsTemplate)
                sc = Helpers.SpawnEditor(settings.LocalSystemsTemplate);
            else
                sc = Helpers.SpawnEditor("URSA/" + URSAConstants.SYSTEMS_LOCAL_NAME);

            Debug.Log(URSAConstants.SYSTEMS_LOCAL_NAME + " spawned");
        } else
            Debug.LogError(URSAConstants.SYSTEMS_LOCAL_NAME + " already exists ");
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
    }
    [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_LEVELS + "/Create Level Structure")]
    public static void LevelStructureConstructor() {
        var settings = Helpers.FindScriptableObject<PrefabToolsSettings>();
        var stat = GameObject.Find(settings.lv_static_root) as GameObject;
        if (stat) {
            Debug.LogError(settings.lv_static_root + " already exists ");
        } else {
            stat = new GameObject(settings.lv_static_root);
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
        } else {
            entities = new GameObject(settings.lv_entities_root);
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
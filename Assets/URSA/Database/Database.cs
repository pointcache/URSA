using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Database : MonoBehaviour {


    #region SINGLETON
    private static Database _instance;
    public static Database instance
    {
        get {
            if (!_instance) _instance = GameObject.FindObjectOfType<Database>();
            return _instance;
        }
    }
    #endregion

    static List<GameObject> prefabObjects = new List<GameObject>(1000);

    static Dictionary<string, Entity> entities = new Dictionary<string, Entity>(1000);
    static Dictionary<string, HashSet<string>> Components = new Dictionary<string, HashSet<string>>(10000);

    static DatabaseManifest _manifest;
    static DatabaseManifest manifest
    {
        get {
            if (_manifest == null) {
                TextAsset manifest_asset = Resources.Load(URSAConstants.PATH_ADDITIONAL_DATA + "/" + URSASettings.current.DatabaseManifest) as TextAsset;
                _manifest = SerializationHelper.LoadFromString<DatabaseManifest>(manifest_asset.text);
            }
            return _manifest;
        }
    }

    private void OnEnable() {

    }


#if UNITY_EDITOR
    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_DATABASE + URSAConstants.MENUITEM_DATABASE_REBUILD)]
    public static void Rebuild() {
        prefabObjects = new List<GameObject>(1000);
        entities = new Dictionary<string, Entity>(1000);
        Components = new Dictionary<string, HashSet<string>>(10000);

        assign_ids_entities();
        assign_ids_components();

        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
    }

    public static void RebuildWithoutReloadOfTheScene() {
        prefabObjects = new List<GameObject>(1000);
        entities = new Dictionary<string, Entity>(1000);
        Components = new Dictionary<string, HashSet<string>>(10000);
        assign_ids_entities();
        assign_ids_components();

    }
#endif

    /// <summary>
    /// Very careful with this one
    /// </summary>
#if UNITY_EDITOR
    [MenuItem("temp/clear ids")]
#endif
    public static void clear_all_entity_IDs() {
        var prefabs = Resources.LoadAll(URSASettings.current.DatabaseRootFolder + "/");

        foreach (var p in prefabs) {
            var ent = ((GameObject)p).GetComponent<Entity>();
            if (ent) {
                ent.database_ID = "";
            }
        }
    }

    static string dbPath
    {
        get {
            return "/Resources/" + URSASettings.current.DatabaseRootFolder + "/";
        }
    }

    public static GameObject GetPrefab(string id) {
        string path = "";
        string idPath = "";
        manifest.entity_id_adress.TryGetValue(id, out idPath);
        if (idPath != "")
            path = URSASettings.current.DatabaseRootFolder + "/" + idPath;
        else
            return null;
        return Resources.Load(path) as GameObject;
    }

    static void assign_ids_entities() {

        HashSet<string> ids = new HashSet<string>();

        DatabaseManifest manifest = new DatabaseManifest();
        manifest.GameVersion = ProjectInfo.current.Version;
        var files = Directory.GetFiles(Application.dataPath + dbPath, "*.prefab", SearchOption.AllDirectories);
        var prefabs = Resources.LoadAll(URSASettings.current.DatabaseRootFolder + "/");

        foreach (var p in prefabs) {
            var ent = ((GameObject)p).GetComponent<Entity>();
            if (ent)
                ids.Add(ent.database_ID);
        }

        foreach (var f in files) {
            string adress = f.Remove(0, f.IndexOf(dbPath) + dbPath.Length);
            adress = adress.Replace(".prefab", string.Empty);

            var prefab = Resources.Load(URSASettings.current.DatabaseRootFolder + "/" + adress) as GameObject;
            var entity = prefab.GetComponent<Entity>();
            if (!entity) {
                Debug.LogError("Database: GameObject without entity found, skipping.", prefab);
                continue;
            }

            if (entity.database_ID == "" || entity.database_ID == null) {
                entity.database_ID = get_unique_id(ids);
            }
            adress = adress.Replace("\\", "/");
            manifest.entity_id_adress.Add(entity.database_ID, adress);
            manifest.entity_adress_id.Add(adress, entity.database_ID);
        }

        SerializationHelper.Serialize(manifest, Application.dataPath + "/Resources/" + URSAConstants.PATH_ADDITIONAL_DATA + "/" + URSASettings.current.DatabaseManifest + ".json", true);
    }

    static void assign_ids_components() {
        var allData = Resources.LoadAll(URSASettings.current.DatabaseRootFolder);
        prefabObjects = new List<GameObject>(1000);
        foreach (var item in allData) {
            GameObject go = item as GameObject;
            var entity = go.GetComponent<Entity>();
            if (!entity) {
                Debug.LogError("Database: GameObject without entity found, skipping.", go);
                continue;
            }

            prefabObjects.Add(go);
            entities.Add(entity.database_ID, entity);

            ComponentBase[] components = go.GetComponentsInChildren<ComponentBase>();
            HashSet<string> comps_in_entity = null;
            Components.TryGetValue(entity.database_ID, out comps_in_entity);

            if (comps_in_entity == null)
                comps_in_entity = new HashSet<string>();

            foreach (var c in components) {
                if (c.ID == string.Empty || c.ID == null) {
                    c.ID = get_unique_id(comps_in_entity);
                }
                comps_in_entity.Add(c.ID);
            }
        }
    }


    public static string get_unique_id(HashSet<string> set) {
        Guid guid = Guid.NewGuid();
        string id = guid.ToString();
        if (set.Contains(id))
            return get_unique_id(set);
        else
            return id;
    }
}

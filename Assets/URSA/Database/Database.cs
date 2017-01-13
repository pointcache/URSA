using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

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
    static URSASettings _settings;
    static URSASettings settings
    {
        get {
            if (_settings == null)
                _settings = Resources.Load(URSAConstants.PATH_URSASETTINGS_ROOT + URSAConstants.PATH_URSASETTINGS_SETTINGS) as URSASettings;
            return _settings;
        }
    }
    static Dictionary<string, Entity> entities = new Dictionary<string, Entity>(1000);
    static Dictionary<string, HashSet<string>> Components = new Dictionary<string, HashSet<string>>(10000);

    DatabaseManifest manifest;

    private void OnEnable() {
        manifest = SerializationHelper.Load<DatabaseManifest>(Application.dataPath + "/Resources/" + settings.DatabaseRootFolder + "/" + settings.DatabaseManifest);
    }


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

    /// <summary>
    /// Very careful with this one
    /// </summary>
    [MenuItem("temp/clear ids")]
    public static void clear_all_entity_IDs() {
        var prefabs = Resources.LoadAll(settings.DatabaseRootFolder + "/");

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
            return "/Resources/" + settings.DatabaseRootFolder + "/";
        }
    }

    public static GameObject GetPrefab(string id) {
        string path = "";
        string idPath = "";
        instance.manifest.entity_id_adress.TryGetValue(id, out idPath);
        if (idPath != "")
            path = settings.DatabaseRootFolder + "/" + idPath;
        else
            return null;
        return Resources.Load(path) as GameObject;
    }

    static void assign_ids_entities() {

        HashSet<string> ids = new HashSet<string>();

        DatabaseManifest manifest = new DatabaseManifest();

        var files = Directory.GetFiles(Application.dataPath + dbPath, "*.prefab", SearchOption.AllDirectories);
        var prefabs = Resources.LoadAll(settings.DatabaseRootFolder + "/");

        foreach (var p in prefabs) {
            var ent = ((GameObject)p).GetComponent<Entity>();
            if (ent)
                ids.Add(ent.database_ID);
        }

        foreach (var f in files) {
            string adress = f.Remove(0, f.IndexOf(dbPath) + dbPath.Length);
            adress = adress.Replace(".prefab", string.Empty);

            var prefab = Resources.Load(settings.DatabaseRootFolder + "/" + adress) as GameObject;
            var entity = prefab.GetComponent<Entity>();
            if (!entity) {
                Debug.LogError("Database: GameObject without entity found, skipping.", prefab);
                continue;
            }

            if (entity.database_ID == "") {
                entity.database_ID = get_unique_id(ids);
            }
            adress = adress.Replace("\\", "/");
            manifest.entity_id_adress.Add(entity.database_ID, adress);
            manifest.entity_adress_id.Add(adress, entity.database_ID);
        }

        SerializationHelper.Serialize(manifest, Application.dataPath + "/Resources/" + settings.DatabaseRootFolder + "/" + settings.DatabaseManifest, true);
    }

    static void assign_ids_components() {
        var allData = Resources.LoadAll(settings.DatabaseRootFolder);
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


    static string get_unique_id(HashSet<string> set) {
        Guid guid = Guid.NewGuid();
        string id = guid.ToString();
        if (set.Contains(id))
            return get_unique_id(set);
        else
            return id;
    }
}

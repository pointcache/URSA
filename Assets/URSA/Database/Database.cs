using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Database : MonoBehaviour
{
    const string databaseAdress = "/Resources/Database/";
    const string databaseFolder = "Database/";

    List<GameObject> prefabObjects = new List<GameObject>(1000);
    Dictionary<string, Entity> entities = new Dictionary<string, Entity>(1000);
    Dictionary<string, List<string>> components = new Dictionary<string, List<string>>(10000);
    Dictionary<string, string> manifest = new Dictionary<string, string>(10000);

    [MenuItem("test/test")]
    public static void Test()
    {
        assign_ids();
    }
    
    //scans the database 
    public void RebuildIDs()
    {

    }

    static void assign_ids()
    {
        var files = Directory.GetFiles(Application.dataPath + "/Resources/Database/", "*.prefab", SearchOption.AllDirectories);
        foreach (var f in files)
        {
            string id = f.Remove(0, f.IndexOf(databaseAdress)+ databaseAdress.Length);
            id = id.Replace(".prefab", string.Empty);

            var prefab = Resources.Load(databaseFolder + id) as GameObject;
            var entity = prefab.GetComponent<Entity>();
            if(!entity)
            {
                Debug.LogError("Database: GameObject without entity found, skipping.", prefab);
                continue;
            }


            prefab.GetComponent<Entity>().database_ID = id;
        }
    }

    void scan_database()
    {
        assign_ids();

        var allData = Resources.LoadAll("Database/");
        prefabObjects = new List<GameObject>(1000);
        foreach (var item in allData)
        {
            GameObject go = item as GameObject;
            var entity = go.GetComponent<Entity>();
            if(!entity)
            {
                Debug.LogError("Database: GameObject without entity found, skipping.", go);
                continue;
            }
            prefabObjects.Add(go);
            entities.Add(entity.database_ID, entity);

            var components = go.GetComponentsInChildren(typeof(ComponentBase));
            foreach (var c in components)
            {

            }
        }
    }
}

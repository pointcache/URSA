using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA.Save;
using UnityEditor;

public class SaveSystem : MonoBehaviour
{

    #region SINGLETON
    private static SaveSystem _instance;
    public static SaveSystem instance
    {
        get
        {
            if (!_instance) _instance = GameObject.FindObjectOfType<SaveSystem>();
            return _instance;
        }
    }
    #endregion
    public string savePath = "/GameSave.sav";

    [MenuItem("Test/Save")]
    public static void SaveFile()
    {
        SaveObject save = CreateSaveFileFromScene();
        SerializationHelper.Serialize(save,Application.dataPath + instance.savePath, true);
    }

    [MenuItem("Test/Load")]
    public static void LoadFile()
    {
        LoadFromSaveFile(Application.dataPath + instance.savePath);
    }

    [MenuItem("Test/Clear")]
    public static void ClearScene()
    {
        var entities = GameObject.FindObjectsOfType<Entity>();

        for (int i = 0; i < entities.Length; i++)
        {
            GameObject.Destroy(entities[i].gameObject);
        }
    }

    public static void LoadFromSaveFile(string path)
    {
        CompRefSerializationProcessor.injectionList = new List<CompRef>();
        SaveObject save = SerializationHelper.Load<SaveObject>(path);
        //entity id, comp id
        Dictionary<string, Dictionary<string,ComponentBase>> compRefInjection = new Dictionary<string,Dictionary<string,ComponentBase>>();

        foreach (var eobj in save.entities)
        {
            var prefab = Database.GetPrefab(eobj.database_ID);
            bool prefabState = prefab.activeSelf;
            prefab.SetActive(false);

            var gameobj = GameObject.Instantiate(prefab);
            var tr = gameobj.transform;

            tr.position = eobj.position;
            tr.rotation = Quaternion.Euler(eobj.rotation);
            tr.localScale = eobj.scale;

            var entity = gameobj.GetComponent<Entity>();
            entity.instance_ID = eobj.instance_ID;

            var comps = gameobj.GetComponentsInChildren<ComponentBase>(true);
            if (comps.Length == 0)
            {
                prefab.SetActive(prefabState);
                entity.gameObject.SetActive(true);
                continue;
            }
            var cobjects = save.components[entity.instance_ID];

            foreach (var component in comps)
            {
                var cobj = cobjects[component.ID];
                component.SetData(cobj.data);

                //Storing for later reference injection
                Dictionary<string, ComponentBase> injectionDict = null;
                compRefInjection.TryGetValue(entity.ID, out injectionDict);
                if(injectionDict == null)
                {
                    injectionDict = new Dictionary<string, ComponentBase>();
                    compRefInjection.Add(entity.ID, injectionDict);
                }
                injectionDict.Add(component.ID, component);
            }
            prefab.SetActive(prefabState);
            entity.gameObject.SetActive(true);
        }

        foreach (var compref in CompRefSerializationProcessor.injectionList)
        {
            compref.setValueDirectly(compRefInjection[compref.entity_ID][compref.component_ID]);
        }
    }

    //creates a file containing all entities in scene, ignoring any scene specifics
    public static SaveObject CreateSaveFileFromScene()
    {
        SaveObject file = new SaveObject();

        foreach (var pair in EntityManager.all)
        {
            EntityObject eobj = new EntityObject();
            Entity entity = pair.Value;

            string eID = entity.instance_ID;

            eobj.database_ID = entity.database_ID;
            eobj.instance_ID = entity.instance_ID;
            eobj.position = entity.transform.position;
            eobj.rotation = entity.transform.rotation.eulerAngles;
            eobj.scale = entity.transform.localScale;

            file.entities.Add(eobj);

            var components = entity.GetComponentsInChildren<ComponentBase>(true);

            foreach (var comp in components)
            {
                ComponentObject cobj = new ComponentObject();

                cobj.component_ID = comp.ID;
                cobj.entity_ID = eID;
                cobj.data = comp.GetData();

                Dictionary<string, ComponentObject> entityComponents = null;
                file.components.TryGetValue(eID, out entityComponents);

                if (entityComponents == null)
                {
                    entityComponents = new Dictionary<string, ComponentObject>();
                    file.components.Add(eID, entityComponents);
                }

                if (entityComponents.ContainsKey(comp.ID))
                {
                    Debug.LogError("Super fatal error with duplicate component id's on entity.", entity.gameObject);
                }
                entityComponents.Add(comp.ID, cobj);
            }
        }

        return file;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA.Save;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.IO;
using URSA;
using UnityEditor.SceneManagement;

public class SaveSystem : MonoBehaviour {

    #region SINGLETON
    private static SaveSystem _instance;
    public static SaveSystem instance
    {
        get {
            if (!_instance) _instance = GameObject.FindObjectOfType<SaveSystem>();
            return _instance;
        }
    }
    #endregion
    public string entitiesRoot = "Entities";

    public string GlobalRootFoder = "CustomData";
    public string FileName = "GameSave";
    public DataPath datapath;
    public string customDataPath;
    public string folderPath = "Saves";
    public string extension = ".sav";

#if UNITY_EDITOR
    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_SAVESTATE + URSAConstants.MENUITEM_SAVESTATE_SAVE)]
#endif
    public static void SaveFromEditor() {
        instance.SaveFile();
    }

    public void SaveFile() {
        SaveObject save = CreateSaveObjectFromScene();

        string directory = getSystemPath() + "/" + folderPath;

        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        string path = directory + "/" + FileName + extension;
        SerializationHelper.Serialize(save, path, true);
    }

    enum SaveObjectType {
        scene,
        persistent,
        blueprint
    }

    static string getSystemPath() {
        var sys = SaveSystem.instance;
        string path = String.Empty;
        switch (sys.datapath) {
            case DataPath.persistent: {
                    path = Application.persistentDataPath;
                }
                break;
            case DataPath.inRootFolder: {
                    path = Application.dataPath;
                }
                break;
            case DataPath.custom: {
                    path = sys.customDataPath;
                }
                break;
            default:
                break;
        }
        return path + "/" + SaveSystem.instance.GlobalRootFoder;
    }


#if     UNITY_EDITOR
    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_SAVESTATE + URSAConstants.MENUITEM_SAVESTATE_LOAD)]
#endif
    public static void LoadFromEditor() {
        instance.LoadFile();
    }

    public void LoadFile() {
        ClearScene();

        Transform root = null;
        if (instance) {
            var rootGO = GameObject.Find(instance.entitiesRoot);
            if (rootGO)
                root = rootGO.transform;
        }

        this.OneFrameDelay(() => LoadFromSaveFile(getSystemPath() + "/" + folderPath + "/" + FileName + extension, root));
    }

#if UNITY_EDITOR
    [MenuItem("Test/Clear")]
#endif
    static public void ClearScene() {
        var entities = GameObject.FindObjectsOfType<Entity>();

        for (int i = 0; i < entities.Length; i++) {
            GameObject.Destroy(entities[i].gameObject);
        }
    }

    public static T DeserializeAs<T>(string path) {
        CompRefSerializationProcessor.injectionList = new List<CompRef>();
        return SerializationHelper.Load<T>(path);
    }

    public static void LoadFromSaveFile(string path, Transform root) {
        SaveObject save = DeserializeAs<SaveObject>(path);
        UnboxSaveObject(save, root);

    }

    public static void LoadBlueprint(string json, Transform root) {
        CompRefSerializationProcessor.injectionList = new List<CompRef>();
        Blueprint bp = SerializationHelper.LoadFromString<Blueprint>(json);
        UnboxSaveObject(bp.saveObject, root);
    }


    public static void UnboxSaveObject(SaveObject save, Transform root) {

        Dictionary<string, Entity> bp_parent_entity = null;
        Dictionary<string, Dictionary<string, ComponentBase>> bp_parent_component = null;
        if (save.isBlueprint) {
            bp_parent_entity = new Dictionary<string, Entity>();
            bp_parent_component = new Dictionary<string, Dictionary<string, ComponentBase>>();
        }
        Dictionary<string, ComponentObject> cobjects = null;
        Dictionary<string, Dictionary<string, ComponentBase>> allComps = new Dictionary<string, Dictionary<string, ComponentBase>>();
        Dictionary<string, Entity> allEntities = new Dictionary<string, Entity>();
        Dictionary<EntityObject, Entity> toParent = new Dictionary<EntityObject, Entity>();

        foreach (var eobj in save.entities) {
            var prefab = Database.GetPrefab(eobj.database_ID);

            if (!prefab) {
                Debug.LogError("When loading, database entity: " + eobj.database_ID + " was not found");
                continue;
            }
            bool prefabState = prefab.activeSelf;
            prefab.SetActive(false);
            GameObject gameobj = null;
#if UNITY_EDITOR
            gameobj = PrefabUtility.InstantiatePrefab(prefab, EditorSceneManager.GetActiveScene()) as GameObject;
#else
            gameobj = GameObject.Instantiate(prefab);
#endif
            gameobj.name = eobj.gameObjectName;
            var tr = gameobj.transform;

            tr.position = eobj.position;
            tr.rotation = Quaternion.Euler(eobj.rotation);
            tr.localScale = eobj.scale;

            GameObject parentGo = null;
            if (eobj.parentName != "null")
                parentGo = GameObject.Find(eobj.parentName);
#if UNITY_EDITOR
            GameObjectUtility.SetParentAndAlign(gameobj, eobj.parentName == "null" ? root.gameObject : parentGo == null ? root.gameObject : parentGo);
#else

            tr.parent = eobj.parentName == "null" ? root : parentGo == null ? root : parentGo.transform;
#endif

            var entity = gameobj.GetComponent<Entity>();

            if (save.isBlueprint) {
                if (eobj.parentIsEntity)
                    bp_parent_entity.Add(eobj.parent_entity_ID, entity);
                if (eobj.parentIsComponent) {
                    Dictionary<string, ComponentBase> ecomps = null;
                    bp_parent_component.TryGetValue(eobj.parent_entity_ID, out ecomps);
                    if (ecomps == null) {
                        ecomps = new Dictionary<string, ComponentBase>();
                        bp_parent_component.Add(eobj.parent_entity_ID, ecomps);
                    }
                }
            }

            entity.instance_ID = eobj.instance_ID;
            allEntities.Add(entity.instance_ID, entity);

            if (eobj.parentIsComponent || eobj.parentIsEntity) {
                toParent.Add(eobj, entity);
            }

            var comps = gameobj.GetComponentsInChildren<ComponentBase>(true);
            if (comps.Length == 0) {
                prefab.SetActive(prefabState);
                entity.gameObject.SetActive(true);
                continue;
            }

            save.components.TryGetValue(entity.instance_ID, out cobjects);
            if (cobjects != null) {

                foreach (var component in comps) {
                    var cobj = cobjects[component.ID];
                    SetDataForComponent(component, cobj.data);

                    //Storing for later reference injection
                    Dictionary<string, ComponentBase> injectionDict = null;
                    allComps.TryGetValue(entity.ID, out injectionDict);
                    if (injectionDict == null) {
                        injectionDict = new Dictionary<string, ComponentBase>();
                        allComps.Add(entity.ID, injectionDict);
                    }
                    injectionDict.Add(component.ID, component);
                }
            }
            if (save.isBlueprint)
                entity.instance_ID = "";
            prefab.SetActive(prefabState);
            entity.gameObject.SetActive(true);
        }

        if (save.isBlueprint) {
            foreach (var pair in toParent) {
                Entity e = pair.Value;
                EntityObject eobj = pair.Key;
                Transform parent = null;
                if (eobj.parentIsComponent) {
                    parent = bp_parent_component[eobj.parent_entity_ID][eobj.parent_component_ID].transform;
                } else if (eobj.parentIsEntity) {
                    parent = bp_parent_entity[eobj.parent_entity_ID].transform;
                }
                e.transform.SetParent(parent);
            }

            foreach (var compref in CompRefSerializationProcessor.injectionList) {
                if (!compref.isNull)
                    compref.setValueDirectly(bp_parent_component[compref.entity_ID][compref.component_ID]);
            }
        } else {

            foreach (var pair in toParent) {
                Entity e = pair.Value;
                EntityObject eobj = pair.Key;
                Transform parent = null;
                if (eobj.parentIsComponent) {
                    parent = allComps[eobj.parent_entity_ID][eobj.parent_component_ID].transform;
                } else if (eobj.parentIsEntity) {
                    parent = allEntities[eobj.parent_entity_ID].transform;
                }
                e.transform.SetParent(parent);
            }

            foreach (var compref in CompRefSerializationProcessor.injectionList) {
                if (!compref.isNull)
                    compref.setValueDirectly(allComps[compref.entity_ID][compref.component_ID]);
            }
        }
    }

    public static SaveObject CreateSaveObjectFromPersistenData() {
        return createSaveFrom(SaveObjectType.persistent, null);
    }
    //creates a file containing all entities in scene, ignoring any scene specifics
    public static SaveObject CreateSaveObjectFromScene() {
        return createSaveFrom(SaveObjectType.scene, null);
    }

    //creates a file containing all entities in scene, ignoring any scene specifics
    public static SaveObject CreateSaveObjectFromTransform(Transform tr) {
        return createSaveFrom(SaveObjectType.blueprint, tr);
    }

    static SaveObject createSaveFrom(SaveObjectType from, Transform root) {
        SaveObject file = new SaveObject();

        Dictionary<string, Entity> entities = null;

        switch (from) {
            case SaveObjectType.scene:
                entities = EntityManager.scene;
                foreach (var pair in entities) {
                    ProcessEntity(file, pair.Value, null);
                }
                break;
            case SaveObjectType.persistent:
                entities = EntityManager.persistent;
                foreach (var pair in entities) {
                    ProcessEntity(file, pair.Value, null);
                }
                break;
            case SaveObjectType.blueprint:
                PackBlueprint(file, root);
                break;
            default:
                break;
        }


        return file;
    }

    static SaveObject PackBlueprint(SaveObject file, Transform root) {
        Entity[] entities_arr = null;
        HashSet<string> bp_ids = new HashSet<string>();
        entities_arr = root.GetComponentsInChildren<Entity>();
        foreach (var ent in entities_arr) {
            ProcessEntity(file, ent, bp_ids);
        }

        return file;
    }

    static void ProcessEntity(SaveObject file, Entity ent, HashSet<string> bp_ids) {
        EntityObject eobj = new EntityObject();
        Entity entity = ent;
        bool isBlueprint = bp_ids != null;
        string eID = "";
        file.isBlueprint = isBlueprint;
        if (isBlueprint)
            eID = Database.get_unique_id(bp_ids);
        else
            eID = entity.instance_ID;

        Transform tr = entity.transform;
        eobj.database_ID = entity.database_ID;
        eobj.instance_ID = eID;

        eobj.position = tr.position;
        eobj.rotation = tr.rotation.eulerAngles;
        eobj.scale = tr.lossyScale;

        ComponentBase parentComp = tr.parent.GetComponent<ComponentBase>();
        if (parentComp) {
            eobj.parentIsComponent = true;
            eobj.parent_entity_ID = parentComp.Entity.ID;
            eobj.parent_component_ID = parentComp.ID;
        } else {
            Entity parentEntity = tr.parent.GetComponent<Entity>();
            if (parentEntity) {
                eobj.parentIsEntity = true;
                eobj.parent_entity_ID = parentEntity.ID;
            } else {
                if (isBlueprint) {
                    eobj.parentName = "null";
                } else
                    eobj.parentName = tr.parent.Null() ? "null" : tr.parent.name;

            }
        }
        eobj.gameObjectName = entity.name;

        file.entities.Add(eobj);

        var components = entity.GetAllEntityComponents();

        foreach (var comp in components) {
            ComponentObject cobj = new ComponentObject();

            if (isBlueprint)
                cobj.component_ID = Database.get_unique_id(bp_ids);
            else
                cobj.component_ID = comp.ID;

            cobj.entity_ID = eID;
            cobj.data = GetDataFromComponent(comp);

            Dictionary<string, ComponentObject> entityComponents = null;
            file.components.TryGetValue(eID, out entityComponents);

            if (entityComponents == null) {
                entityComponents = new Dictionary<string, ComponentObject>();
                file.components.Add(eID, entityComponents);
            }

            if (entityComponents.ContainsKey(comp.ID)) {
                Debug.LogError("Super fatal error with duplicate component id's on entity.", entity.gameObject);
            }
            entityComponents.Add(comp.ID, cobj);
        }
    }

    static SerializedData GetDataFromComponent(ComponentBase comp) {
        if (comp == null)
            return null;
        Type t = comp.GetType();
        var field = findDataField(t);
        var data = field.GetValue(comp) as SerializedData;
        return data;
    }

    static void SetDataForComponent(ComponentBase comp, SerializedData data) {
        Type t = comp.GetType();
        var field = findDataField(t);
        field.SetValue(comp, data);
    }

    static FieldInfo findDataField(Type t) {
        var fields = t.GetFields();
        foreach (var f in fields) {
            if (f.FieldType.BaseType == typeof(SerializedData))
                return f;
        }
        Debug.LogError("SerializedData was not found in component: " + t.Name);
        return null;
    }
}

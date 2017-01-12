using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA.Save;
using UnityEditor;
using System.Reflection;
using System.IO;
using URSA;

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

    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_SAVESTATE + URSAConstants.MENUITEM_SAVESTATE_SAVE)]
    public static void SaveFromEditor() {
        instance.SaveFile();
    }

    public void SaveFile() {
        SaveObject save = CreateSaveFromScene();

        string directory = getSystemPath() + "/" + folderPath;

        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        string path = directory + "/" + FileName + extension;
        SerializationHelper.Serialize(save, path, true);
    }

    enum SaveObjectType {
        scene,
        persistent
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
        return path+ "/" + SaveSystem.instance.GlobalRootFoder;
    }

    
    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_SAVESTATE + URSAConstants.MENUITEM_SAVESTATE_LOAD)]
    public static void LoadFromEditor() {
        instance.LoadFile();
    }

    public void LoadFile() {
        ClearScene();

        Transform root = null;
        var rootGO = GameObject.Find(entitiesRoot);
        if (rootGO)
            root = rootGO.transform;

        this.OneFrameDelay(() => LoadFromSaveFile(getSystemPath() + "/" + folderPath + "/" + FileName + extension , root));
    }

    [MenuItem("Test/Clear")]
    public void ClearScene() {
        var entities = GameObject.FindObjectsOfType<Entity>();

        for (int i = 0; i < entities.Length; i++) {
            GameObject.Destroy(entities[i].gameObject);
        }
    }

    public static T DeserializeAs<T>(string path) {
        CompRefSerializationProcessor.injectionList = new List<CompRef>();
        return SerializationHelper.Load<T>(path);
    }

    public void LoadFromSaveFile(string path, Transform root) {
        SaveObject save = DeserializeAs<SaveObject>(path);
        UnboxSaveObject(save, root);
    }

    public void UnboxSaveObject(SaveObject save, Transform root) {
        Dictionary<string, Dictionary<string, ComponentBase>> compRefInjection = new Dictionary<string, Dictionary<string, ComponentBase>>();

        foreach (var eobj in save.entities) {
            var prefab = Database.GetPrefab(eobj.database_ID);
            bool prefabState = prefab.activeSelf;
            prefab.SetActive(false);

            var gameobj = GameObject.Instantiate(prefab);
            var tr = gameobj.transform;

            tr.position = eobj.position;
            tr.rotation = Quaternion.Euler(eobj.rotation);
            tr.localScale = eobj.scale;
            tr.parent = root;

            var entity = gameobj.GetComponent<Entity>();
            entity.instance_ID = eobj.instance_ID;

            var comps = gameobj.GetComponentsInChildren<ComponentBase>(true);
            if (comps.Length == 0) {
                prefab.SetActive(prefabState);
                entity.gameObject.SetActive(true);
                continue;
            }
            var cobjects = save.components[entity.instance_ID];

            foreach (var component in comps) {
                var cobj = cobjects[component.ID];
                SetDataForComponent(component, cobj.data);

                //Storing for later reference injection
                Dictionary<string, ComponentBase> injectionDict = null;
                compRefInjection.TryGetValue(entity.ID, out injectionDict);
                if (injectionDict == null) {
                    injectionDict = new Dictionary<string, ComponentBase>();
                    compRefInjection.Add(entity.ID, injectionDict);
                }
                injectionDict.Add(component.ID, component);
            }
            prefab.SetActive(prefabState);
            entity.gameObject.SetActive(true);
        }

        foreach (var compref in CompRefSerializationProcessor.injectionList) {
            if (!compref.isNull)
                compref.setValueDirectly(compRefInjection[compref.entity_ID][compref.component_ID]);
        }
    }

    public SaveObject CreateSaveFromPersistenData() {
        return createSaveFrom(SaveObjectType.persistent);
    }
    //creates a file containing all entities in scene, ignoring any scene specifics
    public SaveObject CreateSaveFromScene() {
        return createSaveFrom(SaveObjectType.scene);
    }

    SaveObject createSaveFrom(SaveObjectType from) {
        SaveObject file = new SaveObject();

        Dictionary<string, Entity> entities = null;

        switch (from) {
            case SaveObjectType.scene:
                entities = EntityManager.scene;
                break;
            case SaveObjectType.persistent:
                entities = EntityManager.persistent;
                break;
            default:
                break;
        }

        foreach (var pair in entities) {
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

            foreach (var comp in components) {
                ComponentObject cobj = new ComponentObject();

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

        return file;
    }

    SerializedData GetDataFromComponent(ComponentBase comp) {
        if (comp == null)
            return null;
        Type t = comp.GetType();
        var field = findDataField(t);
        var data = field.GetValue(comp) as SerializedData;
        return data;
    }

    void SetDataForComponent(ComponentBase comp, SerializedData data) {
        Type t = comp.GetType();
        var field = findDataField(t);
        field.SetValue(comp, data);
    }

    FieldInfo findDataField(Type t) {
        var fields = t.GetFields();
        foreach (var f in fields) {
            if (f.FieldType.BaseType == typeof(SerializedData))
                return f;
        }
        Debug.LogError("SerializedData was not found in component: " + t.Name);
        return null;
    }
}

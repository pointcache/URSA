namespace URSA.Serialization {

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.IO;
    using System.Linq;

    using URSA;
    using URSA.ECS;
    using URSA.Blueprint;
    using URSA.Internal;
    using URSA.Utility;
    using URSA.Database;

    public class SaveSystem : MonoBehaviour {

        #region SINGLETON
        private static SaveSystem _instance;
        public static SaveSystem Instance
        {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<SaveSystem>();
                return _instance;
            }
        }
        #endregion

        public string EntitiesRoot = "Entities";
        public string FileName = "GameSave";
        public DataPath Datapath;
        public string CustomDataPath;
        public string FolderPath = "Saves";
        public string Extension = ".sav";


#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_SAVESTATE + URSAConstants.PATH_MENUITEM_SAVESTATE_SAVE)]
#endif
        public static void SaveFromEditor() {
            Instance.SaveFile();
        }

        public void SaveFile() {
            SaveObject save = CreateSaveObjectFromScene();

            string directory = getSystemPath() + "/" + FolderPath;

            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            string path = directory + "/" + FileName + Extension;
            SerializationHelper.Serialize(save, path, true);
        }

        enum SaveObjectType {
            scene,
            persistent,
            blueprint
        }

        static string getSystemPath() {
            var sys = SaveSystem.Instance;
            string path = String.Empty;
            switch (sys.Datapath) {
                case DataPath.persistent: {
                        path = Application.persistentDataPath;
                    }
                    break;
                case DataPath.inRootFolder: {
                        path = Application.dataPath;
                    }
                    break;
                case DataPath.custom: {
                        path = sys.CustomDataPath;
                    }
                    break;
                default:
                    break;
            }
            return path + "/" + URSASettings.Current.CustomDataFolder;
        }


#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_SAVESTATE + URSAConstants.PATH_MENUITEM_SAVESTATE_LOAD)]
#endif
        public static void LoadFromEditor() {
            Instance.LoadFile();
        }

        public void LoadFile() {
            ClearScene();

            Transform root = null;
            if (Instance) {
                var rootGO = GameObject.Find(Instance.EntitiesRoot);
                if (rootGO)
                    root = rootGO.transform;
            }

            this.OneFrameDelay(() => LoadFromSaveFile(getSystemPath() + "/" + FolderPath + "/" + FileName + Extension, root));
        }

#if UNITY_EDITOR
        //[MenuItem("Test/Clear")]
#endif
        static public void ClearScene() {
            var entities = GameObject.FindObjectsOfType<Entity>();

            for (int i = 0; i < entities.Length; i++) {
                GameObject.Destroy(entities[i].gameObject);
            }
        }

        public static T DeserializeAs<T>(string path) {
            return SerializationHelper.Load<T>(path);
        }

        public static void LoadFromSaveFile(string path, Transform root) {
            SaveObject save = DeserializeAs<SaveObject>(path);
            UnboxSaveObject(save, root);

        }

        public static void LoadBlueprint(string json, Transform root) {
            Blueprint bp = SerializationHelper.LoadFromString<Blueprint>(json);
            UnboxSaveObject(bp.SaveObject, root);
        }


        public static void UnboxSaveObject(SaveObject save, Transform root) {

            Dictionary<string, Entity> bp_entity = null;
            Dictionary<string, Dictionary<string, ComponentBase>> bp_parent_component = null;
            Dictionary<string, Dictionary<string, ComponentBase>> bp_all_comp = new Dictionary<string, Dictionary<string, ComponentBase>>();
            if (save.isBlueprint) {
                bp_entity = new Dictionary<string, Entity>();
                bp_parent_component = new Dictionary<string, Dictionary<string, ComponentBase>>();
                bp_all_comp = new Dictionary<string, Dictionary<string, ComponentBase>>();
            }

            bool blueprintEditorMode = save.isBlueprint && !Application.isPlaying;

            Dictionary<string, ComponentObject> cobjects = null;
            Dictionary<string, Dictionary<string, ComponentBase>> allComps = new Dictionary<string, Dictionary<string, ComponentBase>>();
            Dictionary<string, Entity> allEntities = new Dictionary<string, Entity>();
            Dictionary<EntityObject, Entity> toParent = new Dictionary<EntityObject, Entity>();

            foreach (var eobj in save.entities) {
                var prefab = EntityDatabase.GetPrefab(eobj.database_ID);

                if (!prefab) {
                    Debug.LogError("When loading, database entity: " + eobj.database_ID + " was not found");
                    continue;
                }
                bool prefabState = prefab.activeSelf;
                prefab.SetActive(false);
                GameObject gameobj = null;
#if UNITY_EDITOR
                if (blueprintEditorMode) {
                    gameobj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                }
                else
                    gameobj = GameObject.Instantiate(prefab);

#else
            gameobj = GameObject.Instantiate(prefab);
#endif
                gameobj.name = eobj.gameObjectName;
                var tr = gameobj.transform;

                GameObject parentGo = null;
                if (eobj.parentName != "null")
                    parentGo = GameObject.Find(eobj.parentName);

                if (save.isBlueprint) {
                    tr.parent = root;
                    tr.localPosition = eobj.position;
                    tr.localRotation = Quaternion.Euler(eobj.rotation);

                }
                else {
                    tr.position = eobj.position;
                    tr.rotation = Quaternion.Euler(eobj.rotation);
                    tr.parent = eobj.parentName == "null" ? root : parentGo == null ? root : parentGo.transform;
                }

                var entity = gameobj.GetComponent<Entity>();
                Dictionary<string, ComponentBase> ecomps = null;
                Dictionary<string, ComponentBase> bpcomps = null;

                if (save.isBlueprint) {
                    bp_entity.Add(eobj.blueprint_ID, entity);
                    bp_all_comp.TryGetValue(eobj.blueprint_ID, out bpcomps);
                    if (bpcomps == null) {
                        bpcomps = new Dictionary<string, ComponentBase>();
                        bp_parent_component.Add(eobj.blueprint_ID, bpcomps);
                    }
                    bp_parent_component.TryGetValue(eobj.blueprint_ID, out ecomps);
                    if (ecomps == null) {
                        ecomps = new Dictionary<string, ComponentBase>();
                        bp_parent_component.Add(eobj.blueprint_ID, ecomps);
                    }

                }
                else {
                    allEntities.Add(eobj.instance_ID, entity);

                }

                entity.instance_ID = eobj.instance_ID;
                entity.blueprint_ID = eobj.blueprint_ID;

                if (eobj.parentIsComponent || eobj.parentIsEntity) {
                    toParent.Add(eobj, entity);
                }

                var comps = gameobj.GetComponentsInChildren<ComponentBase>(true);
                if (comps.Length != 0) {
                    if (save.isBlueprint)
                        save.components.TryGetValue(entity.blueprint_ID, out cobjects);
                    else
                        save.components.TryGetValue(entity.instance_ID, out cobjects);

                    if (cobjects != null) {

                        foreach (var component in comps) {

                            if (save.isBlueprint)
                                ecomps.Add(component.ID, component);

                            ComponentObject cobj = null;
                            cobjects.TryGetValue(component.ID, out cobj);
                            if (cobj != null)
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
                }

                if (save.isBlueprint)
                    entity.instance_ID = "";
                prefab.SetActive(prefabState);
                entity.gameObject.SetActive(true);
            }


            foreach (var compref in save.comprefs) {
                if (!compref.isNull) {
                    Dictionary<string, ComponentBase> comps = null;
                    ComponentBase cbase = null;

                    if (save.isBlueprint)
                        bp_parent_component.TryGetValue(compref.entity_ID, out comps);
                    else
                        allComps.TryGetValue(compref.entity_ID, out comps);

                    if (comps != null) {
                        if (save.isBlueprint)
                            bp_parent_component[compref.entity_ID].TryGetValue(compref.component_ID, out cbase);
                        else
                            comps.TryGetValue(compref.component_ID, out cbase);
                        if (cbase != null) {

                            if (blueprintEditorMode) {

                                compref.setValueDirectly(cbase);
                            }
                            else
                                compref.setValueDirectly(cbase);
                        }
                        else
                            Debug.LogError("CompRef linker could not find component with id: " + compref.component_ID + " on entity: " + compref.entityName);
                    }
                    else
                        Debug.LogError("CompRef linker could not find entity with id: " + compref.entity_ID + " on entity: " + compref.entityName);
                }
            }
#if UNITY_EDITOR
            if (blueprintEditorMode) {
                foreach (var e in bp_entity) {
                    var go = PrefabUtility.FindPrefabRoot(e.Value.gameObject);
                    PrefabUtility.DisconnectPrefabInstance(go);
                    PrefabUtility.ReconnectToLastPrefab(go);
                }
            }
#endif
            if (save.isBlueprint) {
                if (blueprintEditorMode) {
#if UNITY_EDITOR
                    foreach (var pair in toParent) {
                        Entity e = pair.Value;
                        var go = PrefabUtility.FindPrefabRoot(e.gameObject);
                        PrefabUtility.DisconnectPrefabInstance(go);

                        EntityObject eobj = pair.Key;
                        Transform parent = null;
                        if (eobj.parentIsComponent) {
                            parent = bp_parent_component[eobj.parent_entity_ID][eobj.parent_component_ID].transform;
                        }
                        else if (eobj.parentIsEntity) {
                            parent = bp_entity[eobj.parent_entity_ID].transform;
                        }
                        e.transform.SetParent(parent);
                        PrefabUtility.ReconnectToLastPrefab(go);
                    }
#endif
                }
                else {
                    foreach (var pair in toParent) {
                        Entity e = pair.Value;
                        EntityObject eobj = pair.Key;
                        Transform parent = null;
                        if (eobj.parentIsComponent) {
                            parent = bp_parent_component[eobj.parent_entity_ID][eobj.parent_component_ID].transform;
                        }
                        else if (eobj.parentIsEntity) {
                            parent = bp_entity[eobj.parent_entity_ID].transform;
                        }
                        e.transform.SetParent(parent);
                    }
                }

            }
            else {

                foreach (var pair in toParent) {
                    Entity e = pair.Value;
                    EntityObject eobj = pair.Key;
                    Transform parent = null;
                    if (eobj.parentIsComponent) {
                        parent = allComps[eobj.parent_entity_ID][eobj.parent_component_ID].transform;
                    }
                    else if (eobj.parentIsEntity) {
                        parent = allEntities[eobj.parent_entity_ID].transform;
                    }
                    e.transform.SetParent(parent);
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
                    entities = EntityManager.SceneEntities;
                    foreach (var pair in entities) {
                        ProcessEntity(file, pair.Value, null, null);
                    }
                    break;
                case SaveObjectType.persistent:
                    entities = EntityManager.PersistentEntities;
                    foreach (var pair in entities) {
                        ProcessEntity(file, pair.Value, null, null);
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
            List<Entity> entities_list = null;
            HashSet<string> bp_ids = new HashSet<string>();

            entities_list = root.GetComponentsInChildren<Entity>().ToList();
            var rootEntity = root.GetComponent<Entity>();
            if (rootEntity)
                entities_list.Remove(rootEntity);
            foreach (var ent in entities_list) {
                ProcessEntity(file, ent, bp_ids, root);
            }

            return file;
        }

        static void ProcessEntity(SaveObject file, Entity ent, HashSet<string> bp_ids, Transform root) {
            EntityObject eobj = new EntityObject();
            Entity entity = ent;
            bool isBlueprint = bp_ids != null;
            file.isBlueprint = isBlueprint;
            CompRefSerializationProcessor.blueprint = isBlueprint;
            if (isBlueprint && entity.blueprint_ID == "")
                entity.blueprint_ID = Helpers.GetUniqueID(bp_ids);

            eobj.blueprint_ID = entity.blueprint_ID;
            Transform tr = entity.transform;
            eobj.database_ID = entity.database_ID;
            eobj.instance_ID = entity.instance_ID;
            eobj.prefabPath = EntityDatabase.GetPrefabPath(entity.database_ID);

            if (isBlueprint) {
                eobj.position = root.InverseTransformPoint(tr.position);
                eobj.rotation = tr.localRotation.eulerAngles;
            }
            else {
                eobj.position = tr.position;
                eobj.rotation = tr.rotation.eulerAngles;
            }
            bool hasParent = tr.parent != null;
            ComponentBase parentComp = null;
            if (hasParent) {
                parentComp = tr.parent.GetComponent<ComponentBase>();
            }
            if (tr.parent != root && parentComp) {
                eobj.parentIsComponent = true;
                if (isBlueprint)
                    eobj.parent_entity_ID = parentComp.Entity.blueprint_ID;
                else
                    eobj.parent_entity_ID = parentComp.Entity.ID;
                eobj.parent_component_ID = parentComp.ID;
            }
            else {
                Entity parentEntity = null;
                if (hasParent) {
                    parentEntity = tr.parent.GetComponent<Entity>();
                }
                if (tr.parent != root && parentEntity) {
                    eobj.parentIsEntity = true;
                    if (isBlueprint)
                        eobj.parent_entity_ID = parentEntity.blueprint_ID;
                    else
                        eobj.parent_entity_ID = parentEntity.ID;
                }
                else {
                    if (isBlueprint) {
                        eobj.parentName = "null";
                    }
                    else
                        eobj.parentName = tr.parent.Null() ? "null" : tr.parent.name;
                }
            }
            eobj.gameObjectName = entity.name;

            file.entities.Add(eobj);

            var components = entity.GetAllEntityComponents();

            foreach (var comp in components) {
                ComponentObject cobj = new ComponentObject();

                cobj.component_ID = comp.ID;
                cobj.data = GetDataFromComponent(comp);

                Dictionary<string, ComponentObject> entityComponents = null;
                if (isBlueprint)
                    file.components.TryGetValue(entity.blueprint_ID, out entityComponents);
                else
                    file.components.TryGetValue(entity.instance_ID, out entityComponents);

                if (entityComponents == null) {
                    entityComponents = new Dictionary<string, ComponentObject>();
                    if (isBlueprint)
                        file.components.Add(entity.blueprint_ID, entityComponents);
                    else
                        file.components.Add(entity.instance_ID, entityComponents);

                }

                if (entityComponents.ContainsKey(comp.ID)) {
                    Debug.LogError("Super fatal error with duplicate component id's on entity.", entity.gameObject);
                }
                entityComponents.Add(comp.ID, cobj);

                Type t = cobj.data.GetType();
                var fields = t.GetFields();
                foreach (var f in fields) {
                    if (f.FieldType == typeof(CompRef)) {
                        file.comprefs.Add(f.GetValue(cobj.data) as CompRef);
                    }
                }
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

}
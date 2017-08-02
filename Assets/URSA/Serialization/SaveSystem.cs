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
    using URSA.Serialization.Blueprints;
    using URSA.Internal;
    using URSA.Utility;
    using URSA.Database;
    using URSA.ECS.Entity;
    using URSA.ECS.Systems;

    public class SaveSystem : MonoBehaviour {

        public static event System.Action OnSaveLoaded = delegate { };

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
        public string FolderName = "Saves";
        public string Extension = ".sav";


#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_SAVESTATE + URSAConstants.PATH_MENUITEM_SAVESTATE_SAVE)]
#endif
        public static void SaveFromEditor() {
            Instance.SaveFile();
        }

        public void SaveFile() {
            SaveObject save = CreateSaveObjectFromScene();

            string directory = PathUtilities.CustomDataPath + "/" + FolderName;

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

            this.OneFrameDelay(() => LoadFromSaveFile(PathUtilities.CustomDataPath + "/" + FolderName + "/" + FileName + Extension, root));
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

            ECSController.PauseExecution(true);

            CompRefSerializationProcessor.refs = new List<CompRef>();
            SaveObject save = DeserializeAs<SaveObject>(path);
            UnboxSaveObject(save, root);
            OnSaveLoaded();

            ECSController.ResumeExecutionNextFrame();
        }

        public static void LoadBlueprint(string json, Transform root) {
            Blueprint bp = SerializationHelper.LoadFromString<Blueprint>(json);
            UnboxSaveObject(bp.SaveObject, root);
        }

        public static void UnboxSaveObject(SaveObject save, Transform root) {

            if (save == null) {
                Debug.LogError("Save object is null");
                return;
            }

            var initializedfield = typeof(ECSComponent).GetField("m_initialized", BindingFlags.NonPublic | BindingFlags.Instance);

            Dictionary<int, Entity> bp_entity = null;
            Dictionary<int, Dictionary<int, ECSComponent>> bp_parent_component = null;
            Dictionary<int, Dictionary<int, ECSComponent>> bp_all_comp = new Dictionary<int, Dictionary<int, ECSComponent>>();
            if (save.isBlueprint) {
                bp_entity = new Dictionary<int, Entity>();
                bp_parent_component = new Dictionary<int, Dictionary<int, ECSComponent>>();
                bp_all_comp = new Dictionary<int, Dictionary<int, ECSComponent>>();
            }

            bool blueprintEditorMode = save.isBlueprint && !Application.isPlaying;

            Dictionary<int, ComponentObject> cobjects = null;
            Dictionary<int, Dictionary<int, ECSComponent>> allComps = new Dictionary<int, Dictionary<int, ECSComponent>>();
            Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
            Dictionary<EntityObject, Entity> toParent = new Dictionary<EntityObject, Entity>();

            foreach (var eobj in save.entities) {
                var prefab = EntityDatabase.GetPrefab(eobj.database_ID);

                if (!prefab) {
                    Debug.LogError("When loading, database entity: " + eobj.database_ID + " was not found, this probably means you saved an entity that is not registered in database. Make it a prefab in Entity folder and run database scan.");
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
                Dictionary<int, ECSComponent> ecomps = null;
                Dictionary<int, ECSComponent> bpcomps = null;

                if (save.isBlueprint) {
                    bp_entity.Add(eobj.blueprint_ID, entity);
                    bp_all_comp.TryGetValue(eobj.blueprint_ID, out bpcomps);
                    if (bpcomps == null) {
                        bpcomps = new Dictionary<int, ECSComponent>();
                        bp_parent_component.Add(eobj.blueprint_ID, bpcomps);
                    }
                    bp_parent_component.TryGetValue(eobj.blueprint_ID, out ecomps);
                    if (ecomps == null) {
                        ecomps = new Dictionary<int, ECSComponent>();
                        bp_parent_component.Add(eobj.blueprint_ID, ecomps);
                    }

                }
                else {
                    allEntities.Add(eobj.instance_ID, entity);

                }

                entity.instanceID = eobj.instance_ID;
                entity.blueprintID = eobj.blueprint_ID;

                if (eobj.parentIsComponent || eobj.parentIsEntity) {
                    toParent.Add(eobj, entity);
                }

                var comps = gameobj.GetComponentsInChildren<ECSComponent>(true);
                if (comps.Length != 0) {
                    if (save.isBlueprint)
                        save.components.TryGetValue(entity.blueprintID, out cobjects);
                    else
                        save.components.TryGetValue(entity.instanceID, out cobjects);

                    if (cobjects != null) {

                        foreach (var component in comps) {

                            if (save.isBlueprint)
                                ecomps.Add(component.componentID, component);

                            ComponentObject cobj = null;
                            cobjects.TryGetValue(component.componentID, out cobj);
                            if (cobj != null) {
                                SetDataForComponent(component, cobj.data);
                                initializedfield.SetValue(component, cobj.initialized);
                            }

                            //Storing for later reference injection
                            Dictionary<int, ECSComponent> injectionDict = null;
                            allComps.TryGetValue(entity.ID, out injectionDict);
                            if (injectionDict == null) {
                                injectionDict = new Dictionary<int, ECSComponent>();
                                allComps.Add(entity.ID, injectionDict);
                            }
                            injectionDict.Add(component.componentID, component);
                        }
                    }
                }

                if (save.isBlueprint)
                    entity.instanceID = 0;
                prefab.SetActive(prefabState);
                entity.gameObject.SetActive(true);
            }


            foreach (var compref in CompRefSerializationProcessor.refs) {
                if (!compref.isNull) {
                    Dictionary<int, ECSComponent> comps = null;
                    ECSComponent cbase = null;

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
                            compref.component = cbase;
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
            Dictionary<int, Entity> entities = null;

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
            HashSet<int> bp_ids = new HashSet<int>();

            entities_list = root.GetComponentsInChildren<Entity>().ToList();
            var rootEntity = root.GetComponent<Entity>();
            if (rootEntity)
                entities_list.Remove(rootEntity);
            foreach (var ent in entities_list) {
                ProcessEntity(file, ent, bp_ids, root);
            }

            return file;
        }

        private static List<ECSComponent> getComponentsSwapList = new List<ECSComponent>(10);

        static void ProcessEntity(SaveObject file, Entity ent, HashSet<int> bp_ids, Transform root) {

            EntityObject eobj = new EntityObject();
            Entity entity = ent;
            bool isBlueprint = bp_ids != null;
            file.isBlueprint = isBlueprint;
            CompRefSerializationProcessor.blueprint = isBlueprint;
            if (isBlueprint && entity.blueprintID == 0)
                entity.blueprintID = GameObjectUtils.GetUniqueID(bp_ids);

            eobj.blueprint_ID = entity.blueprintID;
            Transform tr = entity.transform;
            eobj.database_ID = entity.entityID;
            eobj.instance_ID = entity.instanceID;
            eobj.prefabPath = EntityDatabase.GetPrefabPath(entity.entityID);

            if (isBlueprint) {
                eobj.position = root.InverseTransformPoint(tr.position);
                eobj.rotation = tr.localRotation.eulerAngles;
            }
            else {
                eobj.position = tr.position;
                eobj.rotation = tr.rotation.eulerAngles;
            }
            bool hasParent = tr.parent != null;
            ECSComponent parentComp = null;
            if (hasParent) {
                parentComp = tr.parent.GetComponent<ECSComponent>();
            }
            if (tr.parent != root && parentComp) {
                eobj.parentIsComponent = true;
                if (isBlueprint)
                    eobj.parent_entity_ID = parentComp.Entity.blueprintID;
                else
                    eobj.parent_entity_ID = parentComp.Entity.ID;
                eobj.parent_component_ID = parentComp.componentID;
            }
            else {
                Entity parentEntity = null;
                if (hasParent) {
                    parentEntity = tr.parent.GetComponent<Entity>();
                }
                if (tr.parent != root && parentEntity) {
                    eobj.parentIsEntity = true;
                    if (isBlueprint)
                        eobj.parent_entity_ID = parentEntity.blueprintID;
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

            getComponentsSwapList.Clear();

            entity.GetAllEntityComponents(getComponentsSwapList);

            foreach (var comp in getComponentsSwapList) {
                if (comp.componentID == 0) {
                    //Debug.Log("Skipping component without ID : " + comp.GetType(), entity.gameObject);
                    continue;
                }

                ComponentObject cobj = new ComponentObject();

                cobj.component_ID = comp.componentID;
                cobj.data = GetDataFromComponent(comp);
                cobj.initialized = comp.Initialized;

                Dictionary<int, ComponentObject> entityComponents = null;
                if (isBlueprint)
                    file.components.TryGetValue(entity.blueprintID, out entityComponents);
                else
                    file.components.TryGetValue(entity.instanceID, out entityComponents);

                if (entityComponents == null) {
                    entityComponents = new Dictionary<int, ComponentObject>();
                    if (isBlueprint)
                        file.components.Add(entity.blueprintID, entityComponents);
                    else
                        file.components.Add(entity.instanceID, entityComponents);

                }

                if (entityComponents.ContainsKey(comp.componentID)) {
                    Debug.LogError("Super fatal error with duplicate component id's on entity.", entity.gameObject);
                }
                entityComponents.Add(comp.componentID, cobj);
                if (cobj.data != null) {
                    Type t = cobj.data.GetType();
                    var fields = t.GetFields();
                    foreach (var f in fields) {
                        if (f.FieldType == typeof(CompRef)) {
                            file.comprefs.Add(f.GetValue(cobj.data) as CompRef);
                        }
                    }
                }
            }
        }

        static SerializedData GetDataFromComponent(ECSComponent comp) {
            if (comp == null)
                return null;
            Type t = comp.GetType();
            var field = findDataField(t);
            if (field != null) {
                var data = field.GetValue(comp) as SerializedData;
                return data;
            }
            else
                return null;
        }

        static void SetDataForComponent(ECSComponent comp, SerializedData data) {
            Type t = comp.GetType();
            var field = findDataField(t);
            if (field != null)
                field.SetValue(comp, data);
        }

        static FieldInfo findDataField(Type t) {
            var fields = t.GetFields();
            foreach (var f in fields) {
                if (f.FieldType.BaseType == typeof(SerializedData))
                    return f;
            }
            return null;
        }
    }

}
namespace URSA.Utility {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Linq;
    using System.Collections;
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif


    public class Enumerators {

        public static IEnumerator Addframedelay(Action action) {
            yield return new WaitForEndOfFrame();
            action();
        }
    }


    public static class GameObjectUtils {

        public static int GetUniqueID(HashSet<int> set) {
            int id = set.Count+1;

            if (set.Contains(id)) {
                id = GetUniqueIDRecursive(set, id);
            }
            set.Add(id);
            return id;
        }

        private static int GetUniqueIDRecursive(HashSet<int> set, int previous) {
            int id = previous+1;
            if (set.Contains(id))
                return GetUniqueIDRecursive(set, id);
            else
                return id;
        }

        public static Entity GetEntity(this Collider collider) {

            var entity = collider.GetComponent(typeof(Entity)) as Entity;

            if (((object)entity) != null)
                return entity;

            var reference = collider.GetComponent(typeof(EntityReference)) as EntityReference;
            if (((object)reference) != null)
                return reference.Entity;

            return collider.GetComponentInParent(typeof(Entity)) as Entity;

        }

        public static Entity GetEntity(this Component comp) {
            return comp.GetComponentInParent(typeof(Entity)) as Entity;
        }

        public static Entity GetEntity(this GameObject go) {
            return go.GetComponentInParent(typeof(Entity)) as Entity;
        }

        public static void RunOnEachOfType<T>(this GameObject go, Action<T> action) where T : Component {

            var comp = go.GetComponent<T>();
            if(((object)comp)!=null)
                action(comp);

            Transform tr = go.transform;
            int childcount = tr.childCount;

            if(childcount > 0) {

                for (int i = 0; i < childcount; i++) {

                    _runOnEachOfType<T>(tr.GetChild(i), action);

                }
            }
        }

        private static void _runOnEachOfType<T>(Transform tr, Action<T> action) where T : Component {

            int childcount = tr.childCount;

            if(childcount > 0) {

                for (int i = 0; i < childcount; i++) {

                    _runOnEachOfType<T>(tr.GetChild(i), action);

                }
            }

            var comp = tr.gameObject.GetComponent<T>();
            if(((object)comp)!=null)
                action(comp);

        }

#if UNITY_EDITOR
        public static T FindScriptableObject<T>() where T : ScriptableObject {
            T so = null;
            var assets = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            if (assets.Length == 0) {
                Debug.LogError(typeof(T).Name + " file was not found. (If you see this message on project import, and the scriptable object exists, it means it was not yet imported, rerun parsers and updaters.)");
            }
            else {
                so = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(T)) as T;
            }
            return so;
        }
#endif

        public static bool Null(this Component mono) {
            return (object)mono == null ? true : false;
        }

        public static bool DisableIfNotFound(this MonoBehaviour mono, object obj, string errorMessage) {
            if (obj == null) {
                Debug.LogError(errorMessage, mono);
                mono.gameObject.SetActive(false);
                return true;
            }
            else
                return false;
        }

        public static void SetLayerRecursive(this GameObject go, int layer) {
            go.layer = layer;
            setlayerrecursive(go.transform, layer);
        }

        static void setlayerrecursive(Transform tr, int layer) {
            foreach (Transform child in tr) {
                setlayerrecursive(child, layer);
                child.gameObject.layer = layer;
            }
        }


        public static void AddComponentToAllChildren<T>(this GameObject mono, bool removeAllIfExists) where T : Component {
            add_component_recursive<T>(mono.transform, removeAllIfExists);
        }

        public static void AddComponentToAllChildren<T>(this GameObject mono) where T : Component {
            add_component_recursive<T>(mono.transform, false);
        }

        static void add_component_recursive<T>(Transform tr, bool removeIfExists) where T : Component {
            foreach (Transform c in tr) {
                add_component_recursive<T>(c, removeIfExists);
                if (removeIfExists) {
                    var comps = c.gameObject.GetComponents<T>();
                    foreach (var co in comps) {
#if UNITY_EDITOR
                        GameObject.DestroyImmediate(co);
#endif
                    }
                }
                c.gameObject.AddComponent<T>();
            }
        }

        public static void OneFrameDelay(this MonoBehaviour mb, Action action) {
            mb.StartCoroutine(Enumerators.Addframedelay(action));
        }
        public static GameObject Spawn(string path) {
            return Spawn(path, false);
        }

        public static GameObject Spawn(string path, bool startDisabled) {
            bool initstate;
            var pref = Resources.Load(path) as GameObject;
            if (pref == null) {
                Debug.LogError("Did not find prefab by path: " + path);
            }
            initstate = pref.activeSelf;
            if (startDisabled)
                pref.gameObject.SetActive(false);
            if (pref) {
                GameObject go = GameObject.Instantiate(pref);
                pref.SetActive(initstate);
                return go;
            }
            else {
                Debug.LogError("Resource:" + path + " not found.");
                return null;
            }
        }

        public static GameObject Spawn(GameObject prefab, bool startDisabled) {
            bool initstate;
            initstate = prefab.activeSelf;
            if (startDisabled)
                prefab.gameObject.SetActive(false);
            if (prefab) {
                GameObject go = GameObject.Instantiate(prefab);
                prefab.SetActive(initstate);
                return go;
            }
            else {
                Debug.LogError("Prefab:" + prefab + " not found.");
                return null;
            }
        }

#if UNITY_EDITOR
        public static GameObject SpawnEditor(GameObject pref) {
            if (pref) {
                var go = PrefabUtility.InstantiatePrefab(pref) as GameObject;
                Selection.activeGameObject = go;
                //SceneView.lastActiveSceneView.FrameSelected();
                return go;
            }
            else
                return null;
        }

        public static GameObject SpawnEditor(string path) {
            var pref = Resources.Load(path) as GameObject;
            if (pref) {
                var go = PrefabUtility.InstantiatePrefab(pref) as GameObject;
                Selection.activeGameObject = go;
                SceneView.lastActiveSceneView.FrameSelected();
                return go;
            }
            else
                return null;
        }

#endif


        public delegate object ObjectActivator();

        public static ObjectActivator CreateCtor(Type type) {

            if (type == null) {
                Debug.LogError("ObjectActivator: tried to create null type, fail.");
            }
            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            var dynamicMethod = new DynamicMethod("CreateInstance", type, Type.EmptyTypes, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));

        }

        /// <summary>
        /// False if duplicates found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool CheckSingleInstance<T>() where T : MonoBehaviour {
            T[] arr = GameObject.FindObjectsOfType<T>();

            string typename = typeof(T).ToString();

            if (arr.Length > 1) {
                Debug.LogError("Another instance of " + typename + " found, please, make sure there is always only one isntance.");
                Debug.Break();
                return false;
            }
            return true;
        }

        public static T GetInterface<T>(this GameObject inObj) where T : class {
            if (!typeof(T).IsInterface) {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return null;
            }
            var objs = inObj.GetComponents<Component>();
            return objs.OfType<T>().FirstOrDefault();
        }

        public static List<T> GetInterfaces<T>(this GameObject inObj) where T : class {
            if (!typeof(T).IsInterface) {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return null;
            }

            return inObj.GetComponents<Component>().OfType<T>().ToList();
        }

        public static List<T> GetInterfacesInStack<T>(this GameObject inObj) where T : class {
            if (!typeof(T).IsInterface) {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return null;
            }
            List<T> list = new List<T>();
            list.AddRange(inObj.GetComponents<Component>().OfType<T>().ToList());

            foreach (Transform t in inObj.transform) {
                t.gameObject.getinterfacesInStackRecursive<T>(list);
            }
            return list;
        }

        static void getinterfacesInStackRecursive<T>(this GameObject inObj, List<T> list) where T : class {
            if (!typeof(T).IsInterface) {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return;
            }

            list.AddRange(inObj.GetComponents<Component>().OfType<T>().ToList());

            foreach (Transform t in inObj.transform) {
                t.gameObject.getinterfacesInStackRecursive<T>(list);
            }
        }
    }
}
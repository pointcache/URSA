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
public enum Axis {
    x, y, z
}

public enum Channel {
    R,
    G,
    B,
    A
}

public enum Direction {
    forward,
    backward,
    right,
    left,
    up,
    down
}

public class Enumerators {

    public static IEnumerator addframedelay(Action action) {
        yield return new WaitForEndOfFrame();
        action();
    }
}

public class NotEditableStringAttribute : PropertyAttribute { }

public static class Helpers {


    public static string dataPathWithoutAssets
    {
        get {
            string datapath = Application.dataPath;
            int index = datapath.IndexOf("/Assets");
            return datapath.Remove(index, 7);
        }
    }

    public static string ClearPathToResources(this string path) {
        int index = path.IndexOf("/Resources/");
        return path.Remove(0, index + 11);
    }

    public static string RemoveExtension(this string path) {
        int index = path.LastIndexOf('.');
        return path.Remove(index, path.Length - index);
    }

#if UNITY_EDITOR

    public static T FindScriptableObject<T>() where T : ScriptableObject {
        T so = null;
        var assets = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        if (assets.Length == 0) {
            Debug.LogError(typeof(T).Name + " file was not found. (If you see this message on project import, and the scriptable object exists, it means it was not yet imported, rerun parsers and updaters.)");
        } else {
            so = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(T)) as T;
        }
        return so;
    }
#endif

    public static void SetAllChildren(this Transform tr, bool state) {
        foreach (Transform t in tr) {
            t.gameObject.SetActive(state);
        }
    }

    //Traverses upwards until first found in parent or null
    public static T GetComponentInParents<T>(this Component comp) where T : Component {
        T c = comp.GetComponent<T>();
        if (c != null)
            return c;
        else
            return getComponentInParentsRecursive<T>(comp.transform.parent);

    }

    static T getComponentInParentsRecursive<T>(Transform t) where T : Component {

        if (t == null)
            return null;
        T c = t.GetComponent<T>();
        if (c != null)
            return c;
        else
            return getComponentInParentsRecursive<T>(t.parent);

    }

    public static Vector2 GetRealMax(this RectTransform rect) {
        float x = rect.position.x + (rect.sizeDelta.x * (1 - rect.pivot.x));
        float y = rect.position.y + (rect.sizeDelta.y * (1 - rect.pivot.y));
        return new Vector2(x, y);
    }

    public static Vector2 GetRealMin(this RectTransform rect) {
        float x = (rect.position.x - (rect.sizeDelta.x * rect.pivot.x));
        float y = rect.position.y - (rect.sizeDelta.y * rect.pivot.y);
        return new Vector2(x, y);
    }

    public static void DisableAllChildren(this Transform tr) {
        foreach (Transform t in tr) {
            t.gameObject.SetActive(false);
        }
    }

    public static bool Null(this Component mono) {
        return (object)mono == null ? true : false;
    }

    public static bool DisableIfNotFound(this MonoBehaviour mono, object obj, string errorMessage) {
        if (obj == null) {
            Debug.LogError(errorMessage, mono);
            mono.gameObject.SetActive(false);
            return true;
        } else
            return false;
    }

    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName) {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent) {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    public static bool isEmpty<T>(this List<T> list) {
        if (list == null)
            return true;
        if (list.Count == 0)
            return true;
        int c = list.Count;
        for (int i = 0; i < c; i++) {
            if (list[i] == null)
                continue;
            else
                return false;
        }
        return true;
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

#if UNITY_EDITOR
    public static string GetSelectedPathOrFallback() {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
#endif

    public static void ZeroOut(this Transform tr) {
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
    }

    public static void ZeroOutLocal(this Transform tr) {
        tr.localPosition = Vector3.zero;
        tr.localRotation = Quaternion.identity;
    }

    public static Vector3 GetAxis(this Axis axis) {
        switch (axis) {
            case Axis.x:
                return Vector3.right;

            case Axis.y:
                return Vector3.up;

            case Axis.z:
                return Vector3.forward;
        }
        return Vector3.zero;
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
        mb.StartCoroutine(Enumerators.addframedelay(action));
    }
    public static GameObject Spawn(string path) {
        return Spawn(path, false);
    }

    public static GameObject Spawn(string path, bool startDisabled) {
        bool initstate;
        var pref = Resources.Load(path) as GameObject;
        initstate = pref.activeSelf;
        if (startDisabled)
            pref.gameObject.SetActive(false);
        if (pref) {
            GameObject go = GameObject.Instantiate(pref);
            pref.SetActive(initstate);
            return go;
        } else {
            Debug.LogError("Resource:" + path + " not found.");
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
        } else
            return null;
    }

    public static GameObject SpawnEditor(string path) {
        var pref = Resources.Load(path) as GameObject;
        if (pref) {
            var go = PrefabUtility.InstantiatePrefab(pref) as GameObject;
            Selection.activeGameObject = go;
            SceneView.lastActiveSceneView.FrameSelected();
            return go;
        } else
            return null;
    }

#endif
    public static Color SetAlpha(this Color color, float a) {
        return new Color(color.r, color.g, color.b, a);
    }

    public static void Clear<T>(this T[] arr) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] = default(T);
        }
    }

    public static float Remap(this r_float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    public static double Remap(this double value, double from1, double to1, double from2, double to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void GetChildren(this Transform tr, List<Transform> container) {
        if (container == null) {
            Debug.LogError("Attempt to use null List");
            return;
        }
        foreach (Transform child in tr) {
            container.Add(child);
        }
    }

    public static void DestroyChildren(this Transform tr) {
        if (tr.childCount == 0)
            return;
        List<Transform> list = new List<Transform>();
        foreach (Transform child in tr) {
            list.Add(child);
        }
        int count = list.Count;
        for (int i = 0; i < count; i++) {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(list[i].gameObject);
#else
            GameObject.Destroy(list[i].gameObject);

#endif
        }
    }

    public static void GetAllChildren(this Transform tr, List<Transform> container) {
        if (container == null) {
            Debug.LogError("Attempt to use null List");
            return;
        }
        _GetAllChildrenRecursive(tr, container);
    }

    private static void _GetAllChildrenRecursive(Transform tr, List<Transform> container) {
        foreach (Transform child in tr) {
            container.Add(child);
            _GetAllChildrenRecursive(child, container);
        }
    }

    public static Vector2[] GetScreenCorners(this RectTransform tr) {
        Vector3[] corners = new Vector3[4];
        tr.GetWorldCorners(corners);
        Vector2[] s_corners = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            s_corners[i].x = corners[i].x;
            s_corners[i].y = corners[i].y;
        }

        return s_corners;
    }

    public static Vector3 WithX(this Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }

    /// <summary>
    /// Negative values to subtract
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 AddFloats(this Vector3 vec, float x, float y, float z) {
        vec.x += x;
        vec.y += y;
        vec.z += z;
        return vec;
    }
    public static Vector3 AddFloats(this Vector3 vec, float val) {
        return AddFloats(vec, val, val, val);
    }
    public static Vector3 MultiplyFloats(this Vector3 vec, float x, float y, float z) {
        vec.x *= x;
        vec.y *= y;
        vec.z *= z;
        return vec;
    }

    public static Vector3 MultiplyFloat(this Vector3 vec, float x) {
        vec.x *= x;
        vec.y *= x;
        vec.z *= x;
        return vec;
    }

    public static Vector3 DivideFloats(this Vector3 vec, float x, float y, float z) {
        if (x == 0 || y == 0 || z == 0) {
            Debug.LogError("Top lel.");
        }
        vec.x /= x;
        vec.y /= y;
        vec.z /= z;
        return vec;
    }

    //Converts 3d direction vector into 2d, basically flattens it
    public static Vector3 ConvertTo2dCoords(this Vector3 vec) {
        float mag = vec.magnitude;
        vec.y = 0;
        return vec.normalized * mag;
    }

    //Converts 3d direction vector into 2d, basically flattens it
    public static Vector3 ConvertTo2dCoordsNoY(this Vector3 vec) {
        vec.y = 0f;
        float mag = vec.magnitude;
        vec = vec.normalized;
        vec.y = 0;
        return vec * mag;
    }

    //Converts 3d direction vector into 2d, basically flattens it
    public static Vector2 MakeV2(this Vector3 vec) {
        return new Vector2(vec.x, vec.y);
    }

    //Imagine two circles inside each other, this limits vector to only area in between the two
    public static Vector3 LimitRadiusArea(this Vector3 vec, float min, float max) {
        vec.x = _limiradiusArea(vec.x, max, min);
        vec.y = _limiradiusArea(vec.y, max, min);
        vec.z = _limiradiusArea(vec.z, max, min);
        return vec;
    }
    private static float _limiradiusArea(float _in, float max, float min) {
        if (_in > 0) {
            if (_in > max)
                _in = max;
            if (_in < min)
                _in = min;
        } else {
            if (_in < max * -1)
                _in = max * -1;
            if (_in > min * -1)
                _in = min * -1;
        }
        return _in;
    }


    //Converts vector2 to vector3 so that x = x, but y = z, basically from 2d plane to proper 3d
    public static Vector3 ToVector3d(this Vector2 vec) {
        return new Vector3(vec.x, 0, vec.y);
    }

    //Converts vector2 to vector3 so that x = x, but y = z, basically from 2d plane to proper 3d also sets Y value
    public static Vector3 ToVector3d(this Vector2 vec, float Y) {
        return new Vector3(vec.x, Y, vec.y);
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;

        }
    }

    public static T RandomEnumValue<T>() {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(new System.Random().Next(v.Length));
    }

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

    public static float WrapNumber(float value, float min, float max) {
        float result;
        if (value < min)
            result = max - (min - value) % (max - min);
        else
            result = min + (value - min) % (max - min);

        return result;
    }

    public static string[] ListToString<T>(List<T> list) {
        string[] result = new string[list.Count];
        for (int i = 0; i < list.Count; i++) {
            result[i] = list[i].ToString();
        }
        return result;
    }

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    #region Multilerp
    // This is not very optimized. There are at least n + 1 and at most 2n Vector3.Distance
    // calls (where n is the number of waypoints). 
    public static Vector3 MultiLerp(Vector3[] waypoints, float ratio) {
        Vector3 position = Vector3.zero;
        float totalDistance = waypoints.MultiDistance();
        float distanceTravelled = totalDistance * ratio;

        int indexLow = GetVectorIndexFromDistanceTravelled(waypoints, distanceTravelled);
        int indexHigh = indexLow + 1;

        // we're done
        if (indexHigh > waypoints.Length - 1)
            return waypoints[waypoints.Length - 1];


        // calculate the distance along this waypoint to the next
        Vector3[] completedWaypoints = new Vector3[indexLow + 1];

        for (int i = 0; i < indexLow + 1; i++) {
            completedWaypoints[i] = waypoints[i];
        }

        float distanceCoveredByPreviousWaypoints = completedWaypoints.MultiDistance();
        float distanceTravelledThisSegment = distanceTravelled - distanceCoveredByPreviousWaypoints;
        float distanceThisSegment = Vector3.Distance(waypoints[indexLow], waypoints[indexHigh]);

        float currentRatio = distanceTravelledThisSegment / distanceThisSegment;
        position = Vector3.Lerp(waypoints[indexLow], waypoints[indexHigh], currentRatio);

        return position;
    }

    public static float MultiDistance(this Vector3[] waypoints) {
        float distance = 0f;

        for (int i = 0; i < waypoints.Length; i++) {
            if (i + 1 > waypoints.Length - 1)
                break;

            distance += Vector3.Distance(waypoints[i], waypoints[i + 1]);
        }

        return distance;
    }

    public static int GetVectorIndexFromDistanceTravelled(Vector3[] waypoints, float distanceTravelled) {
        float distance = 0f;

        for (int i = 0; i < waypoints.Length; i++) {
            if (i + 1 > waypoints.Length - 1)
                return waypoints.Length - 1;

            float segmentDistance = Vector3.Distance(waypoints[i], waypoints[i + 1]);

            if (segmentDistance + distance > distanceTravelled) {
                return i;
            }

            distance += segmentDistance;
        }

        return waypoints.Length - 1;
    }
    #endregion
}
public static class PrimitiveHelper {
    private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

    public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider) {
        if (withCollider) { return GameObject.CreatePrimitive(type); }

        GameObject gameObject = new GameObject(type.ToString());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = PrimitiveHelper.GetPrimitiveMesh(type);
        gameObject.AddComponent<MeshRenderer>();

        return gameObject;
    }

    public static Mesh GetPrimitiveMesh(PrimitiveType type) {
        if (!PrimitiveHelper.primitiveMeshes.ContainsKey(type)) {
            PrimitiveHelper.CreatePrimitiveMesh(type);
        }

        return PrimitiveHelper.primitiveMeshes[type];
    }

    private static Mesh CreatePrimitiveMesh(PrimitiveType type) {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

#if UNITY_EDITOR
        GameObject.DestroyImmediate(gameObject);

#else
        GameObject.Destroy(gameObject);
#endif


        PrimitiveHelper.primitiveMeshes[type] = mesh;
        return mesh;
    }
}
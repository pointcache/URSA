using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

//made by pointcache
//start painting = "C"
//stop = any other key without modifiers
//or just use toggle button
//remember rotation samples any last selected object, so just select and then paint and it will work.
public class ObjectPaint : EditorWindow
{
    bool paint;
    bool prevpaint;
    bool randomrotation;
    bool randomscale;
    bool rememberLastRotation;
    bool rememberScale;
    bool useMultiple;
    bool customMask;
    LayerMask mask;
    public float scaleModifier = 1f;
    public float minScale = 1f;
    public float maxScale = 1f;
    GameObject prefab;
    Vector3 lastRotation;
    Vector3 lastScale;
    GameObject lastSelected;
    public List<GameObject> multiple = new List<GameObject>();
    SerializedObject serObj;


    // Add menu item named "My Window" to the Window menu
    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_TOOLS + "/ObjectPaint")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ObjectPaint));
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        if (GUILayout.Button("Reset All"))
        {
            paint = false;

            randomrotation = false;
            randomscale = false;
            rememberLastRotation = false;
            rememberScale = false;
            useMultiple = false;
            customMask = false;

            scaleModifier = 1f;
            minScale = 1f;
            maxScale = 1f;
            prefab = null;


            multiple.Clear();

        }
        GUI.color = Color.white;
        GUILayout.Label("made by pointcache");

        SerializedProperty prop = serObj.FindProperty("multiple");
        serObj.Update();
        EditorGUILayout.BeginHorizontal();
        prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
        if (GUILayout.Button("Get selected"))
        {
            if (Selection.gameObjects.Length < 2)
            {

                if (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.PrefabInstance)
                {
                    prefab = PrefabUtility.GetPrefabParent(Selection.activeGameObject) as GameObject;
                }
                else
                if (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.Prefab)
                {
                    prefab = Selection.activeGameObject;
                }
                else
                {
                    Debug.LogError("Not a prefab, or prefab instance, stop screwing around.");
                }

            }
            else
            {
                multiple.Clear();
                foreach (var go in Selection.gameObjects)
                {
                    if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                    {
                        multiple.Add(PrefabUtility.GetPrefabParent(go) as GameObject);
                    }
                    else
                        if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
                    {
                        multiple.Add(go);
                    }
                    else
                    {
                        Debug.LogError("Not a prefab, or prefab instance, stop screwing around.");
                    }
                }
            }

        }
        EditorGUILayout.EndHorizontal();

        useMultiple = EditorGUILayout.BeginToggleGroup("Use Multiple objects", useMultiple);

        EditorGUILayout.PropertyField(prop, true);
        EditorGUILayout.EndToggleGroup();

        customMask = EditorGUILayout.BeginToggleGroup("Use Custom LayerMask", customMask);

        mask = LayerMaskField("LayerMask", mask);
        EditorGUILayout.EndToggleGroup();

        scaleModifier = EditorGUILayout.FloatField("Scale modifier", scaleModifier);

        randomscale = EditorGUILayout.BeginToggleGroup("Random Scale", randomscale);

        EditorGUILayout.BeginHorizontal();
        minScale = EditorGUILayout.FloatField("Min scale", minScale);
        if (GUILayout.Button("Get"))
        {
            if (Selection.activeGameObject)
            {
                minScale = Selection.activeGameObject.transform.localScale.x;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        maxScale = EditorGUILayout.FloatField("Max scale", maxScale);
        if (GUILayout.Button("Get"))
        {
            if (Selection.activeGameObject)
            {
                maxScale = Selection.activeGameObject.transform.localScale.x;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndToggleGroup();


        randomrotation = EditorGUILayout.Toggle("Random Rotation", randomrotation);


        EditorGUILayout.BeginHorizontal();
        rememberLastRotation = EditorGUILayout.Toggle("Remember Rotation", rememberLastRotation);
        if (GUILayout.Button("Reset Rotation"))
        {
            if (Selection.activeGameObject)
            {
                if (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.PrefabInstance)
                {
                    Selection.activeGameObject.transform.rotation = ((GameObject)PrefabUtility.GetPrefabParent(Selection.activeGameObject)).transform.rotation;
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        rememberScale = EditorGUILayout.Toggle("Remember Scale", rememberScale);
        if (GUILayout.Button("Reset Scale"))
        {
            if (Selection.activeGameObject)
            {
                if (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.PrefabInstance)
                {
                    Selection.activeGameObject.transform.localScale = ((GameObject)PrefabUtility.GetPrefabParent(Selection.activeGameObject)).transform.localScale;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        paint = EditorGUILayout.Toggle("Paint", paint);

        serObj.ApplyModifiedProperties();
    }

    void OnEnable()
    {
        serObj = new UnityEditor.SerializedObject(this);
        SceneView.onSceneGUIDelegate += Scene;
        Selection.selectionChanged += select;

        mask.value = EditorPrefs.GetInt("pointcache_objectpaint_mask");
        useMultiple = EditorPrefs.GetBool("pointcache_objectpaint_useMultiple");
        randomrotation = EditorPrefs.GetBool("pointcache_objectpaint_randomRotation");
        randomscale = EditorPrefs.GetBool("pointcache_objectpaint_randomScale");
        rememberLastRotation = EditorPrefs.GetBool("pointcache_objectpaint_rememberLastRotation");
        rememberScale = EditorPrefs.GetBool("pointcache_objectpaint_rememberScale");
        customMask = EditorPrefs.GetBool("pointcache_objectpaint_useCustomMask");
        minScale = EditorPrefs.GetFloat("pointcache_objectpaint_minScale");
        maxScale = EditorPrefs.GetFloat("pointcache_objectpaint_maxScale");
    }

    void select()
    {
        lastSelected = Selection.activeGameObject;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= Scene;
        Selection.selectionChanged -= select;

        EditorPrefs.SetInt("pointcache_objectpaint_mask", mask.value);
        EditorPrefs.SetBool("pointcache_objectpaint_useMultiple", useMultiple);
        EditorPrefs.SetBool("pointcache_objectpaint_randomRotation", randomrotation);
        EditorPrefs.SetBool("pointcache_objectpaint_randomScale", randomscale);
        EditorPrefs.SetBool("pointcache_objectpaint_rememberLastRotation", rememberLastRotation);
        EditorPrefs.SetBool("pointcache_objectpaint_rememberScale", rememberScale);
        EditorPrefs.SetBool("pointcache_objectpaint_useCustomMask", customMask);
        EditorPrefs.SetFloat("pointcache_objectpaint_minScale", minScale);
        EditorPrefs.SetFloat("pointcache_objectpaint_maxScale", maxScale);

    }

    public void Update()
    {
        // This is necessary to make the framerate normal for the editor window.
        Repaint();

    }
    
    int getrandommultiple()
    {
        int random = Random.Range(0, multiple.Count);
        if (multiple[random] == null)
            return getrandommultiple();
        return random;
    }

    void Scene(SceneView sceneView)
    {
        
        string paintingnotification = EditorGUILayout.TextField("Painting");
        string stoppednotification = EditorGUILayout.TextField("Stopped painting");
        if (paint && paint != prevpaint)
        {
            if (lastSelected)
            {
                lastRotation = lastSelected.transform.rotation.eulerAngles;
                lastScale = lastSelected.transform.localScale;
            }
            sceneView.ShowNotification(new GUIContent(paintingnotification));
            prevpaint = paint;
        }
        else if (paint != prevpaint)
        {
            sceneView.RemoveNotification();
            sceneView.ShowNotification(new GUIContent(stoppednotification));
            prevpaint = paint;
        }

        if (Event.current.type == EventType.KeyDown && Event.current.modifiers == EventModifiers.None)
        {
            if (Event.current.keyCode != KeyCode.None)
                paint = false;
            if (Event.current.keyCode == KeyCode.C)
                paint = true;
        }

        if (!paint)
        {
            sceneView.RemoveNotification();
            return;
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        if (Event.current != null)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.modifiers == EventModifiers.None)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit = new RaycastHit();
                LayerMask temp = customMask ? mask.value : Physics.AllLayers;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, temp, QueryTriggerInteraction.Ignore))
                {
                    GameObject go = null;
                    if (!useMultiple)
                    {
                        if (prefab == null)
                            return;
                        go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    }
                    else
                    {
                        if (multiple == null && multiple.Count == 0)
                            return;
                        int random = Random.Range(0, multiple.Count);
                        go = PrefabUtility.InstantiatePrefab(multiple[random]) as GameObject;
                    }
                    Undo.RegisterCreatedObjectUndo(go, go.name);
                    go.transform.position = hit.point;
                    if (randomscale)
                        go.transform.localScale = MultiplyFloat(Vector3.one, Random.Range(minScale, maxScale));
                    else
                    {
                        if (rememberScale)
                            go.transform.localScale = lastScale;
                        else
                            go.transform.localScale = MultiplyFloat(go.transform.localScale, scaleModifier);

                    }

                    if (randomrotation)
                    {
                        Vector3 vec = go.transform.rotation.eulerAngles;
                        vec.y = Random.Range(0f, 360f);
                        go.transform.rotation = Quaternion.Euler(vec);
                    }
                    else if (rememberLastRotation)
                    {
                        go.transform.rotation = Quaternion.Euler(lastRotation);
                    }
                    lastRotation = go.transform.rotation.eulerAngles;
                    lastScale = go.transform.localScale;
                    Event.current.Use();
                    Selection.activeGameObject = go;
                    lastSelected = go;
                    Debug.Log("Spawned " + go.name);
                }
            }
        }
        }

    static Vector3 MultiplyFloat(Vector3 vec, float x)
    {
        vec.x *= x;
        vec.y *= x;
        vec.z *= x;
        return vec;
    }

    static List<int> layerNumbers = new List<int>();

    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = UnityEditor.EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
    }
}

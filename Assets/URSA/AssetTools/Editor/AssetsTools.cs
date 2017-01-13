using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using URSA;
public static class AssetsTools
{

    static AssetToolsSettings settings;


    [MenuItem(URSAConstants.MENUITEM_ROOT + "/ParseResources", priority = 2)]
    public static void ParseResources()
    {
        settings = Helpers.FindScriptableObject<AssetToolsSettings>();

        var @static = Resources.LoadAll(settings.rootPath + settings.@static);
        var dynamic = Resources.LoadAll(settings.rootPath + settings.dynamic);
        var lights = Resources.LoadAll(settings.rootPath + settings.lights);
        var volumes = Resources.LoadAll(settings.rootPath + settings.volumes);
        var fx = Resources.LoadAll(settings.rootPath + settings.fx);
        var enemy = Resources.LoadAll(settings.rootPath + settings.enemy);
        var npc = Resources.LoadAll(settings.rootPath + settings.npc);
        int count = 0;

        addObjectType(@static, AssetType.ObjType.@static, ref count);
        addObjectType(dynamic, AssetType.ObjType.dynamic, ref count);
        addObjectType(lights, AssetType.ObjType.light, ref count);
        addObjectType(volumes, AssetType.ObjType.volume, ref count);
        addObjectType(fx, AssetType.ObjType.fx, ref count);
        addObjectType(enemy, AssetType.ObjType.enemy, ref count);
        addObjectType(npc, AssetType.ObjType.npc, ref count);

        Debug.Log(count + "<color=green> resources parsed</color>");
    }

    static void addObjectType(UnityEngine.Object[] objs, AssetType.ObjType type, ref int count)
    {
        foreach (var obj in objs)
        {
            count++;
            GameObject go = obj as GameObject;

            var objtype = go.GetComponent<AssetType>();
            if (!objtype)
                objtype = go.AddComponent<AssetType>();
            objtype.type = type;

            if (objtype.type == AssetType.ObjType.@static)
            {
                set_static(objtype.transform);
            }
            else
            {
                unset_static_recursive(objtype.transform);
            }

            process_for_modifiers(objtype.transform, objtype.type);
        }
    }

    static void process_for_modifiers(Transform tr, AssetType.ObjType type)
    {
        if (tr.GetComponent<NavmeshArea>())
        {
            var area = tr.GetComponent<NavmeshArea>();
            GameObjectUtility.SetNavMeshArea(tr.gameObject, (int)area.area);
        }
        else
        {
            if (type == AssetType.ObjType.@static)
                GameObjectUtility.SetNavMeshArea(tr.gameObject, (int)settings.defaultNavmeshArea);
        }
        foreach (Transform t in tr)
        {
            process_for_modifiers(t, type);
        }
    }

    static void set_navigation_static_recursive(Transform tr)
    {
        foreach (Transform t in tr)
        {
            set_navigation_static_recursive(t);
            GameObjectUtility.SetStaticEditorFlags(t.gameObject, StaticEditorFlags.NavigationStatic);
        }
    }

    static void set_static(Transform tr)
    {
        set_static_recursive(tr);
        
        if (tr.gameObject.GetComponent<NavmeshIgnore>()) //|| tr.gameObject.GetComponent<FoggyLight>())
        {
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(tr.gameObject);
            flags = flags & ~(StaticEditorFlags.NavigationStatic);
            set_editor_flags_recursive(tr, flags);
            return;
        }
    }

    static void set_static_recursive(Transform tr)
    {
        tr.gameObject.isStatic = true;
        foreach (Transform t in tr)
            set_static_recursive(t);
    }

    static void set_editor_flags_recursive(Transform tr, StaticEditorFlags flags)
    {
        GameObjectUtility.SetStaticEditorFlags(tr.gameObject, flags);
        foreach (Transform t in tr)
            set_editor_flags_recursive(t, flags);
    }

    static void unset_static_recursive(Transform tr)
    {
        tr.gameObject.isStatic = false;
        foreach (Transform t in tr)
        {
            unset_static_recursive(t);
        }
    }

    [MenuItem(URSAConstants.MENUITEM_ROOT + "/Assets/MoveColliderToParent")]
    public static void MoveColliderToParent()
    {
        var sel = Selection.activeGameObject;
        if (sel)
        {
            if (sel.transform.parent != null)
            {
                GameObject parent = sel.transform.parent.gameObject;
                Collider col = sel.GetComponent<Collider>();
                if (col is BoxCollider)
                {
                    var box = col as BoxCollider;
                    var pcol = parent.AddComponent<BoxCollider>();

                    pcol.size = box.size;
                    pcol.center = box.center;

                }
                else
                    if (col is SphereCollider)
                {
                    var sphere = col as SphereCollider;
                    var pcol = parent.AddComponent<SphereCollider>();

                    pcol.center = sphere.center;
                    pcol.radius = sphere.radius;
                }
                else
                    if (col is CapsuleCollider)
                {
                    var capsule = col as CapsuleCollider;
                    var pcol = parent.AddComponent<CapsuleCollider>();

                    pcol.center = capsule.center;
                    pcol.radius = capsule.radius;
                    pcol.height = capsule.height;
                    pcol.direction = capsule.direction;
                }
                else
                    if (col is MeshCollider)
                {
                    var meshcol = col as MeshCollider;
                    var pcol = parent.AddComponent<MeshCollider>();

                    pcol.convex = meshcol.convex;
                    pcol.sharedMesh = meshcol.sharedMesh;
                }

                var parentcol = parent.GetComponent<Collider>();
                parentcol.isTrigger = col.isTrigger;
                parentcol.sharedMaterial = col.sharedMaterial;
                parentcol.material = col.material;
                parentcol.enabled = col.enabled;
                GameObject.DestroyImmediate(col);
            }
        }
    }

    [MenuItem(URSAConstants.MENUITEM_ROOT + "/Assets/FindAndApplyCollider")]
    public static void FindApplyCollider()
    {
        GameObject selected = Selection.activeGameObject;
        if (!selected)
            return;

        var collider = AssetDatabase.FindAssets(selected.name + "_COL");
        string guid = collider[0];

        var path = AssetDatabase.GUIDToAssetPath(guid);
        GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

        List<Transform> children = go.GetComponentsInChildren<Transform>().ToList();
        children.Remove(go.transform);
        var colRoot = new GameObject("_COL");
        colRoot.transform.SetParent(selected.transform);

        int count = 0;

        foreach (var childtr in children)
        {
            var colgo = GameObject.Instantiate(childtr.gameObject, colRoot.transform) as GameObject;
            count++;

            colgo.name = "_COL" + count;
            var mf = colgo.gameObject.GetComponent<MeshFilter>();
            var mc = colgo.AddComponent<MeshCollider>();
            mc.convex = true;
            mc.sharedMesh = mf.sharedMesh;
            GameObject.DestroyImmediate(mf, true);
            GameObject.DestroyImmediate(colgo.GetComponent<MeshRenderer>(), true);
        }
    }

    
    [MenuItem(URSAConstants.MENUITEM_ROOT + "/DANGER/Prefabs/DumpPrefabsFromFolder")]
    public static void DumpPrefabsFromFolder()
    {
        string path = Helpers.GetSelectedPathOrFallback();
        Debug.Log(path);
        if (path == string.Empty)
            return;
        if (!path.Contains("Resources/"))
            return;

        path = path.Replace("Assets/Resources/", string.Empty);

        var all = Resources.LoadAll(path);

        foreach (var prefab in all)
        {
            PrefabUtility.InstantiatePrefab(prefab);
        }
    }

    [MenuItem(URSAConstants.MENUITEM_ROOT + "/DANGER/Prefabs/ApplyChangesToSelectedPrefabs")]
    public static void ApplyChangesToSelectedPrefabs()
    {
        var all = Selection.gameObjects;

        foreach (var pref in all)
        {
            if (pref.transform.parent != null)
                continue;
            GameObject go = pref as GameObject;
            var head = PrefabUtility.GetPrefabParent(go) as GameObject;
            string path = AssetDatabase.GetAssetPath(head);
            Debug.Log(path);
            PrefabUtility.ReplacePrefab(go, head, ReplacePrefabOptions.ReplaceNameBased);
        }
    }


    [MenuItem(URSAConstants.MENUITEM_ROOT +  "/Assets/SlightlyOffsetChildren")]
    public static void SlightlyOffsetChildren()
    {
        var sel = Selection.gameObjects;
        foreach (var go in sel)
        {
            if (go)
            {
                foreach (Transform c in go.transform)
                {
                    c.position = c.position.AddFloats(0f, 0.05f, 0f);
                }
            }
        }
    }
}

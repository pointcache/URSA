using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

public class BlueprintLoader : MonoBehaviour {

    public TextAsset blueprint;
    public bool LoadOnEnable = true;
    public bool CreateRootObject = true;

    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_BLUEPRINT + URSAConstants.MENUITEM_BLUEPRINT_NEW)]
    public static void New() {
        GameObject root = new GameObject("Blueprint");
        Selection.activeGameObject = root;
        root.AddComponent<BlueprintLoader>();
        SceneView.lastActiveSceneView.FrameSelected();
    }

    public Blueprint Save() {
        Blueprint bp = new Blueprint();
        bp.saveObject = SaveSystem.CreateSaveObjectFromTransform(transform);
        bp.gameVersion = ProjectInfo.current.Version;
        bp.Name = gameObject.name;
        return bp;
    }

    //private void OnDrawGizmos() {
    //    Renderer[] rends = transform.GetComponentsInChildren<Renderer>(true);
    //    Vector3 min = Vector3.zero, max = Vector3.zero;
    //    foreach (var r in rends) {
    //        if (min.magnitude > r.bounds.min.magnitude)
    //            min = r.bounds.min;
    //        if (max.magnitude < r.bounds.max.magnitude)
    //            max = r.bounds.max;
    //    }
    //
    //    Vector3 size = new Vector3(
    //        Mathf.Abs(min.x) + Mathf.Abs(max.x),
    //        Mathf.Abs(min.y) + Mathf.Abs(max.y),
    //        Mathf.Abs(min.z) + Mathf.Abs(max.z)
    //
    //        );
    //
    //    Gizmos.DrawWireCube((max - min) * .5f, size);
    //
    //
    //}
}

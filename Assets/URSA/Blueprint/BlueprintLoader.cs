using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using URSA;
public class BlueprintLoader : MonoBehaviour {

    [Tooltip("Used to work with blueprints in editor")]
    public bool disableOnStart;
    public TextAsset blueprint;
    public bool LoadOnEnableAndSelfDestruct;
    public bool DontParent;
    public bool supressWarning;

    [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_BLUEPRINT + URSAConstants.PATH_MENUITEM_BLUEPRINT_NEW)]
    public static void New() {
        GameObject root = new GameObject("Blueprint");
        Selection.activeGameObject = root;
        root.AddComponent<BlueprintLoader>();
        SceneView.lastActiveSceneView.FrameSelected();
    }

    public Blueprint Save() {
        if (transform.childCount == 0)
            return null;
        Blueprint bp = new Blueprint();
        bp.saveObject = SaveSystem.CreateSaveObjectFromTransform(transform);
        bp.gameVersion = ProjectInfo.current.Version;
        bp.Name = gameObject.name;
        return bp;
    }

    public void Load() {
        if (blueprint == null)
            return;
        else {
            transform.DestroyChildren();
            SaveSystem.LoadBlueprint(blueprint.text, DontParent ? null : transform);
        }
    }

    public void OnEnable() {
        if (disableOnStart) {
            gameObject.SetActive(false);
            return;
        }
        if(transform.childCount > 0 && !supressWarning) {
            URSA.Log.Error("You can't have unboxed blueprints on the start of the game, make sure blueprints are cleared before the launch.");
            GetComponent<Entity>().log();
            Debug.Break();
            
        }

        if (LoadOnEnableAndSelfDestruct) {
            Load();
            Destroy(this);
        }
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

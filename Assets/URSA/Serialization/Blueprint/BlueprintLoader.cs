namespace URSA.Serialization.Blueprints {
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using URSA.Serialization;
    using URSA.Internal;
    using URSA.Utility;

    public class BlueprintLoader : MonoBehaviour {

        [Tooltip("Used to work with blueprints in editor")]
        public bool DisableOnStart;
        public TextAsset Blueprint;
        public bool LoadOnEnableAndSelfDestruct;
        public bool DontParent;
        public bool SupressWarning;

#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_BLUEPRINT + URSAConstants.PATH_MENUITEM_BLUEPRINT_NEW)]
        public static void New() {
            GameObject root = new GameObject("Blueprint");
            Selection.activeGameObject = root;
            root.AddComponent<BlueprintLoader>();
            SceneView.lastActiveSceneView.FrameSelected();
        }
#endif
        public Blueprint Save() {
            if (transform.childCount == 0)
                return null;
            Blueprint bp = new Blueprint();
            bp.SaveObject = SaveSystem.CreateSaveObjectFromTransform(transform);
            bp.GameVersion = ProjectInfo.current.Version;
            bp.Name = gameObject.name;
            return bp;
        }

        public void Load() {
            if (Blueprint == null)
                return;
            else {
                transform.DestroyChildren();
                SaveSystem.LoadBlueprint(Blueprint.text, DontParent ? null : transform);
            }
        }

        public void OnEnable() {
            if (DisableOnStart) {
                gameObject.SetActive(false);
                return;
            }
            if (transform.childCount > 0 && !SupressWarning) {
                Debug.LogError("You can't have unboxed blueprints on the start of the game, make sure blueprints are cleared before the launch.");
                Debug.Break();

            }

            if (LoadOnEnableAndSelfDestruct) {
                Load();
                Destroy(this);
            }
        }

    }

}
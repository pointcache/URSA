namespace URSA.Serialization {
    using UnityEngine;
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using URSA;
    using System.IO;
    using URSA.Serialization.Blueprints;
    using URSA.Internal;
    using URSA.Utility;
    using URSA.ECS.Entity;

    public class PersistentDataSystem : MonoBehaviour {


        #region SINGLETON
        private static PersistentDataSystem _instance;
        public static PersistentDataSystem instance
        {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<PersistentDataSystem>();
                return _instance;
            }
        }
        #endregion

        [Serializable]
        public class PersistentDataInfo {
            public SaveObject data;
            //Add your custom info here
            public string profileName;
            public DateTime creationDate;
        }

        public string FileName = "persistentGameData";
        public string FolderName = "PersistenData";
        public string extension = ".data";

        public static event Action OnPersistentDataLoaded = delegate { };

        [SerializeField]
        private Transform persistentDataRoot;

        public Transform PersistentDataRoot
        {
            get {
                if (!persistentDataRoot) {
                    persistentDataRoot = new GameObject("PersistentData").transform;
                    GameObject.DontDestroyOnLoad(persistentDataRoot);
                }
                return persistentDataRoot;
            }
        }

        private void OnEnable() {
        }

        public static void MakePersistent(Entity entity) {

            UnityEngine.Object.DontDestroyOnLoad(entity.gameObject);

            entity.transform.parent = instance.PersistentDataRoot;

            EntityManager.MarkPersistent(entity);

        }


#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_PERSISTENT + URSAConstants.PATH_MENUITEM_PERSISTENT_SAVE)]
#endif
        public static void Save() {
            instance.SaveTo();
        }



#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_PERSISTENT + URSAConstants.PATH_MENUITEM_PERSISTENT_LOAD)]
#endif
        public static void Load() {
            instance.LoadFrom();
        }

        public void SaveTo() {

            SaveObject file = SaveSystem.CreateSaveObjectFromPersistenData();
            PersistentDataInfo info = new PersistentDataInfo();
            info.profileName = "profile";
            info.creationDate = DateTime.Now;
            info.data = file;

            string path = PathUtilities.CustomDataPath + "/" + FolderName;

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            SerializationHelper.Serialize(info, path + "/" + FileName + extension, true);

        }

        public void LoadFrom() {
            ClearAnd(CompleteLoad);
        }

        public void LoadWithoutClear(TextAsset blueprint) {
            var bpLoader = GetComponent<BlueprintLoader>();
            if (!bpLoader) {
                Debug.LogError("Blueprint loader was not found, add it to the PersistentDataSystem");
                return;
            }
            bpLoader.Blueprint = blueprint;
            bpLoader.Load();
        }

        public void ClearAnd(Action and) {
            persistentDataRoot.DestroyChildren();
            this.OneFrameDelay(and);
        }

        public void ClearAndLoadBlueprint(TextAsset blueprint, Action onLoaded) {
            var bpLoader = GetComponent<BlueprintLoader>();
            if (!bpLoader) {
                Debug.LogError("Blueprint loader was not found, add it to the PersistentDataSystem");
                return;
            }
            ClearAnd(() => {
                bpLoader.Blueprint = blueprint;
                bpLoader.Load();
                if (onLoaded != null)
                    onLoaded();
            });
        }

        private void CompleteLoad() {
            string path = PathUtilities.CustomDataPath + "/" + FolderName + "/" + FileName + extension;

            if (File.Exists(path)) {
                var info = SaveSystem.DeserializeAs<PersistentDataInfo>(path);
                SaveSystem.UnboxSaveObject(info.data, persistentDataRoot);
                OnPersistentDataLoaded();
            }
            else
                Debug.LogError("PersistentDataSystem: File at path:" + path + " was not found");
        }
    }
}
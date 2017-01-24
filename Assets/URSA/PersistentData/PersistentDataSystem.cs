using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA.Save;
#if UNITY_EDITOR
using UnityEditor; 
#endif
using URSA;
using System.IO;

public class PersistentDataSystem : MonoBehaviour {


    #region SINGLETON
    private static PersistentDataSystem _instance;
    public static PersistentDataSystem instance
    {
        get {
            if (!_instance) _instance = GameObject.FindObjectOfType<PersistentDataSystem>();
            return _instance;
        }
    }
    #endregion

    public string FileName = "persistentGameData";
    public DataPath datapath;
    public string customDataPath;
    public string folderPath = "PersistenData";
    public string extension = ".data";

    public static event Action OnPersistentDataLoaded = delegate { };

    public static void MakePersistent(Entity entity) {
        UnityEngine.Object.DontDestroyOnLoad(entity.gameObject);
        entity.transform.parent = instance.transform;
    }


#if     UNITY_EDITOR
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

    static string getSystemPath() {
        var sys = PersistentDataSystem.instance;
        string path = String.Empty;
        switch (sys.datapath) {
            case DataPath.persistent: {
                    path = Application.persistentDataPath;
                }
                break;
            case DataPath.inRootFolder: {
                    path = Application.dataPath;
                }
                break;
            case DataPath.custom: {
                    path = sys.customDataPath;
                }
                break;
            default:
                break;
        }
        return path + "/" + URSASettings.current.CustomDataFolder;
    }

    public void SaveTo() {
        SaveObject file = SaveSystem.CreateSaveObjectFromPersistenData();
        PersistentDataInfo info = new PersistentDataInfo();
        info.profileName = "profile";
        info.creationDate = DateTime.Now;
        info.data = file;

        string path = getSystemPath() + "/" + folderPath;

        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        SerializationHelper.Serialize(info, path + "/" + FileName + extension, true);
    }

    public void LoadFrom() {

        ClearAnd(completeLoad);
    }

    public void ClearAnd(Action and) {
        transform.DestroyChildren();
        this.OneFrameDelay(and);
    }

    public void ClearAndLoadBlueprint(TextAsset blueprint, Action onLoaded) {
        var bpLoader = GetComponent<BlueprintLoader>();
        if (!bpLoader) {
            Debug.LogError("Blueprint loader was not found, add it to the PersistentDataSystem");
            return;
        }
        ClearAnd(() =>
        {
            bpLoader.blueprint = blueprint;
            bpLoader.Load();
            if(onLoaded != null) onLoaded();
        });
    }

    void completeLoad() {
        string path = getSystemPath() + "/" + folderPath + "/" + FileName + extension;

        if (File.Exists(path)) {
            var info = SaveSystem.DeserializeAs<PersistentDataInfo>(path);
            SaveSystem.UnboxSaveObject(info.data, transform);
            OnPersistentDataLoaded();
        } else
            Debug.LogError("PersistentDataSystem: File at path:" + path + " was not found");
    }

    [Serializable]
    public class PersistentDataInfo {
        public SaveObject data;
        //Add your custom info here
        public string profileName;
        public DateTime creationDate;
    }
}

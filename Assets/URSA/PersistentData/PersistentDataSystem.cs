using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using URSA.Save;
using UnityEditor;
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

    public static void MakePersistent(Entity entity) {
        UnityEngine.Object.DontDestroyOnLoad(entity.gameObject);
        entity.transform.parent = instance.transform;
    }


    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_PERSISTENT + URSAConstants.MENUITEM_PERSISTENT_SAVE)]
    public static void Save() {
        instance.SaveTo();
    }



    [MenuItem(URSAConstants.MENUITEM_ROOT + URSAConstants.MENUITEM_PERSISTENT + URSAConstants.MENUITEM_PERSISTENT_LOAD)]
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
        return path + "/" + SaveSystem.instance.GlobalRootFoder;
    }

    public void SaveTo() {
        SaveObject file = SaveSystem.instance.CreateSaveObjectFromPersistenData();
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

        transform.DestroyChildren();

        string path = getSystemPath() + "/" + folderPath + "/" + FileName + extension;

        if (File.Exists(path)) {
            var info = SaveSystem.DeserializeAs<PersistentDataInfo>(path);
            SaveSystem.instance.UnboxSaveObject(info.data, transform);
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

namespace URSA {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
    using URSA.Internal;
    using URSA.Utility;
#endif
    public enum DataPath {
        inRootFolder,
        persistent,
        custom
    }
    public class ConfigSystem : MonoBehaviour {
        #region SINGLETON
        private static ConfigSystem _instance;
        public static ConfigSystem instance
        {
            get {
                if (!_instance) _instance = GameObject.FindObjectOfType<ConfigSystem>();
                return _instance;
            }
        }
        #endregion
        public bool SingleFile;
        public DataPath Datapath;
        public string CustomDataPath;
        public string SingleFilePath = "configuration";
        public string FolderPath = "Configs";

        public List<DefaultFolder> DefaultFolders = new List<DefaultFolder>();

        [Serializable]
        public class DefaultFolder {
            public string type;
            public string folder;
        }

        public string specialfolderPath = "/Default";
        public string extension = ".cfg";
        static Dictionary<string, ConfigInfo> current = new Dictionary<string, ConfigInfo>();

        static string getSystemPath() {
            var sys = ConfigSystem.instance;
            string path = String.Empty;
            switch (sys.Datapath) {
                case DataPath.persistent: {
                        path = Application.persistentDataPath;
                    }
                    break;
                case DataPath.inRootFolder: {
                        path = Application.dataPath;
                    }
                    break;
                case DataPath.custom: {
                        path = sys.CustomDataPath;
                    }
                    break;
                default:
                    break;
            }
            return path + "/" + URSASettings.Current.CustomDataFolder;
        }
        
        public static IRVar GetVariable(string config, string variableName) {
            ConfigInfo info;
            current.TryGetValue(config, out info);
            if (info != null) {
                IRVar ivar;
                info.vars.TryGetValue(variableName, out ivar);
                if (ivar != null) {
                    return ivar;
                }
                else
                    return null;
            }
            else
                return null;
        }


#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + URSAConstants.PATH_MENUITEM_CONFIG_SAVE)]
#endif
        public static void Save() {
            var sys = ConfigSystem.instance;
            string path = getSystemPath();
            if (sys.SingleFile) {
                path = path + "/" + sys.SingleFilePath + sys.extension;
                SerializationHelper.Serialize(current, path, true);
            } else {
                path = path + "/" + sys.FolderPath;
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                foreach (var pair in current) {
                    SerializationHelper.Serialize(pair.Value, path + "/" + pair.Value.filename + instance.extension, true);
                }
            }
        }

        public static void LoadDefault<T>(string path) {
            var sys = ConfigSystem.instance;

            DefaultFolder folder = sys.DefaultFolders.FirstOrDefault(x => x.type == typeof(T).Name);
            if(folder == null) {
                Debug.LogError("Include the folder with proper type name of the config in default folders");
                return;
            }

            path = getSystemPath() + "/" + sys.FolderPath + "/" + sys.specialfolderPath + "/" + folder.folder +"/"+ path + instance.extension;
            var loaded = SerializationHelper.Load<ConfigInfo>(path);

            ConfigInfo info = current.FirstOrDefault(x => x.Value.type == loaded.type).Value;

            ApplyLoadedConfig(info, loaded);
        }

        public static void SaveDefault<T>(string path) where T : ConfigBase {
            var sys = ConfigSystem.instance;
            DefaultFolder folder = sys.DefaultFolders.FirstOrDefault(x => x.type == typeof(T).Name);
            if(folder == null) {
                Debug.LogError("Include the folder with proper type name of the config in default folders");
                return;
            }

            path = getSystemPath() + "/" + sys.FolderPath + "/" + sys.specialfolderPath + "/" + folder.folder +"/"+ path + instance.extension;

            ConfigInfo config = current.FirstOrDefault(x => x.Value.type == typeof(T)).Value;
            SerializationHelper.Serialize(config, path, true);
        }

#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + URSAConstants.PATH_MENUITEM_CONFIG_LOAD)]
#endif
        public static void Load() {
            var sys = ConfigSystem.instance;
            string path = getSystemPath();
            Dictionary<string, ConfigInfo> loadedDict = null;
            if (sys.SingleFile) {
                loadedDict = SerializationHelper.Load<Dictionary<string, ConfigInfo>>(path + "/" + sys.SingleFilePath + sys.extension);
            } else {
                if (Directory.Exists(path + "/" + sys.FolderPath)) {
                    loadedDict = new Dictionary<string, ConfigInfo>();
                    var Files = Directory.GetFiles(path + "/" + sys.FolderPath, "*.cfg");
                    foreach (var file in Files) {
                        var loadedFile = SerializationHelper.Load<ConfigInfo>(file);
                        if (loadedFile == null) {
                            Debug.LogError("Error on config deserialization: " + file);
                            continue;
                        }
                        loadedDict.Add(loadedFile.name, loadedFile);
                    }
                }
            }
            if (loadedDict == null) {
                Debug.LogError("Saved Configs were not found");
                return;
            }
            foreach (var pair in current) {
                ConfigInfo info = pair.Value;
                ConfigInfo loadedInfo;
                loadedDict.TryGetValue(pair.Key, out loadedInfo);
                if (loadedInfo == null) {
                    Debug.LogError("Matching config was not found in loaded config file:" + pair.Key);
                    continue;
                }
                ApplyLoadedConfig(info, loadedInfo);
            }
        }

        static void ApplyLoadedConfig(ConfigInfo info, ConfigInfo loadedInfo) {
            foreach (var item in info.vars) {
                IRVar cur = item.Value;
                IRVar loaded = null;
                loadedInfo.vars.TryGetValue(item.Key, out loaded);
                if (loaded == null) {
                    Debug.LogError("Matching variable:" + item.Key + " was not found in loaded config :" + loadedInfo.name);
                    continue;
                }
                cur.SetValue(loaded.GetValue());
            }
        }

        public static void RegisterConfig(ConfigBase cfg) {
            Type t = cfg.GetType();
            var attrs = t.GetCustomAttributes(typeof(ConfigAttribute), false);
            if (attrs.Length == 0) {
                Debug.LogError("A config file without ConfigAttribute is not allowed, add [Config()] to your config class.");
                Debug.Break();
                return;
            }
            var cfgattr = attrs[0] as ConfigAttribute;
            ConfigInfo info = null;
            current.TryGetValue(t.Name, out info);
            if (info == null) {
                info = new ConfigInfo();
                info.description = cfgattr.Description;
                info.filename = cfgattr.FileName;
                info.name = t.Name;
                info.gameVersion = ProjectInfo.current.Version;
                info.type = t;
            } else
                Debug.LogError("Duplicate ConfigInfo found, are you creating duplicates?");
            info.vars.Clear();
            current.Add(t.Name, info);
            var fields = t.GetFields();
            if (fields.Length > 0) {
                foreach (var f in fields) {
                    if (f.FieldType.BaseType.BaseType == typeof(RVar)) {
                        IRVar ivar = f.GetValue(cfg) as IRVar;
                        //Console.RegisterVar(ivar, f);
                        string name = f.Name;
                        info.vars.Add(name, ivar);
                    }
                }
            }
        }
        public static void UnregisterConfig(ConfigBase cfg) {
            Type t = cfg.GetType();
            ConfigInfo info = null;
            current.TryGetValue(t.Name, out info);
            if (info != null) {
                current.Remove(t.Name);
            }
        }
        [Serializable]
        class ConfigInfo {
            public string name;
            public string filename;
            public string description;
            public float gameVersion;
            public Type type;
            public Dictionary<string, IRVar> vars = new Dictionary<string, IRVar>();
        }
    }
    public class ConfigAttribute : Attribute {
        public string FileName;
        public string Description;

        public ConfigAttribute(string filename) {
            FileName = filename;
            Description = "nondescript";
        }
        public ConfigAttribute(string filename, string description) {
            FileName = filename;
            Description = description;
        }
    }

}
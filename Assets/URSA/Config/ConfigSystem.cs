namespace URSA {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
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
        public bool singleFile;
        public DataPath datapath;
        public string customDataPath;
        public string singleFilePath = "configuration";
        public string folderPath = "Configs";

        public List<DefaultFolder> defaultFolders = new List<DefaultFolder>();

        [Serializable]
        public class DefaultFolder {
            public string type;
            public string folder;
        }

        public string specialfolderPath = "/Default";
        public string extension = ".cfg";
        static Dictionary<string, ConfigInfo> current = new Dictionary<string, ConfigInfo>();

        private void OnEnable() {
            URSA.Console.RegisterCommandWithParameters("config.savegfx", SaveGraphicsConfigAs);
            URSA.Console.RegisterCommandWithParameters("config.loadgfx", LoadGraphicsConfig);
        }

        static string getSystemPath() {
            var sys = ConfigSystem.instance;
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
#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + URSAConstants.PATH_MENUITEM_CONFIG_SAVE)]
#endif
        public static void Save() {
            var sys = ConfigSystem.instance;
            string path = getSystemPath();
            if (sys.singleFile) {
                path = path + "/" + sys.singleFilePath + sys.extension;
                SerializationHelper.Serialize(current, path, true);
            } else {
                path = path + "/" + sys.folderPath;
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

            DefaultFolder folder = sys.defaultFolders.FirstOrDefault(x => x.type == typeof(T).Name);
            if(folder == null) {
                Debug.LogError("Include the folder with proper type name of the config in default folders");
                return;
            }

            path = getSystemPath() + "/" + sys.folderPath + "/" + sys.specialfolderPath + "/" + folder.folder +"/"+ path + instance.extension;
            var loaded = SerializationHelper.Load<ConfigInfo>(path);

            ConfigInfo info = current.FirstOrDefault(x => x.Value.type == loaded.type).Value;

            ApplyLoadedConfig(info, loaded);
        }

        public static void SaveDefault<T>(string path) where T : ConfigBase {
            var sys = ConfigSystem.instance;
            DefaultFolder folder = sys.defaultFolders.FirstOrDefault(x => x.type == typeof(T).Name);
            if(folder == null) {
                Debug.LogError("Include the folder with proper type name of the config in default folders");
                return;
            }

            path = getSystemPath() + "/" + sys.folderPath + "/" + sys.specialfolderPath + "/" + folder.folder +"/"+ path + instance.extension;

            ConfigInfo config = current.FirstOrDefault(x => x.Value.type == typeof(T)).Value;
            SerializationHelper.Serialize(config, path, true);
        }

        public static void SaveGraphicsConfigAs(string[] filename) {
            SaveDefault<GraphicsConfig>(filename[0]);
        }

        public static void LoadGraphicsConfig(string[] filename) {
            LoadDefault<GraphicsConfig>(filename[0]);
        }

#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + URSAConstants.PATH_MENUITEM_CONFIG_LOAD)]
#endif
        public static void Load() {
            var sys = ConfigSystem.instance;
            string path = getSystemPath();
            Dictionary<string, ConfigInfo> loadedDict = null;
            if (sys.singleFile) {
                loadedDict = SerializationHelper.Load<Dictionary<string, ConfigInfo>>(path + "/" + sys.singleFilePath + sys.extension);
            } else {
                if (Directory.Exists(path + "/" + sys.folderPath)) {
                    loadedDict = new Dictionary<string, ConfigInfo>();
                    var Files = Directory.GetFiles(path + "/" + sys.folderPath, "*.cfg");
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
                IrVar cur = item.Value;
                IrVar loaded = null;
                loadedInfo.vars.TryGetValue(item.Key, out loaded);
                if (loaded == null) {
                    Debug.LogError("Matching variable:" + item.Key + " was not found in loaded config :" + loadedInfo.name);
                    continue;
                }
                cur.setValue(loaded.getValue());
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
                    if (f.FieldType.BaseType.BaseType == typeof(rVar)) {
                        IrVar ivar = f.GetValue(cfg) as IrVar;
                        Console.RegisterVar(ivar, f);
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
            public Dictionary<string, IrVar> vars = new Dictionary<string, IrVar>();
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
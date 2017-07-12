namespace URSA.Config {

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
    public class ConfigSystem : MonoBehaviour {
        #region SINGLETON
        private static ConfigSystem _instance;
        public static ConfigSystem instance
        {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<ConfigSystem>();
                return _instance;
            }
        }
        #endregion

        public Mode mode;
        public enum Mode {
            SingleConfigFile,
            FilePerConfig
        }

        public string SingleFileName = "main";
        public string FolderName = "Cfg";

        [Serializable]
        public class DefaultFolder {
            public string type;
            public string folder;
        }

        public string extension = ".cfg";

        private static Dictionary<Type, ConfigBase> m_configs = new Dictionary<Type, ConfigBase>();

        public static string ConfigFolderPath { get { return PathUtilities.CustomDataPath + "/" + instance.FolderName + "/"; } }

        private void OnEnable() {
            if (!Directory.Exists(ConfigFolderPath)) {
                Debug.Log("Creating configs directory at : " + ConfigFolderPath);
                Directory.CreateDirectory(ConfigFolderPath);
            }
        }

        public static void RegisterConfig(ConfigBase cfg) {
            m_configs.Add(cfg.GetType(), cfg);
        }
        public static void UnregisterConfig(ConfigBase cfg) {
            m_configs.Remove(cfg.GetType());

        }

#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + URSAConstants.PATH_MENUITEM_CONFIG_SAVE)]
#endif
        public static void Save() {

            SingleFileConfig singleFileConfig = new SingleFileConfig();

            foreach (var pair in m_configs) {
                ConfigBase cfg = pair.Value;

                ConfigInfo info = PackConfig(cfg);

                string filename = String.IsNullOrEmpty(cfg.FileName) ? info.type.Name : cfg.FileName;

                if (instance.mode == Mode.FilePerConfig) {
                    string path = ConfigFolderPath + filename + instance.extension;
                    SerializationHelper.Serialize(info, path, true);
                }
                else {
                    singleFileConfig.configs.Add(filename, info);
                }
            }

            if (instance.mode == Mode.SingleConfigFile) {
                string path = ConfigFolderPath + instance.SingleFileName + instance.extension;
                SerializationHelper.Serialize(singleFileConfig, path, true);
            }
        }

        private static ConfigInfo PackConfig(ConfigBase cfg) {
            //Create container
            ConfigInfo info = new ConfigInfo(ProjectInfo.Current.Version, cfg.GetType());
            var fields = info.type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            //Packing 
            foreach (var field in fields) {
                string name = field.Name;
                object value = field.GetValue(cfg);
                info.vars.Add(name, value);
            }

            var properties = info.type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var prop in properties) {
                string name = prop.Name;
                object value = prop.GetValue(cfg, null);
                info.vars.Add(name, value);
            }

            return info;
        }

        public static void TrySaveSingle(ConfigBase cfg) {
            string filename = String.IsNullOrEmpty(cfg.FileName) ? cfg.GetType().Name : cfg.FileName;
            string path = "";
            if (instance.mode == Mode.FilePerConfig) {
                path = ConfigFolderPath + filename + instance.extension;
                SerializationHelper.Serialize(PackConfig(cfg), path, true);
            }
            else {
                path = ConfigFolderPath + instance.SingleFileName + instance.extension;
                SingleFileConfig singleFileConfig = SerializationHelper.Load<SingleFileConfig>(path) as SingleFileConfig;
                singleFileConfig.configs.Remove(filename);
                singleFileConfig.configs.Add(filename, PackConfig(cfg));
                SerializationHelper.Serialize(singleFileConfig, path, true);
            }
        }

#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_CONFIG + URSAConstants.PATH_MENUITEM_CONFIG_LOAD)]
#endif
        public static void Load() {

            SingleFileConfig singleFileConfig = null;
            if (instance.mode == Mode.SingleConfigFile) {
                string path = ConfigFolderPath + instance.SingleFileName + instance.extension;
                singleFileConfig = SerializationHelper.Load<SingleFileConfig>(path) as SingleFileConfig;
            }

            foreach (var pair in m_configs) {
                ConfigBase cfg = pair.Value;
                string filename = String.IsNullOrEmpty(cfg.FileName) ? cfg.GetType().Name : cfg.FileName;
                string path = ConfigFolderPath + filename + instance.extension;
                if (!File.Exists(path))
                    continue;

                ConfigInfo info = null;
                if (instance.mode == Mode.FilePerConfig)
                    info = SerializationHelper.Load<ConfigInfo>(path);
                else {
                    singleFileConfig.configs.TryGetValue(filename, out info);
                }
                if (info == null) {
                    Debug.LogError("Broken config file it seems : " + path);
                    continue;
                }
                LoadSingleConfig(info, cfg);
            }
        }

        private static void LoadSingleConfig(ConfigInfo info, ConfigBase cfg) {
            Type cfgtype = cfg.GetType();
            var fields = cfgtype.GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var field in fields) {
                string name = field.Name;
                object value = null;
                info.vars.TryGetValue(field.Name, out value);

                if (value != null) {
                    field.SetValue(cfg, value);
                }
            }

            var properties = cfgtype.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var prop in properties) {
                string name = prop.Name;
                object value = null;
                info.vars.TryGetValue(prop.Name, out value);

                if (value != null) {
                    prop.SetValue(cfg, value, null);
                }
            }
        }

        public static void TryLoadSingle(ConfigBase cfg) {
            SingleFileConfig singleFileConfig = null;
            string path = "";
            string filename = "";
            filename = String.IsNullOrEmpty(cfg.FileName) ? cfg.GetType().Name : cfg.FileName;
            if (instance.mode == Mode.SingleConfigFile) {
                path = ConfigFolderPath + instance.SingleFileName + instance.extension;
                singleFileConfig = SerializationHelper.Load<SingleFileConfig>(path) as SingleFileConfig;
            }
            else {
                path = ConfigFolderPath + filename + instance.extension;
                if (!File.Exists(path))
                    return;
            }
            ConfigInfo info = null;
            if (instance.mode == Mode.FilePerConfig)
                info = SerializationHelper.Load<ConfigInfo>(path);
            else {
                singleFileConfig.configs.TryGetValue(filename, out info);
            }
            if (info == null) {
                Debug.LogError("Broken config file it seems : " + path);
                return;
            }
            LoadSingleConfig(info, cfg);
        }



        [Serializable]
        class ConfigInfo {
            public ConfigInfo(float gameVersion, Type type) {
                this.gameVersion = gameVersion;
                this.type = type;
            }

            public float gameVersion;
            public Type type;
            public Dictionary<string, object> vars = new Dictionary<string, object>();
        }

        class SingleFileConfig {
            public Dictionary<string, ConfigInfo> configs = new Dictionary<string, ConfigInfo>();
        }
    }


}
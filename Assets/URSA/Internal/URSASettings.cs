namespace URSA.Internal {

    using UnityEngine;

    public class URSASettings : ScriptableObject {

        static URSASettings _settings;
        public static URSASettings current
        {
            get {
                if (_settings == null)
                    _settings = Resources.Load(URSAConstants.PATH_URSASETTINGS_ROOT + URSAConstants.PATH_URSASETTINGS_SETTINGS) as URSASettings;
                return _settings;
            }
        }

        [Header("UpdateEverything Sequence")]
        public bool RebuildDatabase = true;
        public bool CollectSceneData = true;
        public bool SaveAndReloadScene = true;


        [Header("Systems")]
        public GameObject CustomGlobalSystemsPrefab;
        public GameObject GlobalSystemsTemplate;
        public GameObject CustomLocalSystemsPrefab;
        public GameObject LocalSystemsTemplate;

        [Header("Database")]
        public string DatabaseRootFolder = "Entities";
        public string DatabaseManifest = "manifest.db";

        [Header("Serialization")]
        public string CustomDataFolder = "CustomData";
    }

}
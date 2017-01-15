using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
    public bool ParsePrefabs;
    public bool OrganizeLevel;
    public bool CollectLevelsData = true;
    public bool SaveAndReloadScene = true;


    [Header("Systems")]
    public GameObject CustomGlobalSystemsPrefab;
    public GameObject GlobalSystemsTemplate;
    public GameObject CustomSceneSystemsPrefab;
    public GameObject SceneSystemsTemplate;

    [Header("Database")]
    public string DatabaseRootFolder = "GamePrefabs/ENTITY";
    public string DatabaseManifest = "manifest.db";
}

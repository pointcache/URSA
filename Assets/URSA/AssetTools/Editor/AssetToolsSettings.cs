using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AssetToolsSettings : ScriptableObject {

    [Header("Asset Folders Paths")]
    [Tooltip("Keeps your plugins/addons safe")]
    public string rootPath = "GamePrefabs/";


    [Tooltip("Dynamic game objects - coins, pickups..")]
    public string entity = "ENTITY";
    [Tooltip("Level geometry, bushes... Will be marked as static")]
    public string @static = "STATIC";
    public string lights = "LIGHTS";
    [Tooltip("Trigger boxes and other volumes")]
    public string volumes = "VOLUMES";
    public string fx = "FX";
    public string npc = "NPC";

    [Header("Settings"), Tooltip("Static assets by default will be marked as")]
    public NavmeshArea.Area defaultNavmeshArea = NavmeshArea.Area.notWalkable;

    [Header("Level structure")]
    public string lv_static_root = "StaticObjects";
    public string lv_entities_root = "Entities";
    public string lv_entity = "Other";

    public string lv_static = "Static";
    public string lv_lights = "Lights";

    public string lv_volumes = "Volumes";
    public string lv_fx = "Fx";
    public string lv_npc = "Npcs";


}


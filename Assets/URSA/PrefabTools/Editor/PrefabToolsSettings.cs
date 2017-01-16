using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PrefabToolsSettings : ScriptableObject {

    [Header("Asset Folders Paths")]
    [Tooltip("Keeps your plugins/addons safe")]
    public string rootPath = "GamePrefabs/";


    [Tooltip("Dynamic game objects - coins, pickups..")]
    public string entity = "ENTITY";
    [Tooltip("Level geometry, bushes... Will be marked as static")]
    public string @static = "STATIC";
    public string lights = "LIGHT";
    [Tooltip("Trigger boxes and other volumes")]
    public string volumes = "VOLUME";
    public string fx = "FX";
    public string npc = "ENTITY/NPC";
    [Tooltip("Paths containing these words will be marked as Ignore")]
    public string[] Ignore = new string[] { "Persistent" };

    [Header("Settings"), Tooltip("Static assets by default will be marked as")]
    public NavmeshArea.Area defaultNavmeshArea = NavmeshArea.Area.notWalkable;

    [Header("Level structure")]
    public string lv_static_root = "StaticContent";
    public string lv_entities_root = "DynamicContent";
    public string lv_entity = "Entities";
    public string lv_static = "Static";
    public string lv_lights = "Lights";
    public string lv_volumes = "Volumes";
    public string lv_fx = "Fx";
    public string lv_npc = "Npcs";


}


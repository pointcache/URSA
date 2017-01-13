using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AssetToolsSettings : ScriptableObject {

    [Header("Asset Folders Paths")]
    [Tooltip("Keeps your plugins/addons safe")]
    public string rootPath = "GamePrefabs/";


    [Tooltip("Dynamic game objects - coins, pickups..")]
    public string dynamic = "DYNAMIC";
    [Tooltip("Level geometry, bushes... Will be marked as static")]
    public string @static = "STATIC";
    public string lights = "LIGHTS";
    [Tooltip("Trigger boxes and other volumes")]
    public string volumes = "VOLUMES";
    public string fx = "FX";
    public string enemy = "ENEMIES";
    public string npc = "NPC";

    [Header("Settings"), Tooltip("Static assets by default will be marked as")]
    public NavmeshArea.Area defaultNavmeshArea = NavmeshArea.Area.notWalkable;

}


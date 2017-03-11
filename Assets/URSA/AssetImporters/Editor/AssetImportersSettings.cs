using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;

public class AssetImportersSettings : ScriptableObject {

    [Header("Textures")]
    //This will protect your from changing textures in plugins/addons
    public string textureRulesApplyToFolder = "_Textures";

    public bool convertToSprites;
    public string convertToSpriteToken = "_S";

    public bool convertToNormal;
    public string convertToNormalToken = "_N";

    public bool setPointFilter;
    public string setPointFilterToken = "_PF";

    public bool setTrueColor;
    public string setTrueColorToken = "_TC";

    public bool setLevelGeometry;
    public string setLevelGeometryToken = "_LGEO";

    [Header("Models")]
    //This will protect your from changing textures in plugins/addons
    public string modelRulesApplyToFolder = "_Models";
    public ModelImporterTangents tangentCalculationMode = ModelImporterTangents.CalculateMikk;
    public ModelImporterMaterialName materialImportMode = ModelImporterMaterialName.BasedOnMaterialName;
    public ModelImporterMaterialSearch materialSearchMode = ModelImporterMaterialSearch.Everywhere;
    public string animationImportToken = "_ANIM";
    public bool generateSecondaryUV = false;
    public bool renameMeshesToFileName = false;
    public string retainUniqueNameToken = "_UN";    
    public bool overrideModelScale = false;
    public Vector3 newModelScale = Vector3.one;

}

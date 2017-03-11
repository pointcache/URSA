using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class FBXSettingsOverride : AssetPostprocessor {

    static AssetImportersSettings settings;
    int InModelCount;

    void OnPreprocessModel() {
        ModelImporter importer = assetImporter as ModelImporter;
        if (!settings)
            settings = Helpers.FindScriptableObject<AssetImportersSettings>();
        if (!settings)
            return;
        if (importer.assetPath.Contains(settings.modelRulesApplyToFolder)) {
            string name = importer.assetPath.ToLower();

            importer.importTangents = settings.tangentCalculationMode;
            importer.materialName = settings.materialImportMode;
            importer.materialSearch = settings.materialSearchMode;
            if (!assetPath.Contains(settings.animationImportToken))
                importer.animationType = ModelImporterAnimationType.None;
            if (name.Substring(name.Length - 4, 4) == ".fbx") {

            }
            importer.generateSecondaryUV = settings.generateSecondaryUV;
        }
    }

    void OnPostprocessModel(GameObject g) {
        if (!settings)
            settings = Helpers.FindScriptableObject<AssetImportersSettings>();
        if (!settings)
            return;
        AssetImporter importer = assetImporter as AssetImporter;
        if (!importer.assetPath.Contains(settings.modelRulesApplyToFolder))
            return;
        if (settings.overrideModelScale)
            g.transform.localScale = settings.newModelScale;

        if (settings.renameMeshesToFileName && !g.name.Contains(settings.retainUniqueNameToken)) {
            InModelCount = 0;
            Rename(g.transform, g.name);
        }

        if (settings.setLevelGeometry && assetPath.Contains(settings.setLevelGeometryToken)) {
            LevelGeometryProcess(g);
        }
    }

    void LevelGeometryProcess(GameObject g) {
        var trs = g.GetComponentsInChildren<Transform>();
        var reloader = g.GetComponent<MeshColliderReloader>();
        if (!reloader)
            g.AddComponent<MeshColliderReloader>();


        foreach (Transform t in trs) {
            var mc = t.GetComponent<MeshCollider>();
            var mf = t.GetComponent<MeshFilter>();
            if (!mf)
                continue;
            if (!mc) {
                mc = t.gameObject.AddComponent<MeshCollider>();
            }
            mc.sharedMesh = mf.sharedMesh;
        }
        if (Application.isPlaying) {
            var reloaders = GameObject.FindObjectsOfType<MeshColliderReloader>();
            foreach (var r in reloaders) {
                r.Reload();
            }
        }
    }

    
    void Rename(Transform transform, string name) {
        MeshFilter m = transform.GetComponent<MeshFilter>();
        if (m) {
            if (InModelCount == 0)
                m.sharedMesh.name = name;
            else {
                string newname = name + "_" + InModelCount;
                m.gameObject.name = newname;
                m.sharedMesh.name = newname;
            }
        }
        InModelCount++;
        // Recurse
        foreach (Transform child in transform)
            Rename(child, name);
    }
}
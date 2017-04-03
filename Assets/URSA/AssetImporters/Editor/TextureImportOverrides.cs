using UnityEngine;
using UnityEditor;

class TextureImportOverrides : AssetPostprocessor
{
    static AssetImportersSettings settings;

    void OnPreprocessTexture()
    {
        if (!settings)
            settings = Helpers.FindScriptableObject<AssetImportersSettings>();
        if (!settings)
            return;
        TextureImporter textureImporter = (TextureImporter)assetImporter;

        if (assetPath.Contains(settings.textureRulesApplyToFolder))
        {
            if (settings.convertToSprites && assetPath.Contains(settings.convertToSpriteToken))
            {
                textureImporter.textureType = TextureImporterType.Sprite;
            }

            if(settings.setTrueColor && assetPath.Contains(settings.setTrueColorToken))

            if (settings.convertToNormal && assetPath.Contains(settings.convertToNormalToken))
            {
                textureImporter.textureType = TextureImporterType.NormalMap;
                textureImporter.normalmap = true;
                textureImporter.convertToNormalmap = false;
                textureImporter.normalmapFilter = TextureImporterNormalFilter.Sobel;
            }

            if (settings.setPointFilter &&  assetPath.Contains(settings.setPointFilterToken))
            {
                textureImporter.filterMode = FilterMode.Point;
            }
        }
    }
}
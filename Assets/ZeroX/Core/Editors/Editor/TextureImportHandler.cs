using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    public class TextureImportHandler : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if(assetImporter.importSettingsMissing == false)
                return;

            TextureImporter textureImporter = assetImporter as TextureImporter;
            if(textureImporter == null)
                return;
            
            
            CheckAndConvertTextureToSpriteIfImportInUIFolder(textureImporter);
            CheckAndImportTextureAsSingleSprite(textureImporter);
        }

        private void CheckAndConvertTextureToSpriteIfImportInUIFolder(TextureImporter textureImporter)
        {
            if(textureImporter.assetPath.ToLower().Contains("ui") == false)
                return;
            
            if(textureImporter.textureType == TextureImporterType.Sprite)
                return;
            
            
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.mipmapEnabled = false; //Tắt Mipmap cho Sprite
        }

        private void CheckAndImportTextureAsSingleSprite(TextureImporter textureImporter)
        {
            if(textureImporter.textureType != TextureImporterType.Sprite)
                return;

            if(textureImporter.spriteImportMode == SpriteImportMode.Single)
                return;
            
            
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.spritePivot = new Vector2(0.5f, 0.5f); //Multiple sẽ có pivot kiểu khác nên cần set về 0.5f, 0.5f
        }
    }
}
/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityEditor;

    internal enum EMaxTextureSize
    {
        _32,
        _64,
        _128,
        _256,
        _512,
        _1024,
        _2048,
        _4096,
        _8192,
        Other,
    }

    [System.Serializable]
    internal class SearchState
    {
        public string stringValue = "";
        public int filterValue = 0;

        public bool HasValue => !string.IsNullOrEmpty(stringValue);

        /** ********************************************************************************
        * @summary フィルタリングにマッチするか判定
        ***********************************************************************************/
        public bool DoesItemMatch(EHeaderColumnId headerId, TextureTreeElement element)
        {
            int typeAsBit = 0;
            var textureImporter = element.TextureImporter;
            var texture = element.Texture;
            switch (headerId)
            {
                case EHeaderColumnId.TextureName:
                    return DoesStringMatch(stringValue, element.Texture.name);
                case EHeaderColumnId.TextureType:
                    typeAsBit = TypeBitConverter.ConvertTextureImpoterType(textureImporter.textureType);
                    return (filterValue | typeAsBit) > 0;
                case EHeaderColumnId.NPot:
                    typeAsBit = TypeBitConverter.ConvertTextureImporterNPOTScale(textureImporter.npotScale);
                    return (filterValue | typeAsBit) > 0;
                case EHeaderColumnId.MaxSize:
                    typeAsBit = TypeBitConverter.ConvertMaxTextureSize(textureImporter.maxTextureSize);
                    return (filterValue | typeAsBit) > 0;
                case EHeaderColumnId.GenerateMips:
                    typeAsBit = TypeBitConverter.ConvertMipMapEnabled(textureImporter.mipmapEnabled);
                    return (filterValue | typeAsBit) > 0;
                case EHeaderColumnId.AlphaIsTransparency:
                    typeAsBit = TypeBitConverter.ConvertAlphaIsTransparency(textureImporter.alphaIsTransparency);
                    return (filterValue | typeAsBit) > 0;
                case EHeaderColumnId.TextureSize:
                case EHeaderColumnId.DataSize:
                    return DoesStringMatch(stringValue, element.Texture.name);
                default:
                    return true;
            }

        }

        /** ********************************************************************************
        * @summary 検索文字にマッチするか判定
        ***********************************************************************************/
        private bool DoesStringMatch(string searchString, string displayText)
        {
            // 何も表示されていない場合は無条件でマッチ
            if (string.IsNullOrEmpty(displayText)) { return true; } 

            return displayText.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /** ********************************************************************************
        * @summary フィルタリングにマッチするか判定
        ***********************************************************************************/
        private bool DoesFilterMatch(string searchString, string displayText)
        {
            return displayText.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
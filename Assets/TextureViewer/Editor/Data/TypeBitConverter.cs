/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System;
    using UnityEditor;

    /// <summary>
    /// ビットマスク用のビット生成クラス
    /// </summary>
    internal static class TypeBitConverter
    {
        public static int ConvertTextureImporterNPOTScale(TextureImporterNPOTScale type)
        {
            return 1 << (int)type; 
        }

        internal static int ConvertTextureImpoterType(TextureImporterType type)
        {
            return 1 << (int)type;
        }

        internal static int ConvertMaxTextureSize(int maxTextureSize)
        {
            int bitShift = 0;
            switch (maxTextureSize)
            {
                case (int)EMaxTextureSize._32:
                    bitShift = 0;
                    break;
                case (int)EMaxTextureSize._64:
                    bitShift = 1;
                    break;
                case (int)EMaxTextureSize._128:
                    bitShift = 2;
                    break;
                case (int)EMaxTextureSize._256:
                    bitShift = 3;
                    break;
                case (int)EMaxTextureSize._512:
                    bitShift = 4;
                    break;
                case (int)EMaxTextureSize._1024:
                    bitShift = 5;
                    break;
                case (int)EMaxTextureSize._2048:
                    bitShift = 6;
                    break;
                case (int)EMaxTextureSize._4096:
                    bitShift = 7;
                    break;
                case (int)EMaxTextureSize._8192:
                    bitShift = 8;
                    break;
            }

            return 1 << bitShift;
        }

        internal static int ConvertMipMapEnabled(bool mipmapEnabled)
        {
            if (mipmapEnabled)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        internal static int ConvertAlphaIsTransparency(bool alphaIsTransparency)
        {
            if (alphaIsTransparency)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
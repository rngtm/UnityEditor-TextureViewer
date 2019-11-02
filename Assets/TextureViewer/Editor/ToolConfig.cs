/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEngine;
    
    /** ********************************************************************************
    * @summary ツール設定
    ***********************************************************************************/
    internal static class ToolConfig
    {
        public readonly static string[] TargetDirectories = new string[] { "Assets" };  // テクスチャ読み込み対象のディレクトリ

        public readonly static string ProgressTitle = "テクスチャ表示ウィンドウを初期化しています";
        public readonly static string CreatingMessage = "Creating...";
        public readonly static string LoadingMessage = "Loading...";

        public readonly static GUIContent WindowTitle = new GUIContent("Texture Viewer");

        public const int MB = 1024 * 1024; // メガバイト
        public const int YellowDataSize = 2 * MB; // データサイズがこれを超えたら黄色で警告
        public const int RedDataSize = 3 * MB; // データサイズがこれを超えたら赤で警告
        public const int YellowTextureSize = 2048; // テクスチャサイズがこれを超えたら黄色で警告
        public const int RedTextureSize = 4096; // テクスチャサイズがこれを超えたら黄色で警告
        public const int RedMaxTextureSize = 2048; // max texture sizeがこれを超えたら赤で警告

        // TreeViewのヘッダーの数
        public static int HeaderColumnNum => HeaderColumns.Length; // ヘッダー列の個数

        // TreeViewのヘッダー定義
        public static readonly TextureColumn[] HeaderColumns = new[] {
            new TextureColumn("Texture", 180f), // 0
            new TextureColumn("Texture Type", 105f), // 1
            new TextureColumn("Non Power of 2", 105f), // 2
            new TextureColumn("Max Size", 70f), // 3
            new TextureColumn("Generate\nMip Maps", 70f), // 4
            new TextureColumn("Alpha is\nTransparency", 96f), // 5
            new TextureColumn("Texture Size", 105f), // 6
            new TextureColumn("Data Size", 80f), // 7
        };

        public static float InitialHeaderTotalWidth
        {
            get
            {
                float width = 0f;
                for (int i = 0; i < HeaderColumns.Length; i++)
                {
                    width += HeaderColumns[i].width;
                }
                return width;
            }
        }

    }
}
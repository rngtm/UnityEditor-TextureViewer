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
    }
}
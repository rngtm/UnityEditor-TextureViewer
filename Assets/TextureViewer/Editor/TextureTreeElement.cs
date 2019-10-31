/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /** ********************************************************************************
     * @summary TreeViewの要素
     ***********************************************************************************/
    internal class TextureTreeElement
    {
        private ulong textureByteLength = 0;
        private string textureDataSizeText = "";
        public string AssetPath { get; set; } // 背景アセットパス
        public string AssetName { get; set; } // 背景アセット名
        public ulong TextureByteLength => textureByteLength; // テクスチャデータサイズ(Byte)
        public string TextureDataSizeText => textureDataSizeText;// テクスチャデータサイズテキスト
        public Texture2D Texture { get; set; } // ロードしたテクスチャ 
        public TextureImporter TextureImporter { get; set; } // テクスチャインポート設定
        public int Index { get; set; } // 何番目の要素か
        public TextureTreeElement Parent { get; private set; } // 親の要素
        public List<TextureTreeElement> Children { get; } = new List<TextureTreeElement>(); // 子の要素

        /** ********************************************************************************
        * @summary データサイズ更新
        ***********************************************************************************/
        public void UpdateDataSize()
        {
            textureByteLength = (Texture != null) ? (ulong)Texture?.GetRawTextureData().Length : 0;
            textureDataSizeText = Utils.ConvertToHumanReadableSize(textureByteLength);
        }

        /** ********************************************************************************
        * @summary 子を追加
        ***********************************************************************************/
        internal void AddChild(TextureTreeElement child)
        {
            // 既に親がいたら削除
            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }

            // 親子関係を設定
            Children.Add(child);
            child.Parent = this;
        }

        /** ********************************************************************************
        * @summary 子を削除
        ***********************************************************************************/
        public void RemoveChild(TextureTreeElement child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
                child.Parent = null;
            }
        }
    }
}
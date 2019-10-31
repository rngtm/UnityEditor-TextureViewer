/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    /** ********************************************************************************
    * @summary MultiColumnHeaderの派生クラス
    ***********************************************************************************/
    internal class TextureColumnHeader : MultiColumnHeader
    {
        public static readonly float headerHeight = 36f; // ヘッダーの高さ
        static readonly float labelY = 4f; // ラベル位置
        private GUIStyle style;

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            height = headerHeight; // ヘッダーの高さ 上書き
        }

        /** ********************************************************************************
        * @summary TreeViewのヘッダー描画
        ***********************************************************************************/
        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            if (canSort && column.canSort)
            {
                SortingButton(column, headerRect, columnIndex);
            }

            if (style == null)
            {
                style = new GUIStyle(DefaultStyles.columnHeader);
                style.alignment = TextAnchor.LowerLeft;
            }

            float labelHeight = headerHeight;
            Rect labelRect = new Rect(headerRect.x, headerRect.yMax - labelHeight - labelY, headerRect.width, labelHeight);
            GUI.Label(labelRect, column.headerContent, style);
        }
    }
}
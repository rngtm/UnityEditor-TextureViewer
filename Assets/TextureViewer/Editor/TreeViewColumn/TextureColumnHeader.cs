/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    /** ********************************************************************************
    * @summary MultiColumnHeaderの派生クラス
    ***********************************************************************************/
    internal class TextureColumnHeader : MultiColumnHeader
    {
        private const float searchY = 2f; // 検索ボックス すき間 上
        private const float searchMarginLeft = 4f; // 検索ボックス すき間 左
        private const float searchMarginRight = 4f; // 検索ボックス すき間 右
        private const float searchHeight = 14f; // 検索ボックス サイズ
        private const float searchSpace = 0f; // 検索ボックスとソートボタンの間のすき間
        private const float sortHeight = labelHeight + sortSpace; // ソートボタン 高さ
        private const float sortSpace = 6f; // ソート上部とラベルの間のすき間
        private const float labelHeight = 32f; // ラベル 高さ
        private const float labelY = 4f; // ラベル位置

        private GUIStyle style = null;

        public System.Action<string> searchChanged { get; set; } // 検索が変化したときに実行されるコールバック

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            height = searchY + searchHeight + searchSpace + sortHeight; // ヘッダーの高さ 上書き
        }

        /** ********************************************************************************
        * @summary TreeViewのヘッダー描画
        ***********************************************************************************/
        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            headerRect.y += searchY;
            headerRect.height -= searchY;

            Rect searchRect = new Rect(headerRect);
            searchRect.height = searchHeight;
            searchRect.width -= searchMarginLeft + searchMarginRight;
            searchRect.x += searchMarginLeft;
            var textureState = state as TextureColumnHeaderState;
            var searchField = textureState.SearchFields[columnIndex];

            EditorGUI.BeginChangeCheck();

            string s = textureState.SearchStrings[columnIndex];
            s = searchField.SearchField.OnToolbarGUI(searchRect, s);
            if (EditorGUI.EndChangeCheck()) // 検索文字が変化
            {
                textureState.SearchStrings[columnIndex] = s;
                searchChanged?.Invoke(s);
                searchField.searchChanged?.Invoke();
            }

            if (canSort && column.canSort)
            {
                Rect sortRect = headerRect;
                sortRect.height = sortHeight;
                sortRect.y = searchRect.height + searchSpace;
                SortingButton(column, sortRect, columnIndex);
            }

            Rect labelRect = new Rect(headerRect.x, headerRect.yMax - labelHeight - labelY, headerRect.width, labelHeight);
            GUI.Label(labelRect, column.headerContent, MyStyle.TreeViewColumnHeader);
        }
    }
}
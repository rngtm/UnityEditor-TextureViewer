/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;

    /** ********************************************************************************
    * @summary MultiColumnHeaderStateの派生クラス
    ***********************************************************************************/
    internal class TextureColumnHeaderState : MultiColumnHeaderState
    {
        public ColumnSearchField[] SearchFields { get; private set; }
        public string[] SearchStrings { get; private set; } // 検索テキスト

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumnHeaderState(Column[] columns, string[] searchStrings) : base(columns)
        {
            SearchStrings = searchStrings;
            SearchFields = new ColumnSearchField[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                SearchFields[i] = new ColumnSearchField();
            }
        }
    }
}
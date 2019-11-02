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
        //public string[] SearchStrings { get; private set; } // 検索テキスト]
        public SearchState[] SearchStates { get; private set; }

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumnHeaderState(Column[] columns, SearchState[] searchStates) : base(columns)
        {
            //SearchStrings = searchStrings;
            SearchStates = searchStates;
            SearchFields = new ColumnSearchField[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                SearchFields[i] = new ColumnSearchField();
            }
        }
    }
}
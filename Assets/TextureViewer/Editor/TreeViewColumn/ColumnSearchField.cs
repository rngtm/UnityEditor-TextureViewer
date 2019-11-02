/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class ColumnSearchField
    {
        public System.Action searchChanged { get; set; }
        public SearchField SearchField { get; private set; } = new SearchField();

        public void OnGUI(Rect searchRect, TextureColumnHeaderState headerState, int columnIndex)
        {
            var columnHeaderId = (EHeaderColumnId)columnIndex;
            var state = headerState.SearchStates[columnIndex];

            switch (columnHeaderId)
            {
                case EHeaderColumnId.TextureName:
                    state.stringValue = SearchField.OnToolbarGUI(searchRect, state.stringValue);
                    break;
                case EHeaderColumnId.TextureType:
                    break;
                case EHeaderColumnId.NPot:
                    break;
                case EHeaderColumnId.MaxSize:
                    break;
                case EHeaderColumnId.GenerateMips:
                    break;
                case EHeaderColumnId.AlphaIsTransparency:
                    break;
                case EHeaderColumnId.TextureSize:
                    break;
                case EHeaderColumnId.DataSize:
                    break;
                default:
                    break;
            }
        }
    }
}
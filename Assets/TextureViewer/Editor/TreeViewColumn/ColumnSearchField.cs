/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;

    public class ColumnSearchField
    {
        public System.Action searchChanged { get; set; }
        public SearchField SearchField { get; private set; } = new SearchField();
    }
}
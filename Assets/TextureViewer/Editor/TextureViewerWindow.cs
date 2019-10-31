/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    /** ********************************************************************************
     * @summary プロジェクト内テクスチャ一覧ツール
     ***********************************************************************************/
    internal class TextureViewerWindow : EditorWindow
    {
        // TreeView レイアウト
        private static readonly GUILayoutOption[] TreeViewLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandHeight(true)
        };

        [SerializeField] private TreeViewState treeViewState = null; // TreeViewの状態
        [SerializeField] private string searchText = ""; // 検索文字
        [SerializeField] private Texture2D[] textures = new Texture2D[0]; // ロードしたテクスチャ
        [SerializeField] private TextureImporter[] textureImporters = new TextureImporter[0];

        private TextureTreeView treeView = null; // TreeView
        private SearchField searchField = null; // 検索窓
        private ECreateMode createMode = ECreateMode.None;

        /** ********************************************************************************
        * @summary ウィンドウを開く
        ***********************************************************************************/
        [MenuItem("Tools/Texture Viewer")]
        private static void OpenWindow()
        {
            var window = GetWindow<TextureViewerWindow>();
            window.titleContent = ToolConfig.WindowTitle;

            var position = window.position;
            position.width = TextureTreeView.InitialHeaderTotalWidth + 50f;
            position.height = 400f;
            window.position = position;

            window.CreateTreeView(ECreateMode.Create); // 起動直後にTreeView作成
        }

        /** ********************************************************************************
        * @summary ウィンドウの描画
        ***********************************************************************************/
        private void OnGUI()
        {
            MyStyle.CreateGUIStyleIfNull();

            DrawHeader();
            DrawTreeView();

            switch (createMode)
            {
                case ECreateMode.Create:
                    if (treeView != null) 
                    {
                        createMode = ECreateMode.None;
                    }
                    break;
                case ECreateMode.Load:
                    if (treeView != null && treeView.IsInitialized) 
                    {
                        createMode = ECreateMode.None; 
                    }
                    break;
                default:
                    break;
            }
        }
        
        /** ********************************************************************************
        * @summary TreeViewなどの描画
        ***********************************************************************************/
        private void DrawTreeView()
        {
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));

            switch (createMode)
            {
                case ECreateMode.Create:
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        treeView?.OnGUI(rect);
                        EditorGUI.EndDisabledGroup();

                        rect.position += MyStyle.LoadingLabelPosition;
                        EditorGUI.LabelField(rect, ToolConfig.CreatingMessage, MyStyle.LoadingLabel);
                    }
                    break;
                case ECreateMode.Load:
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        treeView?.OnGUI(rect);
                        EditorGUI.EndDisabledGroup();

                        rect.position += MyStyle.LoadingLabelPosition;
                        EditorGUI.LabelField(rect, ToolConfig.LoadingMessage, MyStyle.LoadingLabel);
                    }
                    break;
                default:
                    {
                        treeView?.OnGUI(rect);
                    }
                    break;
            }
        }

        /** ********************************************************************************
        * @summary ウィンドウ上部のヘッダー描画
        ***********************************************************************************/
        private void DrawHeader()
        {
            var defaultColor = GUI.backgroundColor;

            // 検索窓を描画
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Reload", EditorStyles.toolbarButton))
                {
                    ReloadTexture();
                    CreateTreeView(ECreateMode.Load);
                }
                GUI.backgroundColor = defaultColor;

                GUILayout.Space(100);

                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();
                searchText = searchField?.OnToolbarGUI(searchText, GUILayout.MaxWidth(280f));
                if (EditorGUI.EndChangeCheck())
                {
                    if (treeView != null)
                    {
                        // TreeView.searchStringに検索文字列を入れると表示するItemを絞ってくれる
                        treeView.searchString = searchText;
                    }
                }
            }

            GUI.backgroundColor = defaultColor;
        }

        /** ********************************************************************************
        * @summary TreeViewの更新
        ***********************************************************************************/
        private void CreateTreeView(ECreateMode mode)
        {
            if (createMode != ECreateMode.None) { return; } // 作成中なら無視する

            createMode = mode;
            if (treeView != null)
            {
                treeView?.Clean();
                Repaint();
            }

            EditorApplication.delayCall += () =>
            {
                // TreeView作成
                treeViewState = treeViewState ?? new TreeViewState();
                treeView = treeView ?? new TextureTreeView(treeViewState);
                treeView.Setup(textures, textureImporters);

                // SearchFieldを初期化
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
            };
        }

        /** ********************************************************************************
        * @summary 指定したディレクトリからアセット一覧を取得
        ***********************************************************************************/
        public static IEnumerable<string> GetAssetPaths(string[] directories, string filter = "")
        {
            for (int i = 0; i < directories.Length; i++)
            {
                var directory = directories[i];
                if (directory[directory.Length - 1] == '/') 
                {
                    directory = directory.Substring(0, directory.Length - 1);
                }
            }

            var paths = AssetDatabase.FindAssets(filter, directories)
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Where(x => !string.IsNullOrEmpty(x)) 
                .OrderBy(x => x); 

            return paths;
        }


        /** ********************************************************************************
        * @summary ウィンドウにフォーカスが乗ったら呼ばれる
        ***********************************************************************************/
        private void OnFocus()
        {
            if (treeView != null)
            {
                treeView.UpdateDataSize();
            }
        }

        /** ********************************************************************************
        * @summary テクスチャのロード
        ***********************************************************************************/
        public void ReloadTexture()
        {
            CustomUI.DisplayProgressLoadTexture();

            // 指定ディレクトリからテクスチャロード
            var paths = GetAssetPaths(ToolConfig.TargetDirectories, "t:texture2d");
            var textureList = new List<Texture2D>();
            var importerList = new List<TextureImporter>();
            foreach (var path in paths)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null) continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                textureList.Add(texture);
                importerList.Add(importer);
            }
            textures = textureList.ToArray();
            textureImporters = importerList.ToArray();

            EditorUtility.ClearProgressBar();
        }

        public enum ECreateMode
        {
            None,
            Create,
            Load,
        }
    }
}
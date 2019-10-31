/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    /** ********************************************************************************
     * @summary テクスチャツール用TreeView
     ***********************************************************************************/
    internal partial class TextureTreeView : TreeView
    {
        public static float InitialHeaderTotalWidth 
        {
            get
            {
                float width = 0f;
                for (int i = 0; i < headerColumns.Length; i++)
                {
                    width += headerColumns[i].width;
                }
                return width;
            }
        }

        private static readonly TextAnchor fieldLabelAnchor = TextAnchor.MiddleLeft; 
        private const int yellowDataSize = ToolConfig.YellowDataSize; 
        private const int redDataSize = ToolConfig.RedDataSize; 
        private const int yellowTextureSize = ToolConfig.YellowDataSize;
        private const int redTextureSize = ToolConfig.RedTextureSize;
        private const int redMaxTextureSize = ToolConfig.RedMaxTextureSize;

        private Texture2D prefabIconTexture = null; // Prefabアイコン
        private TextureTreeElement[] baseElements = new TextureTreeElement[0]; // TreeViewで描画する要素

        // TreeViewのヘッダー定義
        static readonly TextureColumn[] headerColumns = new[] {
            new TextureColumn("Texture", 180f), // 0
            new TextureColumn("Texture Type", 105f), // 1
            new TextureColumn("Non Power of 2", 105f), // 2
            new TextureColumn("Max Size", 70f), // 3
            new TextureColumn("Generate\nMip Maps", 70f), // 4
            new TextureColumn("Alpha is\nTransparency", 96f), // 5
            new TextureColumn("Texture Size", 105f), // 6
            new TextureColumn("Data Size", 80f), // 7
        };

        public bool IsInitialized => isInitialized;
        public bool IsEmpty => baseElements.Length == 0;
        public int ElementCount => baseElements.Length;

        private static readonly TreeViewItem DummyTreeViewItem = new TreeViewItem { id = -999, depth = 0, displayName = "なし" };
        private static readonly List<TreeViewItem> DummyTreeViewList = new List<TreeViewItem> { DummyTreeViewItem };

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureTreeView(TreeViewState state)
        : base(new TreeViewState(), new TextureColumnHeader(new MultiColumnHeaderState(headerColumns)))
        {
            showAlternatingRowBackgrounds = true; // 背景のシマシマを表示
            showBorder = true; // 境界線を表示

            multiColumnHeader.sortingChanged += OnSortingChanged; // ソート変化時の処理を登録
        }

        /** ********************************************************************************
        * @summary 列の行を描画
        ***********************************************************************************/
        private void DrawRowColumn(RowGUIArgs args, Rect rect, int columnIndex)
        {
            if (args.item.id < 0) { return; }  // 検索がヒットしない場合はid=-999のダミー(DummyTreeViewItem)が入ってくる。ここでは描画をスキップする

            TextureTreeElement element = baseElements[args.item.id];
            if (element == null) { return; }

            var texture = element.Texture;
            var textureImporter = element.TextureImporter;
            if (texture == null || textureImporter == null)
            {
                EditorGUI.LabelField(rect, "(null)");
                return;
            }

            GUIStyle labelStyle = EditorStyles.label;

            switch (columnIndex)
            {
                case (int)EHeaderColumnId.TextureName:
                    rect.x += 2f;

                    // アイコンを描画する
                    Rect toggleRect = rect;
                    toggleRect.y += 2f;
                    toggleRect.size = new Vector2(12f, 12f);
                    GUI.DrawTexture(toggleRect, texture);

                    // テキストを描画する
                    Rect labelRect = new Rect(rect);
                    labelRect.x += toggleRect.width;
                    EditorGUI.LabelField(labelRect, args.label);
                    break;
                case (int)EHeaderColumnId.TextureType: // TextureType
                    EditorGUI.LabelField(rect, textureImporter.textureType.ToString());
                    break;
                case (int)EHeaderColumnId.NPot: // Non power of 2
                    if (textureImporter.npotScale == TextureImporterNPOTScale.None)
                    {
                        labelStyle = MyStyle.RedLabel;
                    }
                    EditorGUI.LabelField(rect, textureImporter.npotScale.ToString(), labelStyle);
                    break;
                case (int)EHeaderColumnId.MaxSize: // Max size
                    if (textureImporter.maxTextureSize > redMaxTextureSize)
                    {
                        labelStyle = MyStyle.RedLabel;
                    }
                    EditorGUI.LabelField(rect, textureImporter.maxTextureSize.ToString(), labelStyle);
                    break;
                case (int)EHeaderColumnId.GenerateMips: // Generate mip maps
                    if (textureImporter.mipmapEnabled == true)
                    {
                        labelStyle = MyStyle.RedLabel;
                    }
                    EditorGUI.LabelField(rect, textureImporter.mipmapEnabled.ToString(), labelStyle);
                    break;
                case (int)EHeaderColumnId.AlphaIsTransparency: // Alpha is Transparency
                    EditorGUI.LabelField(rect, textureImporter.alphaIsTransparency.ToString());
                    break;
                case (int)EHeaderColumnId.TextureSize: // Texture Size
                    switch ((element.Texture.width, element.Texture.height))
                    {
                        case var values when values.width > redTextureSize || values.height > redTextureSize:
                            labelStyle = MyStyle.RedLabel;
                            break;
                        case var values when values.width > yellowTextureSize || values.height > yellowTextureSize:
                            labelStyle = MyStyle.YellowLabel;
                            break;
                    }
                    EditorGUI.LabelField(rect, $"{element.Texture.width}x{element.Texture.height}", labelStyle);
                    break;
                case (int)EHeaderColumnId.DataSize: // データサイズ
                    switch ((int)element.TextureByteLength)
                    {
                        case int len when len > redDataSize:
                            labelStyle = MyStyle.RedLabel;
                            break;
                        case int len when len > yellowDataSize:
                            labelStyle = MyStyle.YellowLabel;
                            break;
                        default:
                            break;
                    }
                    EditorGUI.LabelField(rect, element.TextureDataSizeText, labelStyle);
                    //EditorGUI.LabelField(rect, element.TextureByteLength.ToString(), labelStyle);

                    break;
            }
        }

        /** ********************************************************************************
        * @summary 要素のクリア
        ***********************************************************************************/
        public void Clean()
        {
            baseElements = new TextureTreeElement[0];
            Reload();
        }

        /** ********************************************************************************
        * @summary キー入力イベント
        ***********************************************************************************/
        protected override void KeyEvent()
        {
            base.KeyEvent();

            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return) // キーボードのエンターキーを押した場合
            {
                var selection = this.GetSelection();
                if (selection.Count == 0) { return; }

                int id = selection.ElementAt(0);
                if (id < 0) { return; }

                // Prefabを開く
                var path = baseElements[id].AssetPath;
                var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                AssetDatabase.OpenAsset(prefab);
            }
        }

        /** ********************************************************************************
        * @summary 選択が変化
        ***********************************************************************************/
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            if (selectedIds.Count == 0) { return; }

            selectedIds = selectedIds.Distinct().ToArray();
            Object[] objects = new Object[selectedIds.Count];
            for (int i = 0; i < selectedIds.Count; i++)
            {
                int id = selectedIds.ElementAt(i);
                var path = baseElements[id].AssetPath;
                objects[i] = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            }

            Selection.objects = objects;
            EditorGUIUtility.PingObject(objects[objects.Length - 1]); // 強調表示
        }

        /** ********************************************************************************
        * @summary ダブルクリック時に呼ばれる
        ***********************************************************************************/
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

            // 強調表示する
            var path = baseElements[id].AssetPath;
            var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            AssetDatabase.OpenAsset(prefab); // Prefabを開く
        }

        /** ********************************************************************************
        * @summary 要素の取得
        ***********************************************************************************/
        public TextureTreeElement GetElement(int index)
        {
            return baseElements[index];
        }

        /** ********************************************************************************
        * @summary データサイズ更新
        ***********************************************************************************/
        public void UpdateDataSize()
        {
            foreach (var element in baseElements)
            {
                element.UpdateDataSize();
            }
        }

        /** ********************************************************************************
        * @summary 選択中の要素を取得
        ***********************************************************************************/
        public IEnumerable<TextureTreeElement> GetSelectionElement()
        {
            return GetSelection().Select(id => GetElement(id));
        }

        /** ********************************************************************************
        * @summary 列の作成
        * @note    BuildRows()で返されたIListを元にしてTreeView上で描画が実行されます。
        ***********************************************************************************/
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root); // TreeView.BuildRowsの内部では検索による絞り込みを行っている
            if (hasSearch && rows.Count == 0) // 検索ヒットなし
            {
                return DummyTreeViewList;
            }

            //　TreeViewItemの親子関係を構築
            var elements = new List<TreeViewItem>();

            CustomUI.RowCount = baseElements.Count();
            foreach (var baseElement in baseElements)
            {
                var baseItem = CreateTreeViewItem(baseElement) as TextureTreeViewItem;
                baseItem.data = baseElement; // 要素を紐づける

                root.AddChild(baseItem);
                rows.Add(baseItem); // 列に追加
            }

            // 親子関係に基づいてDepthを自動設定するメソッド
            SetupDepthsFromParentsAndChildren(root);

            return rows;
        }


        /** ********************************************************************************
        * @summary ルートの作成
        ***********************************************************************************/
        protected override TreeViewItem BuildRoot()
        {
            // BuildRootではRootだけを返す
            return new TextureTreeViewItem { id = -1, depth = -1, displayName = "Root" };
        }

        /** ********************************************************************************
        * @summary 要素を作成
        ***********************************************************************************/
        private TreeViewItem CreateTreeViewItem(TextureTreeElement model)
        {
            return new TextureTreeViewItem { id = model.Index, displayName = model.AssetName };
        }

        /** ********************************************************************************
        * @summary TreeView初期化
        ***********************************************************************************/
        public void Setup(Texture2D[] textures, TextureImporter[] importers)
        {
            // TreeViewの要素を作成
            baseElements = new TextureTreeElement[textures.Length];
            for (int i = 0; i < baseElements.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(textures[i]);
                baseElements[i] = new TextureTreeElement
                {
                    AssetPath = path,
                    AssetName = System.IO.Path.GetFileNameWithoutExtension(path),
                    Texture = textures[i],
                    TextureImporter = importers[i],
                };
            }

            for (int i = 0; i < baseElements.Length; i++)
            {
                var element = baseElements[i];
                element.Index = i;
                element.UpdateDataSize();
            }

            Reload(); // Reloadを呼ぶとBuildRootが実行され、次にBuildRowsが実行されます。
        }

        /** ********************************************************************************
         * @summary TreeViewの列の描画
         ***********************************************************************************/
        protected override void RowGUI(RowGUIArgs args)
        {
            if (prefabIconTexture == null)
            {
                // Prefabアイコンをロード
                prefabIconTexture = EditorGUIUtility.Load("Prefab Icon") as Texture2D;
            }

            // TreeView 各列の描画
            for (var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++)
            {
                var rect = args.GetCellRect(visibleColumnIndex);
                var columnIndex = args.GetColumn(visibleColumnIndex);
                var labelStye = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;
                labelStye.alignment = fieldLabelAnchor;

                DrawRowColumn(args, rect, columnIndex);
            }
        }
    }
}
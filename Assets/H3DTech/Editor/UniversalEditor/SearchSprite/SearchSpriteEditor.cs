using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SearchSpriteInfo
{
    public string AtlasPath = string.Empty;
    public Texture AtlasTexture = null;
    public string SpriteName = string.Empty;
    public Rect SpriteRect;
}
public class SearchResultInfo
{
    public string SearchName = string.Empty;
    public List<SearchSpriteInfo> SearchSpriteInfo = null;

    //public Rect GetSpirteUVRect(int index)
    //{
    //    Rect spriteUVRect = new Rect(0, 0, 0, 0);
    //    if (
    //        (m_RefAtlasInfo == null)
    //        || (index < 0)
    //        || (index >= m_RefAtlasInfo.Count)
    //        || (m_RefAtlasInfo[index] == null)
    //        )
    //    {
    //        return spriteUVRect;
    //    }

    //    m_RefAtlasInfo[index].SpriteInfo.TryGetValue(m_SpriteName, out spriteUVRect);

    //    return spriteUVRect;
    //}
}

public class SearchSpriteEditor : EditorRoot
{
    static SearchSpriteEditor m_EditorRoot = null;

    static float m_RootWidth = 600f;

    static string m_SearchTextBoxName = "_SearchBox";
    static string m_VagueSearchBtnName = "_VagueSearchBtn";
    static string m_SearchBtnName = "_SearchBtn";
    static string m_SetBtnName = "_SetBtn";
    static string m_HelpBtnName = "_HelpBtn";
    static string m_SearchResultListName = "_SearchResultList";
    static string m_SpriteViewName = "_SpriteView";
    static string m_AtlasViewName = "_AltasView";
    static string m_SpriteInfoLabel = "_SpriteInfoLabel";
    static string m_AtlasInfoLabel = "_AtlasInfoLabel";

    static SearchResultInfo m_SearchResultInfo = null;

    static string m_helpURL = "http://192.168.2.121:8090/pages/viewpage.action?pageId=9306137";

    [UnityEditor.MenuItem("H3D/UI/sprite搜索工具")]
    static void EditorInit()
    {//创建主窗口

        EditorRoot root = EditorManager.GetInstance().FindEditor("Sprite搜索工具") as SearchSpriteEditor;
        if (root == null) 
        {
            EditorManager.GetInstance().RemoveEditor("Sprite搜索工具");
            root = EditorManager.GetInstance().CreateEditor<SearchSpriteEditor>("Sprite搜索工具", false, InitControls) as SearchSpriteEditor;
        }
    }

    public static void InitControls(EditorRoot editorRoot)
    {
        SearchSpriteEditor searchSpriteEditor = editorRoot as SearchSpriteEditor;

        if (editorRoot == null)
        {
            //提示程序错误Message
            EditorUtility.DisplayDialog("运行错误",
                                         "窗口初始化失败",
                                         "确认");
            return;
        }

        m_EditorRoot = searchSpriteEditor;

        m_EditorRoot.position = new Rect(100f, 100f, m_RootWidth, 768f);


        #region 创建窗口布局元素
        Rect searchTextRect = new Rect(0, 0, 300, 20);

        #region 第一级分割
        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f);

        HSpliterCtrl hs2 = new HSpliterCtrl();
        hs2.MinOffset = 100f;
        hs2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f, true);
        hs2.Dragable = true;

        HBoxCtrl hb1 = new HBoxCtrl();      //搜索栏 HB
        HBoxCtrl hb2 = new HBoxCtrl();      //预览区 HB
        VBoxCtrl vb3 = new VBoxCtrl();      //结果List HB
        #endregion

        #region 第二级分割
        VSpliterCtrl vs2_1 = new VSpliterCtrl();
        vs2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_RootWidth / 2);
        vs2_1.Dragable = true;

        VBoxCtrl vb2_1 = new VBoxCtrl();
        VBoxCtrl vb2_2 = new VBoxCtrl();
        #endregion

        #region 第三级分割
        HSpliterCtrl hs2_1_1 = new HSpliterCtrl();
        hs2_1_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(10f, true);

        HSpliterCtrl hs2_2_1 = new HSpliterCtrl();
        hs2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(10f, true);

        HBoxCtrl hb2_1_1 = new HBoxCtrl();
        HBoxCtrl hb2_1_2 = new HBoxCtrl();

        HBoxCtrl hb2_2_1 = new HBoxCtrl();
        HBoxCtrl hb2_2_2 = new HBoxCtrl();
        #endregion
        #endregion

        #region 布置窗口
        TextBoxCtrl searchTextBox = new TextBoxCtrl();
        searchTextBox.Size = searchTextRect;
        searchTextBox.Icon = UnityInternalIconCache.GetInstance().GetCacheIcon("d_ViewToolZoom");
        searchTextBox.Name = m_SearchTextBoxName;
        searchTextBox.Caption = "搜索";

        ButtonCtrl vagueSearchBtn = new ButtonCtrl();
        vagueSearchBtn.Name = m_VagueSearchBtnName;
        vagueSearchBtn.Caption = "模糊搜索";
        vagueSearchBtn.onClick = searchSpriteEditor.OnSearchBtn;

        ButtonCtrl searchBtn = new ButtonCtrl();
        searchBtn.Name = m_SearchBtnName;
        searchBtn.Caption = "精确搜索";
        searchBtn.onClick = searchSpriteEditor.OnSearchBtn;

        ButtonCtrl setBtn = new ButtonCtrl();
        setBtn.Name = m_SetBtnName;
        setBtn.Caption = "设置";
        setBtn.onClick = searchSpriteEditor.OnSetBtn;

        ButtonCtrl helpBtn = new ButtonCtrl();
        helpBtn.Name = m_HelpBtnName;
        helpBtn.Caption = "帮助";
        helpBtn.onClick = searchSpriteEditor.OnHelpBtnClick;

        hb1.Add(searchTextBox);
        hb1.Add(vagueSearchBtn);
        hb1.Add(searchBtn);
        hb1.Add(setBtn);
        hb1.Add(helpBtn);

        MainViewCtrl spriteView = new MainViewCtrl();
        spriteView.Name = m_SpriteViewName;
        spriteView.bkColor = Color.gray;
        spriteView.Is2DView = true;

        LabelCtrl spriteInfoLabel = new LabelCtrl();
        spriteInfoLabel.Name = m_SpriteInfoLabel;
        spriteInfoLabel.Caption = "";

        MainViewCtrl atlasView = new MainViewCtrl();
        atlasView.Name = m_AtlasViewName;
        atlasView.bkColor = Color.gray;
        atlasView.Is2DView = true;

        LabelCtrl atlasInfoLabel = new LabelCtrl();
        atlasInfoLabel.Name = m_AtlasInfoLabel;
        atlasInfoLabel.Caption = "";

        hb2_1_1.Add(spriteView);
        hb2_1_2.Add(spriteInfoLabel);
      
        hb2_2_1.Add(atlasView);
        hb2_2_2.Add(atlasInfoLabel);

        hs2_1_1.Add(hb2_1_1);
        hs2_1_1.Add(hb2_1_2);

        hs2_2_1.Add(hb2_2_1);
        hs2_2_1.Add(hb2_2_2);

        vb2_1.Add(hs2_1_1);
        vb2_2.Add(hs2_2_1);

        vs2_1.Add(vb2_1);
        vs2_1.Add(vb2_2);

        hb2.Add(vs2_1);

        ListViewCtrl searchResultList = new ListViewCtrl();
        searchResultList.Name = m_SearchResultListName;
        searchResultList.onItemSelected = searchSpriteEditor.OnSelectListItem;
        searchResultList.onItemSelectedR = searchSpriteEditor.OnSelectListItem;

        SpaceCtrl spaceCtrl = new SpaceCtrl();
        spaceCtrl.CurrValue = 30f;

        vb3.Add(searchResultList);
        vb3.Add(spaceCtrl);

        hs1.Add(hb1);
        hs1.Add(hs2);

        hs2.Add(hb2);
        hs2.Add(vb3);

        m_EditorRoot.RootCtrl = hs1;
        #endregion

        m_EditorRoot.onGUI = searchSpriteEditor.OnEditorGUI;

    }

    void OnEditorGUI(EditorRoot root)
    {
        if ((Event.current.type == EventType.MouseDrag)
            || (Event.current.type == EventType.ScrollWheel))
        {
            RequestRepaint();
        }
    }

    public T _GetControl<T>(string ctrlName) where T : EditorControl
    {
        return FindControl(ctrlName) as T;
    }


    void OnSearchBtn(EditorControl c)
    {
        TextBoxCtrl searchText = _GetControl<TextBoxCtrl>(m_SearchTextBoxName);
        if(null == searchText)
        {
            return;
        }
     
        string spriteName = searchText.Text;
        List<AtlasInfoForSearchSprite> atlasInfoTbl = null;

        SEARCHSPRITE_ERROR_TYPE errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;

        if(c.Name == m_VagueSearchBtnName)
        {
            errorType = SearchSpriteEidtorModel.GetInstance().VagueSearchSprite(spriteName, out atlasInfoTbl);
        }
        else
        {
            errorType = SearchSpriteEidtorModel.GetInstance().SearchSprite(spriteName, out atlasInfoTbl);
        }
      
        switch(errorType)
        {
            //搜索成功
            case SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR:
                FixSearchResultAndList(spriteName, atlasInfoTbl);
                ClearPreview();

                if(
                    (null == m_SearchResultInfo)
                    || (null == m_SearchResultInfo.SearchSpriteInfo)
                    || (0 == m_SearchResultInfo.SearchSpriteInfo.Count)
                    )
                {//搜索结果为空

                    EditorUtility.DisplayDialog("查找完毕", "\n" + spriteName + "不存在", "确认");
                }
                else
                {//不为空

                    EditorUtility.DisplayDialog("查找完毕", "\n有" + m_SearchResultInfo.SearchSpriteInfo.Count + "个Atlas包含" + spriteName, "确认");
                }

                break;

            //未指定Sprite名称
            case SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_SEARCH_WITH_EMPTY_NAME:
                EditorUtility.DisplayDialog("操作失败", "\n未指定Sprite名称", "确认");
                if (m_SearchResultInfo != null)
                {
                    searchText.Text = m_SearchResultInfo.SearchName;
                }
                break;

            default:
                break;
        }


    }

    void OnSetBtn(EditorControl c)
    {
        if (null == Selection.activeGameObject)
        {
            EditorUtility.DisplayDialog("操作失败", "\n未指定任何UISprite", "确认");
            return;
        }
  
        if (
            (m_SearchResultInfo == null)
            || (m_SearchResultInfo.SearchSpriteInfo == null)
            )
        {
            EditorUtility.DisplayDialog("操作失败", "\n未指定有效Sprite", "确认");
            return;
        }

        ListViewCtrl searchList = _GetControl<ListViewCtrl>(m_SearchResultListName);
        if (null == searchList)
        {
            return;
        }


        int index = searchList.LastSelectItem;
        if(
            (index < 0)
            || (index > m_SearchResultInfo.SearchSpriteInfo.Count)
            )
        {
            EditorUtility.DisplayDialog("操作失败", "\n未指定有效Sprite", "确认");
            return;
        }

        SEARCHSPRITE_ERROR_TYPE errorType = SearchSpriteEidtorModel.GetInstance().SetUISprite(Selection.activeGameObject, m_SearchResultInfo.SearchSpriteInfo[index].SpriteName, m_SearchResultInfo.SearchSpriteInfo[index].AtlasPath);
        
        switch(errorType)
        {
            //设定成功
            case SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR:
                string atlasName = Path.GetFileNameWithoutExtension(m_SearchResultInfo.SearchSpriteInfo[index].AtlasPath);
                EditorUtility.DisplayDialog("设置完成", "\n已设置" + atlasName + "的" + m_SearchResultInfo.SearchSpriteInfo[index].SpriteName, "确认");
           
                break;

            //设定对象不是UISpirte
            case SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_SET_IS_NOT_UISPRITE:
                string goName = Selection.activeGameObject.name;
                EditorUtility.DisplayDialog("设置失败", "\n" + goName + "不是UISprite", "确认");
                                              
                break;

            default:
                break;
        }

        RequestRepaint();
    }

    void OnHelpBtnClick(EditorControl c)
    {
        System.Diagnostics.Process.Start(m_helpURL);
    }

    void OnSelectListItem(EditorControl c, int index)
    {
        if(
            (m_SearchResultInfo == null)
            || (m_SearchResultInfo.SearchSpriteInfo == null)
            )
        {
            return;
        }

        ListViewCtrl searchList = c as ListViewCtrl;
        if (null == searchList)
        {
            return;
        }

        MainViewCtrl spriteView = _GetControl<MainViewCtrl>(m_SpriteViewName);
        if(null == spriteView)
        {
            return;
        }

        MainViewCtrl atlasView = _GetControl<MainViewCtrl>(m_AtlasViewName);
        if(null == atlasView)
        {
            return;
        }

        LabelCtrl spriteInfo = _GetControl<LabelCtrl>(m_SpriteInfoLabel);
        if(null == spriteInfo)
        {
            return;
        }

        LabelCtrl atlasInfo = _GetControl<LabelCtrl>(m_AtlasInfoLabel);
        if(null == atlasInfo)
        {
            return;
        }

        SearchSpriteInfo info = m_SearchResultInfo.SearchSpriteInfo[index];

        Texture atlasTex = info.AtlasTexture;

        //获取Sprite在Atlas中的位置
        Rect spriteUVRect = info.SpriteRect; 
        //m_SearchResultInfo.GetSpirteUVRect(index);
        Rect spriteUVRectReal = UtilityForNGUI.ConvertToTexCoords(spriteUVRect, atlasTex.width, atlasTex.height);

        float aspect = (float)atlasTex.width / (float)atlasTex.height;
        float w1 = 10.0f;
        float h1 = w1 / aspect;

        float aspect2 = (float)spriteUVRect.width / (float)spriteUVRect.height;
        float w2 = 3.0f;
        float h2 = w2 / aspect2;

        //创建预览Object
        GameObject spritePreviewObj = _GenTexturePreviewObject(w2, h2, atlasTex, spriteUVRectReal);
        GameObject atlasPreviewObj = _GenTexturePreviewObject(w1, h1, atlasTex, new Rect(0, 0, 1, 1));

        //将预览Object绑定至MainView的主相机之下
        UniversalEditorUtility.DestoryChildren(spriteView.GetBindingTarget());
        spritePreviewObj.transform.parent = spriteView.GetBindingTarget().transform;
        spritePreviewObj.transform.localPosition = Vector3.zero;

        UniversalEditorUtility.DestoryChildren(atlasView.GetBindingTarget());
        atlasPreviewObj.transform.parent = atlasView.GetBindingTarget().transform;
        atlasPreviewObj.transform.localPosition = Vector3.zero;

        //更新预览信息
        string atlasName = Path.GetFileNameWithoutExtension(info.AtlasPath);
        spriteInfo.Caption = "Sprite: " + info.SpriteName + " , " + spriteUVRect.width + " * " + spriteUVRect.height;
        atlasInfo.Caption = "Atlas: " + atlasName + " , " + info.AtlasTexture.width + " * " + info.AtlasTexture.height;
        //spriteView.mainViewUVRect = spriteUVRect;
     
        RequestRepaint();
    }

    void FixSearchResultAndList(string searchName, List<AtlasInfoForSearchSprite> atlasInfoTbl)
    {
        if (null == atlasInfoTbl)
        {
            return;
        }

        ListViewCtrl searchList = _GetControl<ListViewCtrl>(m_SearchResultListName);
        if (null == searchList)
        {
            return;
        }

        searchList.ClearItems();

        m_SearchResultInfo = new SearchResultInfo();
        m_SearchResultInfo.SearchName = searchName;
        m_SearchResultInfo.SearchSpriteInfo = new List<SearchSpriteInfo>();
     
        for (int index = 0; index < atlasInfoTbl.Count; index++)
        {
            foreach (var item in atlasInfoTbl[index].SpriteInfo)
            {
                SearchSpriteInfo newInfo = new SearchSpriteInfo();
                newInfo.AtlasPath = atlasInfoTbl[index].AtlasPath;
                newInfo.AtlasTexture = atlasInfoTbl[index].AtlasTexture;
                newInfo.SpriteName = item.Key;
                newInfo.SpriteRect = item.Value;

                m_SearchResultInfo.SearchSpriteInfo.Add(newInfo);

                ListCtrlItem newItem = new ListCtrlItem();
                newItem.name = item.Key + "  " + newInfo.AtlasPath;
                newItem.color = Color.white;
                newItem.onSelectColor = Color.blue;
                searchList.AddItem(newItem);
            }
        }

    }

    void ClearPreview()
    {
        MainViewCtrl spriteView = _GetControl<MainViewCtrl>(m_SpriteViewName);
        if (null == spriteView)
        {
            return;
        }

        MainViewCtrl atlasView = _GetControl<MainViewCtrl>(m_AtlasViewName);
        if (null == atlasView)
        {
            return;
        }

        LabelCtrl spriteInfo = _GetControl<LabelCtrl>(m_SpriteInfoLabel);
        if (null == spriteInfo)
        {
            return;
        }

        LabelCtrl atlasInfo = _GetControl<LabelCtrl>(m_AtlasInfoLabel);
        if (null == atlasInfo)
        {
            return;
        }

        UniversalEditorUtility.DestoryChildren(spriteView.GetBindingTarget());
        UniversalEditorUtility.DestoryChildren(atlasView.GetBindingTarget());

        spriteInfo.Caption = "";
        atlasInfo.Caption = "";
    }

    private GameObject _GenTexturePreviewObject(float width, float height, Texture tex, Rect uvRect)
    {
        GameObject previewObj = new GameObject();
        previewObj.transform.localScale = new Vector3(width, height, 1.0f);
        MeshRenderer meshRenderer = previewObj.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = previewObj.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[4];
        verts[0].Set(-0.5f, 0.5f, 0.0f);
        verts[1].Set(0.5f, 0.5f, 0.0f);
        verts[2].Set(0.5f, -0.5f, 0.0f);
        verts[3].Set(-0.5f, -0.5f, 0.0f);

        Vector3[] norms = new Vector3[4];
        norms[0].Set(0.0f, 0.0f, -1.0f);
        norms[1].Set(0.0f, 0.0f, -1.0f);
        norms[2].Set(0.0f, 0.0f, -1.0f);
        norms[3].Set(0.0f, 0.0f, -1.0f);

        Vector2[] uv = new Vector2[4];
        uv[0].Set(uvRect.x, uvRect.y + uvRect.height);
        uv[1].Set(uvRect.x + uvRect.width, uvRect.y + uvRect.height);
        uv[2].Set(uvRect.x + uvRect.width, uvRect.y);
        uv[3].Set(uvRect.x, uvRect.y);

        Color[] vertcolor = new Color[4];
        vertcolor[0] = Color.white;
        vertcolor[1] = Color.white;
        vertcolor[2] = Color.white;
        vertcolor[3] = Color.white;

        int[] indices = new int[4];
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 3;

        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uv;
        mesh.colors = vertcolor;
        mesh.SetIndices(indices, MeshTopology.Quads, 0);

        meshFilter.mesh = mesh;

        Shader shader = Shader.Find("Unlit/Transparent Colored");

        Material mat = new Material(shader);
        mat.mainTexture = tex;
        tex.filterMode = FilterMode.Point;

        meshRenderer.material = mat;

        return previewObj;
    }

}
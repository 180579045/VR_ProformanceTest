
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Windows.Forms;
using System.IO;
using System;
public struct OPERATE_AREA
{
    public LabelCtrl OldView_Name;
    public TextureBoxCtrl OldView_Texture;
    public LabelCtrl NewView_Name;
    public TextureBoxCtrl NewView_Texture;
    public ListViewCtrl SpriteList;
    public string AtlasPath;

    public OPERATE_AREA(string a)
    {
        OldView_Name = null;
        OldView_Texture = null;
        NewView_Name = null;
        NewView_Texture = null;
        SpriteList = null;
        AtlasPath = string.Empty;
    }
}

public enum DRAG_TYPE
{
    DRAG_TYPE_ATLAS = 0,
    DRAG_TYPE_SPRITE,
    DRAG_TYPE_DEFAULT = -1
}
public class UIAdjustAtlasEditor
{//Atlas调整工具
    static EditorRoot m_EditorRoot = null;                        //根控件
    static private int m_OperateAreaNum = 4;
    static private int m_CurrentSelectIndex = -1;

    static private OPERATE_AREA[] m_OperateArea = new OPERATE_AREA[m_OperateAreaNum];
    static private string[] m_AtlasPathInSpriteView = new string[m_OperateAreaNum];
    static private ListViewCtrl m_AtlasList = null;
    static private InspectorViewCtrl m_inspector = null;
    //static private VBoxCtrl m_inspectorView = null;
    static private LabelCtrl m_LoadingStr = null;

    static private UIAtlasCommandCounter m_CommandCounter = null;
    static private GameObject m_Counter = null;             //命令计数器

    static private string m_SpriteWidth = string.Empty;
    static private string m_SpriteHeight = string.Empty;

    static private string m_AtlasOutputPath = string.Empty;

    static private string m_helpURL = "http://192.168.2.121:8090/pages/viewpage.action?pageId=7536670";

    static private bool m_IsNeedReloadTexture = false;
    static private EditorWindow m_ConfigWindow = null;

    [UnityEditor.MenuItem("Assets/H3D/Atlas调整工具")]
    [UnityEditor.MenuItem("H3D/UI/Atlas调整工具")]
    static void Init()
    {//创建主窗口

        EditorRoot root = EditorManager.GetInstance().FindEditor("Atlas调整工具");
        if (root == null)
        {
            EditorManager.GetInstance().RemoveEditor("Atlas调整工具");
            root = EditorManager.GetInstance().CreateEditor("Atlas调整工具", false, InitControls);
        }
    }

    public static void InitControls(EditorRoot editorRoot)
    {
        if (editorRoot == null)
        {
            //提示程序错误Message
            EditorUtility.DisplayDialog("运行错误",
                                         "窗口初始化失败",
                                         "确认");
            return;
        }

        m_EditorRoot = editorRoot;
        //m_EditorRoot.position = new Rect(100f, 100f, 1280, 768f);

        {
            m_EditorRoot.onEnable = OnEnable;
            m_EditorRoot.onDisable = OnDisable;
        }

        #region 创建布置窗口元素
        Rect btnRect = new Rect(0, 0, 80, 20);
        //Rect labelRect = new Rect(0, 0, 80, 5);
        Rect textureRect = new Rect(0, 0, 400, 400);

        #region 第一级分割
        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f);
        HSpliterCtrl hs2 = new HSpliterCtrl();
        hs2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f, true);

        HBoxCtrl hb1 = new HBoxCtrl();      //上方菜单栏
        HBoxCtrl hb2 = new HBoxCtrl();      //主窗口
        HBoxCtrl hb3 = new HBoxCtrl();      //下方状态栏
        #endregion

        #region 第二级分割
        VSpliterCtrl vs2_1 = new VSpliterCtrl();
        vs2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);
        vs2_1.Dragable = true;
        VSpliterCtrl vs2_2 = new VSpliterCtrl();
        vs2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(300f, true);
        vs2_2.Dragable = true;

        VBoxCtrl vb2_1 = new VBoxCtrl();               //Atlas列表      
        VBoxCtrl vb2_2 = new VBoxCtrl();               //操作、预览区      
        VBoxCtrl vb2_3 = new VBoxCtrl();               //Inspector视图 
        //m_inspectorView = new VBoxCtrl();
        vb2_2.Name = "OperateArea";
        #endregion


        #region 第三级分割
        VSpliterCtrl vs2_2_1 = new VSpliterCtrl();
        vs2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(300f);

        vs2_2_1.Dragable = true;

        VBoxCtrl vb2_2_1 = new VBoxCtrl();               //移动对象1      
        VBoxCtrl vb2_2_2 = new VBoxCtrl();               //移动对象2   
        #endregion

        #region 第四级分割
        HSpliterCtrl hs2_2_1_1 = new HSpliterCtrl();
        hs2_2_1_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_1_1.Dragable = true;
        
        HSpliterCtrl hs2_2_1_2 = new HSpliterCtrl();
        hs2_2_1_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_2_1 = new HSpliterCtrl();
        hs2_2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_2_1.Dragable = true;
 
        HSpliterCtrl hs2_2_2_2 = new HSpliterCtrl();
        hs2_2_2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        VBoxCtrl hb2_2_1_1 = new VBoxCtrl();      //变更前预览1
        VBoxCtrl hb2_2_1_2 = new VBoxCtrl();      //变更后预览1
        VBoxCtrl hb2_2_1_3 = new VBoxCtrl();      //SpriteList1

        VBoxCtrl hb2_2_2_1 = new VBoxCtrl();      //变更前预览2
        VBoxCtrl hb2_2_2_2 = new VBoxCtrl();      //变更后预览2
        VBoxCtrl hb2_2_2_3 = new VBoxCtrl();      //SpriteList2
        #endregion
        #endregion

        #region 布置窗口（由高至低布置）
        #region 第四级分割
        m_OperateArea[0].OldView_Name = new LabelCtrl();
        m_OperateArea[0].OldView_Name.Name = "Area0_OldView_Name";
        m_OperateArea[0].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[0].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].OldView_Texture.Name = "Area0_OldView_Texture";
        m_OperateArea[0].OldView_Texture.Size = textureRect;

        m_OperateArea[0].NewView_Name = new LabelCtrl();
        m_OperateArea[0].NewView_Name.Name = "Area0_NewView_Name";
        m_OperateArea[0].NewView_Name.Caption = "";

        m_OperateArea[0].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].NewView_Texture.Name = "Area0_NewView_Texture";
        m_OperateArea[0].NewView_Texture.Size = textureRect;

        m_OperateArea[0].SpriteList = new ListViewCtrl();
        m_OperateArea[0].SpriteList.Name = "Area0_SpriteList";
        m_OperateArea[0].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[0].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[0].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;
        m_OperateArea[0].SpriteList.IsTextureView = false;

        hb2_2_1_1.Add(m_OperateArea[0].OldView_Name);
        hb2_2_1_1.Add(m_OperateArea[0].OldView_Texture);

        hb2_2_1_2.Add(m_OperateArea[0].NewView_Name);
        hb2_2_1_2.Add(m_OperateArea[0].NewView_Texture);

        hb2_2_1_3.Add(m_OperateArea[0].SpriteList);

        hs2_2_1_2.Add(hb2_2_1_2);
        hs2_2_1_2.Add(hb2_2_1_3);

        hs2_2_1_1.Add(hb2_2_1_1);
        hs2_2_1_1.Add(hs2_2_1_2);

        m_OperateArea[1].OldView_Name = new LabelCtrl();
        m_OperateArea[1].OldView_Name.Name = "Area1_OldView_Name";
        m_OperateArea[1].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[1].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].OldView_Texture.Name = "Area1_OldView_Texture";
        m_OperateArea[1].OldView_Texture.Size = textureRect;

        m_OperateArea[1].NewView_Name = new LabelCtrl();
        m_OperateArea[1].NewView_Name.Name = "Area1_NewView_Name";
        m_OperateArea[1].NewView_Name.Caption = "";

        m_OperateArea[1].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].NewView_Texture.Name = "Area1_NewView_Texture";
        m_OperateArea[1].NewView_Texture.Size = textureRect;

        m_OperateArea[1].SpriteList = new ListViewCtrl();
        m_OperateArea[1].SpriteList.Name = "Area1_SpriteList";
        m_OperateArea[1].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[1].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[1].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;


        hb2_2_2_1.Add(m_OperateArea[1].OldView_Name);
        hb2_2_2_1.Add(m_OperateArea[1].OldView_Texture);

        hb2_2_2_2.Add(m_OperateArea[1].NewView_Name);
        hb2_2_2_2.Add(m_OperateArea[1].NewView_Texture);

        hb2_2_2_3.Add(m_OperateArea[1].SpriteList);

        hs2_2_2_2.Add(hb2_2_2_2);
        hs2_2_2_2.Add(hb2_2_2_3);

        hs2_2_2_1.Add(hb2_2_2_1);
        hs2_2_2_1.Add(hs2_2_2_2);
        #endregion

        #region 第三级分割
        vb2_2_1.Add(hs2_2_1_1);
        vb2_2_2.Add(hs2_2_2_1);

        vs2_2_1.Add(vb2_2_1);
        vs2_2_1.Add(vb2_2_2);
        #endregion

        #region 第二级分割
        m_AtlasList = new ListViewCtrl();
        m_AtlasList.Name = "AtlasList";
        m_AtlasList.onPrepareCustomDrag = onAtlasListPrepareDrag;
        m_AtlasList.onItemSelected = onAtlasListSelect;
        m_AtlasList.onItemCtrlSelected = onAtlasListSelect;
        m_AtlasList.onItemSelectedR = onAtlasListSelect;
        m_AtlasList.onItemSelectedRU = onAtlasListSelectRU;
      
        m_inspector = new InspectorViewCtrl();
        m_inspector.Name = "inspector";
        m_inspector.onInspector = null;

        vb2_1.Add(m_AtlasList);
        vb2_2.Add(vs2_2_1);
        vb2_3.Add(m_inspector);

        vs2_2.Add(vb2_2);
        vs2_2.Add(vb2_3);
        //vs2_2.Add(m_inspectorView);

        vs2_1.Add(vb2_1);
        vs2_1.Add(vs2_2);
        #endregion

        #region 第一级分割
        ButtonCtrl projectBtn = new ButtonCtrl();
        projectBtn.Caption = "文件";
        projectBtn.Name = "AddProjectButton";
        projectBtn.Size = btnRect;
        projectBtn.onClick = OnProjectBtnClick;

        ButtonCtrl OperateBtn = new ButtonCtrl();
        OperateBtn.Caption = "操作";
        OperateBtn.Name = "OperateButton";
        OperateBtn.Size = btnRect;
        OperateBtn.onClick = OnOperateBtnClick;

        ButtonCtrl OptionBtn = new ButtonCtrl();
        OptionBtn.Caption = "选项";
        OptionBtn.Name = "OptionButton";
        OptionBtn.Size = btnRect;
        OptionBtn.onClick = OnOptionBtnClick;

        ButtonCtrl layoutBtn = new ButtonCtrl();
        layoutBtn.Caption = "操作区布局";
        layoutBtn.Name = "LayoutButton";
        layoutBtn.Size = btnRect;
        layoutBtn.onClick = OnLayoutBtnClick;

        ButtonCtrl helpBtn = new ButtonCtrl();
        helpBtn.Caption = "帮助";
        helpBtn.Size = btnRect;
        helpBtn.onClick = OnHelp;

        m_LoadingStr = new LabelCtrl();
        m_LoadingStr.Name = "LoadingStr";
        m_LoadingStr.Caption = "";
        m_LoadingStr.fontSize = 11;

        hb1.Add(projectBtn);
        hb1.Add(OperateBtn);
        hb1.Add(OptionBtn);
        hb1.Add(layoutBtn);
        hb1.Add(helpBtn);

        hb2.Add(vs2_1);

        hb3.Add(m_LoadingStr);

        hs2.Add(hb2);
        hs2.Add(hb3);

        hs1.Add(hb1);
        hs1.Add(hs2);
        #endregion

        #endregion

        m_EditorRoot.RootCtrl = hs1;
        m_EditorRoot.onGUI = OnEditorGUI;

        UIAdjustAtlasEditorModel.GetInstance().onLoadProject = onLoadProjectCommand;
        UIAdjustAtlasEditorModel.GetInstance().onRemoveProject = onRemoveProjectCommand;
        UIAdjustAtlasEditorModel.GetInstance().onAddSprite = onRemoveSpriteCommand;
        UIAdjustAtlasEditorModel.GetInstance().onRemoveSprite = onRemoveSpriteCommand;
        UIAdjustAtlasEditorModel.GetInstance().onZoomSprite = onZoomSpriteCommand;

        UIAdjustAtlasEditorModel.GetInstance().onMoveSpriteCommand = onMoveSpriteCommand;
        UIAdjustAtlasEditorModel.GetInstance().onCopySpriteCommand = onCopySpriteCommand;
        UIAdjustAtlasEditorModel.GetInstance().onRebuildModifyProject = onRebuildProjectCommand;
        UIAdjustAtlasEditorModel.GetInstance().onUpdateAssetsCommand = onUpdateAssetsCommand;

        UIAdjustAtlasEditorModel.GetInstance().onProjectLoadProgress = onProjectLoadProgressCommand;
        UIAdjustAtlasEditorModel.GetInstance().onCheckConsistency = onCheckConsistencyCommand;
        UIAdjustAtlasEditorModel.GetInstance().onExportConsistencyOnLoadProject = onExportConsistencyOnLoadProjectCommand;

        UIAdjustAtlasEditorModel.GetInstance().onExportReference = onExportDependency;

        AnalyseConsistencyProgresser.GetInstance().onUpdateProgress = onUpdateConsistencyProgress;
        AnalyseConsistencyProgresser.GetInstance().onInitProgress = onInitProgress;
        WriteFileProgresser.GetInstance().onUpdateProgress = onUpdateConsistencyProgress;
        WriteFileProgresser.GetInstance().onInitProgress = onInitProgress;
        AnnalyseReferenceProgresser.GetInstance().onUpdateProgress = onUpdateConsistencyProgress;
        AnnalyseReferenceProgresser.GetInstance().onInitProgress = onInitProgress;
        UpdateReferenceProgresser.GetInstance().onUpdateProgress = onUpdateConsistencyProgress;
        UpdateReferenceProgresser.GetInstance().onInitProgress = onInitProgress;
        //UIAdjustAtlasEditorModel.GetInstance().onRebuildProgress = onRebuildProgressCommand;
    }

    static void OnEnable(EditorRoot root)
    {
        string projectPath = string.Empty;
        string imageBasePath = string.Empty;
        string consistencyResultPath = string.Empty;
        string consistencyPrefabPath = string.Empty;
        string referencePrefabPath = string.Empty;
        string referenceScenePath = string.Empty;
        string referenceResultPath = string.Empty;

        InitUndoCommand();
        UIAdjustAtlasEditorModel.GetInstance().ReadProjectPathConfig(out projectPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadImageBasePathConfig(out imageBasePath);
        UIAdjustAtlasEditorModel.GetInstance().ReadConsistencyResultPathConfig(out consistencyResultPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadConsistencyPrefabPathConfig(out consistencyPrefabPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadReferencePrefabPathConfig(out referencePrefabPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadReferenceScenePathConfig(out referenceScenePath);
        UIAdjustAtlasEditorModel.GetInstance().ReadReferenceResultPathConfig(out referenceResultPath);
        CheckReadOnlyFile(); 

        Undo.undoRedoPerformed += OnUndoRedo;

    }

    static void OnDisable(EditorRoot root)
    {
        string projectPath = string.Empty;

        if (UIAdjustAtlasEditorModel.GetInstance().HaveModifyProject(out projectPath))
        {
            if(EditorUtility.DisplayDialog("警告！", "\n某些工程已变更，是否重新生成Atlas", "重新生成", "放弃变更"))
            {
                //string dispStr = "生成" + Path.GetFileNameWithoutExtension(projectPath);

                //EditorUtility.DisplayProgressBar("重新生成", dispStr, 0f);

                UIAdjustAtlasEditorModel.GetInstance().RebuildModifyProjects();
            }

        }

        UIAtlasTempTextureManager.DestroyInstance();
        UIAdjustAtlasEditorModel.DestoryInstance();
        EditorCommandManager.GetInstance().Clear();
        ClearAtlasPathInArea();

        Undo.undoRedoPerformed -= OnUndoRedo;
        if (m_ConfigWindow != null)
        {
            m_ConfigWindow.Close();
        }
    }

    static void OnUndoRedo()
    {//Undo/Redo 命令响应函数

        int commandCount = 0;

        if (m_CommandCounter == null)
        {
            return;
        }
        
        if (m_CommandCounter.IsRedo(out commandCount))
        {//当前操作是Redo

            for (int i = 0; i < commandCount; i++)
            {
                EditorCommandManager.GetInstance().PerformRedo();
            }
        }
        else
        {//当前操作是Undo

            for (int i = 0; i < commandCount; i++)
            {
                EditorCommandManager.GetInstance().PerformUndo();
            }
        }

        //更新前次命令计数器
        m_CommandCounter.PreCommandCounter = m_CommandCounter.CommandCounter;
    }

    static void OnCmdNotify(IEditorCommand cmd, int removeCmdCount)
    {
        do
        {
            AdjustAtlas_MoveSpriteCommand moveCommand = cmd as AdjustAtlas_MoveSpriteCommand;
            if (moveCommand != null)
            {
                if (removeCmdCount > 0)
                {
                    List<string> operateAtlasList = new List<string>();
                    foreach (var item in moveCommand.m_sourceInfoTable)
                    {
                        operateAtlasList.Add(item.SourceProjectPath);
                    }
                    operateAtlasList.Add(moveCommand.m_destProjectPath);

                    UIAdjustAtlasEditorModel.GetInstance().ClearSaveOperateCount(operateAtlasList.ToArray());
                    break;
                }
            }

            AdjustAtlas_CopySpriteCommand copyCommand = cmd as AdjustAtlas_CopySpriteCommand;
            if (copyCommand != null)
            {
                if (removeCmdCount > 0)
                {
                    List<string> operateAtlasList = new List<string>();
                    operateAtlasList.Add(copyCommand.m_destProjectPath);

                    UIAdjustAtlasEditorModel.GetInstance().ClearSaveOperateCount(operateAtlasList.ToArray());
                    break;
                }
            }

            AdjustAtlas_RemoveSpriteCommand removeSpriteCommand = cmd as AdjustAtlas_RemoveSpriteCommand;
            if (removeSpriteCommand != null)
            {
                if (removeCmdCount > 0)
                {
                    List<string> operateAtlasList = new List<string>();
                    foreach (var item in removeSpriteCommand.m_sourceInfoTable)
                    {
                        operateAtlasList.Add(item.SourceProjectPath);
                    }

                    UIAdjustAtlasEditorModel.GetInstance().ClearSaveOperateCount(operateAtlasList.ToArray());
                    break;
                }
            }

            AdjustAtlas_ImageZoomCommand imageZoomCommand = cmd as AdjustAtlas_ImageZoomCommand;
            if (imageZoomCommand != null)
            {
                if (removeCmdCount > 0)
                {
                    List<string> operateAtlasList = new List<string>();
                    operateAtlasList.Add(imageZoomCommand.m_ProjectPath);

                    UIAdjustAtlasEditorModel.GetInstance().ClearSaveOperateCount(operateAtlasList.ToArray());
                    break;
                }
            }

        } while (false);
        
    }

    static void OnEditorGUI(EditorRoot root)
    {
        if (
            (Event.current.type == EventType.MouseDrag)
              || (Event.current.type == EventType.ScrollWheel)
              || (Event.current.type == EventType.MouseDown)
              || (Event.current.type == EventType.MouseUp)
            )
        {
            RequestRepaint();
        }

        if (m_IsNeedReloadTexture)
        {
            m_IsNeedReloadTexture = false;
            UIAdjustAtlasEditorModel.GetInstance().ReloadProjectTexture();
            UpdateAllAtlasList();
            UpdateAllOperateArea();
        }
    }

    static void OnProjectBtnClick(EditorControl c)
    {
        GenericMenu menu = new GenericMenu();
       
        //Project下拉菜单
        menu.AddItem(new GUIContent("载入Atlas工程"), false, procLoadAtlasProject, "item 1");
        menu.AddItem(new GUIContent("检查工程一致性"), false, procCheckProjectConsistency, "item 2");
        menu.AddItem(new GUIContent("导出引用关系/导出正向引用"), false, procExportDependency, "item 3");
        menu.AddItem(new GUIContent("导出引用关系/导出反向引用"), false, procExportReverseDependency, "item 4");
        menu.AddItem(new GUIContent("导出引用关系/导出无引用"), false, procExportNoneDependency, "item 5");
        menu.AddItem(new GUIContent("导出引用关系/导出全部引用关系"), false, procExportAllDependency, "item 6");
        menu.AddItem(new GUIContent("导出引用关系/配置过滤器"), false, procConfigDependencyFilter, "item 7");
        //menu.AddItem(new GUIContent("载入配置路径全部"), false, null, "item 2");

        menu.DropDown(c.LastRect);
    }

    static void procLoadAtlasProject(object command)
    {
        if (IsCommonConfigError())
        {
            return;
        }

        OpenFileDialog openfiledialog = new OpenFileDialog();
        InitializeOpenFileDialog(openfiledialog);
        bool isOpen = false;
        
        //显示添加小图对话框
        if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            isOpen = true;
        }

        if (isOpen)
        {
            isOpen = false;

            for (int fi = 0; fi < openfiledialog.FileNames.Length; fi++)
            {
                //执行添加小图命令
                AtlasLoadProjectCommand cmd = new AtlasLoadProjectCommand();
                cmd.m_ProjectPath = openfiledialog.FileNames[fi].ToString();
                EditorCommandManager.GetInstance().Add(cmd);

                RegisterUndo("Load AtlasProject");
            }

            UIAdjustAtlasEditorModel.GetInstance().ExportConsistencyInfoOnLoadProject();
        }
    }

    static void procCheckProjectConsistency(object command)
    {
        UIAdjustAtlasEditorModel.GetInstance().CheckAtlasConsistency();
    }

    static void procExportDependency(object command)
    {
        RequestRepaint();

        UIAdjustAtlasEditorModel.GetInstance().ExportDependency();
    }

    static void procExportReverseDependency(object command)
    {  
        UIAdjustAtlasEditorModel.GetInstance().ExportReverseDependency();
    }

    static void procExportNoneDependency(object command)
    {
        UIAdjustAtlasEditorModel.GetInstance().ExportNoneDependency();
    }

    static void procExportAllDependency(object command)
    {
        UIAdjustAtlasEditorModel.GetInstance().ExportAllDependency();
    }

    static void procConfigDependencyFilter(object command)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIAdjustAtlasEditorModel.GetInstance().ConfigDependencyFilter();

        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_CONFIGFILE_PATHERROR:
                EditorUtility.DisplayDialog("操作失败", "\n配置文件路径错误，请确认！", "确认");
                break;

            default:
                break;
        }
    }

    static private void onLoadProjectCommand(string projectPath, string atlasOutputPath, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        EditorUtility.ClearProgressBar();

        if (string.IsNullOrEmpty(projectPath))
        {
            return;
        }

        string projectName = Path.GetFileNameWithoutExtension(projectPath);
        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR:
                AddProjectItem(projectPath);
                ClearInspectorView();
                break;
                
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH:
                EditorUtility.DisplayDialog("载入失败！", "\n请确认Project工程路径并载入该路径下的工程文件", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_LOADSPRITE_TEXTUREERROR:
                EditorUtility.DisplayDialog("载入失败！", "\nSprite加载失败，请确认图库路径和Sprite源文件是否正确", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ADD_SAMEPROJECT:
                EditorUtility.DisplayDialog("载入失败！", "\n该工程已存在、不能重复载入", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_LOADPROJECT_CORRELATIONFILE_NOEXIST:
                EditorUtility.DisplayDialog("载入失败！", "\n" + projectName + "相关的prefab、png、mat文件已缺失！" + "请在" + atlasOutputPath +"中进行确认", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR:
                EditorUtility.DisplayDialog("载入失败！", "\nSprite图库路径错误，请确认", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR:
                EditorUtility.DisplayDialog("载入失败！", "\nAtlas指定Prefab路径错误，请确认", "确认");
                break;

            default:
                break;
        }
    }

    static private void onExportConsistencyOnLoadProjectCommand(ANALYSERESULT_TYPE analyseType, string resultPath)
    {
        EditorUtility.ClearProgressBar();

        switch (analyseType)
        {
            case ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING:
                if (EditorUtility.DisplayDialog("警告", "\n某些工程存在一致性警告信息，请确认", "查看详细信息", "确认"))
                {
                    UIAdjustAtlasEditorModel.GetInstance().OpenExportFile(resultPath);
                }  
                break;

            case ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT:
                if (EditorUtility.DisplayDialog("操作失败", "\n存在不一致工程，这些工程无法加载，请确认", "查看详细信息", "确认"))
                {
                    UIAdjustAtlasEditorModel.GetInstance().OpenExportFile(resultPath);
                }
                break;

            default:
                break;
        }
    }

    static private void onExportDependency(string resultDir, string consistencyPath, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        EditorUtility.ClearProgressBar();

        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                if (EditorUtility.DisplayDialog("导出完成", "\n导出成功，请确认", "打开文件夹", "确认"))
                {
                    resultDir = resultDir.Replace('/', '\\');
                    //"explorer.exe", 
                    System.Diagnostics.Process.Start(@resultDir);
                }
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_WARNING_REFERENCEINFO_NONE:
                EditorUtility.DisplayDialog("导出完成", "\n引用关系为空，不生成任何引用关系文件", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATE_WITH_NONEPROJECT:
                EditorUtility.DisplayDialog("导出完成", "\n不存在工程文件，请确认", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_WITH_UNCONSISTPROJECT_ERROR:
                if (EditorUtility.DisplayDialog("导出失败", "\nProject目录下存在不一致工程,无法导出引用关系，请确认", "查看详细信息", "确认"))
                {
                    UIAdjustAtlasEditorModel.GetInstance().OpenExportFile(consistencyPath);
                }
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR:
                EditorUtility.DisplayDialog("导出失败", "\n关联Prefab路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR:
                EditorUtility.DisplayDialog("导出失败", "\n关联Scene路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_RESULTPATH_ERROR:
                EditorUtility.DisplayDialog("导出失败", "\n导出结果保存目录错误，请确认！", "确认");
                break;
             
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTDIC_ERROR:
                EditorUtility.DisplayDialog("导出失败", "\nProject路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR:
                EditorUtility.DisplayDialog("导出失败", "\n一致性检查结果输出路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR:
                EditorUtility.DisplayDialog("导出失败", "\nSprite图库路径错误，请确认！", "确认");
                break;

            default:
                break;
        }

        RequestRepaint();
    }

    static private void onCheckConsistencyCommand(ANALYSERESULT_TYPE analyseType, string resultPath, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        EditorUtility.ClearProgressBar();

        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                if (analyseType == ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT)
                {
                    EditorUtility.DisplayDialog("检查完毕", "\n工程全部一致！", "确认");
                }
                else if (analyseType == ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING)
                {
                    if(EditorUtility.DisplayDialog("检查完毕", "\n工程全部一致，但存在警告信息，请确认", "查看详细信息",  "确认"))
                    {
                        UIAdjustAtlasEditorModel.GetInstance().OpenExportFile(resultPath);
                    }          
                }
                else
                {
                    if (EditorUtility.DisplayDialog("检查完毕", "\n存在不一致工程，请确认", "查看详细信息", "确认"))
                    {
                        UIAdjustAtlasEditorModel.GetInstance().OpenExportFile(resultPath);
                    }
                }

                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR:
                EditorUtility.DisplayDialog("检查失败", "\n目标Prefab路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTDIC_ERROR:
                EditorUtility.DisplayDialog("检查失败", "\nProject目录错误，请重新配置！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR:
                EditorUtility.DisplayDialog("检查失败", "\nSprite图库路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR:
                EditorUtility.DisplayDialog("检查失败", "\n检查结果保存路径错误，请确认！", "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATE_WITH_NONEPROJECT:
                EditorUtility.DisplayDialog("检查完毕", "\n不存在工程文件，请确认", "确认");
                break;

            default:
                break;
        }

        RequestRepaint();
    }

    static private void onUpdateConsistencyProgress(float currentProgress, string dispStr)
    {
        EditorUtility.DisplayProgressBar(dispStr, dispStr, currentProgress);
    }

    static private void onUpdateReferenceProgress(float currentProgress, string dispStr)
    {
        EditorUtility.DisplayProgressBar("Atlas引用关系导出", dispStr, currentProgress);
    }

    static private void onInitProgress()
    {
        EditorUtility.ClearProgressBar();
    }

    static private void onProjectLoadProgressCommand(UIAdjust_ProjectLoadProgress progressInfo)
    {
        if (0 == progressInfo.m_Total)
        {
            return;
        }

        float progress = ((float)progressInfo.m_CurrentSpriteCount) / ((float)progressInfo.m_Total);

        EditorUtility.DisplayProgressBar("工程加载中", progressInfo.m_DispMsg, progress);

    }

    static private void onRebuildProgressCommand(UIAdjust_RebuildProgress progressInfo)
    {
        if (0 == progressInfo.m_Total)
        {
            return;
        }

        float progress = ((float)progressInfo.m_CurrentAtlasCount) / ((float)progressInfo.m_Total);

        EditorUtility.DisplayProgressBar("重新生成", progressInfo.m_DispMsg, progress);
    }

    static private void onRemoveProjectCommand(string projectPath, UIADJUSTATLAS_ERROR_TYPE erroType)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            return;
        }
        string projectName = Path.GetFileNameWithoutExtension(projectPath);

        switch (erroType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                RemoveProjectItem(projectPath);
                ClearOperateArea(projectPath);
                ClearInspectorView();
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REMOVEPROJECT_MODIFY:
                if (EditorUtility.DisplayDialog("警告！", "\n" + projectName + "工程已变更，是否重新生成Atlas", "重新生成", "放弃变更"))
                {
                    UIAdjustAtlasEditorModel.GetInstance().RebuildModifyProjects();
                }
                else
                {
                    UIAdjustAtlasEditorModel.GetInstance().InitAllOperateCount(projectPath);
                }

                UIAdjustAtlasEditorModel.GetInstance().RemoveAtlasProject(projectPath);
                break;

            default:
                break;
        }
    }

    static private void onRemoveSpriteCommand(List<UIAdjust_SpriteOperateInfo> sourceInfo, UIADJUSTATLAS_ERROR_TYPE errorType) 
    {
        if(null == sourceInfo)
        {
            return;
        }

        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                UpdateScrrenAfterRemoveSprite(sourceInfo);
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REMOVESPRITE_LEAVENOSPRITE:
                EditorUtility.DisplayDialog("删除Sprite失败！", "\nAtlas中不能少于1个Sprite", "确认");
                break;

            default:
                break;
        }
    }

    static private void onZoomSpriteCommand(string projectPath, string spritePath, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        if(
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return;
        }

        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                UpdateScrrenAfterZoomSprite(projectPath, spritePath);
                break;

            default:
                break;
        }
    }

    static private void onMoveSpriteCommand(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        if(
               (null == sourceInfo)
            || (string.IsNullOrEmpty(destProjectPath))
            )
        {
            return;
        }

        switch(errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                UpdateScreenAfterMoveSprite(sourceInfo, destProjectPath);
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REMOVESPRITE_LEAVENOSPRITE:
                EditorUtility.DisplayDialog("移动Sprite失败！", "\n移动后的源Atlas中不能少于1个Sprite!", "确认");
                RequestRepaint();
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_MOVESPRITE_ALREADYEXIST:
                EditorUtility.DisplayDialog("移动Sprite失败！", "\n被移动的Sprite中,某些已存在与目标Atlas工程!", "确认");
                RequestRepaint();
                break;

            default:
                break;
        }
    }

    static private void onCopySpriteCommand(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        if (
               (null == sourceInfo)
            || (string.IsNullOrEmpty(destProjectPath))
            )
        {
            return;
        }

        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                UpdateScreenAfterCopySprite(destProjectPath);
                break;
    
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_MOVESPRITE_ALREADYEXIST:
                EditorUtility.DisplayDialog("复制Sprite失败！", "\n被复制的Sprite中,某些已存在与目标Atlas工程!", "确认");
                RequestRepaint();
                break;

            default:
                break;
        }
    }

    static private void onRebuildProjectCommand(string[] projectPaths, bool isMoidfyRefFile, UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        EditorUtility.ClearProgressBar();
 
        if (null == projectPaths)
        {
            return;
        }

        string dispStr = string.Empty;

        switch(errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                if (isMoidfyRefFile)
                {
                    dispStr = "\n相关的prefab、scene文件已修改";
                    m_IsNeedReloadTexture = true;
                }
                else
                {
                    dispStr = "\n本次生成未修改任何prefab、scene文件";
                }
                UpdateScreenAfterRebuild(projectPaths);
                EditorUtility.DisplayDialog("生成成功",
                                             dispStr,
                                             "确认");

                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REBUILD_WITHNONEMODIFY:
                EditorUtility.DisplayDialog("生成结束",
                                             "没有变更的工程，不需要重新生成Atlas！",
                                             "确认");
                break;
            default:
                break;
        }
    }

    static private void onUpdateAssetsCommand(UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        switch (errorType)
        {
            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE:
                ClearInspectorView();
                EditorUtility.DisplayDialog("刷新成功",
                                             "\nAsset刷新完成",
                                             "确认");
                break;

            case UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UPDATEASSETS_WITHMODIFY:
                EditorUtility.DisplayDialog("刷新失败",
                                             "工程已经变更，请重新生成Atlas后再刷新资源！",
                                             "确认");
                break;
            default:
                break;
        }
    }

    static private void UpdateScreenAfterRebuild(string[] projectPaths)
    {
        for(int index = 0; index < m_OperateAreaNum; index++)
        {
            UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
        }

        UpdateAtlasList(projectPaths);

        ClearInspectorView();

        RequestRepaint();
    }

    static private void UpdateScrrenAfterRemoveSprite(List<UIAdjust_SpriteOperateInfo> sourceInfo)
    {
        if (
               (null == sourceInfo)
            || (null == m_inspector)
            )
        {
            return;
        }


        List<string> operateAtlasList = new List<string>();

        foreach (var sourceItem in sourceInfo)
        {
            for (int index = 0; index < m_OperateAreaNum; index++)
            {
                if (m_AtlasPathInSpriteView[index] == sourceItem.SourceProjectPath)
                {
                    UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
                    break;
                }
            }

            operateAtlasList.Add(sourceItem.SourceProjectPath);
        }

        UpdateAtlasList(operateAtlasList.ToArray());

        ClearInspectorView();

        RequestRepaint();
    }

    static private void UpdateScrrenAfterZoomSprite(string projectPath, string spritePath)
    {
        if (
               (string.IsNullOrEmpty(projectPath))
            || (string.IsNullOrEmpty(spritePath))
            || (null == m_inspector)
            )
        {
            return;
        }

        List<string> operateAtlasList = new List<string>();
        Texture2D tex = null;
        for (int index = 0; index < m_OperateAreaNum; index++)
        {
            if (m_AtlasPathInSpriteView[index] == projectPath)
            {
                UpdateOperateAreaTextureView(m_AtlasPathInSpriteView[index], index);
                UIAdjustAtlasEditorModel.GetInstance().GetSpriteImageZoomTexture2D(spritePath, projectPath, out tex);

                m_OperateArea[index].SpriteList.Items[m_OperateArea[index].SpriteList.LastSelectItem].image = tex;
                m_SpriteWidth = tex.width.ToString();
                m_SpriteHeight = tex.height.ToString();
                operateAtlasList.Add(projectPath);
                break;
            }
        }

        UpdateAtlasList(operateAtlasList.ToArray());

        //ClearInspectorView();

        RequestRepaint();
    }

    static private void UpdateScreenAfterMoveSprite(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath)
    {
        if (
               (null == sourceInfo)
            || (string.IsNullOrEmpty(destProjectPath))
            || (null == m_inspector)
            )
        {
            return;
        }

        List<string> operateAtlasList = new List<string>();

        foreach (var sourceItem in sourceInfo)
        {
            for (int index = 0; index < m_OperateAreaNum; index++)
            {
                if (m_AtlasPathInSpriteView[index] == sourceItem.SourceProjectPath)
                {
                    UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
                    break;
                }
            }

            operateAtlasList.Add(sourceItem.SourceProjectPath);
        }

        for (int index = 0; index < m_OperateAreaNum; index++)
        {
            if (m_AtlasPathInSpriteView[index] == destProjectPath)
            {
                UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
                break;
            }
        }

        operateAtlasList.Add(destProjectPath);

        UpdateAtlasList(operateAtlasList.ToArray());

        ClearInspectorView();

        RequestRepaint();
    }

    static private void UpdateScreenAfterCopySprite(string destProjectPath)
    {
        if(string.IsNullOrEmpty(destProjectPath))
        {
            return;
        }
        List<string> operateAtlasList = new List<string>();

        for (int index = 0; index < m_OperateAreaNum; index++)
        {
            if (m_AtlasPathInSpriteView[index] == destProjectPath)
            {
                UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
                operateAtlasList.Add(destProjectPath);
                break;
            }
        }


        UpdateAtlasList(operateAtlasList.ToArray());

        ClearInspectorView();

        RequestRepaint();
    }

    static private void onAtlasListSelect(EditorControl c, int index)
    {
        ListViewCtrl atlasList = c as ListViewCtrl;
        if (atlasList != m_AtlasList)
        {
            return;
        }
      
        ClearInspectorView();
        if (m_AtlasList.LastSelectItem != -1)
        {
            UIAdjustAtlasEditorModel.GetInstance().GetAtlasOutputPath(m_AtlasList.Items[index].tooltip, out m_AtlasOutputPath);
            m_inspector.onInspector = InitAtlasInspectorView;
        }
        else
        {
            m_inspector.onInspector = null;
        }

        RequestRepaint();

    }

    static private void onAtlasListSelectRU(EditorControl c, int index) 
    {
        ListViewCtrl list = c as ListViewCtrl;

        if(null == list)
        {
            return;
        }

        string viewStr = string.Empty;
        if (list.IsTextureView)
        {
            viewStr = "至List视图";
        }
        else
        {
            viewStr = "至纹理视图";
        }

        GenericMenu menu = new GenericMenu();
        //弹出删除小图下拉菜单
        menu.AddItem(new GUIContent("移除工程"), false, procRemoveAtlasProject, "item 1");
        menu.AddItem(new GUIContent("切换" + viewStr), false, procChangeListView, list);


        menu.ShowAsContext();

        RequestRepaint();
    }

    static void procRemoveAtlasProject(object command)
    {
        if(null == m_AtlasList)
        {
            return;
        }


        bool isModify = false;

        foreach (var index in m_AtlasList.SelectItems)
        {
            UIAdjustAtlasEditorModel.GetInstance().IsProjectModify(m_AtlasList.Items[index].tooltip, out isModify);

            if (isModify)
            {
                if (EditorUtility.DisplayDialog("警告！", "\n被删除工程已变更，是否重新生成Atlas", "重新生成", "放弃变更"))
                {
                    //string dispStr = "生成" + Path.GetFileNameWithoutExtension(m_AtlasList.Items[index].tooltip);

                    //EditorUtility.DisplayProgressBar("重新生成", dispStr, 0f);


                    UIAdjustAtlasEditorModel.GetInstance().RebuildModifyProjects();
                }

                break;
            }
        }

        List<string> deleteProjectList = new List<string>();
        foreach(var item in m_AtlasList.SelectItems)
        {
            string deleteProjectPath = m_AtlasList.Items[item].tooltip;
            deleteProjectList.Add(deleteProjectPath);
        }


        foreach (var projectPath in deleteProjectList)
        {
            AtlasRemoveProjectCommand cmd = new AtlasRemoveProjectCommand();
            cmd.m_ProjectPath = projectPath;
            EditorCommandManager.GetInstance().Add(cmd);

            RegisterUndo("Remove AtlasProject");
        }

        RequestRepaint();
    }


    static void OnOperateBtnClick(EditorControl c)
    {
        GenericMenu menu = new GenericMenu();

        //Operate下拉菜单
        menu.AddItem(new GUIContent("生成Atlas"), false, procRebuildAtlas, "item 1");
        menu.AddItem(new GUIContent("清空操作区/清空操作区1"), false, procClearOperateArea, 0);
        menu.AddItem(new GUIContent("清空操作区/清空操作区2"), false, procClearOperateArea, 1);
        menu.AddItem(new GUIContent("清空操作区/清空操作区3"), false, procClearOperateArea, 2);
        menu.AddItem(new GUIContent("清空操作区/清空操作区4"), false, procClearOperateArea, 3);
        menu.AddItem(new GUIContent("清空操作区/清空全部操作区"), false, procClearOperateArea, 4);
        menu.AddItem(new GUIContent("刷新Assets"), false, procUpdateAssets, "item 2");

        //menu.AddItem(new GUIContent("刷新Assets"), false, null, "item 2");

        menu.DropDown(c.LastRect);
    }

    static void procRebuildAtlas(object command)
    {
        //EditorUtility.DisplayProgressBar("重新生成", "", 0f);

        UIAdjustAtlasEditorModel.GetInstance().RebuildModifyProjects();
    }

    static void procClearOperateArea(object command)
    {
        int areaIndex = (int)command;
        if(
               (areaIndex < 0)
            || (areaIndex > m_OperateAreaNum)
            )
        {
            return;
        }

        switch(areaIndex)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                AdjustAtlas_ClearOperateArea clearCmd = new AdjustAtlas_ClearOperateArea();
                clearCmd.m_ProjectPath = m_AtlasPathInSpriteView[areaIndex];
                clearCmd.m_AreaIndex = areaIndex;
                EditorCommandManager.GetInstance().Add(clearCmd);
            
                RegisterUndo("Clear OperateArea");

                break;

            case 4:
                AdjustAtlas_ClearAllOperateArea clearAllcmd = new AdjustAtlas_ClearAllOperateArea();
                for (int i = 0; i < m_OperateAreaNum; i++)
                {
                    clearAllcmd.m_ProjectPaths[i] = m_AtlasPathInSpriteView[i];
                }

                EditorCommandManager.GetInstance().Add(clearAllcmd);
                RegisterUndo("Clear All OperateArea");
                break;

            default:
                break;

        }


    }

    static void procUpdateAssets(object command)
    {
        UIAdjustAtlasEditorModel.GetInstance().UpdateAssets();
    }

    static void OnOptionBtnClick(EditorControl c)
    {
        GenericMenu menu = new GenericMenu();

        //Option下拉菜单
        menu.AddItem(new GUIContent("配置"), false, procAdjustAtlasConfig, "item 1");

        menu.DropDown(c.LastRect);
    }

    static void procAdjustAtlasConfig(object command)
    {
        m_ConfigWindow = EditorWindow.GetWindow<UIAdjustAtlasConfig>(true, "配置", true);
    }

    static void OnLayoutBtnClick(EditorControl c)
    {
        GenericMenu menu = new GenericMenu();

        //Layout下拉菜单
        menu.AddItem(new GUIContent("x2"), false, procChangeView, 2);
        menu.AddItem(new GUIContent("x3"), false, procChangeView, 3);
        menu.AddItem(new GUIContent("x4"), false, procChangeView, 4);

        menu.DropDown(c.LastRect);
    }

    static void procChangeView(object command)
    {
        int viewNum = (int)command;
        if (
               viewNum <= 0
            || viewNum > 4
            )
        {
            return;
        }

        switch (viewNum)
        {
            case 2:
                InitWndBy2View();
                break;
            case 3:
                InitWndBy3View();
                break;
            case 4:
                InitWndBy4View();
                break;
            default:
                break;
        }

        for (int index = 0; index < viewNum; index++)
        {
            if (string.IsNullOrEmpty(m_AtlasPathInSpriteView[index]))
            {
                continue;
            }

            UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
        }
    }

    static void OnHelp(EditorControl c)
    {
        GenericMenu menu = new GenericMenu();

        //Layout下拉菜单
        menu.AddItem(new GUIContent("查看帮助"), false, procHelp, 0);

        menu.DropDown(c.LastRect);

    }

    static void procHelp(object command)
    {
        System.Diagnostics.Process.Start(m_helpURL);   
    }


    static private object onAtlasListPrepareDrag(EditorControl c)
    {
        ListViewCtrl list = c as ListViewCtrl;

        if((null == list) || (null == m_AtlasList)
            || (list.Name != m_AtlasList.Name))
        {
            return null;
        }

        List<string> selectAtlasList = new List<string>();

        foreach (var item in m_AtlasList.SelectItems)
        {
            selectAtlasList.Add(m_AtlasList.Items[item].tooltip);
        }

        return selectAtlasList;
    }


    static private void OnSelectSpriteListItem(EditorControl c, int index)
    {
        ListViewCtrl spriteList = c as ListViewCtrl;
        if (null == spriteList)
        {
            return;
        }

        int areaIndex = GetSpriteListIndex(spriteList);
        if (areaIndex < 0)
        {
            return;
        }

        ClearInspectorView();

        if(m_OperateArea[areaIndex].SpriteList.LastSelectItem != -1)
        {
            m_CurrentSelectIndex = areaIndex;
            if (null != m_OperateArea[areaIndex].SpriteList.Items[m_OperateArea[areaIndex].SpriteList.LastSelectItem].image)
            {
                m_SpriteWidth = m_OperateArea[areaIndex].SpriteList.Items[m_OperateArea[areaIndex].SpriteList.LastSelectItem].image.width.ToString();
                m_SpriteHeight = m_OperateArea[areaIndex].SpriteList.Items[m_OperateArea[areaIndex].SpriteList.LastSelectItem].image.height.ToString();               
            }

            m_inspector.onInspector = InitSpirteInspectorView;
        }
        else
        {
            m_CurrentSelectIndex = -1;
            m_inspector.onInspector = null;
        }

        //非ctrl鼠标左键压下
        if(!c.frameTriggerInfo.isCtrlSelectItem)
        {
            for (int i = 0; i < m_OperateAreaNum; i++)
            {
                if (i == areaIndex)
                {
                    continue;
                }

                if (null == m_OperateArea[i].SpriteList)
                {
                    continue;
                }

                m_OperateArea[i].SpriteList.ClearSelectItems();
            }
        }

        RequestRepaint();
    }

    static private void OnSelectSpriteListItemRU(EditorControl c, int index)
    {
        ListViewCtrl spriteList = c as ListViewCtrl;
        if (null == spriteList)
        {
            return;
        }

        int areaIndex = GetSpriteListIndex(spriteList);
        if (areaIndex < 0)
        {
            return;
        }

        string viewStr = string.Empty;
        if(spriteList.IsTextureView)
        {
            viewStr = "至List视图";
        }
        else
        {
            viewStr = "至纹理视图";
        }
        GenericMenu menu = new GenericMenu();
        //弹出删除小图下拉菜单
        menu.AddItem(new GUIContent("删除Sprite"), false, procRemoveSprite, "item 1");
        menu.AddItem(new GUIContent("切换" + viewStr), false, procChangeListView, spriteList);

        menu.ShowAsContext();

        RequestRepaint();
    }

    static private void procRemoveSprite(object command)
    {
        List<UIAdjust_SpriteOperateInfo> operateInfoList = new List<UIAdjust_SpriteOperateInfo>();

        for (int index = 0; index < m_OperateAreaNum; index++)
        {
            if(null == m_OperateArea[index].SpriteList)
            {
                continue;
            }

            if(m_OperateArea[index].SpriteList.SelectItems.Count > 0)
            {
                UIAdjust_SpriteOperateInfo newInfo = new UIAdjust_SpriteOperateInfo();
                newInfo.SourceProjectPath = m_AtlasPathInSpriteView[index];
                foreach (var item in m_OperateArea[index].SpriteList.SelectItems)
                {
                    newInfo.OperateSpriteTable.Add(m_OperateArea[index].SpriteList.Items[item].tooltip);
                }

                operateInfoList.Add(newInfo);
            }
        }

        AdjustAtlas_RemoveSpriteCommand cmd = new AdjustAtlas_RemoveSpriteCommand();
        cmd.m_sourceInfoTable = operateInfoList;
        EditorCommandManager.GetInstance().Add(cmd);

        RegisterUndo("Remove Sprites"); 
    }

    static private void procChangeListView(object command)
    {
        ListViewCtrl list = command as ListViewCtrl;
        if (null == list)
        {
            return;
        }

        list.IsTextureView = !list.IsTextureView;

        RequestRepaint();
    }

    static private object onSpriteListPrepareDrag(EditorControl c)
    {
        ListViewCtrl list = c as ListViewCtrl;
        if(null == list)
        {
            return null;
        }

        int index = CheckOperateAreaIndex(c);
        if(index < 0)
        {
            return null;
        }

        List<UIAdjust_SpriteOperateInfo> spriteOperateInfo = new List<UIAdjust_SpriteOperateInfo>();

        for(int i = 0; i < m_OperateAreaNum; i++)
        {
            if(null == m_OperateArea[i].SpriteList)
            {
                continue;
            }

            if(m_OperateArea[i].SpriteList.SelectItems.Count != 0)
            {
                UIAdjust_SpriteOperateInfo newInfo = new UIAdjust_SpriteOperateInfo();

                newInfo.SourceProjectPath = m_AtlasPathInSpriteView[i];
                foreach(var listItem in m_OperateArea[i].SpriteList.SelectItems)
                {
                    newInfo.OperateSpriteTable.Add(m_OperateArea[i].SpriteList.Items[listItem].tooltip);
                }

                spriteOperateInfo.Add(newInfo);
            }
        }

        return spriteOperateInfo;
    }

    static private bool onSpriteListTryAcceptDrag(EditorControl c, object dragObject)
    {
        bool bRet = false;
        int areaIndex = -1;

        do{
            areaIndex = CheckOperateAreaIndex(c);

            List<string> selectAtlasList = dragObject as List<string>;
            if (selectAtlasList != null)
            {
                bRet = true;
                break;
            }

            List<UIAdjust_SpriteOperateInfo> spriteOperateInfoList = dragObject as List<UIAdjust_SpriteOperateInfo>;
            if (spriteOperateInfoList != null)
            {
                if(!string.IsNullOrEmpty(m_AtlasPathInSpriteView[areaIndex]))
                {
                    bRet = true;
                }

                break;
            }

        }while(false);


        return bRet;
    }

    static private void onSpriteListAcceptDrag(EditorControl c, object dragObject)
    {
        int areaIndex = -1;
        //DRAG_TYPE dragType = DRAG_TYPE.DRAG_TYPE_DEFAULT;

        areaIndex = CheckOperateAreaIndex(c);
        if ((areaIndex < 0) || (areaIndex >= m_OperateAreaNum))
        {
            return;
        }

        DragAcceptOperate(dragObject, areaIndex, false);
 
    }

    static private void onSpriteListAcceptDragCtrl(EditorControl c, object dragObject)
    {
        int areaIndex = -1;
        //DRAG_TYPE dragType = DRAG_TYPE.DRAG_TYPE_DEFAULT;

        areaIndex = CheckOperateAreaIndex(c);
        if ((areaIndex < 0) || (areaIndex >= m_OperateAreaNum))
        {
            return;
        }

        DragAcceptOperate(dragObject, areaIndex, true);
    }

    static private void AcceptAtlasObject(string projectPath, int areaIndex)
    {
        for (int index = 0; index < m_OperateAreaNum; index++)
        {
            if (projectPath == m_AtlasPathInSpriteView[index])
            {
                EditorUtility.DisplayDialog("操作失败！", "\n抱歉，该工程已存在于操作区域中，不能重复添加", "确认");
                return;
            }
        }

        AdjustAtlas_AddToOperateArea cmd = new AdjustAtlas_AddToOperateArea();

        cmd.m_ProjectPath = projectPath;
        cmd.m_AreaIndex = areaIndex;
        EditorCommandManager.GetInstance().Add(cmd);

        RegisterUndo("Update OperateArea");

        //UpdateOperateArea(projectPath, areaIndex);
    }

    static private void AcceptSpriteOject(List<UIAdjust_SpriteOperateInfo> operateInfoTable, int areaIndex, bool isCtrl)
    {
        if (
               (null == operateInfoTable)
            || ((areaIndex < 0) || (areaIndex >= m_OperateAreaNum))
            )
        {
            return;
        }

        if(!isCtrl)
        {
            AdjustAtlas_MoveSpriteCommand cmd = new AdjustAtlas_MoveSpriteCommand();

            cmd.m_sourceInfoTable = operateInfoTable;
            cmd.m_destProjectPath = m_AtlasPathInSpriteView[areaIndex];
            EditorCommandManager.GetInstance().Add(cmd);
            EditorCommandManager.GetInstance().onBeforeCmdExecute = OnCmdNotify;
            RegisterUndo("Move Sprites");
        }
        else
        {
            AdjustAtlas_CopySpriteCommand cmd = new AdjustAtlas_CopySpriteCommand();

            cmd.m_sourceInfoTable = operateInfoTable;
            cmd.m_destProjectPath = m_AtlasPathInSpriteView[areaIndex];
            EditorCommandManager.GetInstance().Add(cmd);

            RegisterUndo("Copy Sprites");
        }

    }

    static private void UpdateAtlasList(string[] projectPath)
    {
        if (
               (null == projectPath)
            || (null == m_AtlasList)
            )
        {
            return;
        }

        bool isModify = false;

        foreach(var listItem in m_AtlasList.Items)
        {
            foreach (var item in projectPath)
            {
                if (item == listItem.tooltip)
                {
                    UIAdjustAtlasEditorModel.GetInstance().IsProjectModify(item, out isModify);
                    if (isModify)
                    {
                        if (listItem.name == Path.GetFileNameWithoutExtension(item))
                        {
                            listItem.name = listItem.name + "*";              
                        }
                    }
                    else
                    {
                        if (
                               (listItem.name != Path.GetFileNameWithoutExtension(item))
                            && (listItem.name.EndsWith("*"))
                            )
                        {
                            listItem.name = listItem.name.TrimEnd('*');
                        }

                    }

                    listItem.image = UIAdjustAtlasEditorModel.GetInstance().GetAtlasTexture(item);
                    break;
                }
            }
        }

        RequestRepaint();
    }

    static private void UpdateAllAtlasList()
    {
        if(null == m_AtlasList)
        {
            return;
        }

        List<string> allProjectPath = new List<string>();

        foreach(var item in m_AtlasList.Items)
        {
            allProjectPath.Add(item.tooltip);
        }

        UpdateAtlasList(allProjectPath.ToArray());
    }

    static public void UpdateOperateArea(string projectPath, int areaIndex)
    {
        if (
               string.IsNullOrEmpty(projectPath)
            || ((areaIndex < 0) || (areaIndex >= m_OperateAreaNum))
            )
        {
            return;
        }

        ClearOperateArea(areaIndex);

        AddSpriteItem(projectPath, m_OperateArea[areaIndex].SpriteList);

        m_OperateArea[areaIndex].AtlasPath = projectPath;

        m_AtlasPathInSpriteView[areaIndex] = projectPath;

        UpdateOperateAreaTextureView(projectPath, areaIndex);

        RequestRepaint();
    }

    static public void UpdateAllOperateArea()
    {
        for (int index = 0; index < m_OperateAreaNum; index++)
        {
            UpdateOperateArea(m_AtlasPathInSpriteView[index], index);
        }
    }

    static private void UpdateOperateAreaTextureView(string projectPath, int areaIndex)
    {
        if (
               string.IsNullOrEmpty(projectPath)
            || ((areaIndex < 0) || (areaIndex >= m_OperateAreaNum))
            )
        {
            return;
        }

        bool isModify = false;

        UIAdjustAtlasEditorModel.GetInstance().IsProjectModify(projectPath, out isModify);
       
        m_OperateArea[areaIndex].OldView_Name.Caption = Path.GetFileNameWithoutExtension(projectPath);
        m_OperateArea[areaIndex].OldView_Texture.Image = UIAdjustAtlasEditorModel.GetInstance().GetAtlasTextureBak(projectPath);

        if (isModify)
        {
            m_OperateArea[areaIndex].NewView_Name.Caption = "变更后";
            m_OperateArea[areaIndex].NewView_Texture.Image = UIAdjustAtlasEditorModel.GetInstance().GetAtlasTexture(projectPath);
        }
        else
        {
            m_OperateArea[areaIndex].NewView_Name.Caption = "未变更";
            m_OperateArea[areaIndex].NewView_Texture.Image = null;
        }

        RequestRepaint();
    }

    static private void ClearOperateArea(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            return;
        }

        for (int i = 0; i < m_OperateAreaNum; i++)
        {
            if (m_OperateArea[i].AtlasPath == projectPath)
            {
                m_OperateArea[i].OldView_Name.Caption = "未指定Atlas";
                m_OperateArea[i].OldView_Texture.Image = null;
                m_OperateArea[i].NewView_Name.Caption = "";
                m_OperateArea[i].NewView_Texture.Image = null;
                m_OperateArea[i].SpriteList.ClearItems();
                m_OperateArea[i].SpriteList.RemoveAll();
                m_OperateArea[i].AtlasPath = string.Empty;
                m_AtlasPathInSpriteView[i] = string.Empty;
                break;
            }
        }

        RequestRepaint();
    }

    static public void ClearOperateArea(int areaIndex)
    {
        if ((areaIndex < 0) || (areaIndex >= m_OperateAreaNum))
        {
            return;
        }

        m_OperateArea[areaIndex].OldView_Name.Caption = "未指定Atlas";
        m_OperateArea[areaIndex].OldView_Texture.Image = null;
        m_OperateArea[areaIndex].NewView_Name.Caption = "";
        m_OperateArea[areaIndex].NewView_Texture.Image = null;
        m_OperateArea[areaIndex].SpriteList.ClearItems();
        m_OperateArea[areaIndex].SpriteList.RemoveAll();
        m_OperateArea[areaIndex].AtlasPath = string.Empty;
        m_AtlasPathInSpriteView[areaIndex] = string.Empty;

        RequestRepaint();
    }

    static public void ClearAllOperateArea()
    {
        for (int i = 0; i < m_OperateAreaNum; i++)
        {
            if(!string.IsNullOrEmpty(m_AtlasPathInSpriteView[i]))
            {
                m_OperateArea[i].OldView_Name.Caption = "未指定Atlas";
                m_OperateArea[i].OldView_Texture.Image = null;
                m_OperateArea[i].NewView_Name.Caption = "";
                m_OperateArea[i].NewView_Texture.Image = null;
                m_OperateArea[i].SpriteList.ClearItems();
                m_OperateArea[i].SpriteList.RemoveAll();
                m_OperateArea[i].AtlasPath = string.Empty;
                m_AtlasPathInSpriteView[i] = string.Empty;
            }

        }

        RequestRepaint();
    }

    static public int GetOperateAreaNum()
    {
        return m_OperateAreaNum;
    }

    static private DRAG_TYPE DragAcceptOperate(object dragObject, int areaIndex, bool isCtrl)
    {
        DRAG_TYPE dragType = DRAG_TYPE.DRAG_TYPE_DEFAULT;

        do
        {
            List<string> selectAtlasList = dragObject as List<string>;
            if (selectAtlasList != null)
            {
                dragType = DRAG_TYPE.DRAG_TYPE_ATLAS;
                AcceptAtlasObject(selectAtlasList[0], areaIndex);
                break;
            }

            List<UIAdjust_SpriteOperateInfo> spriteOperateInfoList = dragObject as List<UIAdjust_SpriteOperateInfo>;
            if (spriteOperateInfoList != null)
            {
                AcceptSpriteOject(spriteOperateInfoList, areaIndex, isCtrl);
                dragType = DRAG_TYPE.DRAG_TYPE_SPRITE;

            }

        } while (false);

        return dragType;
    }


    static private int CheckOperateAreaIndex(EditorControl targetCtrl)
    {
        int index = -1;

        do
        {
            ListViewCtrl list = targetCtrl as ListViewCtrl;
            if(list != null)
            {
                switch (list.Name)
                {
                    case "Area0_SpriteList":
                        index = 0;
                        break;
                    case "Area1_SpriteList":
                        index = 1;
                        break;
                    case "Area2_SpriteList":
                        index = 2;
                        break;
                    case "Area3_SpriteList":
                        index = 3;
                        break;
                    default:
                        break;
                }
                break;
            }
        } while (false);

        return index;
    }

    static private void InitializeOpenFileDialog(OpenFileDialog dialog)
    {//初始化添加小图Dialog
        if (dialog == null)
        {
            return;
        }
        string projectPath = UIAdjustAtlasEditorModel.GetInstance().GetProjectPathConfig();
        dialog.Multiselect = true;
        dialog.InitialDirectory = projectPath;
        dialog.Filter = "Atlas工程文件(*.atlasproj)|*.atlasproj";
        dialog.FilterIndex = 2;
        dialog.RestoreDirectory = true;
        dialog.Title = "载入Atlas工程";
    }

    static private void AddProjectItem(string projectPath)
    {
        if ((m_AtlasList == null) || (string.IsNullOrEmpty(projectPath)))
        {
            return;
        }

        ListCtrlItem newItem = new ListCtrlItem();
        newItem.name = Path.GetFileNameWithoutExtension(projectPath);
        newItem.tooltip = projectPath;
        newItem.color = Color.white;
        newItem.onSelectColor = Color.blue;
        newItem.image = UIAdjustAtlasEditorModel.GetInstance().GetAtlasTexture(projectPath);

        m_AtlasList.AddItem(newItem);

        RequestRepaint();
    }

    static private void RemoveProjectItem(string projectPath)
    {
        ListCtrlItem deleteItem = null;

        foreach(var item in m_AtlasList.Items)
        {
            if (item.tooltip == projectPath)
            {
                deleteItem = item;
                break;
            }
        }

        if (deleteItem != null)
        {
            m_AtlasList.RemoveItem(deleteItem);
        }

        RequestRepaint();
    }

    static private void AddSpriteItem(string projectPath, ListViewCtrl list)
    {
        if ((null == list) || (string.IsNullOrEmpty(projectPath)))
        {
            return;
        }

        list.ClearItems();

        List<SpriteItemInfo> spriteInfo = UIAdjustAtlasEditorModel.GetInstance().GetSpriteItems(projectPath);
        if(null == spriteInfo)
        {
            return;
        }

        foreach (var item in spriteInfo)
        {
            ListCtrlItem newItem = new ListCtrlItem();
            newItem.name = item.Name;
            newItem.tooltip = item.Path;
            newItem.image = item.Image;
            newItem.color = Color.white;
            newItem.onSelectColor = Color.blue;
            list.AddItem(newItem);
        }

        list.ClearSelectItems();

        RequestRepaint();
    }

    static private void InitAtlasInspectorView(EditorControl c, object obj)
    {
        InspectorViewCtrl inspector = c as InspectorViewCtrl;
        if (null == inspector)
        {
            return;
        }

        if (
               (null == m_AtlasList)
            || (m_AtlasList.LastSelectItem < 0)
            )
        {
            inspector.RemoveAll();
            return;
        }

        ListCtrlItem selectItem = m_AtlasList.Items[m_AtlasList.LastSelectItem];
        if (null == selectItem)
        {
            inspector.RemoveAll();
            return;
        }

        if (null == selectItem.image)
        {
            inspector.RemoveAll();
            return;
        }

        int atlasRefCount = 0;
        UIAdjustAtlasEditorModel.GetInstance().GetAtlasRefCount(selectItem.tooltip, out atlasRefCount);

        string atlasOutputPath = string.Empty;
        UIAdjustAtlasEditorModel.GetInstance().GetAtlasOutputPath(selectItem.tooltip, out atlasOutputPath);
        string atlasProjectPath = selectItem.tooltip;
        string atlasSize = selectItem.image.width.ToString() + "*" + selectItem.image.height.ToString();


        GUILayout.Space(20f);
        
        GUILayout.BeginVertical();
    
        GUILayout.Label(selectItem.name + ":", GUILayout.ExpandWidth(true));
        GUILayout.Space(20f);
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.MinWidth(200f), GUILayout.MinHeight(100f) , GUILayout.ExpandWidth(false)});
        Rect textureRect = GUILayoutUtility.GetLastRect();
        //GUI.DrawTexture(textureRect, selectItem.image, ScaleMode.ScaleToFit, false);
        EditorGUI.DrawTextureTransparent(textureRect, selectItem.image, ScaleMode.ScaleToFit);

        GUILayout.Space(20f);
        GUILayout.Label("Atlas尺寸：", GUILayout.ExpandWidth(true));
        GUILayout.Label(atlasSize, GUILayout.ExpandWidth(true));
       
        GUILayout.Space(20f);
        GUILayout.Label("Project路径:", GUILayout.ExpandWidth(true));
        GUILayout.Label("\"" + atlasProjectPath + "\"", GUILayout.ExpandWidth(true));
    
        GUILayout.Space(20f);
        GUILayout.Label("Atlas输出路径:", GUILayout.Width(80f));    
      
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("Atlas输出路径111");
        //m_AtlasOutputPath = GUILayout.TextField(m_AtlasOutputPath, GUILayout.Width(180f));
        GUILayout.Label("\"" + m_AtlasOutputPath + "\"", GUILayout.MaxWidth(200f));
  
        if(GUILayout.Button("配置", GUILayout.Width(60f)))
        {
            ConfigAtlasOutputPath(atlasProjectPath);
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.Used)
        {
            switch(GUI.GetNameOfFocusedControl())
            {
                case "Atlas输出路径111":
                    UIAdjustAtlasEditorModel.GetInstance().SetAtlasOutputPath(atlasProjectPath, m_AtlasOutputPath);

                    break;

                default:
                    break;
            }
            RequestRepaint();
        }

        EditorControl refTree = m_EditorRoot.FindControl("atlasRefTree");
        if (null == refTree)
        {
            LabelCtrl refLabel = new LabelCtrl();
            refLabel.Name = "refLabel";
            refLabel.fontSize = 11;
            refLabel.textColor = Color.gray;
            refLabel.Caption = "Atlas反向依赖(" + "引用" + atlasRefCount.ToString() + "次):";

            refTree = new TreeViewCtrl();
            refTree.Name = "atlasRefTree";

            inspector.Add(refLabel);
            inspector.Add(refTree);
            RebuildAtlasRefTreeView(atlasProjectPath);
        }

        RequestRepaint();
    }

    static private void InitSpirteInspectorView(EditorControl c, object obj)
    {
        InspectorViewCtrl inspector = c as InspectorViewCtrl;
        if (null == inspector)
        {
            return;
        }

        if (-1 == m_CurrentSelectIndex)
        {
            inspector.RemoveAll();       
            return;
        }

        ListViewCtrl spriteList = m_OperateArea[m_CurrentSelectIndex].SpriteList;
        if (
               (null == spriteList)
            || (-1 == spriteList.LastSelectItem)
            )
        {
            inspector.RemoveAll();
            return;
        }

        ListCtrlItem selectItem = spriteList.Items[spriteList.LastSelectItem];
        if (null == selectItem)
        {
            inspector.RemoveAll();
            return;
        }

        string atlasPathInView = m_AtlasPathInSpriteView[m_CurrentSelectIndex];
        //string spriteName = selectItem.name;
        string spritePath = selectItem.tooltip;
        //string spriteSize = string.Empty;           
        if(selectItem.image != null)
        {
            //spriteSize = selectItem.image.width.ToString() + "*" + selectItem.image.height.ToString();
        }

        int spriteRefCount = 0;
        UIAdjustAtlasEditorModel.GetInstance().GetSpriteRefCount(atlasPathInView, selectItem.tooltip, out spriteRefCount);

        GUILayout.Space(20f);

        GUILayout.BeginVertical();
        GUILayout.Label(selectItem.name + ":", GUILayout.ExpandWidth(true));
        GUILayout.Space(20f);
        //GUILayout.Box(selectItem.image, new GUILayoutOption[] { GUILayout.MinWidth(100f), GUILayout.MinHeight(100f), GUILayout.ExpandWidth(true) });
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.MinWidth(200f), GUILayout.MinHeight(100f), GUILayout.ExpandWidth(false) });
        Rect textureRect = GUILayoutUtility.GetLastRect();
        //GUI.DrawTexture(textureRect, selectItem.image, ScaleMode.ScaleToFit, false);
        EditorGUI.DrawTextureTransparent(textureRect, selectItem.image, ScaleMode.ScaleToFit);       
        GUILayout.Space(20f);
       
        GUILayout.BeginHorizontal();   
        GUILayout.Label("宽度：", GUILayout.MaxWidth(50f));
        GUI.SetNextControlName("Sprite宽度");
        m_SpriteWidth = GUILayout.TextField(m_SpriteWidth, GUILayout.Width(80));
        GUILayout.Label("pix", GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();  
        GUILayout.Space(20f);
  
        GUILayout.BeginHorizontal();
        GUILayout.Label("高度：", GUILayout.MaxWidth(50f));
        GUI.SetNextControlName("Sprite高度");
        m_SpriteHeight = GUILayout.TextField(m_SpriteHeight, GUILayout.Width(80));
        GUILayout.Label("pix", GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
       
        GUILayout.EndVertical();

        if(Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.Used)
        {
            do
            {
                Texture2D oldTex = null;
                UIAdjustAtlasEditorModel.GetInstance().GetSpriteImageTexture2D(spritePath, atlasPathInView, out oldTex);
                if (null == oldTex)
                {
                    break;
                }

                float zoomScale = 0f;
                UIAdjustAtlasEditorModel.GetInstance().GetSpriteImageZoom(spritePath, m_AtlasPathInSpriteView[m_CurrentSelectIndex], out zoomScale);
                if (0f == zoomScale)
                {
                    break;
                }

                AdjustAtlas_ImageZoomCommand cmd = new AdjustAtlas_ImageZoomCommand();
                cmd.m_oldScaleFactor = zoomScale;
                cmd.m_SpritePath = spritePath;
                cmd.m_ProjectPath = m_AtlasPathInSpriteView[m_CurrentSelectIndex];
                //string text = GUI.GetNameOfFocusedControl();
                if (GUI.GetNameOfFocusedControl() == "Sprite宽度")
                {
                    int w = 0;
                    int.TryParse(m_SpriteWidth, out w);
                    cmd.m_newScaleFactor = (float)w / (float)oldTex.width;
                }
                else if (GUI.GetNameOfFocusedControl() == "Sprite高度")
                {
                    int h = 0;
                    int.TryParse(m_SpriteHeight, out h);
                    cmd.m_newScaleFactor = (float)h / (float)oldTex.height;
                }

                if (0f == cmd.m_newScaleFactor)
                {
                    break;
                }

                EditorCommandManager.GetInstance().Add(cmd);
                RegisterUndo("Zoom Image");
            
                RequestRepaint();
            } while (false);

        }

        EditorControl refTree = m_EditorRoot.FindControl("spriteRefTree");
        if (null == refTree)
        {
            LabelCtrl refLabel = new LabelCtrl();
            refLabel.Name = "spriterefLabel";
            refLabel.fontSize = 11;
            refLabel.textColor = Color.gray;
            refLabel.Caption = "Sprite反向依赖(" + "引用" + spriteRefCount.ToString() + "次):";

            refTree = new TreeViewCtrl();
            refTree.Name = "spriteRefTree";

            inspector.Add(refLabel);
            inspector.Add(refTree);
            RebuildSpriteRefTreeView(atlasPathInView, spritePath);
        }

        RequestRepaint();

    }

    static private void ConfigAtlasOutputPath(string projectPath)
    {
        if(string.IsNullOrEmpty(projectPath))
        {
            return;
        }
        string outputPath = string.Empty;

        UIAdjustAtlasEditorModel.GetInstance().GetAtlasOutputPath(projectPath, out outputPath);
        string U3DAssetPath = UnityEngine.Application.dataPath;

        outputPath = EditorUtility.SaveFolderPanel("配置Atlas输出路径", outputPath, "");
        if (!string.IsNullOrEmpty(outputPath))
        {
            if (outputPath.Contains(U3DAssetPath))
            {
                outputPath = outputPath.Substring(U3DAssetPath.Length - "Assets".Length) + "/";
                UIAdjustAtlasEditorModel.GetInstance().SetAtlasOutputPath(projectPath, outputPath);
                m_AtlasOutputPath = outputPath;
            }
            else
            {
                EditorUtility.DisplayDialog("配置失败",
                                             "请选择Assets/下的路径！",
                                             "确认");
            }
        }
    }

    static private void RebuildAtlasRefTreeView(string projectPath)
    {
        if(null == m_EditorRoot)
        {
            return;
        }

        TreeViewCtrl refTreeList = m_EditorRoot.FindControl("atlasRefTree") as TreeViewCtrl;
        if (null == refTreeList)
        {
            return;
        }

        refTreeList.Clear();
        List<string> refTable = null;

        UIAdjustAtlasEditorModel.GetInstance().GetAtlasRefTable(projectPath, out refTable);
        foreach (var item in refTable)
        {
            AddRefItemToTreeView(item, refTreeList);
        }

        GC.Collect();

        RequestRepaint();
    }

    static private void RebuildSpriteRefTreeView(string projectPath, string spritePath)
    {
        if (null == m_EditorRoot)
        {
            return;
        }

        TreeViewCtrl refTreeList = m_EditorRoot.FindControl("spriteRefTree") as TreeViewCtrl;
        if (null == refTreeList)
        {
            return;
        }

        refTreeList.Clear();
        List<string> refTable = null;

        UIAdjustAtlasEditorModel.GetInstance().GetSpriteRefTable(projectPath, spritePath, out refTable);
        foreach (var item in refTable)
        {
            AddRefItemToTreeView(item, refTreeList);
        }

        GC.Collect();

        RequestRepaint();
    }

    static private void AddRefItemToTreeView(string refAssetPath, TreeViewCtrl treeCtrl)
    {
        if (string.IsNullOrEmpty(refAssetPath))
        {
            return;
        }

        if (treeCtrl == null)
        {
            return;
        }

        bool expandTreeNode = true;
        //if ((ResourceManageToolModel.GetInstance().CurrFilter as NullTypeFilter) == null)
        //{//非过滤器为全部文件则节点都展开
        //    expandTreeNode = true;
        //}

        string currPath = refAssetPath;
        List<TreeViewNode> currLevelNodeList = treeCtrl.Roots;
        TreeViewNode parentNode = null;
        int len = 0;
        while (currPath != "")
        {
            int i = currPath.IndexOf('/');
            if (i < 0)
            {
                i = currPath.Length;
            }
            len += i + 1;
            string pathNodeName = currPath.Substring(0, i);
            string currNodeFullPath = refAssetPath.Substring(0, len - 1);
            if (i + 1 < currPath.Length)
            {
                currPath = currPath.Substring(i + 1);
            }
            else
            {
                currPath = "";
            }


            bool findNode = false;
            foreach (var treeNode in currLevelNodeList)
            {
                if (treeNode.name == pathNodeName)
                {
                    findNode = true;
                    parentNode = treeNode;
                    currLevelNodeList = treeNode.children;
                    break;
                }
            }

            if (!findNode)
            {
                TreeViewNode newNode = new TreeViewNode();
                newNode.name = pathNodeName;
                newNode.image = AssetDatabase.GetCachedIcon(currNodeFullPath);
                newNode.userObject = AssetDatabase.AssetPathToGUID(currNodeFullPath);
                newNode.state.IsExpand = expandTreeNode;
                
                if (parentNode == null)
                {//说明需要作为根节点插入树视图中
                    currLevelNodeList.Add(newNode);
                }
                else
                {
                    parentNode.Add(newNode);
                }

                parentNode = newNode;
                currLevelNodeList = newNode.children;
            }

        }
    }

    static private int GetSpriteListIndex(ListViewCtrl list)
    {
        int index = -1;
       
        if (null == list)
        {
            return -1;
        }

        switch (list.Name)
        {
            case "Area0_SpriteList":
                index = 0;
                break;

            case "Area1_SpriteList":
                index = 1;
                break;

            case "Area2_SpriteList":
                index = 2;
                break;
  
            case "Area3_SpriteList":
                index = 3;
                break;
            default:
                break;
        }

        return index;
    }

    static private void ClearInspectorView()
    {
        m_inspector.onInspector = null;
        m_inspector.RemoveAll();
    }

    static private void ClearAtlasPathInArea()
    {
        for(int index = 0; index < m_OperateAreaNum; index++)
        {
            m_AtlasPathInSpriteView[index] = string.Empty;
        }
    }

    static private bool IsCommonConfigError()
    {
        bool isError = true;

        do
        {
            isError = true;

            if (string.IsNullOrEmpty(UIAtlasEditorConfig.ProjectPath))
            {
                EditorUtility.DisplayDialog("载入失败", "\nProject目录错误，请确认！", "确认");
                break;
            }

            if (string.IsNullOrEmpty(UIAtlasEditorConfig.ImageBasePath))
            {
                EditorUtility.DisplayDialog("载入失败", "\nSprite图库路径错误，请确认！", "确认");
                break;
            }

            if (string.IsNullOrEmpty(UIAtlasEditorConfig.ConsistencyResultPath))
            {
                EditorUtility.DisplayDialog("载入失败", "\n一致性检查结果输出路径错误，请确认！", "确认");
                break;
            }

            isError = false;
        } while (false);

        return isError;
    }

    static private void InitWndBy2View()
    {
        VBoxCtrl opearteArea = m_EditorRoot.FindControl("OperateArea") as VBoxCtrl;
        if (null == opearteArea)
        {
            return;
        }

        opearteArea.RemoveAll();

        #region 创建布置窗口元素
        //Rect labelRect = new Rect(0, 0, 80, 5);
        Rect textureRect = new Rect(0, 0, 400, 400);

        #region 第三级分割

        VSpliterCtrl vs2_2_1 = new VSpliterCtrl();
        vs2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(300f);
        vs2_2_1.Dragable = true;

        VBoxCtrl vb2_2_1 = new VBoxCtrl();               //移动对象1      
        VBoxCtrl vb2_2_2 = new VBoxCtrl();               //移动对象2   
        //VBoxCtrl vb2_2_3 = new VBoxCtrl();               //移动对象3   
        #endregion

        #region 第四级分割
        HSpliterCtrl hs2_2_1_1 = new HSpliterCtrl();
        hs2_2_1_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_1_1.Dragable = true;
        HSpliterCtrl hs2_2_1_2 = new HSpliterCtrl();
        hs2_2_1_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_2_1 = new HSpliterCtrl();
        hs2_2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_2_1.Dragable = true;
        HSpliterCtrl hs2_2_2_2 = new HSpliterCtrl();
        hs2_2_2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);


        VBoxCtrl hb2_2_1_1 = new VBoxCtrl();      //变更前预览1
        VBoxCtrl hb2_2_1_2 = new VBoxCtrl();      //变更后预览1
        VBoxCtrl hb2_2_1_3 = new VBoxCtrl();      //SpriteList1

        VBoxCtrl hb2_2_2_1 = new VBoxCtrl();      //变更前预览2
        VBoxCtrl hb2_2_2_2 = new VBoxCtrl();      //变更后预览2
        VBoxCtrl hb2_2_2_3 = new VBoxCtrl();      //SpriteList2

        #endregion
        #endregion
        #region 第四级分割
        #region Area0
        m_OperateArea[0].OldView_Name = new LabelCtrl();
        m_OperateArea[0].OldView_Name.Name = "Area0_OldView_Name";
        m_OperateArea[0].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[0].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].OldView_Texture.Name = "Area0_OldView_Texture";
        m_OperateArea[0].OldView_Texture.Size = textureRect;

        m_OperateArea[0].NewView_Name = new LabelCtrl();
        m_OperateArea[0].NewView_Name.Name = "Area0_NewView_Name";
        m_OperateArea[0].NewView_Name.Caption = "";

        m_OperateArea[0].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].NewView_Texture.Name = "Area0_NewView_Texture";
        m_OperateArea[0].NewView_Texture.Size = textureRect;

        m_OperateArea[0].SpriteList = new ListViewCtrl();
        m_OperateArea[0].SpriteList.Name = "Area0_SpriteList";
        m_OperateArea[0].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[0].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[0].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;
        m_OperateArea[0].SpriteList.IsTextureView = false;

        hb2_2_1_1.Add(m_OperateArea[0].OldView_Name);
        hb2_2_1_1.Add(m_OperateArea[0].OldView_Texture);

        hb2_2_1_2.Add(m_OperateArea[0].NewView_Name);
        hb2_2_1_2.Add(m_OperateArea[0].NewView_Texture);

        hb2_2_1_3.Add(m_OperateArea[0].SpriteList);

        hs2_2_1_2.Add(hb2_2_1_2);
        hs2_2_1_2.Add(hb2_2_1_3);

        hs2_2_1_1.Add(hb2_2_1_1);
        hs2_2_1_1.Add(hs2_2_1_2);
        #endregion

        #region Area1
        m_OperateArea[1].OldView_Name = new LabelCtrl();
        m_OperateArea[1].OldView_Name.Name = "Area1_OldView_Name";
        m_OperateArea[1].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[1].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].OldView_Texture.Name = "Area1_OldView_Texture";
        m_OperateArea[1].OldView_Texture.Size = textureRect;

        m_OperateArea[1].NewView_Name = new LabelCtrl();
        m_OperateArea[1].NewView_Name.Name = "Area1_NewView_Name";
        m_OperateArea[1].NewView_Name.Caption = "";

        m_OperateArea[1].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].NewView_Texture.Name = "Area1_NewView_Texture";
        m_OperateArea[1].NewView_Texture.Size = textureRect;

        m_OperateArea[1].SpriteList = new ListViewCtrl();
        m_OperateArea[1].SpriteList.Name = "Area1_SpriteList";
        m_OperateArea[1].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[1].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[1].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;


        hb2_2_2_1.Add(m_OperateArea[1].OldView_Name);
        hb2_2_2_1.Add(m_OperateArea[1].OldView_Texture);

        hb2_2_2_2.Add(m_OperateArea[1].NewView_Name);
        hb2_2_2_2.Add(m_OperateArea[1].NewView_Texture);

        hb2_2_2_3.Add(m_OperateArea[1].SpriteList);

        hs2_2_2_2.Add(hb2_2_2_2);
        hs2_2_2_2.Add(hb2_2_2_3);

        hs2_2_2_1.Add(hb2_2_2_1);
        hs2_2_2_1.Add(hs2_2_2_2);
        #endregion

        #endregion

        #region 第三级分割
        vb2_2_1.Add(hs2_2_1_1);
        vb2_2_2.Add(hs2_2_2_1);

        vs2_2_1.Add(vb2_2_1);
        vs2_2_1.Add(vb2_2_2);

        opearteArea.Add(vs2_2_1);
        #endregion


        RequestRepaint();

    }

    static private void InitWndBy3View()
    {
        VBoxCtrl opearteArea = m_EditorRoot.FindControl("OperateArea") as VBoxCtrl;
        if(null == opearteArea)
        {
            return;
        }

        opearteArea.RemoveAll();
 
        #region 创建布置窗口元素
        //Rect labelRect = new Rect(0, 0, 80, 5);
        Rect textureRect = new Rect(0, 0, 400, 400);

        #region 第三级分割
   
        VSpliterCtrl vs2_2_1 = new VSpliterCtrl();
        vs2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);
        vs2_2_1.Dragable = true;

        VSpliterCtrl vs2_2_2 = new VSpliterCtrl();
        vs2_2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);
        vs2_2_2.Dragable = true;
        
        VBoxCtrl vb2_2_1 = new VBoxCtrl();               //移动对象1      
        VBoxCtrl vb2_2_2 = new VBoxCtrl();               //移动对象2   
        VBoxCtrl vb2_2_3 = new VBoxCtrl();               //移动对象3   
        #endregion

        #region 第四级分割
        HSpliterCtrl hs2_2_1_1 = new HSpliterCtrl();
        hs2_2_1_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_1_1.Dragable = true;
       
        HSpliterCtrl hs2_2_1_2 = new HSpliterCtrl();
        hs2_2_1_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_2_1 = new HSpliterCtrl();
        hs2_2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_2_1.Dragable = true;
       
        HSpliterCtrl hs2_2_2_2 = new HSpliterCtrl();
        hs2_2_2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_3_1 = new HSpliterCtrl();
        hs2_2_3_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_3_1.Dragable = true;
        
        HSpliterCtrl hs2_2_3_2 = new HSpliterCtrl();
        hs2_2_3_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);
       
        VBoxCtrl hb2_2_1_1 = new VBoxCtrl();      //变更前预览1
        VBoxCtrl hb2_2_1_2 = new VBoxCtrl();      //变更后预览1
        VBoxCtrl hb2_2_1_3 = new VBoxCtrl();      //SpriteList1

        VBoxCtrl hb2_2_2_1 = new VBoxCtrl();      //变更前预览2
        VBoxCtrl hb2_2_2_2 = new VBoxCtrl();      //变更后预览2
        VBoxCtrl hb2_2_2_3 = new VBoxCtrl();      //SpriteList2

        VBoxCtrl hb2_2_3_1 = new VBoxCtrl();      //变更前预览2
        VBoxCtrl hb2_2_3_2 = new VBoxCtrl();      //变更后预览2
        VBoxCtrl hb2_2_3_3 = new VBoxCtrl();      //SpriteList2
        #endregion
        #endregion
        #region 第四级分割
        #region Area0
        m_OperateArea[0].OldView_Name = new LabelCtrl();
        m_OperateArea[0].OldView_Name.Name = "Area0_OldView_Name";
        m_OperateArea[0].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[0].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].OldView_Texture.Name = "Area0_OldView_Texture";
        m_OperateArea[0].OldView_Texture.Size = textureRect;

        m_OperateArea[0].NewView_Name = new LabelCtrl();
        m_OperateArea[0].NewView_Name.Name = "Area0_NewView_Name";
        m_OperateArea[0].NewView_Name.Caption = "";

        m_OperateArea[0].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].NewView_Texture.Name = "Area0_NewView_Texture";
        m_OperateArea[0].NewView_Texture.Size = textureRect;

        m_OperateArea[0].SpriteList = new ListViewCtrl();
        m_OperateArea[0].SpriteList.Name = "Area0_SpriteList";
        m_OperateArea[0].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[0].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[0].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;
        m_OperateArea[0].SpriteList.IsTextureView = false;

        hb2_2_1_1.Add(m_OperateArea[0].OldView_Name);
        hb2_2_1_1.Add(m_OperateArea[0].OldView_Texture);

        hb2_2_1_2.Add(m_OperateArea[0].NewView_Name);
        hb2_2_1_2.Add(m_OperateArea[0].NewView_Texture);

        hb2_2_1_3.Add(m_OperateArea[0].SpriteList);

        hs2_2_1_2.Add(hb2_2_1_2);
        hs2_2_1_2.Add(hb2_2_1_3);

        hs2_2_1_1.Add(hb2_2_1_1);
        hs2_2_1_1.Add(hs2_2_1_2);
        #endregion

        #region Area1
        m_OperateArea[1].OldView_Name = new LabelCtrl();
        m_OperateArea[1].OldView_Name.Name = "Area1_OldView_Name";
        m_OperateArea[1].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[1].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].OldView_Texture.Name = "Area1_OldView_Texture";
        m_OperateArea[1].OldView_Texture.Size = textureRect;

        m_OperateArea[1].NewView_Name = new LabelCtrl();
        m_OperateArea[1].NewView_Name.Name = "Area1_NewView_Name";
        m_OperateArea[1].NewView_Name.Caption = "";

        m_OperateArea[1].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].NewView_Texture.Name = "Area1_NewView_Texture";
        m_OperateArea[1].NewView_Texture.Size = textureRect;

        m_OperateArea[1].SpriteList = new ListViewCtrl();
        m_OperateArea[1].SpriteList.Name = "Area1_SpriteList";
        m_OperateArea[1].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[1].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[1].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;


        hb2_2_2_1.Add(m_OperateArea[1].OldView_Name);
        hb2_2_2_1.Add(m_OperateArea[1].OldView_Texture);

        hb2_2_2_2.Add(m_OperateArea[1].NewView_Name);
        hb2_2_2_2.Add(m_OperateArea[1].NewView_Texture);

        hb2_2_2_3.Add(m_OperateArea[1].SpriteList);

        hs2_2_2_2.Add(hb2_2_2_2);
        hs2_2_2_2.Add(hb2_2_2_3);

        hs2_2_2_1.Add(hb2_2_2_1);
        hs2_2_2_1.Add(hs2_2_2_2);
        #endregion

        #region Area2
        m_OperateArea[2].OldView_Name = new LabelCtrl();
        m_OperateArea[2].OldView_Name.Name = "Area2_OldView_Name";
        m_OperateArea[2].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[2].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[2].OldView_Texture.Name = "Area2_OldView_Texture";
        m_OperateArea[2].OldView_Texture.Size = textureRect;

        m_OperateArea[2].NewView_Name = new LabelCtrl();
        m_OperateArea[2].NewView_Name.Name = "Area2_NewView_Name";
        m_OperateArea[2].NewView_Name.Caption = "";

        m_OperateArea[2].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[2].NewView_Texture.Name = "Area2_NewView_Texture";
        m_OperateArea[2].NewView_Texture.Size = textureRect;

        m_OperateArea[2].SpriteList = new ListViewCtrl();
        m_OperateArea[2].SpriteList.Name = "Area2_SpriteList";
        m_OperateArea[2].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[2].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[2].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[2].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[2].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[2].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[2].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[2].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;


        hb2_2_3_1.Add(m_OperateArea[2].OldView_Name);
        hb2_2_3_1.Add(m_OperateArea[2].OldView_Texture);

        hb2_2_3_2.Add(m_OperateArea[2].NewView_Name);
        hb2_2_3_2.Add(m_OperateArea[2].NewView_Texture);

        hb2_2_3_3.Add(m_OperateArea[2].SpriteList);

        hs2_2_3_2.Add(hb2_2_3_2);
        hs2_2_3_2.Add(hb2_2_3_3);

        hs2_2_3_1.Add(hb2_2_3_1);
        hs2_2_3_1.Add(hs2_2_3_2);
        #endregion

        #endregion

        #region 第三级分割
        vb2_2_1.Add(hs2_2_1_1);
        vb2_2_2.Add(hs2_2_2_1);
        vb2_2_3.Add(hs2_2_3_1);

        vs2_2_2.Add(vb2_2_2);
        vs2_2_2.Add(vb2_2_3);

        vs2_2_1.Add(vb2_2_1);
        vs2_2_1.Add(vs2_2_2);

        opearteArea.Add(vs2_2_1);
        #endregion


        RequestRepaint();
    }

    static private void InitWndBy4View()
    {
        VBoxCtrl opearteArea = m_EditorRoot.FindControl("OperateArea") as VBoxCtrl;
        if (null == opearteArea)
        {
            return;
        }

        opearteArea.RemoveAll();

        #region 创建布置窗口元素
        //Rect labelRect = new Rect(0, 0, 80, 5);
        Rect textureRect = new Rect(0, 0, 400, 400);

        #region 第三级分割

        VSpliterCtrl vs2_2_1 = new VSpliterCtrl();
        vs2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(100f);
        vs2_2_1.Dragable = true;

        VSpliterCtrl vs2_2_2 = new VSpliterCtrl();
        vs2_2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(100f);
        vs2_2_2.Dragable = true;

        VSpliterCtrl vs2_2_3 = new VSpliterCtrl();
        vs2_2_3.layoutConstraint = LayoutConstraint.GetSpliterConstraint(100f);
        vs2_2_3.Dragable = true;

        VBoxCtrl vb2_2_1 = new VBoxCtrl();               //移动对象1      
        VBoxCtrl vb2_2_2 = new VBoxCtrl();               //移动对象2   
        VBoxCtrl vb2_2_3 = new VBoxCtrl();               //移动对象3   
        VBoxCtrl vb2_2_4 = new VBoxCtrl();               //移动对象3     
        #endregion

        #region 第四级分割
        HSpliterCtrl hs2_2_1_1 = new HSpliterCtrl();
        hs2_2_1_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_1_1.Dragable = true;

        HSpliterCtrl hs2_2_1_2 = new HSpliterCtrl();
        hs2_2_1_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_2_1 = new HSpliterCtrl();
        hs2_2_2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_2_1.Dragable = true;

        HSpliterCtrl hs2_2_2_2 = new HSpliterCtrl();
        hs2_2_2_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_3_1 = new HSpliterCtrl();
        hs2_2_3_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_3_1.Dragable = true;

        HSpliterCtrl hs2_2_3_2 = new HSpliterCtrl();
        hs2_2_3_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        HSpliterCtrl hs2_2_4_1 = new HSpliterCtrl();
        hs2_2_4_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(m_EditorRoot.position.height / 3 + 30f);
        hs2_2_4_1.Dragable = true;

        HSpliterCtrl hs2_2_4_2 = new HSpliterCtrl();
        hs2_2_4_2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f, true);

        VBoxCtrl hb2_2_1_1 = new VBoxCtrl();      //变更前预览1
        VBoxCtrl hb2_2_1_2 = new VBoxCtrl();      //变更后预览1
        VBoxCtrl hb2_2_1_3 = new VBoxCtrl();      //SpriteList1

        VBoxCtrl hb2_2_2_1 = new VBoxCtrl();      //变更前预览2
        VBoxCtrl hb2_2_2_2 = new VBoxCtrl();      //变更后预览2
        VBoxCtrl hb2_2_2_3 = new VBoxCtrl();      //SpriteList2

        VBoxCtrl hb2_2_3_1 = new VBoxCtrl();      //变更前预览3
        VBoxCtrl hb2_2_3_2 = new VBoxCtrl();      //变更后预览3
        VBoxCtrl hb2_2_3_3 = new VBoxCtrl();      //SpriteList3

        VBoxCtrl hb2_2_4_1 = new VBoxCtrl();      //变更前预览3
        VBoxCtrl hb2_2_4_2 = new VBoxCtrl();      //变更后预览3
        VBoxCtrl hb2_2_4_3 = new VBoxCtrl();      //SpriteList3

        #endregion
        #endregion
        #region 第四级分割
        #region Area0
        m_OperateArea[0].OldView_Name = new LabelCtrl();
        m_OperateArea[0].OldView_Name.Name = "Area0_OldView_Name";
        m_OperateArea[0].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[0].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].OldView_Texture.Name = "Area0_OldView_Texture";
        m_OperateArea[0].OldView_Texture.Size = textureRect;

        m_OperateArea[0].NewView_Name = new LabelCtrl();
        m_OperateArea[0].NewView_Name.Name = "Area0_NewView_Name";
        m_OperateArea[0].NewView_Name.Caption = "";

        m_OperateArea[0].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[0].NewView_Texture.Name = "Area0_NewView_Texture";
        m_OperateArea[0].NewView_Texture.Size = textureRect;

        m_OperateArea[0].SpriteList = new ListViewCtrl();
        m_OperateArea[0].SpriteList.Name = "Area0_SpriteList";
        m_OperateArea[0].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[0].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[0].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[0].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[0].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;
        m_OperateArea[0].SpriteList.IsTextureView = false;

        hb2_2_1_1.Add(m_OperateArea[0].OldView_Name);
        hb2_2_1_1.Add(m_OperateArea[0].OldView_Texture);

        hb2_2_1_2.Add(m_OperateArea[0].NewView_Name);
        hb2_2_1_2.Add(m_OperateArea[0].NewView_Texture);

        hb2_2_1_3.Add(m_OperateArea[0].SpriteList);

        hs2_2_1_2.Add(hb2_2_1_2);
        hs2_2_1_2.Add(hb2_2_1_3);

        hs2_2_1_1.Add(hb2_2_1_1);
        hs2_2_1_1.Add(hs2_2_1_2);
        #endregion

        #region Area1
        m_OperateArea[1].OldView_Name = new LabelCtrl();
        m_OperateArea[1].OldView_Name.Name = "Area1_OldView_Name";
        m_OperateArea[1].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[1].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].OldView_Texture.Name = "Area1_OldView_Texture";
        m_OperateArea[1].OldView_Texture.Size = textureRect;

        m_OperateArea[1].NewView_Name = new LabelCtrl();
        m_OperateArea[1].NewView_Name.Name = "Area1_NewView_Name";
        m_OperateArea[1].NewView_Name.Caption = "";

        m_OperateArea[1].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[1].NewView_Texture.Name = "Area1_NewView_Texture";
        m_OperateArea[1].NewView_Texture.Size = textureRect;

        m_OperateArea[1].SpriteList = new ListViewCtrl();
        m_OperateArea[1].SpriteList.Name = "Area1_SpriteList";
        m_OperateArea[1].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[1].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[1].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[1].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[1].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;


        hb2_2_2_1.Add(m_OperateArea[1].OldView_Name);
        hb2_2_2_1.Add(m_OperateArea[1].OldView_Texture);

        hb2_2_2_2.Add(m_OperateArea[1].NewView_Name);
        hb2_2_2_2.Add(m_OperateArea[1].NewView_Texture);

        hb2_2_2_3.Add(m_OperateArea[1].SpriteList);

        hs2_2_2_2.Add(hb2_2_2_2);
        hs2_2_2_2.Add(hb2_2_2_3);

        hs2_2_2_1.Add(hb2_2_2_1);
        hs2_2_2_1.Add(hs2_2_2_2);
        #endregion

        #region Area2
        m_OperateArea[2].OldView_Name = new LabelCtrl();
        m_OperateArea[2].OldView_Name.Name = "Area2_OldView_Name";
        m_OperateArea[2].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[2].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[2].OldView_Texture.Name = "Area2_OldView_Texture";
        m_OperateArea[2].OldView_Texture.Size = textureRect;

        m_OperateArea[2].NewView_Name = new LabelCtrl();
        m_OperateArea[2].NewView_Name.Name = "Area2_NewView_Name";
        m_OperateArea[2].NewView_Name.Caption = "";

        m_OperateArea[2].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[2].NewView_Texture.Name = "Area2_NewView_Texture";
        m_OperateArea[2].NewView_Texture.Size = textureRect;

        m_OperateArea[2].SpriteList = new ListViewCtrl();
        m_OperateArea[2].SpriteList.Name = "Area2_SpriteList";
        m_OperateArea[2].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[2].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[2].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[2].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[2].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[2].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[2].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[2].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;

        hb2_2_3_1.Add(m_OperateArea[2].OldView_Name);
        hb2_2_3_1.Add(m_OperateArea[2].OldView_Texture);

        hb2_2_3_2.Add(m_OperateArea[2].NewView_Name);
        hb2_2_3_2.Add(m_OperateArea[2].NewView_Texture);

        hb2_2_3_3.Add(m_OperateArea[2].SpriteList);

        hs2_2_3_2.Add(hb2_2_3_2);
        hs2_2_3_2.Add(hb2_2_3_3);

        hs2_2_3_1.Add(hb2_2_3_1);
        hs2_2_3_1.Add(hs2_2_3_2);
        #endregion

        #region Area3
        m_OperateArea[3].OldView_Name = new LabelCtrl();
        m_OperateArea[3].OldView_Name.Name = "Area3_OldView_Name";
        m_OperateArea[3].OldView_Name.Caption = "未指定atlas";

        m_OperateArea[3].OldView_Texture = new TextureBoxCtrl();
        m_OperateArea[3].OldView_Texture.Name = "Area3_OldView_Texture";
        m_OperateArea[3].OldView_Texture.Size = textureRect;

        m_OperateArea[3].NewView_Name = new LabelCtrl();
        m_OperateArea[3].NewView_Name.Name = "Area3_NewView_Name";
        m_OperateArea[3].NewView_Name.Caption = "";

        m_OperateArea[3].NewView_Texture = new TextureBoxCtrl();
        m_OperateArea[3].NewView_Texture.Name = "Area3_NewView_Texture";
        m_OperateArea[3].NewView_Texture.Size = textureRect;

        m_OperateArea[3].SpriteList = new ListViewCtrl();
        m_OperateArea[3].SpriteList.Name = "Area3_SpriteList";
        m_OperateArea[3].SpriteList.onItemSelected = OnSelectSpriteListItem;
        m_OperateArea[3].SpriteList.onItemSelectedR = OnSelectSpriteListItem;
        m_OperateArea[3].SpriteList.onItemSelectedRU = OnSelectSpriteListItemRU;
        m_OperateArea[3].SpriteList.onItemCtrlSelected = OnSelectSpriteListItem;
        m_OperateArea[3].SpriteList.onPrepareCustomDrag = onSpriteListPrepareDrag;
        m_OperateArea[3].SpriteList.onTryAcceptCustomDrag = onSpriteListTryAcceptDrag;
        m_OperateArea[3].SpriteList.onAcceptCustomDrag = onSpriteListAcceptDrag;
        m_OperateArea[3].SpriteList.onAcceptCustomDragCtrl = onSpriteListAcceptDragCtrl;

        hb2_2_4_1.Add(m_OperateArea[3].OldView_Name);
        hb2_2_4_1.Add(m_OperateArea[3].OldView_Texture);

        hb2_2_4_2.Add(m_OperateArea[3].NewView_Name);
        hb2_2_4_2.Add(m_OperateArea[3].NewView_Texture);

        hb2_2_4_3.Add(m_OperateArea[3].SpriteList);

       
        hs2_2_4_2.Add(hb2_2_4_2);
        hs2_2_4_2.Add(hb2_2_4_3);

        hs2_2_4_1.Add(hb2_2_4_1);
        hs2_2_4_1.Add(hs2_2_4_2);
        #endregion

        #endregion

        #region 第三级分割
        vb2_2_1.Add(hs2_2_1_1);
        vb2_2_2.Add(hs2_2_2_1);
        vb2_2_3.Add(hs2_2_3_1);
        vb2_2_4.Add(hs2_2_4_1);

        vs2_2_3.Add(vb2_2_3);
        vs2_2_3.Add(vb2_2_4);

        vs2_2_2.Add(vb2_2_2);
        vs2_2_2.Add(vs2_2_3);

        vs2_2_1.Add(vb2_2_1);
        vs2_2_1.Add(vs2_2_2);

        opearteArea.Add(vs2_2_1);
        #endregion

        RequestRepaint();

    }

    static private void CheckReadOnlyFile()
    {
        bool isHaveReadOnlyFile = false;
        string firstFile = string.Empty;

        isHaveReadOnlyFile = UIAdjustAtlasEditorModel.GetInstance().CheckReadOnlyFile(out firstFile);
        
        if (isHaveReadOnlyFile)
        {
            if (!EditorUtility.DisplayDialog("警告！", "工程中某些prefab、scene文件是只读的，如：" + firstFile + "\n您最好确认这些文件是否被CheckOut\n如果选择[继续操作]，则会强制修改这些文件", "继续操作", "关闭工具"))
            {
                m_EditorRoot.Close();
            }
        }
    }

    static private void InitUndoCommand()
    {
        m_Counter = new GameObject();
        m_Counter.name = "AdjustAtlasCmdCounter";
        m_Counter.hideFlags = HideFlags.HideAndDontSave;
        m_Counter.AddComponent<UIAtlasCommandCounter>();
        m_CommandCounter = m_Counter.GetComponent<UIAtlasCommandCounter>();
    }

    static private void RegisterUndo(string cmdName)
    {
#if UNITY_4_5 || UNITY_4_6 || UNITY_5_0
        Undo.RegisterCompleteObjectUndo(m_CommandCounter, cmdName);
#endif

        m_CommandCounter.CommandCounter += 1;
        m_CommandCounter.PreCommandCounter = m_CommandCounter.CommandCounter;
    }

    static private void RequestRepaint()
    {
        if (m_EditorRoot != null)
        {
            m_EditorRoot.RequestRepaint();
        }
    }
}
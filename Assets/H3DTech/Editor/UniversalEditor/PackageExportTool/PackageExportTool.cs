
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;


public class PackageExportTool 
{
    static EditorRoot s_root = null;

    private static string m_ExportPath = string.Empty;
    private static string m_ManualPath = string.Empty;
    private static string m_MainVer = string.Empty;
    private static string m_UpgradeVer = string.Empty;
    private static string m_P4Ver = string.Empty;
    private static bool m_IsWithManual = false;

    [MenuItem("H3D/编辑器版本发布/编辑器发布工具")]
    public static void Show()
    {
        EditorRoot root = EditorManager.GetInstance().FindEditor("资源导出工具");
        if (root == null)
        {
            root = EditorManager.GetInstance().CreateEditor("资源导出工具", false, InitControls);
        }
    }
    public static void InitControls(EditorRoot editorRoot)
    {
        s_root = editorRoot;

        //s_root.position = new Rect(100f, 100f, 1024, 768f);
        s_root.onDestroy = OnDestroy;
        s_root.onEnable = OnEnable;
     
        Rect btnRect = new Rect(0, 0, 80, 20);
        Rect comboBoxRect = new Rect(0, 0, 100, 20);

        //Modify by liteng for 发布工具改善Start
        #region 创建布置窗口元素
        #region 第一级
        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f);
        HSpliterCtrl hs2 = new HSpliterCtrl();
        hs2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f, true);

        HBoxCtrl hb1 = new HBoxCtrl();      //布置上方菜单条
        HBoxCtrl hb2 = new HBoxCtrl();      //布置主窗口
        HBoxCtrl hb3 = new HBoxCtrl();      //布置下方状态栏
        #endregion

        #region 第二级
        VSpliterCtrl vs2_1 = new VSpliterCtrl();
        vs2_1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(300f);
        vs2_1.Dragable = true;

        VBoxCtrl vb2_1 = new VBoxCtrl();               //资源视图       
        VBoxCtrl vb2_2 = new VBoxCtrl();               //发布信息视图       
        #endregion
        #endregion

        #region 布置窗口（由高至低布置）
        #region 第二级
        TreeViewCtrl m_treeView = new TreeViewCtrl();   //资源列表
        m_treeView.Name = "_MainTreeView";
        m_treeView.onValueChange = OnTreeViewNodeToggle;

        InspectorViewCtrl m_inspector = new InspectorViewCtrl();  //发布信息视图
        m_inspector.Name = "_Inspector";
        m_inspector.onInspector = OnInspector;

        vb2_1.Add(m_treeView);
        vb2_2.Add(m_inspector);
        vs2_1.Add(vb2_1);
        vs2_1.Add(vb2_2);

        hb2.Add(vs2_1);
        #endregion

        #region 第一级
        //[导出]按钮
        ButtonCtrl exportBtn = new ButtonCtrl();
        exportBtn.Caption = "导出";
        exportBtn.Name = "_ExportButton";
        exportBtn.Size = btnRect;
        exportBtn.onClick = OnExportBtnClick;

        //[发布]按钮
        //ButtonCtrl publishBtn = new ButtonCtrl();
        //publishBtn.Caption = "发布";
        //publishBtn.Name = "_PublishButton";
        //publishBtn.Size = btnRect;
        //publishBtn.onClick = OnPublishBtnClick;


        //[配置AB路径]按钮
        //ButtonCtrl configPublishBtn = new ButtonCtrl();
        //configPublishBtn.Caption = "配置AB路径";
        //configPublishBtn.Name = "_ConfigPublishButton";
        //configPublishBtn.Size = btnRect;
        //configPublishBtn.onClick = OnConfigPublishPathBtnClick;

        //版本下拉菜单
        ComboBoxCtrl<int> debugCombo = new ComboBoxCtrl<int>(0);
        debugCombo.Size = comboBoxRect;
        debugCombo.Name = "_DebugCombo";
        debugCombo.onValueChange = OnDebugComboSelect;
        debugCombo.AddItem(new ComboItem("Debug", 0));
        debugCombo.AddItem(new ComboItem("Release", 1));

        hb1.Add(exportBtn);
        //hb1.Add(publishBtn);
        //hb1.Add(configPublishBtn);
        hb1.Add(debugCombo);

        hs1.Add(hb1);
        hs1.Add(hs2);
        hs2.Add(hb2);
        hs2.Add(hb3);
        #endregion


        #endregion
        s_root.RootCtrl = hs1;

        UpdateDebugCombobox();
        PackageExportToolModel.GetInstance().onExportComplete = OnExportComplete;
        //Modify by liteng for 发布工具改善 end

        #region 原有代码
        //HSpliterCtrl hspliter = new HSpliterCtrl();
        //hspliter.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30f, true);

        //上方树状视图
        //VBoxCtrl vb0 = new VBoxCtrl();
        //hspliter.Add(vb0);

        ////下方工具条
        //HBoxCtrl hb0 = new HBoxCtrl();
        //hspliter.Add(hb0);

        //TreeViewCtrl treeView = new TreeViewCtrl();
        //treeView.Name = "_MainTreeView";
        //vb0.Add(treeView);

        //TextBoxCtrl tbVersion = new TextBoxCtrl();
        //tbVersion.Size = new Rect(0, 0, 30, 20);
        //tbVersion.Name = "_VersionBox";
        //tbVersion.Caption = "版本号";
        //tbVersion.Enable = true;
        //tbVersion.layoutConstraint = LayoutConstraint.GetInspectorViewConstraint(10, 50);

        //TextBoxCtrl tbFileName = new TextBoxCtrl();
        //tbFileName.Size = new Rect(0, 0, 60, 20);
        //tbFileName.Name = "_SaveFileName";
        //tbFileName.Caption = "输出文件";
        //tbFileName.Enable = true;
        //tbFileName.layoutConstraint = LayoutConstraint.GetInspectorViewConstraint(10, 50);

        //Rect btnRect = new Rect(0, 0, 60, 20);

        //ButtonCtrl ChooseFileBtn = new ButtonCtrl();
        //ChooseFileBtn.Caption = "选择";
        //ChooseFileBtn.Name = "_ChooseFileName";
        //ChooseFileBtn.Size = btnRect;
        //ChooseFileBtn.onClick = OnChooseFileNameButtonClick;

        //ButtonCtrl okBtn = new ButtonCtrl();
        //okBtn.Caption = "确定";
        //okBtn.Name = "_OkButton";
        //okBtn.Size = btnRect;
        //okBtn.onClick = OnOkButtonClick;

        //ButtonCtrl cancelBtn = new ButtonCtrl();
        //cancelBtn.Caption = "取消";
        //cancelBtn.Name = "_CancelButton";
        //cancelBtn.Size = btnRect;
        //cancelBtn.onClick = OnCancelButtonClick;

        //hb0.Add(tbVersion);
        //hb0.Add(tbFileName);
        //hb0.Add(ChooseFileBtn);
        //hb0.Add(okBtn);
        //hb0.Add(cancelBtn);
        #endregion
    }

    static void UpdateTreeView()
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }

        treeView.Clear();

        string[] allAssetPaths = ResourceManageToolUtility.GetAllAssetPaths();

        foreach (var path in allAssetPaths)
        {
            if (ResourceManageToolUtility.PathIsFolder(path))
            {
                AddAssetToResourceTreeView(path);
            }
        }
    }

    static void AddAssetToResourceTreeView(string path)
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;

        if (treeView == null)
        {
            return;
        }

        string totalPath = path;
        string currPath = path;
        List<TreeViewNode> currLevelNodeList = treeView.Roots;
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
            string currNodeFullPath = totalPath.Substring(0, len - 1);
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
                newNode.image = ResourceManageToolUtility.GetCachedIcon(path);
                if (string.Equals(pathNodeName.ToLower(), "assets"))
                    newNode.state.IsExpand = true;
                else
                    newNode.state.IsExpand = false;

                TreeViewNodeUserParam userParam = new TreeViewNodeUserParam();

                bool toggleState = false;
                //Modify by liteng for 发布工具改善
                foreach (string p in PackageExportToolModel.GetInstance().GetPackageInfo().ExportAssets)
                {
                    if (p.Equals(currNodeFullPath))
                    {
                        toggleState = true;
                    }
                }
                userParam.param = toggleState;
                newNode.state.userParams.Add(userParam);


                if (parentNode == null)
                {//说明需要作为根节点插入树视图中
                    currLevelNodeList.Add(newNode);
                }
                else
                {
                    parentNode.Add(newNode);
                    //Add by liteng for 发布工具改善 start
                    if (true == toggleState)
                    {
                        parentNode.state.IsExpand = true;
                    }
                    //Add by liteng for 发布工具改善 end
                }
                parentNode = newNode;
                currLevelNodeList = newNode.children;
            }
        }
    }

    static void OnExportBtnClick(EditorControl c)
    {
        CollectAllPaths();
        PackageExportToolModel.GetInstance().Export();
    }

    static void OnPublishBtnClick(EditorControl c)
    {
        PackageExportToolModel.GetInstance().Publish();
    }

    static void OnConfigPublishPathBtnClick(EditorControl c)
    {

    }

    static void OnDebugComboSelect(EditorControl c, object value)
    {
        if (null == value)
        {
            return;
        }

        int sel = (int)value;
 
        switch(sel)
        {
            case 0:
                EditorHelper.SetPublishMode(true);
                break;
            case 1:
                EditorHelper.SetPublishMode(false);
                break;
            default:
                break;
        }

        RequestRepaint();
    }

    static void OnExportComplete()
    {
        string exportPath = null;

        switch (PackageExportToolModel.GetInstance().ErrorType)
        {
            case PACKAGE_FAILED_TYPE.PACKAGE_FAILED_NONEERROR:
                EditorUtility.DisplayDialog("导出成功",
                                            "Package保存至\n\n" + PackageExportToolModel.GetInstance().GetExportPath(),
                                            "确认");
                EditorHelper.SetPublishMode(true);
                UpdateDebugCombobox();

                break;
            case PACKAGE_FAILED_TYPE.PACKAGE_FAILED_EXPORT_PATH_ERROR:
                if (EditorUtility.DisplayDialog("导出失败", "Package导出路径无效，请重新配置", "配置", "取消"))
                {
                    exportPath = EditorUtility.SaveFolderPanel("配置导出路径", PackageExportToolModel.GetInstance().GetExportPath(), "");
                    if (!string.IsNullOrEmpty(exportPath))
                    {
                        PackageExportToolModel.GetInstance().Export(exportPath);
                    }
                }

                break;
            case PACKAGE_FAILED_TYPE.PACKAGE_FAILED_NONE_ASSETS_ERROR:
                EditorUtility.DisplayDialog("导出失败",
                                            "未选中任何资源，请选择资源",
                                            "确认");

                break;
            case PACKAGE_FAILED_TYPE.PACKAGE_FAILED_VERSION_NONE_ERROR:
                EditorUtility.DisplayDialog("导出失败",
                                            "XML配置文件生成失败，请确认版本号是否正确",
                                            "确认");

                break;
            case PACKAGE_FAILED_TYPE.PACKAGE_FAILED_MANUAL_PATH_ERROR:
                EditorUtility.DisplayDialog("导出失败",
                                            "手册路径无效，请重新指定",
                                            "确认");

                break;
            default:
                break;
        }

        RequestRepaint();
    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnTreeViewNodeToggle(EditorControl c, object value)
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }

        RecalcTreeNodes();
        RequestRepaint();
    }
    static void OnInspector(EditorControl c, object target)
    {        
        GUILayout.Space(50f);

        GUILayout.Label("发布信息：", GUILayout.Width(60f));
    
        GUILayout.Space(20f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("版本号：  主版本", GUILayout.Width(90f));
        GUI.SetNextControlName("主版本");
        m_MainVer = GUILayout.TextField(m_MainVer, GUILayout.Width(60));

        GUILayout.Space(5f);

        GUILayout.Label("功能升级号", GUILayout.Width(60f));
        GUI.SetNextControlName("功能升级号");
        m_UpgradeVer = GUILayout.TextField(m_UpgradeVer, GUILayout.Width(60));
        GUILayout.Space(5f);

        GUILayout.Label("Perforce版本号", GUILayout.Width(90f));
        GUI.SetNextControlName("Perforce版本号");
        m_P4Ver = GUILayout.TextField(m_P4Ver, GUILayout.Width(80));
        GUILayout.Space(5f);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(20f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Package导出路径:", GUILayout.Width(100f));
        GUI.SetNextControlName("Package导出路径");
        m_ExportPath = GUILayout.TextField(m_ExportPath, GUILayout.Width(400));
        if (GUILayout.Button("配置路径", GUILayout.Width(90f)))
        {
            ConfigExportPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20f);
        m_IsWithManual = GUILayout.Toggle(m_IsWithManual, "是否附带手册");
        if (m_IsWithManual)
        {
            PackageExportToolModel.GetInstance().IsWithManual = true;
            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("手册路径:", GUILayout.Width(100f));
            GUI.SetNextControlName("手册路径");
            m_ManualPath = GUILayout.TextField(m_ManualPath, GUILayout.Width(400));
            if (GUILayout.Button("配置路径", GUILayout.Width(90f)))
            {
                ConfigManualPath();
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            PackageExportToolModel.GetInstance().IsWithManual = false;
        }

        //GUILayout.Space(20f);
        //GUILayout.BeginHorizontal();  
        //GUILayout.Label("AB路径:", GUILayout.Width(100f));
        //GUILayout.TextField("", GUILayout.Width(400));
        //GUILayout.EndHorizontal();

        if(Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.Used)
        {
            switch(GUI.GetNameOfFocusedControl())
            {
                case "Package导出路径":
                    PackageExportToolModel.GetInstance().UpdateExportPath(m_ExportPath);
                    break;
                case "手册路径":
                    PackageExportToolModel.GetInstance().UpdateManualPath(m_ManualPath);
                    break;
                case "主版本":
                case "功能升级号":
                case "Perforce版本号":
                    PackageExportToolModel.GetInstance().SetMainVer(m_MainVer);
                    PackageExportToolModel.GetInstance().SetUpgradeVer(m_UpgradeVer);
                    PackageExportToolModel.GetInstance().SetP4Ver(m_P4Ver);
                    break;
                default:
                    break;
            }

            RequestRepaint();
        }

        InitInsipectorText();
    }

    static void OnEnable(EditorRoot root)
    {
        //Modify by liteng for 发布工具改善 start
        PackageExportToolModel.GetInstance().ReadExportPath();
        PackageExportToolModel.GetInstance().ReadManualPath();
        PackageExportToolModel.GetInstance().Load();

        m_ExportPath = PackageExportToolModel.GetInstance().GetExportPath();
        m_ManualPath = PackageExportToolModel.GetInstance().GetManualPath();
        
        m_MainVer = PackageExportToolModel.GetInstance().PackageVer.MainVers;
        m_UpgradeVer = PackageExportToolModel.GetInstance().PackageVer.UpgradeVer;
        m_P4Ver = PackageExportToolModel.GetInstance().PackageVer.P4Ver;

        //Modify by liteng for 发布工具改善 end

        UpdateTreeView();
    }

    static void OnDestroy(EditorRoot root)
    {
    }

    static void RequestRepaint()
    {
        s_root.RequestRepaint();
    }

    static void RecalcTreeNodes()
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }

        foreach (var root in treeView.Roots)
        {
            TreeViewCtrl.PreorderTraverse(root, RecalcTreeNodeVisitCallback);
        }
    }

    static bool RecalcTreeNodeVisitCallback(TreeViewNode n)
    {
        bool parentAlreadyToggled = false;
        TreeViewNode parent = n.parent;
        while (parent != null)
        {
            if ((bool)parent.state.userParams[0].param)
            {
                parentAlreadyToggled = true;
            }
            parent = parent.parent;
        }

        if (parentAlreadyToggled)
        {
            n.state.userParams[0].param = false;
        }
        return true;
    }

    static void CollectAllPaths()
    {
        //Modify by liteng for 发布工具改善
        PackageExportToolModel.GetInstance().GetPackageInfo().ExportAssets.Clear();

        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }

        foreach (var root in treeView.Roots)
        {
            TreeViewCtrl.PreorderTraverse(root, CollectAllPathsTreeNodeVisitCallback);
        }
    }

    static bool CollectAllPathsTreeNodeVisitCallback(TreeViewNode n)
    {
        if ((bool)n.state.userParams[0].param)
        {
            string path = n.GetPathString();
            //Modify by liteng for 发布工具改善
            PackageExportToolModel.GetInstance().AddAssets(path);
        }
        return true;
    }

    static void ConfigExportPath()
    {
        string exportPath = null;
        exportPath = EditorUtility.SaveFolderPanel("配置导出路径", PackageExportToolModel.GetInstance().GetExportPath(), "");
        if (!string.IsNullOrEmpty(exportPath))
        {
            PackageExportToolModel.GetInstance().UpdateExportPath(exportPath);
            m_ExportPath = PackageExportToolModel.GetInstance().GetExportPath();

            RequestRepaint();
        }
    }

    static void ConfigManualPath()
    {
        string manualPath = null;
        manualPath = EditorUtility.SaveFolderPanel("配置手册路径", PackageExportToolModel.GetInstance().GetManualPath(), "");
        if (!string.IsNullOrEmpty(manualPath))
        {
            PackageExportToolModel.GetInstance().UpdateManualPath(manualPath);
            m_ManualPath = PackageExportToolModel.GetInstance().GetManualPath();

            RequestRepaint();
        }
    }

    static void UpdateDebugCombobox()
    {
        ComboBoxCtrl<int> debugCombo = s_root.FindControl("_DebugCombo") as ComboBoxCtrl<int>;

        if(null == debugCombo)
        {
            return;
        }

        if (EditorHelper.IsDebugMode())
        {
            debugCombo.CurrValue = 0;
        }
        else
        {
            debugCombo.CurrValue = 1;
        }
    }

    static void InitInsipectorText()
    {

        if ((GUI.GetNameOfFocusedControl() != "主版本")
         && (GUI.GetNameOfFocusedControl() != "功能升级号")
         && (GUI.GetNameOfFocusedControl() != "Perforce版本号"))
        {
            m_MainVer = PackageExportToolModel.GetInstance().PackageVer.MainVers;
            m_UpgradeVer = PackageExportToolModel.GetInstance().PackageVer.UpgradeVer;
            m_P4Ver = PackageExportToolModel.GetInstance().PackageVer.P4Ver;

            RequestRepaint();
        }

        if (GUI.GetNameOfFocusedControl() != "Package导出路径")
        {
            m_ExportPath = PackageExportToolModel.GetInstance().GetExportPath();

            RequestRepaint();
        }
    }
}

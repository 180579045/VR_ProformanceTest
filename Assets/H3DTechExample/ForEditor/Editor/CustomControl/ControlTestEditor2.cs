using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
public class ControlTestEditor2
{
    static EditorRoot m_EditorRoot = null;                        //根控件

    [UnityEditor.MenuItem("Assets/H3D/控件测试工具/控件置灰测试2")]
    [UnityEditor.MenuItem("H3D/UI/控件测试工具/控件置灰测试2")]
    static void Init()
    {
        EditorRoot root = EditorManager.GetInstance().FindEditor("控件置灰测试2");
        if (root == null)
        {
            EditorManager.GetInstance().RemoveEditor("控件置灰测试2");
            root = EditorManager.GetInstance().CreateEditor("控件置灰测试2", false, InitControls);
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
        //m_EditorRoot.position = new Rect(100f, 100f, 1920f, 1080f);

        m_EditorRoot.onEnable = OnEnable;
        //m_EditorRoot.onDisable = OnDisable;

        //Rect btnRect = new Rect(0, 0, 60, 20);

        HSpliterCtrl hs1 = new HSpliterCtrl();
        hs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200f);
      
        VSpliterCtrl vs1 = new VSpliterCtrl();
        vs1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(400f);

        VBoxCtrl hb1 = new VBoxCtrl();
        //hb1.onDoubleClick = OnInspectorDoubleClick;
        //hb1.onClick = OnInspectorClick;
        //hb1.onOnPress = OnInspectorPress;

        HBoxCtrl hb2 = new HBoxCtrl();

        //VBoxCtrl tabBox = new VBoxCtrl();
        VBoxCtrl treeGroup = new VBoxCtrl();
        VBoxCtrl tabGroup = new VBoxCtrl();
        treeGroup.onDoubleClick = OnVboxDoubleClick;
        treeGroup.onClick = OnVboxClick;
        treeGroup.onOnPress = OnVboxPress;

        VBoxCtrl VbGroup = new VBoxCtrl();
        VBoxCtrl VbGroup2 = new VBoxCtrl();

        VBoxCtrl VbScroll = new VBoxCtrl(true);      //上方菜单栏

        TreeViewCtrl treeView = new TreeViewCtrl();
        treeView.Name = "_MainTreeView";
        treeView.onDoubleClick = OnTreeDoubleClick;
        treeView.onClick = OnTreeClick;
        treeView.onOnPress = OnTreePress;

        ButtonCtrl treeViewEnableBtn = new ButtonCtrl();
        treeViewEnableBtn.Name = "_TreeViewEnableBtn";
        treeViewEnableBtn.Caption = "无效";
        treeViewEnableBtn.UserDefData = treeView;
        treeViewEnableBtn.onClick = OnEnableBtnClick;

        
        TabViewCtrl tabView = new TabViewCtrl();
        tabView.onDoubleClick = OnTabDoubleClick;
        tabView.onClick = OnTabClick;
        tabView.onOnPress = OnTabPress;

        VbGroup.Caption = "Test";
        VbGroup2.Caption = "Test2";

        ButtonCtrl tabViewEnableBtn = new ButtonCtrl();
        tabViewEnableBtn.Name = "_TabViewEnableBtn";
        tabViewEnableBtn.Caption = "无效";
        tabViewEnableBtn.UserDefData = tabView;
        tabViewEnableBtn.onClick = OnEnableBtnClick;

        InspectorViewCtrl inspector = new InspectorViewCtrl();
        inspector.Name = "inspector";
        inspector.onInspector = InitInspectorView;
        inspector.onDoubleClick = OnInspectorDoubleClick;
        inspector.onClick = OnInspectorClick;
        inspector.onOnPress = OnInspectorPress;

        ButtonCtrl inspectorEnableBtn = new ButtonCtrl();
        inspectorEnableBtn.Name = "_InspectorViewEnableBtn";
        inspectorEnableBtn.Caption = "无效";
        inspectorEnableBtn.UserDefData = inspector;
        inspectorEnableBtn.onClick = OnEnableBtnClick;

        TextBoxCtrl textBox = new TextBoxCtrl();
        textBox.Name = "_TextBox";


        hs1.Add(hb1);
        hs1.Add(hb2);

        hb1.Add(inspector);
        hb1.Add(inspectorEnableBtn);
        hb1.Add(textBox);

        hb2.Add(vs1);

        vs1.Add(treeGroup);
        vs1.Add(tabGroup);

        treeGroup.Add(treeViewEnableBtn);
        treeGroup.Add(treeView);


        tabGroup.Add(tabViewEnableBtn);
        tabGroup.Add(tabView);


        tabView.Add(VbGroup);
        tabView.Add(VbGroup2);
        VbGroup.Add(VbScroll);

        for (int i = 0; i < 50; i++)
        {
            VBoxCtrl subVbox = new VBoxCtrl();
            ButtonCtrl subButton = new ButtonCtrl();
            subButton.Caption = "TestBtn" + i.ToString();
            subButton.onClick = OnTestBtnClick;
            subVbox.Add(subButton);
            VbScroll.Add(subVbox);
        }

        m_EditorRoot.RootCtrl = hs1;
    }

    static private void OnEnableBtnClick(EditorControl c)
    {
        ButtonCtrl btn = c as ButtonCtrl;
        if (null == btn)
        {
            return;
        }

        EditorControl targetCtrl = btn.UserDefData as EditorControl;

        if (targetCtrl != null)
        {
            targetCtrl.Enable = !targetCtrl.Enable;
            if (targetCtrl.Enable)
            {
                btn.Caption = "无效";
            }
            else
            {
                btn.Caption = "有效";
            }
        }
    }

    static void InitTab(TabViewCtrl tab)
    {

        VBoxCtrl boxWithScroll = new VBoxCtrl(true);
   
        tab.Add(boxWithScroll);

        InitBoxWithScroll(boxWithScroll);
    }

    static void InitBoxWithScroll(VBoxCtrl boxWithScroll)
    {
        HBoxCtrl h = new HBoxCtrl();

        boxWithScroll.Add(h);

        Rect rc = new Rect(0, 0, 70, 16);
        ButtonCtrl btnAdd = new ButtonCtrl();
        btnAdd.Caption = "增加";
        btnAdd.Size = rc;
        h.Add(btnAdd);

        ButtonCtrl btnDel = new ButtonCtrl();
        btnDel.Caption = "移除";
        btnDel.Size = rc;
        h.Add(btnDel);

        for (int i = 0; i < 5; ++i)
        {
            VBoxCtrl subCtrl = new VBoxCtrl();
            boxWithScroll.Add(subCtrl);
            InitSubCtrlTest(subCtrl);
        }

    }

    static void InitSubCtrlTest(VBoxCtrl SubCtrl)
    {
        DataFieldCtrl<float> m_df_finishFrame = null;
        TextBoxCtrl m_tb_finishActions = null;

        m_df_finishFrame = new DataFieldCtrl<float>(0);
        m_df_finishFrame.Caption = "控件1";
        SubCtrl.Add(m_df_finishFrame);

        m_tb_finishActions = new TextBoxCtrl();
        m_tb_finishActions.Caption = "控件2";
        SubCtrl.Add(m_tb_finishActions);
    }

    static void OnEnable(EditorRoot root)
    {
        UpdateTreeView();
    }

    static void UpdateTreeView()
    {
        TreeViewCtrl treeView = m_EditorRoot.FindControl("_MainTreeView") as TreeViewCtrl;
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
        TreeViewCtrl treeView = m_EditorRoot.FindControl("_MainTreeView") as TreeViewCtrl;

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
                newNode.state.IsExpand = true;
                TreeViewNodeUserParam userParam = new TreeViewNodeUserParam();

                bool toggleState = false;
                foreach (var p in ResourceManageConfig.GetInstance().Paths)
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
                }
                parentNode = newNode;
                currLevelNodeList = newNode.children;
            }
        }
    }

    static void OnCancelButtonClick(EditorControl c)
    {
        // s_root.ShutDown();
        UpdateTreeView();
        TreeViewCtrl refTree = m_EditorRoot.FindControl("_MainTreeView") as TreeViewCtrl;
      
        if (refTree != null)
        {
            refTree.currSelectPath = "Assets";
            if (refTree.currSelectPath.Equals("Assets/Resources"))
            {
                refTree.currSelectPath = "Assets/Resources/NGUI";
                return;
            }

            //if (refTree.currSelectPath.Equals("Assets/H3DTechTest/ForScripts/CommonControl/H3DScrollView"))
            //{
            //    refTree.currSelectPath = "Assets/H3DTechTest";
            //}
        }
    }

    static private void InitInspectorView(EditorControl c, object obj)
    {
        if (GUILayout.Button("Test1", GUILayout.Width(60f)))
        {
            EditorUtility.DisplayDialog("测试1", "Button可用", "确认");
        }

        if (GUILayout.Button("Test2", GUILayout.Width(60f)))
        {
            EditorUtility.DisplayDialog("测试2", "Button可用", "确认");
        }

    }

    static private void OnInspectorDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("Inspector", "Inspector双击", "确认");
    }

    static private void OnInspectorClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("Inspector", "Inspector单击", "确认");
    }

    static private void OnInspectorPress(EditorControl c, object clickObject)
    {
        TextBoxCtrl textBoxCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (null == textBoxCtrl)
        {
            return;
        }

        textBoxCtrl.Text += "a";
    }

    static private void OnTestBtnClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("测试1", "Button可用", "确认");
    }

    static private void OnTabDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("TabCtrl", "TabCtrl双击", "确认");
    }

    static private void OnTabClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("TabCtrl", "TabCtrl单击", "确认");
    }

    static private void OnTabPress(EditorControl c, object clickObject)
    {
        TextBoxCtrl textBoxCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (null == textBoxCtrl)
        {
            return;
        }

        textBoxCtrl.Text += "a";
    }

    static private void OnTreeDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("TreeViewCtrl", "TreeViewCtrl双击", "确认");
    }

    static private void OnTreeClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("TreeViewCtrl", "TreeViewCtrl单击", "确认");
    }

    static private void OnVboxDoubleClick(EditorControl c, object clickObject)
    {
        EditorUtility.DisplayDialog("VBox", "VBox双击", "确认");
    }

    static private void OnVboxPress(EditorControl c, object clickObject)
    {
        TextBoxCtrl textBoxCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (null == textBoxCtrl)
        {
            return;
        }

        textBoxCtrl.Text += "a";
    }

    static private void OnVboxClick(EditorControl c)
    {
        EditorUtility.DisplayDialog("VBox", "VBox单击", "确认");
    }
    static private void OnTreePress(EditorControl c, object clickObject)
    {
        TextBoxCtrl textBoxCtrl = m_EditorRoot.FindControl("_TextBox") as TextBoxCtrl;
        if (null == textBoxCtrl)
        {
            return;
        }

        textBoxCtrl.Text += "a";
    }

}
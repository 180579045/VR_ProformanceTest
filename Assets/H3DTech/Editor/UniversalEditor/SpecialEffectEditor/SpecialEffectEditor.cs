using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
public enum SPEITEMTYPE
{
    TYPE_EDITSPE_ELEM = 0,
    TYPE_REFSPE,
    TYPE_DEFAULT = -1,
}
public class SpeEditorTimeLineItem
{
    public SPEITEMTYPE ItemType = SPEITEMTYPE.TYPE_DEFAULT;
    public bool Enable = true;
    public bool Visiable = true;
    public TimeLineItem RealTimeLineItem = null;
    public SpeEditorTimeLineItem Parent = null;
    public List<SpeEditorTimeLineItem> Children = new List<SpeEditorTimeLineItem>();
    public SpecialEffectElement BlindSpeItem = null;
    public SpecialEffectEditProxy RefSpe = null;
    public SpeEditorTimeLineItem Copy()
    {
        return this.MemberwiseClone() as SpeEditorTimeLineItem;
    }
    public void Clear()
    {

    }
}

public class SpecialEffectEditor
{

    static EditorRoot s_root = null;

    static private string m_helpURL = "http://192.168.2.121:8090/pages/viewpage.action?pageId=6619470";

    [MenuItem("H3D/特效编辑/特效编辑器")]
    static void Init()
    {
        EditorRoot root =
        EditorManager.GetInstance().FindEditor("特效编辑器");
        if (root == null)
        {
            root = EditorManager.GetInstance().CreateEditor("特效编辑器", false, InitControls);
        }
    }

    //构建控件
    public static void InitControls(EditorRoot editorRoot)
    {

        //注册命令管理器变更通知回调
        EditorCommandManager.GetInstance().callback += OnCmdMgrChange;

        s_root = editorRoot;

        //设置默认大小
        //s_root.position = new Rect(200f, 100f, 1024f, 900f);

        {//注册Model变化回调
            SpecialEffectEditorModel.GetInstance().onSetNewEditTarget = OnSetNewEditTarget;
            SpecialEffectEditorModel.GetInstance().onEditTargetDestroy = OnSpeDestroy;
            SpecialEffectEditorModel.GetInstance().onEditTargetValueChange = OnSpeValueChange;
            SpecialEffectEditorModel.GetInstance().onEditTargetSaved = OnSpeSaved;
            SpecialEffectEditorModel.GetInstance().onEditTargetDirty = OnSpeSetDirty;
            SpecialEffectEditorModel.GetInstance().onCurrPlayTimeChange = OnCurrPlayTimeChange;
            SpecialEffectEditorModel.GetInstance().onActionListChange = OnActionListChange;
            SpecialEffectEditorModel.GetInstance().onRefSpeListChange = OnRefSpeListChange;

            SpecialEffectEditorModel.GetInstance().onVirtualSceneCreate = OnVirtualSceneCreate;
            SpecialEffectEditorModel.GetInstance().onRefModelOpen = OnRefModelOpen;
            SpecialEffectEditorModel.GetInstance().onRefModelDestroy = OnRefModelDestroy;
            SpecialEffectEditorModel.GetInstance().onItemSelectChange = OnSpeItemSelectChanged;

            SpecialEffectEditorModel.GetInstance().onGridMeshVisiableChange = OnGridMeshVisiableChange;
        }

        {//注册窗体回调
            editorRoot.onEnable = OnEditorEnable;
            editorRoot.onDisable = OnEditorDisable;
            editorRoot.onUpdate = OnEditorUpdate;
            editorRoot.onDestroy = OnEditorDestroy;
            editorRoot.onGUI = OnEditorGUI;
            editorRoot.onMessage = OnEditorMessage;
        }

        editorRoot.RootCtrl = new HSpliterCtrl();

        (editorRoot.RootCtrl as SpliterCtrl).Dragable = true;

        HSpliterCtrl h1 = new HSpliterCtrl();
        HSpliterCtrl h2 = new HSpliterCtrl();
        HSpliterCtrl h3 = new HSpliterCtrl();

        HSpliterCtrl h4 = new HSpliterCtrl();

        VSpliterCtrl v1 = new VSpliterCtrl();
        VSpliterCtrl v2 = new VSpliterCtrl();
        v2.Dragable = true;
        //Inspector右侧垫条
        VSpliterCtrl v3 = new VSpliterCtrl();

        editorRoot.RootCtrl.layoutConstraint = LayoutConstraint.GetSpliterConstraint(700);
        h1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(10);
        h2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(20);
        h3.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30);

        h4.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30, true);

        //分割右侧Inspector的分割器
        v1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(300, true);
        v2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200);

        //Inspector右侧垫条
        v3.layoutConstraint = LayoutConstraint.GetSpliterConstraint(20f, true);

        editorRoot.RootCtrl.Add(h1);
        editorRoot.RootCtrl.Add(h3);

        //用于上方状态条
        EditorControl vb0 = new VBoxCtrl();
        vb0.Size = new Rect(0, 0, 1000, 20);
        vb0.layoutConstraint = LayoutConstraint.GetToolBarConstraint(30f);
        h1.Add(vb0);
        h1.Add(v1);

        //右侧Inspector
        EditorControl vb1 = new VBoxCtrl();
        vb1.Size = new Rect(0, 0, 200, 500);
        vb1.layoutConstraint = LayoutConstraint.GetInspectorViewConstraint(200, 200);
        v1.Add(h2);
        v1.Add(v3);
        v3.Add(vb1);
        EditorControl inspectorRightVb = new VBoxCtrl();
        v3.Add(inspectorRightVb);
        //v1.Add(vb1);
        v1.Dragable = true;

        //用于菜单
        EditorControl hb0 = new HBoxCtrl();
        hb0.Size = new Rect(0, 0, 1000, 30);
        hb0.layoutConstraint = LayoutConstraint.GetToolBarConstraint(30f);

        //用于主视图
        EditorControl hb1 = new HBoxCtrl();
        hb1.Size = new Rect(0, 0, 600, 600);
        hb1.layoutConstraint = LayoutConstraint.GetExtensibleViewConstraint();

        h2.Add(hb0);
        h2.Add(hb1);

        //播放控制区域
        EditorControl hb2 = new HBoxCtrl();
        hb2.Size = new Rect(0, 0, 1000, 30);
        hb2.layoutConstraint = LayoutConstraint.GetToolBarConstraint(30f);

        h3.Add(hb2);
        //h3.Add(v2);
        h3.Add(h4);

        //用来放置底部状态条
        HBoxCtrl hb3 = new HBoxCtrl();
        h4.Add(v2);
        h4.Add(hb3);

        //动画元素列表区域
        EditorControl vb2 = new VBoxCtrl();
        vb2.Size = new Rect(0, 0, 200, 300);
        vb2.layoutConstraint = LayoutConstraint.GetListViewConstraint(200, 200);

        //动画时间线
        EditorControl vb3 = new VBoxCtrl();
        vb3.Size = new Rect(0, 0, 800, 300);
        vb3.layoutConstraint = LayoutConstraint.GetListViewConstraint(200, 200);

        v2.Add(vb2);
        v2.Add(vb3);


        _BuildMenuButtons(hb0);

        _BuildFrameControlBar(hb2);

        EditorControl stateLabel = new LabelCtrl();
        stateLabel.Caption = "请打开特效!";
        //vb0.Add(stateLabel); 

        EditorControl mainView = new MainViewCtrl();
        mainView.onAcceptDragObjs = OnMainViweAcceptDragingObjs;
        mainView.onDragingObjs = OnMainViewDragingObjs;
        mainView.onDropObjs = OnMainViewDropObjs;
        (mainView as MainViewCtrl).IsShowAxis = true;

        hb1.Add(mainView);


        TabViewCtrl tabView = new TabViewCtrl();

        SliderCtrl<float> speSpeedScaleSlider = new SliderCtrl<float>();
        speSpeedScaleSlider.Name = "_SpeSpeedScaleSlider";
        speSpeedScaleSlider.Caption = "Spe速度缩放:";
        speSpeedScaleSlider.ValueRange = new Vector2(0f, 10f);
        speSpeedScaleSlider.layoutConstraint.expandWidth = true;
        speSpeedScaleSlider.CurrValue = SpecialEffectEditorModel.GetInstance().SpeSpeedScale; 
        speSpeedScaleSlider.onValueChange = OnSpeSpeedScaleChange;

        SliderCtrl<float> modelSpeedScaleSlider = new SliderCtrl<float>();
        modelSpeedScaleSlider.Name = "_ModelSpeedScale";
        modelSpeedScaleSlider.Caption = "模型速度缩放:";
        modelSpeedScaleSlider.ValueRange = new Vector2(0f, 10f);
        modelSpeedScaleSlider.layoutConstraint.expandWidth = true;
        modelSpeedScaleSlider.CurrValue = SpecialEffectEditorModel.GetInstance().ModeSpeedScale;
        modelSpeedScaleSlider.onValueChange = OnModelSpeedScaleChange;

        vb1.Add(tabView);

        vb1.Add(speSpeedScaleSlider);
        vb1.Add(modelSpeedScaleSlider);

        InspectorViewCtrl inspectorView = new InspectorViewCtrl();
        inspectorView.Name = "_SpeElemInspector";
        inspectorView.Caption = "特效元素";
        inspectorView.editTarget = new SpeElemInspectorTarget();
        inspectorView.onInspector = SpecialEffectEditorInspectorRenderDelegate.OnSpeElemInspector;
        inspectorView.onValueChange = OnSpeElemInspectorValueChange;
        tabView.Add(inspectorView);

   
        TreeViewCtrl treeTabView = new TreeViewCtrl();
        treeTabView.Name = "_RefModelTreeView";
        treeTabView.Caption = "模型";
        treeTabView.onValueChange = OnRefModelTreeViewValueChanged;
        tabView.Add(treeTabView);

        InspectorViewCtrl speTabView = new InspectorViewCtrl();
        speTabView.Name = "_SpeInspector";
        speTabView.Caption = "特效属性";
        speTabView.onInspector = SpecialEffectEditorInspectorRenderDelegate.OnSpeInspector;
        speTabView.onValueChange = OnSpeInspectorValueChange;
        tabView.Add(speTabView);

        VBoxCtrl actionListVBox = new VBoxCtrl();
        actionListVBox.Caption = "动作列表";
        actionListVBox.layoutConstraint = LayoutConstraint.GetExtensibleViewConstraint();
    
        ListViewCtrl actionListListView = new ListViewCtrl();
        actionListListView.Name = "_RefModelActionList";
        actionListListView.Caption = "动作列表";
        actionListListView.onItemSelected = OnActionListSelectionChange;
        actionListVBox.Add(actionListListView);
        tabView.Add(actionListVBox);


        Rect clearActionBtnRect = new Rect(0, 0, 60, 20);
        ButtonCtrl clearActionBtnCtrl = new ButtonCtrl();
        clearActionBtnCtrl.Name = "_ClearActionBtn";
        clearActionBtnCtrl.Caption = "清空动作";
        clearActionBtnCtrl.BtnColor = Color.yellow;
        clearActionBtnCtrl.Size = clearActionBtnRect;
        clearActionBtnCtrl.layoutConstraint = LayoutConstraint.GetExtensibleViewConstraint();
        actionListVBox.Add(clearActionBtnCtrl);
        clearActionBtnCtrl.onClick = OnClearAction;


        VBoxCtrl refSpeListVBox = new VBoxCtrl();
        refSpeListVBox.Caption = "参考特效";
        refSpeListVBox.layoutConstraint = LayoutConstraint.GetExtensibleViewConstraint();
        ListViewCtrl refSpeListListView = new ListViewCtrl();
        refSpeListListView.Name = "_RefSpeList";
        refSpeListListView.Caption = "参考特效";
        refSpeListListView.onItemSelected = OnRefSpeListSelectionChange;
        refSpeListVBox.Add(refSpeListListView);
        tabView.Add(refSpeListVBox);

        InspectorViewCtrl inspectorVirSceneView = new InspectorViewCtrl();
        inspectorVirSceneView.Name = "_VirtualSceneInspector";
        inspectorVirSceneView.Caption = "虚拟场景";
        inspectorVirSceneView.onInspector = SpecialEffectEditorInspectorRenderDelegate.OnVirtualSceneInspector;
        inspectorVirSceneView.editTarget = new VirturalSceneInspectorTarget();
        inspectorVirSceneView.onValueChange = OnVirtualSceneInspectorValueChange;
        tabView.Add(inspectorVirSceneView);

        Rect clearSpeBtnRect = new Rect(0, 0, 60, 20);
        ButtonCtrl clearSpeBtnCtrl = new ButtonCtrl();
        clearSpeBtnCtrl.Name = "_ClearSpeBtn";
        clearSpeBtnCtrl.Caption = "清空参考特效";
        clearSpeBtnCtrl.BtnColor = Color.yellow;
        clearSpeBtnCtrl.Size = clearSpeBtnRect;
        clearSpeBtnCtrl.layoutConstraint = LayoutConstraint.GetExtensibleViewConstraint();
        refSpeListVBox.Add(clearSpeBtnCtrl);
        clearSpeBtnCtrl.onClick = OnClearRefSpe;

        ListViewCtrl listView = new ListViewCtrl();
        listView.Name = "_SpeItemList";
        //vb2.Add(listView);
        listView.onItemSelected += OnTimeLineListViewSelectChange;
        listView.onScroll += OnTimeLineListViewScroll;

        TreeViewCtrl treeView = new TreeViewCtrl();
        treeView.Name = "_MainTreeView";
        vb2.Add(treeView);
        treeView.onItemSelected += OnTimeLineListViewSelectChange;
        treeView.onScroll += OnTimeLineListViewScroll;
        treeView.onCtrlBehaveChange += OnTreeViewBehaveChange;
        treeView.onValueChange += OnTreeViewValueChange;

        TimeLineViewCtrl timelineView = new TimeLineViewCtrl();  
        timelineView.onItemSelected += OnTimeLineSelectChange; 

        vb3.Add(timelineView);
    }


    static void _BuildFrameControlBar(EditorControl parent)
    {

        PlayCtrl playCtrl = new PlayCtrl();
        playCtrl.TotalTime = 10f;
        playCtrl.onValueChange += OnPlayCtrlValueChange;
        playCtrl.onStop = OnPlayCtrlStop;
        parent.Add(playCtrl);

    }

    static void _BuildMenuButtons(EditorControl parent)
    {
        Rect btnRect = new Rect(0, 0, 60, 20);
        //Rect undoRect = new Rect(0, 0, 120, 20);
        Rect colorCtrlRect = new Rect(0, 0, 200, 20);
        Rect returnBtnRect = new Rect(0, 0, 120, 20);



        ButtonCtrl returnBtn = new ButtonCtrl();
        returnBtn.Name = "_FinishEditBtn";
        returnBtn.Caption = "编辑完成";
        returnBtn.Size = returnBtnRect;
        returnBtn.onClick += OnReturnSpe;
        returnBtn.BtnColor = Color.green;
        returnBtn.Visiable = false;



        ButtonCtrl resetCameraBtn = new ButtonCtrl();
        resetCameraBtn.Caption = "相机复位";
        resetCameraBtn.Size = btnRect;
        resetCameraBtn.onClick += OnResetCamera;

        ColorCtrl setBkColorBtn = new ColorCtrl();
        setBkColorBtn.Caption = "设背景色";
        setBkColorBtn.Size = colorCtrlRect;
        setBkColorBtn.onValueChange += OnSetBkColorValueChanged;

        ButtonCtrl helpBtn = new ButtonCtrl();
        helpBtn.Caption = "帮助";
        helpBtn.Size = btnRect;
        helpBtn.onClick += OnHelp;

        TextBoxCtrl refModelScaleX = new TextBoxCtrl();
        refModelScaleX.Name = "_RefModelScaleX";
        refModelScaleX.Caption = "参考模型缩放: x";
        refModelScaleX.Text = "1.0";
        refModelScaleX.onValueChange = OnRefModelScaleXChange;
        TextBoxCtrl refModelScaleY = new TextBoxCtrl();
        refModelScaleY.Name = "_RefModelScaleY";
        refModelScaleY.Caption = "y";
        refModelScaleY.Text = "1.0";
        refModelScaleY.onValueChange = OnRefModelScaleYChange;
        TextBoxCtrl refModelScaleZ = new TextBoxCtrl();
        refModelScaleZ.Name = "_RefModelScaleZ";
        refModelScaleZ.Caption = "z";
        refModelScaleZ.Text = "1.0";
        refModelScaleZ.onValueChange = OnRefModelScaleZChange;
         

        parent.Add(returnBtn);
        parent.Add(resetCameraBtn);
        parent.Add(setBkColorBtn);
        parent.Add(helpBtn);
        parent.Add(refModelScaleX);
        parent.Add(refModelScaleY);
        parent.Add(refModelScaleZ);
    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnRefModelScaleXChange( EditorControl c , object v )
    { 
        TextBoxCtrl textBox = c as TextBoxCtrl;
        if( SpecialEffectEditorModel.GetInstance().GetRefModel() == null )
        {
            textBox.Text = "1.0";
            RequestRepaint();
            return;
        }

        Vector3 localScale = SpecialEffectEditorModel.GetInstance().GetRefModelScale();
        float scaleX = localScale.x;
        if ( !float.TryParse(textBox.Text, out scaleX) )
        {
            textBox.Text = localScale.x.ToString();
        }
        else
        {
            Vector3 newScale = new Vector3(scaleX, localScale.y, localScale.z);
            SpecialEffectEditorModel.GetInstance().SetRefModelScale(newScale);
        } 
    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnRefModelScaleYChange(EditorControl c, object v)
    { 
        TextBoxCtrl textBox = c as TextBoxCtrl; 
        if (SpecialEffectEditorModel.GetInstance().GetRefModel() == null)
        {
            textBox.Text = "1.0";
            RequestRepaint();
            return;
        }

        Vector3 localScale = SpecialEffectEditorModel.GetInstance().GetRefModelScale();
        float scaleY = localScale.y;
        if (!float.TryParse(textBox.Text, out scaleY))
        {
            textBox.Text = localScale.y.ToString();
        }
        else
        {
            Vector3 newScale = new Vector3(localScale.x, scaleY, localScale.z);
            SpecialEffectEditorModel.GetInstance().SetRefModelScale(newScale);
        } 
    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnRefModelScaleZChange(EditorControl c, object v)
    {
        TextBoxCtrl textBox = c as TextBoxCtrl;
        if (SpecialEffectEditorModel.GetInstance().GetRefModel() == null)
        {
            textBox.Text = "1.0";
            RequestRepaint();
            return;
        }

        Vector3 localScale = SpecialEffectEditorModel.GetInstance().GetRefModelScale();
        float scaleZ = localScale.z;
        if (!float.TryParse(textBox.Text, out scaleZ))
        {
            textBox.Text = localScale.z.ToString();
        }
        else
        {
            Vector3 newScale = new Vector3(localScale.x, localScale.y, scaleZ);
            SpecialEffectEditorModel.GetInstance().SetRefModelScale(newScale);
        } 
    }

    static void OnSpeSpeedScaleChange(EditorControl c, object value)
    {
        if(
               (null == c)
            || (null == value)
            )
        {
            return;
        }

        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();
        if(null == playCtrl)
        {
            return;
        }

        float fVaule = (float)value;

        playCtrl.SpeedScale = fVaule;

        if (
               (fVaule > 0.01f)
            && ((SpecialEffectEditorModel.GetInstance().ModeSpeedScale - 0f) > Mathf.Epsilon)
            )
        {
            SpecialEffectEditorModel.GetInstance().ModelCurrPlayTime = SpecialEffectEditorModel.GetInstance().GetCurrPlayTime() * (SpecialEffectEditorModel.GetInstance().ModeSpeedScale / fVaule);
        }

        SpecialEffectEditorModel.GetInstance().SpeSpeedScale = fVaule;
    }

    static void OnModelSpeedScaleChange(EditorControl c, object value)
    {
        if (
               (null == c)
            || (null == value)
            )
        {
            return;
        }

        float fVaule = (float)value;

        if (
               (fVaule > 0.01f)
            && (SpecialEffectEditorModel.GetInstance().SpeSpeedScale > 0.01f)
            )
        {
            SpecialEffectEditorModel.GetInstance().ModelCurrPlayTime = SpecialEffectEditorModel.GetInstance().GetCurrPlayTime() * (fVaule / SpecialEffectEditorModel.GetInstance().SpeSpeedScale);
        }

        SpecialEffectEditorModel.GetInstance().ModeSpeedScale = fVaule;

    }

    static void OnPlayCtrlStop(EditorControl c)
    {
        SpecialEffectEditorModel.GetInstance().StopSpe();
    }

    static void OnEditorMessage( ControlMessage msg )
    {
        
        switch( msg.msg )
        {
            case ControlMessage.Message.TIMELINECTRL_BEGIN_DRAG_TAG: 
            case ControlMessage.Message.TIMELINECTRL_DRAG_TAG: 
            case ControlMessage.Message.TIMELINECTRL_END_DRAG_TAG:
                if ((int)msg.param0 != 0)
                    break;
 
                TimeLineViewCtrl timelineView = msg.sender as TimeLineViewCtrl;
                if (timelineView != null)
                { 
                    SpecialEffectEditorModel.GetInstance().SetEditTargetStartTime(timelineView.Tags[0].time);
                }

                break;
            case ControlMessage.Message.TIMELINECTRL_BEGIN_DRAG_ITEMS:
                OnTimeLineDragItemsBegin(msg.sender, msg.param0 as List<TimeLineViewCtrl.ItemSelectInfo>); 
                break;
            case ControlMessage.Message.TIMELINECTRL_DRAG_ITEMS:
                OnTimeLineDragItems(msg.sender, msg.param0 as List<TimeLineViewCtrl.ItemSelectInfo>); 
                break;
            case ControlMessage.Message.TIMELINECTRL_END_DRAG_ITEMS:
                OnTimeLineDragItemsEnd(msg.sender, msg.param0 as List<TimeLineViewCtrl.ItemSelectInfo>); 
                break;
            default:
                break;
        }
    }


    static void OnOpenSpe(EditorControl c)
    { 
    }

    static void OnSaveSpe(EditorControl c)
    { 
    }

    static void OnReturnSpe(EditorControl c)
    {
        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();
        playCtrl.Stop();
        SpecialEffectEditorModel.GetInstance().RetargetSpeToOldTarget();
    }

    static void OnOpenModel(EditorControl c)
    {
        SpecialEffectEditorModel.GetInstance().OpenRefModelFromFile();
    }

    static void OnSetAction(EditorControl c)
    {
        SpecialEffectEditorModel.GetInstance().LoadActionFromFile();
    }

    static void OnClearAction(EditorControl c )
    {
        SpecialEffectEditorModel.GetInstance().ClearActionList();
    }

    static void OnClearRefSpe(EditorControl c )
    {
        SpecialEffectEditorModel.GetInstance().ClearRefSpeList();
    }

    static void OnResetCamera(EditorControl c)
    {
        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            {//初始化相机位置

                mainView.center = Vector3.zero;
                mainView.radius = 10.0f;

                Transform camTrans = mainView.camObj.transform;
                camTrans.localPosition = new Vector3(0f, 0f, -5f);
                camTrans.localRotation = Quaternion.identity;

                Transform assistantCamTrans = mainView.assistantCamObj.transform;
                assistantCamTrans.localPosition = new Vector3(mainView.assCentter.x, mainView.assCentter.y, mainView.assCentter.z - mainView.assRadius);
                assistantCamTrans.localRotation = Quaternion.identity;

                float angleRotateAroundUp = 135.0f;
                float angleRotateAroundRight = 45.0f;

                Vector3 localPos = (camTrans.localPosition - mainView.center).normalized * mainView.radius;

                Quaternion q0 = Quaternion.AngleAxis(angleRotateAroundUp, camTrans.up);
                camTrans.localPosition = q0 * localPos;
                camTrans.Rotate(Vector3.up, angleRotateAroundUp, Space.Self);

                Quaternion q1 = Quaternion.AngleAxis(angleRotateAroundRight, camTrans.right);
                camTrans.Rotate(Vector3.right, angleRotateAroundRight, Space.Self);
                camTrans.localPosition = q1 * camTrans.localPosition;
                camTrans.localPosition += mainView.center;

                if(mainView.IsShowAxis)
                {
                    Vector3 assLocalPos = (assistantCamTrans.localPosition - mainView.assCentter).normalized * mainView.assRadius;

                    Quaternion q2 = Quaternion.AngleAxis(angleRotateAroundUp, assistantCamTrans.up);
                    assistantCamTrans.localPosition = q2 * assLocalPos;
                    assistantCamTrans.Rotate(Vector3.up, angleRotateAroundUp, Space.Self);

                    Quaternion q3 = Quaternion.AngleAxis(angleRotateAroundRight, assistantCamTrans.right);
                    assistantCamTrans.Rotate(Vector3.right, angleRotateAroundRight, Space.Self);
                    assistantCamTrans.localPosition = q3 * assistantCamTrans.localPosition;

                    assistantCamTrans.localPosition += mainView.assCentter;
                }
            }

            mainView.RequestRepaint();
        }
    }

    static void OnSetBkColor(EditorControl c)
    {

    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnSetBkColorValueChanged(EditorControl c, object v)
    {
        ColorCtrl colorCtrl = c as ColorCtrl;
        if (colorCtrl == null)
            return;

        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            mainView.mainCam.backgroundColor = colorCtrl.currColor;
        }
    }

    static void OnHelp(EditorControl c)
    {
        System.Diagnostics.Process.Start(m_helpURL);
    }

    static void OnUndoBtnClick(EditorControl c)
    {
        EditorCommandManager.GetInstance().PerformUndo();
    }

    static void OnRedoBtnClick(EditorControl c)
    {
        EditorCommandManager.GetInstance().PerformRedo();
    }

    static void OnCmdMgrChange()
    {
        //if (s_root == null)
        //    return;

        //EditorControl undoBtn = s_root.FindControl("UndoBtn");
        //EditorControl redoBtn = s_root.FindControl("RedoBtn");

        //if( null != undoBtn )
        //{
        //    undoBtn.Caption = "撤消/" + EditorCommandManager.GetInstance().GetNextUndoCmdName();
        //}
        //if( null != redoBtn )
        //{
        //    redoBtn.Caption = "重做/" + EditorCommandManager.GetInstance().GetNextRedoCmdName();
        //}

        //UpdateSpeInspector();
        //undoBtn.RequestRepaint();
    }


    /*
     * 参照模型树状控件响应函数
     */


    //当参照模型树的绑定选项有变化时
    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnRefModelTreeViewValueChanged(EditorControl c, object v)
    {
        TreeViewCtrl refModelTreeView = c as TreeViewCtrl;

        if (refModelTreeView == null)
            return;

        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();

        if (spe == null)
            return;

        string newPath = string.Empty;
        if((bool)refModelTreeView.currSelectNode.state.userParams[0].param)
        {
            newPath = refModelTreeView.lastValueChangeNodePath;
        }

        SpeBindingTargetChangeCmd cmd = new SpeBindingTargetChangeCmd();
        cmd.oldPath = spe.BindingTargetPath;
        cmd.newPath = newPath;
        EditorCommandManager.GetInstance().Add(cmd);
    }

    static void ForceBindSpeToDefaultTarget()
    {
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();
         
        GameObject defaultTarget = null;

        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            defaultTarget = mainView.GetBindingTarget();
            mainView.RequestRepaint();
        }

        if (defaultTarget == null)
            return;

        if (spe != null)
        {
            spe.ForceBindTarget(defaultTarget);
        } 
    }

     

    static void TryBindSpeToRefModel()
    {
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();

        if (spe == null)
            return;

        GameObject refModel = SpecialEffectEditorModel.GetInstance().GetRefModel();

        if (spe.BindingTargetPath.Equals("") || refModel == null)
        {
            ForceBindSpeToDefaultTarget(); 
            return;
        }

        if (!spe.BindTarget(refModel))
        {
            ForceBindSpeToDefaultTarget();
        }
         
    }

    static void TryBindRefSpeToRefModel()
    { 
        GameObject defaultTarget = null;

        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            defaultTarget = mainView.GetBindingTarget();
            mainView.RequestRepaint();
        }

        if (defaultTarget == null)
            return;

        GameObject refModel = SpecialEffectEditorModel.GetInstance().GetRefModel(); 
        SpecialEffectEditorModel.GetInstance().TryBindToTarget(refModel, defaultTarget);
    }

    static void ForceBindRefSpeToDefaultTarget()
    { 
        GameObject defaultTarget = null;

        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            defaultTarget = mainView.GetBindingTarget();
            mainView.RequestRepaint();
        }

        if (defaultTarget == null)
            return;

        SpecialEffectEditorModel.GetInstance().ForceBindRefSpeToTarget(defaultTarget);
    }

    //参照特效绑定物体，同步TreeView的绑定选项
    static void SyncTreeViewBindingSelection()
    {
        TreeViewCtrl refModelTreeView = s_root.FindControl<TreeViewCtrl>();

        if (refModelTreeView == null)
        {
            return;
        }

        if (refModelTreeView.Roots.Count == 0)
        {
            return;
        }

        TreeViewCtrl.PreorderTraverse(refModelTreeView.Roots[0], SyncTreeViewBindingSelectionVisitCallBack);
    }


    //参照特效绑定物体，同步TreeView的绑定选项
    static bool SyncTreeViewBindingSelectionVisitCallBack(TreeViewNode n)
    {
        string bindTargetPath = "";
        if (!SpecialEffectEditorModel.GetInstance().HasRefModel())
        {//此时说明TreeView没有创建
            return false;
        }

        if (SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            bindTargetPath = SpecialEffectEditorModel.GetInstance().GetEditTarget().BindingTargetPath;
        }

        if (n.GetPathString().Equals(bindTargetPath))
        {
            n.state.userParams[0].param = true;
        }
        else
        {
            n.state.userParams[0].param = false;
        }
        return true;
    }

    //根据Model中的参考模型构建模型树视图
    //此函数会删除当前树，重新构建
    static void ReBuildTreeView()
    {
        GameObject refModel =
        SpecialEffectEditorModel.GetInstance().GetRefModel();

        TreeViewCtrl refModelTreeView = s_root.FindControl<TreeViewCtrl>();

        if (refModelTreeView == null)
            return;

        //若没有载入参考模型
        if (refModel == null)
        {
            refModelTreeView.Clear();
            return;
        }

        Transform rootTrans = refModel.transform;
        TreeViewNode rootNode = refModelTreeView.CreateNode(rootTrans.gameObject.name);

        TreeViewNodeUserParam rootParam = new TreeViewNodeUserParam();
        rootParam.name = "bind";
        rootParam.desc = "绑定";
        rootParam.param = false;

        rootNode.state.userParams.Add(rootParam);

        Queue<Transform> q = new Queue<Transform>();
        Queue<TreeViewNode> q2 = new Queue<TreeViewNode>();


        q.Enqueue(rootTrans);
        q2.Enqueue(rootNode);
        while (q.Count > 0)
        {
            Transform t = q.Dequeue();
            TreeViewNode n = q2.Dequeue();
            for (int i = 0; i < t.childCount; i++)
            {
                TreeViewNode nn = refModelTreeView.CreateNode(t.GetChild(i).gameObject.name);

                TreeViewNodeUserParam newParam = new TreeViewNodeUserParam();
                newParam.name = "bind";
                newParam.desc = "绑定";
                newParam.param = false;
                nn.state.userParams.Add(newParam);

                n.Add(nn);

                q.Enqueue(t.GetChild(i));
                q2.Enqueue(nn);
            }
        }

        refModelTreeView.Clear(); 
        refModelTreeView.Roots.Add(rootNode); 

        SyncTreeViewBindingSelection();

        refModelTreeView.RequestRepaint();
    }


    /*
     * 动作列表视图函数
     */
    
   static void UpdateActionListView()
   {
       ListViewCtrl actionListView = s_root.FindControl("_RefModelActionList") as ListViewCtrl;

       if (actionListView == null)
           return;

       actionListView.ClearItems();

       List<SpeEditorAction> actionList = SpecialEffectEditorModel.GetInstance().RefModelActionList();

       foreach( var action in actionList )
       {
           ListCtrlItem item = new ListCtrlItem();
           item.name = action.AnimClip.name;
           actionListView.AddItem(item);
       }

       RequestRepaint();
   } 

   static void OnActionListSelectionChange(EditorControl c, int i)
   {
       RequestRepaint();
   }
     

    /*
     * 参照特效列表视图响应函数
     */

    static void UpdateRefSpeListView()
    {
        ListViewCtrl refSpeListView = s_root.FindControl("_RefSpeList") as ListViewCtrl;

        if (refSpeListView == null)
            return;

        refSpeListView.ClearItems();

        int refSpeCount = SpecialEffectEditorModel.GetInstance().GetRefSpeCount();
        for (int i = 0; i < refSpeCount; i++)
        {
            ListCtrlItem item = new ListCtrlItem();
            string speName = "";
            SpecialEffectEditorModel.GetInstance().GetRefSpeName(i, ref speName);
            item.name = speName;
            refSpeListView.AddItem(item);
        }

        RequestRepaint();
    }

    static void OnRefSpeListSelectionChange(EditorControl c , int i)
    {
        RequestRepaint();
    }

    /*
    * 播放器控件响应函数
    */
    //当播放控制器值发生变化
    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnPlayCtrlValueChange(EditorControl c, object v)
    {
        PlayCtrl playCtrl = c as PlayCtrl;

        if (playCtrl == null)
            return;

        SpecialEffectEditorModel.GetInstance().SetCurrPlayTime(playCtrl.PlayTime);
    }


    /*
    * Inspector控件响应函数
    */
    static void OnVirtualSceneInspectorValueChange(EditorControl c, object v)
    {
        InspectorViewCtrl inspectorCtrl = c as InspectorViewCtrl;
        if (null == inspectorCtrl)
        {
            return;
        }

        VirturalSceneInspectorTarget target = inspectorCtrl.editTarget as VirturalSceneInspectorTarget;
        if(null == target)
        {
            return;
        }

        VirturalSceneInspectorTarget oldValue = new VirturalSceneInspectorTarget();
        oldValue.Set(SpecialEffectEditorModel.GetInstance().GetVirturalScene(), SpecialEffectEditorModel.GetInstance().GetGridMeshVisiable());

        VirtualSceneInspectorValueChangeCmd cmd = new VirtualSceneInspectorValueChangeCmd();
        cmd.oldValue = oldValue;
        cmd.newValue = target.Copy();
        EditorCommandManager.GetInstance().Add(cmd);
    }
    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnSpeElemInspectorValueChange(EditorControl c, object v)
    {
        InspectorViewCtrl inspectorCtrl = c as InspectorViewCtrl;

        if (inspectorCtrl == null)
            return;

        SpeElemInspectorTarget target =
        inspectorCtrl.editTarget as SpeElemInspectorTarget;

        if (target == null)
            return;

        SpeElemInspectorTarget oldValue = new SpeElemInspectorTarget();
        oldValue.Set(SpecialEffectEditorModel.GetInstance().GetEditTarget() , target.selectItem);

        SpeElemInspectorValueChangeCmd cmd = new SpeElemInspectorValueChangeCmd();
        if(v != null)
        {
            cmd.changeType = (int)v;
        }
        else
        {
            cmd.changeType = -1;
        }
        cmd.oldValue = oldValue;
        cmd.newValue = target.Copy();
        EditorCommandManager.GetInstance().Add(cmd);
    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    static void OnSpeInspectorValueChange(EditorControl c, object v)
    {
        InspectorViewCtrl inspectorCtrl = c as InspectorViewCtrl;

        if (inspectorCtrl == null)
            return;

        if (inspectorCtrl.editTarget == null)
            return;

        SpeInspectorTarget target = inspectorCtrl.editTarget as SpeInspectorTarget;

        if (target == null)
            return;

        SpeInspectorTarget oldValue = new SpeInspectorTarget();
        oldValue.Set(SpecialEffectEditorModel.GetInstance().GetEditTarget());

        SpeInspectorValueChangeCmd cmd = new SpeInspectorValueChangeCmd();
        cmd.oldValue = oldValue;
        cmd.newValue = target.Copy();
        EditorCommandManager.GetInstance().Add(cmd);

    }

    //指定索引项更新Inspector
    static void UpdateSpeElemInspector(int sel)
    {
        InspectorViewCtrl inspectorCtrl = s_root.FindControl("_SpeElemInspector") as InspectorViewCtrl;

        if (inspectorCtrl == null)
            return;
        
        SpeElemInspectorTarget inspectorTarget = inspectorCtrl.editTarget as SpeElemInspectorTarget;
        if (sel != -1)
        {
            int speElemsNum = 0;
            if (SpecialEffectEditorModel.GetInstance().HasEditTarget())
            {
                speElemsNum = SpecialEffectEditorModel.GetInstance().GetEditTarget().GetItemCount();
                if (sel < speElemsNum && sel >= 0)
                {
                    //inspectorTarget.Set(SpecialEffectEditorModel.GetInstance().GetEditTarget(), sel);
                    inspectorTarget.Set(SpecialEffectEditorModel.GetInstance().GetEditTarget(), speEditorTimeLineItem[sel].BlindSpeItem);
                    SpecialEffectEditorModel.GetInstance().GetEditTarget().ShowSelectElemInspector(speEditorTimeLineItem[sel].BlindSpeItem);
                    RequestRepaint();
                    return;
                }
            }
            SpecialEffectEditorModel.GetInstance().ShowSelectRefSpeInspector(sel - speElemsNum);
        }
        else
        {
            inspectorTarget.Set(null, -1);
        }
        RequestRepaint();
    }

    static void UpdateSpeInspector()
    {
        InspectorViewCtrl inspectorCtrl = s_root.FindControl("_SpeInspector") as InspectorViewCtrl;
        if (inspectorCtrl != null)
        {
            if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
            {
                inspectorCtrl.editTarget = null;
            }
            else
            {
                inspectorCtrl.editTarget = new SpeInspectorTarget();
                SpeInspectorTarget inspectorTarget = inspectorCtrl.editTarget as SpeInspectorTarget;
                inspectorTarget.Set(SpecialEffectEditorModel.GetInstance().GetEditTarget());
            }
        }
    }

    /*
     * 时间线视图控件响应函数
     */

    static void OnTimeLineDragItemsBegin(EditorControl c, List<TimeLineViewCtrl.ItemSelectInfo> infoList)
    {
        TimeLineViewCtrl timeLineCtrl = c.Root.FindControl<TimeLineViewCtrl>(); 
        if (timeLineCtrl == null)
            return;

        SpeItemChangeCmd.ClearTmpItems();
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();
        int speElems = 0;

        if (spe != null)
        {
            speElems = spe.GetItemCount();
            foreach (var item in infoList)
            {
                if (item.indx >= 0 && item.indx < speElems)
                {
                    //SpeItemChangeCmd.tmpItems.Add(timeLineCtrl.Items[i].Copy());
                    SpeItemChangeCmd.tmpItems.Add(speEditorTimeLineItem[item.indx].Copy());

                }
            }
        }

        RequestRepaint();
    }

    //时间线在拖拽中
    static void OnTimeLineDragItems(EditorControl c, List<TimeLineViewCtrl.ItemSelectInfo> infoList)
    {
        TimeLineViewCtrl timeLineCtrl = c.Root.FindControl<TimeLineViewCtrl>();

        if (timeLineCtrl == null)
            return;

        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();
        int speElems = 0;
        if (spe != null)
        {
            speElems = spe.GetItemCount();
            foreach (var item in infoList)
            {
                if (item.indx >= 0 && item.indx < speElems)
                {
                    if (item.side == TimeLineViewCtrl.SIDE_LEFT)
                    {
                        spe.SetItemStartTime(speEditorTimeLineItem[item.indx].BlindSpeItem, speEditorTimeLineItem[item.indx].RealTimeLineItem.startTime);
                    }
                    else if(item.side == TimeLineViewCtrl.SIDE_RIGHT)
                    {
                        spe.SetItemPlayTime(speEditorTimeLineItem[item.indx].BlindSpeItem, speEditorTimeLineItem[item.indx].RealTimeLineItem.length);
                    }
                    else if(item.side == TimeLineViewCtrl.SIDE_MID)
                    {
                        spe.SetItemDelayTime(speEditorTimeLineItem[item.indx].BlindSpeItem, speEditorTimeLineItem[item.indx].RealTimeLineItem.startTime);
                    }
                    //spe.SetItemTimeInfo(i, timeLineCtrl.Items[i].startTime, timeLineCtrl.Items[i].length);
                    //spe.SetItemStateInfo(i, timeLineCtrl.Items[i].loop);
                    //spe.SetItemTimeInfo(speEditorTimeLineItem[item.indx].BlindSpeItem, speEditorTimeLineItem[item.indx].RealTimeLineItem.startTime, speEditorTimeLineItem[item.indx].RealTimeLineItem.length);
                    spe.SetItemStateInfo(speEditorTimeLineItem[item.indx].BlindSpeItem, speEditorTimeLineItem[item.indx].RealTimeLineItem.loop);
                }
            }            
        }

        foreach (var item in infoList)
        {
            if (item.indx >= speElems)
            { 
                //选中时间线为参考特效
                SpecialEffectEditorModel.GetInstance().SetRefSpeStartTime(item.indx - speElems, timeLineCtrl.Items[item.indx].startTime);
            }
        }

       SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();

       RequestRepaint();
    }

    static void OnTimeLineDragItemsEnd(EditorControl c, List<TimeLineViewCtrl.ItemSelectInfo> infoList)
    {
        TimeLineViewCtrl timeLineCtrl = c.Root.FindControl<TimeLineViewCtrl>();

        if (timeLineCtrl == null)
            return;

        int speElems = 0;
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();
        if( spe != null )
        {
            speElems = spe.GetItemCount();
        }

        //更新参考特效
        foreach (var item in infoList)
        {
            if (item.indx >= speElems)
            {
                //选中时间线为参考特效
                SpecialEffectEditorModel.GetInstance().SetRefSpeStartTime(item.indx - speElems, timeLineCtrl.Items[item.indx].startTime);
            }
        }
        
        if (spe != null)
        { 

            SpeItemChangeCmd cmd = new SpeItemChangeCmd();
            cmd.indices = infoList; 
            cmd.oldTimeLineItems = SpeItemChangeCmd.tmpItems;
            foreach (var item in infoList)
            {
                if (item.indx >= 0 && item.indx < speElems)
                {
                    //cmd.newTimeLineItems.Add(timeLineCtrl.Items[i].Copy());  
                    cmd.newTimeLineItems.Add(speEditorTimeLineItem[item.indx].Copy());  
                }
            }


            //int indx = 0;
            //foreach( var old in cmd.oldTimeLineItems )
            //{
            //    int i = infoList[indx].indx;
            //    if (i >= 0 && i < speElems)
            //    {
            //        //spe.SetItemTimeInfo(i, old.startTime, old.length);
            //        //spe.SetItemStateInfo(i, old.loop);
            //        if (infoList[indx].side == TimeLineViewCtrl.SIDE_LEFT)
            //        {
            //            spe.SetItemStartTime(speEditorTimeLineItem[i].BlindSpeItem, old.RealTimeLineItem.startTime);
            //        }
            //        else if (infoList[indx].side == TimeLineViewCtrl.SIDE_RIGHT)
            //        {
            //            spe.SetItemPlayTime(speEditorTimeLineItem[i].BlindSpeItem, old.RealTimeLineItem.length);
            //        }
            //        else if (infoList[indx].side == TimeLineViewCtrl.SIDE_MID)
            //        {
            //            spe.SetItemDelayTime(speEditorTimeLineItem[i].BlindSpeItem, old.RealTimeLineItem.startTime);
            //        }
            //        //spe.SetItemTimeInfo(speEditorTimeLineItem[i].BlindSpeItem, old.RealTimeLineItem.startTime, old.RealTimeLineItem.length);
            //        spe.SetItemStateInfo(speEditorTimeLineItem[i].BlindSpeItem, old.RealTimeLineItem.loop);

            //    }
            //    indx++;
            //}


            EditorCommandManager.GetInstance().Add(cmd);
        }

        RequestRepaint();
    }


    static void OnTimeLineSelectChange(EditorControl c, int i)
    {
        SpeItemSelectChangeCmd cmd = new SpeItemSelectChangeCmd();
        cmd.oldSelection = SpecialEffectEditorModel.GetInstance().selectItem;
        cmd.newSelection = i;
        EditorCommandManager.GetInstance().Add(cmd);
    }



    /*
    * 时间线列表控件响应函数
    */
    static void OnTimeLineListViewScroll(EditorControl c, Vector2 pos)
    {
        List<EditorControl> list = c.Root.FindControls<TimeLineViewCtrl>();

        if (list.Count > 0)
        {
            TimeLineViewCtrl timeLineCtrl = (list[0] as TimeLineViewCtrl);
            if (timeLineCtrl != null)
            {
                Vector2 oldPos = timeLineCtrl.ScrollPos;
                timeLineCtrl.ScrollPos = new Vector2(oldPos.x, pos.y);
                timeLineCtrl.RequestRepaint();
            }
        }
    }
    static void OnTimeLineListViewSelectChange(EditorControl c, int i)
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;

        if (null == treeView)
        {
            return;
        }

        for(int index = 0; index < speEditorTimeLineItem.Count; index++)
        {
            if (speEditorTimeLineItem[index].ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
            {
                if (UnityEngine.Object.ReferenceEquals(speEditorTimeLineItem[index].BlindSpeItem, treeView.currSelectNode.userObject))
                {
                    SpeItemSelectChangeCmd cmd = new SpeItemSelectChangeCmd();
                    cmd.oldSelection = SpecialEffectEditorModel.GetInstance().selectItem;
                    cmd.newSelection = index;
                    EditorCommandManager.GetInstance().Add(cmd);

                    break;
                }
            }
            else if (speEditorTimeLineItem[index].ItemType == SPEITEMTYPE.TYPE_REFSPE)
            {
                if (UnityEngine.Object.ReferenceEquals(speEditorTimeLineItem[index].RefSpe, treeView.currSelectNode.userObject))
                {
                    SpeItemSelectChangeCmd cmd = new SpeItemSelectChangeCmd();
                    cmd.oldSelection = SpecialEffectEditorModel.GetInstance().selectItem;
                    cmd.newSelection = index;
                    EditorCommandManager.GetInstance().Add(cmd);

                    break;
                }
            }

        }
    }

    static void OnTreeViewBehaveChange(EditorControl c)
    {
        UpdateTimeLineItem();
    }

    static void OnTreeViewValueChange(EditorControl c, object value)
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;

        if (null == treeView)
        {
            return;
        }

        TreeViewNode valueChangeNode = value as TreeViewNode;

        //foreach(var item in speEditorTimeLineItem)
        //{
        //    TreeViewNode blindNode = null;

        //    GetBlindNode(treeView.Roots, item, out blindNode);

        //    if (UnityEngine.Object.ReferenceEquals(blindNode, valueChangeNode))
        //    {
        //        bool enable = IsCurrNodeEnable(blindNode);

        //        item.RealTimeLineItem.enable = enable;

        //        if (item.ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
        //        {
        //            item.BlindSpeItem.CanShow = enable;
        //            SpecialEffectEditorModel.GetInstance().GetEditTarget().ShowSelectElemInspector(item.BlindSpeItem);
        //        }
        //        else if (item.ItemType == SPEITEMTYPE.TYPE_REFSPE)
        //        {
        //            item.RefSpe.RealSpe.CanShow = enable;
        //            SpecialEffectEditorModel.GetInstance().ShowSelectRefSpeInspector(index - SpecialEffectEditorModel.GetInstance().GetEditTarget().GetItemCount());
        //        }

        //        break;
        //    }

        //    index++;
        //}
        int index = 0;

        foreach (var item in speEditorTimeLineItem)
        {
            TreeViewNode blindNode = null;

            GetBlindNode(treeView.Roots, item, out blindNode);

            bool enable = IsCurrNodeEnable(blindNode);

            item.RealTimeLineItem.enable = enable;

            if (item.ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
            {
                item.BlindSpeItem.CanShow = enable;
            }
            else if (item.ItemType == SPEITEMTYPE.TYPE_REFSPE)
            {
                item.RefSpe.RealSpe.CanShow = enable;
            }

            if (UnityEngine.Object.ReferenceEquals(blindNode, valueChangeNode))
            {
                if (item.ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
                {
                    SpecialEffectEditorModel.GetInstance().GetEditTarget().ShowSelectElemInspector(item.BlindSpeItem);
                }
                else if (item.ItemType == SPEITEMTYPE.TYPE_REFSPE)
                {
                    SpecialEffectEditorModel.GetInstance().ShowSelectRefSpeInspector(index - SpecialEffectEditorModel.GetInstance().GetEditTarget().GetItemCount());
                }
            }

            index++;
        }

        //SpecialEffectEditorModel.GetInstance().SetCurrPlayTime(SpecialEffectEditorModel.GetInstance().GetEditTarget().RealSpe.CurrPlayTime);
    }

    static bool GetBlindNode(List<TreeViewNode> root, SpeEditorTimeLineItem speItem, out TreeViewNode blindNode)
    {
        bool bFind = false;
        blindNode = null;

        if(
            (null == speItem)
            || (null == root)
            )
        {
            return false; 
        }

        foreach (var item in root)
        {
            if (speItem.ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
            {
                if (UnityEngine.Object.ReferenceEquals(item.userObject, speItem.BlindSpeItem))
                {
                    bFind = true;
                    blindNode = item;
                    break;
                }
            }
            else if(speItem.ItemType == SPEITEMTYPE.TYPE_REFSPE)
            {
                if (UnityEngine.Object.ReferenceEquals(item.userObject, speItem.RefSpe))
                {
                    bFind = true;
                    blindNode = item;
                    break;
                }
            }

            bFind = GetBlindNode(item.children, speItem, out blindNode);
            if (bFind)
            {
                break;
            }
        }

        return bFind;
    }
    static bool IsCurrNodeEnable(TreeViewNode currNode)
    {
        bool bRet = true;

        if(null == currNode)
        {
            return false;
        }

        TreeViewNode parentNode = currNode.parent;

        while(parentNode != null)
        {
            if(!(bool)parentNode.state.userParams[0].param)
            {
                bRet = false;
                break;
            }

            parentNode = parentNode.parent;
        }

        if (bRet)
        {
            bRet = (bool)currNode.state.userParams[0].param;
        }

        return bRet;
    }
    /*
     * Model变化回调
     */

    static void OnVirtualSceneCreate(SpecialEffectEditorModel m)
    {
        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            m.GetVirturalScene().transform.parent = mainView.GetRefModelBindingTarget().transform;
            m.GetVirturalScene().transform.localPosition = Vector3.zero;
            m.SetVirtualSceneVisiable(true);
            m.SetGridMeshVisiable(true);
            m.SetVirturalSceneScale(new Vector2(1, 1));

            mainView.RequestRepaint();
        }

        InspectorViewCtrl inspectorCtrl = s_root.FindControl("_VirtualSceneInspector") as InspectorViewCtrl;
        if(inspectorCtrl != null)
        {
            VirturalSceneInspectorTarget target = inspectorCtrl.editTarget as VirturalSceneInspectorTarget;
            if(target != null)
            {
                target.Set(m.GetVirturalScene(), true);
            }
        }
        
    }

    static void OnGridMeshVisiableChange(SpecialEffectEditorModel m)
    {
        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            mainView.gridMeshObj.SetActive(m.GetGridMeshVisiable());
        }
    }

    static void OnRefModelOpen(SpecialEffectEditorModel m)
    {
        if (!m.HasRefModel())
            return;

        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();

        MainViewCtrl mainView = s_root.FindControl<MainViewCtrl>();
        if (mainView != null)
        {
            m.GetRefModel().transform.parent = mainView.GetRefModelBindingTarget().transform;
            m.GetRefModel().transform.localPosition = Vector3.zero;
            mainView.RequestRepaint();
        }

        Vector3 localScale = m.GetRefModel().transform.localScale;
        TextBoxCtrl refModelScaleXTextBox = s_root.FindControl("_RefModelScaleX") as TextBoxCtrl;
        TextBoxCtrl refModelScaleYTextBox = s_root.FindControl("_RefModelScaleY") as TextBoxCtrl;
        TextBoxCtrl refModelScaleZTextBox = s_root.FindControl("_RefModelScaleZ") as TextBoxCtrl;

        if (refModelScaleXTextBox != null)
        {
            refModelScaleXTextBox.Text = localScale.x.ToString();
        }
        if (refModelScaleYTextBox != null)
        {
            refModelScaleYTextBox.Text = localScale.y.ToString();
        }
        if (refModelScaleZTextBox != null)
        {
            refModelScaleZTextBox.Text = localScale.z.ToString();
        }

        ReBuildTreeView();

        //同步树视图绑定选项
        SyncTreeViewBindingSelection();

        //重新绑定模型
        TryBindSpeToRefModel();

        //重新绑定参照特效
        TryBindRefSpeToRefModel();

        OnPlayCtrlValueChange(playCtrl, 0);
    }

    static void OnRefModelDestroy(SpecialEffectEditorModel m)
    {
        //当引用模型销毁之前，将所有特效绑定到
        //默认绑定点，防止被模型销毁。
        ForceBindSpeToDefaultTarget();
        ForceBindRefSpeToDefaultTarget();
    }

    static void OnSpeSetDirty(SpecialEffectEditorModel m)
    {
        //string title = m.GetEditTargetFilePath();
        //if (m.IsDirty)
        //    title += "*";

        //s_root.title = title;
    }


    static void OnSetNewEditTarget(SpecialEffectEditorModel m)
    {
        EditorCommandManager.GetInstance().Clear();
   
        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();
        playCtrl.PlayTime = 0.0f;
        playCtrl.TotalTime = m.GetPreviewTotalTime(); 
        
        SpecialEffectEditProxy spe = m.GetEditTarget();

        UpdateTreeView(m);

        UpdateTimeLineItem();

        InitSpeItemValue();

        if (spe != null)
        {
            //待编辑特效
            SetFinishEditButtonEnable(true);
        }
        else
        {
            SetFinishEditButtonEnable(false);
        }

        ForceBindSpeToDefaultTarget();

        ForceBindRefSpeToDefaultTarget();

        OnSpeValueChange(m);

        //UpdateSpeInspector();


        ////同步树视图绑定选项
        //SyncTreeViewBindingSelection();

        ////尝试绑定模型
        //TryBindSpeToRefModel();

        ////尝试绑定参照特效到模型
        //TryBindRefSpeToRefModel();

        //RequestRepaint();
    }

    static void InitSpeItemValue()
    {
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();

        if(null == spe)
        {
            return;
        }
        if (SpecialEffectEditorUtility.IsSpeValueVaild(spe.RealSpe))
        {
            return;
        }

        foreach (var item in speEditorTimeLineItem)
        {
            if (item.ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
            {
                spe.SetItemStartTime(item.BlindSpeItem, item.RealTimeLineItem.startTime);
            }
        }

        foreach (var item in speEditorTimeLineItem)
        {
            if (item.ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
            {
                spe.SetItemPlayTime(item.BlindSpeItem, item.RealTimeLineItem.length);
            }
        }
    }
    static void SetFinishEditButtonEnable(bool enable)
    {
        ButtonCtrl finishEditBtn = s_root.FindControl("_FinishEditBtn") as ButtonCtrl;
        if (finishEditBtn != null)
        {
            finishEditBtn.Visiable = enable;
        }
    }

    static void OnSpeValueChange(SpecialEffectEditorModel m)
    {
        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();
        TimeLineViewCtrl timeLineViewCtrl = s_root.FindControl<TimeLineViewCtrl>();
        //ListViewCtrl listViewCtrl = s_root.FindControl("_SpeItemList") as ListViewCtrl;

        SpecialEffectEditProxy spe = m.GetEditTarget();

        playCtrl.TotalTime = m.GetPreviewTotalTime();
        timeLineViewCtrl.TotalTime = m.GetPreviewTotalTime();

        int itemIndx = 0;

        if (spe != null)
        {//更新待编辑特效值
            for (int i = 0; i < spe.GetItemCount(); i++)
            {
                //spe.GetItemTimeInfo(i, ref timeLineViewCtrl.Items[i].startTime, ref timeLineViewCtrl.Items[i].length);
                //spe.GetItemStateInfo(i, ref timeLineViewCtrl.Items[i].loop);
                spe.GetItemTimeInfo(speEditorTimeLineItem[i].BlindSpeItem, ref timeLineViewCtrl.Items[i].startTime, ref timeLineViewCtrl.Items[i].length);
                spe.GetItemStateInfo(speEditorTimeLineItem[i].BlindSpeItem, ref timeLineViewCtrl.Items[i].loop);
                itemIndx++;
            }

            if (timeLineViewCtrl.Tags.Count > 0)
            {
                timeLineViewCtrl.Tags[0].time = spe.StartTime;
            }
        }

        {//处理参考特效

            for( int i = 0 ; i < m.GetRefSpeCount() ; i++ )
            {
                m.GetRefSpeStartTime(i, ref timeLineViewCtrl.Items[itemIndx].startTime);
                m.GetRefSpeLength(i, ref timeLineViewCtrl.Items[itemIndx].length);
                itemIndx++;
            }

        }

        UpdateSpeInspector();

        //同步树视图绑定选项
        SyncTreeViewBindingSelection();

        //重新绑定模型
        TryBindSpeToRefModel();

        //重新绑定参照特效到参照模型
        TryBindRefSpeToRefModel();

        OnPlayCtrlValueChange(playCtrl, 0.0f);

        RequestRepaint();
    }

    static void OnActionListChange(SpecialEffectEditorModel m)
    {
        UpdateActionListView();
    }

    static void OnRefSpeListChange(SpecialEffectEditorModel m )
    {
        UpdateRefSpeListView();
    }

    static void OnSpeDestroy(SpecialEffectEditorModel m)
    {
        EditorCommandManager.GetInstance().Clear();

        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();
        TimeLineViewCtrl timeLineViewCtrl = s_root.FindControl<TimeLineViewCtrl>();
        //ListViewCtrl listViewCtrl = s_root.FindControl("_SpeItemList") as ListViewCtrl;
        TreeViewCtrl treeViewCtrl = s_root.FindControl("_MainTreeView") as TreeViewCtrl;

        if(
               (null == playCtrl)
            || (null == timeLineViewCtrl)
            || (null == treeViewCtrl)
            )
        {
            return;
        }

        playCtrl.PlayTime = 0.0f;
        playCtrl.TotalTime = 5.0f;

   //     listViewCtrl.Items.Clear();
        treeViewCtrl.Clear();
        timeLineViewCtrl.Items.Clear();
        timeLineViewCtrl.TotalTime = 5.0f;
        UpdateSpeInspector();
        RequestRepaint();
    }

    static void RequestRepaint()
    {
        s_root.RequestRepaint();
    }

    static void OnSpeSaved(SpecialEffectEditorModel m)
    {
        EditorCommandManager.GetInstance().Clear();
    }

    static bool GetSelectNode(int index, List<TreeViewNode> root, out TreeViewNode node)
    {
        node = null;
        bool bRet = false;

        if(
            (null == root)
            || (index < 0)
            || (index > (speEditorTimeLineItem.Count - 1))
            )
        {
            return false;
        }

        foreach(var item in root)
        {
            if(item != null)
            {
                if(speEditorTimeLineItem[index].ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
                {
                    if (UnityEngine.Object.ReferenceEquals(speEditorTimeLineItem[index].BlindSpeItem, item.userObject))
                    {
                        bRet = true;
                        node = item;
                        break;
                    }
                }
                else if (speEditorTimeLineItem[index].ItemType == SPEITEMTYPE.TYPE_REFSPE)
                {
                    if (UnityEngine.Object.ReferenceEquals(speEditorTimeLineItem[index].RefSpe, item.userObject))
                    {
                        bRet = true;
                        node = item;
                        break;
                    }
                }
            }

            bRet = GetSelectNode(index, item.children, out node);
            if(bRet)
            {
                break;
            }
        }

        return bRet;
    }
    static void OnSpeItemSelectChanged(SpecialEffectEditorModel m)
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView != null)
        {
            TreeViewNode node = null;
            GetSelectNode(m.selectItem, treeView.Roots, out node);
            treeView.currSelectNode = node;
            treeView.RequestRepaint();
        }

        //ListViewCtrl listCtrl = s_root.FindControl("_SpeItemList") as ListViewCtrl;
        //if (listCtrl != null)
        //{
        //    listCtrl.LastSelectItem = m.selectItem;
        //    listCtrl.RequestRepaint();
        //}

        TimeLineViewCtrl timeLineCtrl = s_root.FindControl<TimeLineViewCtrl>();
        if (timeLineCtrl != null)
        {
            timeLineCtrl.LastSelectedItem = m.selectItem;
            timeLineCtrl.RequestRepaint();
        }

        bool enable = IsCurrNodeEnable(treeView.currSelectNode);

        if(
            (m.selectItem >= 0)
            && (m.selectItem < speEditorTimeLineItem.Count)
            )
        {
            if (speEditorTimeLineItem[m.selectItem].ItemType == SPEITEMTYPE.TYPE_EDITSPE_ELEM)
            {
                speEditorTimeLineItem[m.selectItem].BlindSpeItem.CanShow = enable;

            }
            else if (speEditorTimeLineItem[m.selectItem].ItemType == SPEITEMTYPE.TYPE_REFSPE)
            {
                speEditorTimeLineItem[m.selectItem].RefSpe.RealSpe.CanShow = enable;
            }
        }

        UpdateSpeElemInspector(m.selectItem);

        //OnTreeViewValueChange(treeView, 0);
        //OnPlayCtrlValueChange(playCtrl, 0);
    }


    static void OnCurrPlayTimeChange(SpecialEffectEditorModel m)
    {

        PlayCtrl playCtrl = s_root.FindControl<PlayCtrl>();
        if (playCtrl != null)
        {
            if (playCtrl.PlayTime != m.GetCurrPlayTime())
            {
                playCtrl.PlayTime = m.GetCurrPlayTime();
            }
        }

        //在播放控制器值发生变化时需要通知时间线视口
        TimeLineViewCtrl timeLineCtrl = s_root.FindControl<TimeLineViewCtrl>();
        if (timeLineCtrl != null)
        {
            timeLineCtrl.CurrPlayTime = m.GetCurrPlayTime();
        }

        RequestRepaint();
    }

    static void OnEditorEnable(EditorRoot root)
    {
        {//对来自编辑器的事件进行监听
            Undo.postprocessModifications += _OnUndoPostProcessModification;
            Undo.undoRedoPerformed += _OnUndoRedo;
            Undo.undoRedoPerformed += SpecialEffectEditorModel.GetInstance().OnUndoRedo;

            EditorCommandManager.GetInstance().onBeforeCmdExecute += SpecialEffectEditorModel.GetInstance().OnBeforeCmdExecute;
            EditorCommandManager.GetInstance().onAfterCmdExecute += SpecialEffectEditorModel.GetInstance().OnAfterCmdExecute;
        }
    }

    static void OnEditorDisable(EditorRoot root)
    {
        {//对来自编辑器的事件进行监听
            Undo.postprocessModifications -= _OnUndoPostProcessModification;
            Undo.undoRedoPerformed -= _OnUndoRedo;
            Undo.undoRedoPerformed -= SpecialEffectEditorModel.GetInstance().OnUndoRedo;

            EditorCommandManager.GetInstance().onBeforeCmdExecute -= SpecialEffectEditorModel.GetInstance().OnBeforeCmdExecute;
            EditorCommandManager.GetInstance().onAfterCmdExecute -= SpecialEffectEditorModel.GetInstance().OnAfterCmdExecute;
        }

        //if (SpecialEffectEditorModel.GetInstance().IsDirty)
        //{
        //    if (EditorUtility.DisplayDialog("警告!", "当前编辑的特效已被修改，是否保存?", "是", "否"))
        //    {
        //        SpecialEffectEditorModel.GetInstance().SaveSpeChange();
        //    }
        //}
        SpecialEffectEditorModel.GetInstance().Destroy();
        EditorCommandManager.GetInstance().Destroy();
    }

    static void OnEditorGUI(EditorRoot root)
    {
    }

    static void OnEditorUpdate(EditorRoot root)
    {

    }

    static void OnEditorDestroy(EditorRoot root)
    {

    }



    static void OnMainViewDragingObjs(EditorControl c, UnityEngine.Object[] objs, string[] paths)
    {
    }

    static void OnMainViewDropObjs(EditorControl c, UnityEngine.Object[] objs, string[] paths)
    {//目前只支持单一物体拖放
        if (c == null)
            return;

        if (objs == null)
            return;

        if (objs.Length == 0)
            return;

        GameObject go = objs[0] as GameObject;
        if (go == null)
            return;

        SpecialEffect spe = go.GetComponent<SpecialEffect>();
        if (spe != null)
        {
            string errorMsg = string.Empty;

            SpecialEffectEditorUtility.RefreshSpecialEffect(go);

            if (!SpecialEffectEditorUtility.IsSPELegal(go, out errorMsg))
            {
                EditorUtility.DisplayDialog("操作失败！", errorMsg, "确认");
                return;
            }

            if (SpecialEffectEditorModel.GetInstance().HasEditTarget())
            {//如果已经有编辑目标则作为参考特效加载
                SpecialEffectEditorModel.GetInstance().AddRefSpe(go);
            }
            else
            {
                SpecialEffectEditorModel.GetInstance().SetEditTarget(go);
            }
        }
        else
        {
            if (paths.Length == 0)
                return;

            string resPath = paths[0].Substring(paths[0].IndexOf("Assets"));

#if UNITY_EDITOR
            AnimationClip animClip = AssetDatabase.LoadAssetAtPath(resPath, typeof(AnimationClip)) as AnimationClip; 
#else
            AnimationClip animClip = AssetDatabase.LoadAssetAtPath(Trans2ResourcesPath(resPath), typeof(AnimationClip)) as AnimationClip; 
#endif
            if (animClip == null)
            {//为模型 
                int operateType = 0;
                operateType = EditorUtility.DisplayDialogComplex("", "将该Prefab作为", "参考模型", "虚拟场景", "取消");
              
                switch(operateType)
                {
                    case 0:
                        SpecialEffectEditorModel.GetInstance().OpenRefModelFromFile(resPath);
                        break;

                    case 1:
                        SpecialEffectEditorModel.GetInstance().OpenVirtualSceneFromFile(resPath);
                        break;

                    case 2:
                        break;

                    default:
                        break;
                }

            }
            else
            {//为动作  
                SpecialEffectEditorModel.GetInstance().AddRefModelAction(animClip);
            }
        }

    }

    //测试拖拽进编辑器的物体编辑器是否接受
    static bool OnMainViweAcceptDragingObjs(EditorControl c, UnityEngine.Object[] objs, string[] paths)
    {
        if (c == null)
            return false;

        if (objs == null)
            return false;

        if (objs.Length == 0)
            return false;

        GameObject spe = objs[0] as GameObject;
        if (spe == null)
            return false;

        //查看是否为特效
        if (null != spe.GetComponent<SpecialEffect>())
        {
            return true;
        }
        else
        {//查看是否为动作或模型
            if (paths.Length == 0)
                return false;
            return true;          
        }
         
    }

    static void _OnHierarchyWndItemOnGUI(int instanceID, Rect selectionRect)
    {
    }

    static void _OnUndoRedo()
    {
        RequestRepaint();
    }

    static UndoPropertyModification[] _OnUndoPostProcessModification(UndoPropertyModification[] modifications)
    {
        //当Inspector值发生变化时重绘
        RequestRepaint();

        //Debug.Log("ModProcess");
        //foreach( var m in modifications )
        //{
        //    PropertyModification propMod = m.propertyModification;

        //    string objRefName = propMod.objectReference == null ? "null" : propMod.objectReference.name;
        //    string targetName = propMod.target == null ? "null" : propMod.target.name; 

        //    Debug.Log(
        //        "objRefName=" + objRefName +
        //        " target=" + targetName +
        //        " value="+propMod.value+
        //        " propPath="+propMod.propertyPath
        //        );
        //}
        return modifications;
    }

    static void UpdateTreeView(SpecialEffectEditorModel m)
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }
   
        treeView.Clear();

        SpecialEffectEditProxy spe = m.GetEditTarget();
        if(spe != null)
        {
            foreach(var item in spe.RealSpe.elems)
            {
                AddElementsToTreeView(item);
            }
        }

        for (int i = 0; i < SpecialEffectEditorModel.GetInstance().GetRefSpeCount(); i++)
        {
            AddRefSpeToTreeView(i);
        }
    }

    static void UpdateTimeLineItem()
    {
        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }
 
        TimeLineViewCtrl timeLineViewCtrl = s_root.FindControl<TimeLineViewCtrl>();
        if (timeLineViewCtrl == null)
        {
            return;
        }

        timeLineViewCtrl.Items.Clear();
        timeLineViewCtrl.Tags.Clear();
        speEditorTimeLineItem.Clear();
  
        timeLineViewCtrl.TotalTime = SpecialEffectEditorModel.GetInstance().GetPreviewTotalTime();

        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();

        if(spe != null)
        {
            UpdateTimeLineItemForEditSpe(treeView.Roots, null);

            //为编辑特效加入起始时间标记
            TimeTag newTag = new TimeTag();
            newTag.time = spe.StartTime;
            timeLineViewCtrl.Tags.Add(newTag);
        }

        UpdateTimeLineItemForRefSpe();


    }

    static void UpdateTimeLineItemForEditSpe(List<TreeViewNode> nodeTbl, SpeEditorTimeLineItem parentItem)
    {
        TimeLineViewCtrl timeLineViewCtrl = s_root.FindControl<TimeLineViewCtrl>();

        if(
               (null == nodeTbl)
            || (0 == nodeTbl.Count)
            || (null == timeLineViewCtrl)
            )
        {
            return;
        }


        foreach (var item in nodeTbl)
        {
            SpeEditorTimeLineItem newSpeEditorTimeLineItem = new SpeEditorTimeLineItem();
            TimeLineItem newTimeLineItem = new TimeLineItem();
            SpecialEffectElement elem = item.userObject as SpecialEffectElement;
            if (null == elem)
            {
                continue;
            }
            newTimeLineItem.startTime = elem.startTime + SpecialEffectEditorModel.GetInstance().GetEditTarget().StartTime;
            newTimeLineItem.length = elem.playTime;
            newTimeLineItem.loop = elem.isLoop;
            newTimeLineItem.visiable = IsItemVisiable(item);
            newTimeLineItem.enable = (bool)item.state.userParams[0].param;

            newSpeEditorTimeLineItem.ItemType = SPEITEMTYPE.TYPE_EDITSPE_ELEM;
            newSpeEditorTimeLineItem.Visiable = newTimeLineItem.visiable;
            newSpeEditorTimeLineItem.Enable = (bool)item.state.userParams[0].param;
            newSpeEditorTimeLineItem.BlindSpeItem = elem;
            newSpeEditorTimeLineItem.RealTimeLineItem = newTimeLineItem;
            newSpeEditorTimeLineItem.Parent = parentItem;
            if (parentItem != null)
            {
                parentItem.Children.Add(newSpeEditorTimeLineItem);
            }

            timeLineViewCtrl.Items.Add(newTimeLineItem);
            speEditorTimeLineItem.Add(newSpeEditorTimeLineItem);

            UpdateTimeLineItemForEditSpe(item.children, newSpeEditorTimeLineItem);
        }
    }

    static void UpdateTimeLineItemForRefSpe()
    {
        for (int i = 0; i < SpecialEffectEditorModel.GetInstance().GetRefSpeCount(); i++)
        {
            AddTimeLineItemForRefSpe(i);
        }
    }

    static void AddTimeLineItemForRefSpe(int refSpeIndex)
    {
        TimeLineViewCtrl timeLineViewCtrl = s_root.FindControl<TimeLineViewCtrl>();
        if (timeLineViewCtrl == null)
        {
            return;
        }

        string name = "";
        float startTime = 0f;
        float length = 0f;

        SpecialEffectEditorModel.GetInstance().GetRefSpeName(refSpeIndex, ref name);
        SpecialEffectEditorModel.GetInstance().GetRefSpeStartTime(refSpeIndex, ref startTime);
        SpecialEffectEditorModel.GetInstance().GetRefSpeLength(refSpeIndex, ref length);

        TimeLineItem newTimeLineItem = new TimeLineItem();

        //newItem.name = name + "(参考)";
        //newItem.color = Color.gray; 

        newTimeLineItem.startTime = startTime;
        newTimeLineItem.length = length;
        newTimeLineItem.color = Color.blue;
        newTimeLineItem.onSelectedColor = Color.cyan;
        newTimeLineItem.dragBoxColor = Color.blue;
        newTimeLineItem.dragBoxSelectedColor = Color.cyan;
        newTimeLineItem.visiable = true;
        newTimeLineItem.enable = true;

        SpeEditorTimeLineItem newSpeEditorTimeLineItem = new SpeEditorTimeLineItem();

        newSpeEditorTimeLineItem.ItemType = SPEITEMTYPE.TYPE_REFSPE;
        newSpeEditorTimeLineItem.Visiable = newTimeLineItem.visiable;
        newSpeEditorTimeLineItem.Enable = true;
        newSpeEditorTimeLineItem.BlindSpeItem = null;
        newSpeEditorTimeLineItem.RealTimeLineItem = newTimeLineItem;
        newSpeEditorTimeLineItem.RefSpe = SpecialEffectEditorModel.GetInstance().GetRefSpe(refSpeIndex);

        timeLineViewCtrl.Items.Add(newTimeLineItem);
        speEditorTimeLineItem.Add(newSpeEditorTimeLineItem);

    }
    static bool IsItemVisiable(TreeViewNode node)
    {
        bool bRet = true;

        if(null == node)
        {
            return false;
        }
        
        TreeViewNode parentNode = node.parent;

        while(parentNode != null)
        {
            if(!parentNode.state.IsExpand)
            {
                bRet = false;
                break;
            }

            parentNode = parentNode.parent;
        }

        return bRet;
    }

    static void AddRefSpeToTreeView(int refSpeIndex)
    {
        if(
            (refSpeIndex < 0)
            || (refSpeIndex > SpecialEffectEditorModel.GetInstance().GetRefSpeCount())
            )
        {
            return;
        }

        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;
        if (treeView == null)
        {
            return;
        }

        string name = "";

        SpecialEffectEditorModel.GetInstance().GetRefSpeName(refSpeIndex, ref name);

        TreeViewNode newNode = new TreeViewNode();
        newNode.name = name + "(参考)";
        newNode.image = UnityInternalIconCache.GetInstance().GetCacheIcon("Project");
        newNode.state.IsExpand = true;
        newNode.userObject = SpecialEffectEditorModel.GetInstance().GetRefSpe(refSpeIndex);

        TreeViewNodeUserParam userParam = new TreeViewNodeUserParam();
        userParam.param = true;
        newNode.state.userParams.Add(userParam);

        treeView.Roots.Add(newNode);

    }

    static void AddElementsToTreeView(SpecialEffectElement elem)
    {
        if(null == elem)
        {
            return;
        }

        TreeViewCtrl treeView = s_root.FindControl("_MainTreeView") as TreeViewCtrl;

        if(null == treeView)
        {
            return;
        }

        List<TreeViewNode> currLevelNodeList = treeView.Roots;
        TreeViewNode parentNode = null;
        SpecialEffectElement parentElem = null;

        List<SpecialEffectElement> relationElemList = new List<SpecialEffectElement>();

        parentElem = elem.gameObject.transform.parent.gameObject.GetComponent<SpecialEffectElement>();
        while(parentElem != null)
        {
            relationElemList.Add(parentElem);

            parentElem = parentElem.gameObject.transform.parent.gameObject.GetComponent<SpecialEffectElement>();
        }

        relationElemList.Reverse();
        relationElemList.Add(elem);


        foreach(var item in relationElemList)
        {
            bool findNode = false;

            foreach (var treeNode in currLevelNodeList)
            {
                if (UnityEngine.Object.ReferenceEquals(treeNode.userObject, item))
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
                newNode.name = item.name;
                newNode.image = GetElemIcon(elem);
                newNode.state.IsExpand = true;
                newNode.userObject = item;

                TreeViewNodeUserParam userParam = new TreeViewNodeUserParam();
                userParam.param = true;
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

    static Texture GetElemIcon(SpecialEffectElement elem)
    {
        Texture tex = null;

        if(null == elem)
        {
            return null;
        }

        Type elemType = elem.GetType();

        if (elemType == typeof(SpecialEffectParticleSys))
        {
            tex = UnityInternalIconCache.GetInstance().GetCacheIcon("Particle Effect");
        }
        else if (elemType == typeof(SpecialEffectAnimation))
        {
            tex = UnityInternalIconCache.GetInstance().GetCacheIcon("UnityEditor.AnimationWindow");
        }
        else if (elemType == typeof(SpecialEffectAnimator))
        {
            tex = UnityInternalIconCache.GetInstance().GetCacheIcon("UnityEditor.Graphs.AnimatorControllerTool");
        }

        return tex;
    }
    static private List<SpeEditorTimeLineItem> speEditorTimeLineItem = new List<SpeEditorTimeLineItem>();
}

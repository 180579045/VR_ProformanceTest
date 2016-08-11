using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class SpecialEffectAnimClipEditor
{
    static EditorRoot s_root = null;

    static string s_playCtrlName = "_PlayCtrl";

    static string s_speListCtrlName = "_SpeListCtrl";

    static string s_speTimeLineCtrlName = "_SpeTimeLineCtrl";

    static string s_tabViewCtrlName = "_SpeTabViewCtrl";

    static string s_newBtnName = "_NewBtnCtrl";

    static string s_saveBtnName = "_SaveBtnCtrl";

    static string s_undoBtnName = "_UndoBtnCtrl";

    static string s_redoBtnName = "_RedoBtnCtrl";

    static string s_openFileTextBoxName = "_OpenFileTextBoxCtrl";

    static string s_previewModelCtrlName = "_PreviewModelCtrl";

    static string s_bindingTargetComboBoxName = "_BindingTargetComboBox";

    static string s_bindingTargetTextBoxName = "_BindingTargetTextBox";

    static string s_deathTypeComboBoxName = "_DeathTypeComboBox";

    static string s_editorName = "特效动画编辑器";

    static string s_openFileName = "空";

    [MenuItem("H3D/特效编辑/特效动画编辑器")]
    static void Init()
    {
        EditorRoot root = EditorManager.GetInstance().FindEditor(s_editorName);
        if (root == null)
        {
            root = EditorManager.GetInstance().CreateEditor(s_editorName, false, InitControls);
        } 
    }

    //构建控件
    public static void InitControls(EditorRoot editorRoot)
    {
        s_root = editorRoot;

        editorRoot.RootCtrl = new HSpliterCtrl();
        (editorRoot.RootCtrl as SpliterCtrl).Dragable = false;
        editorRoot.RootCtrl.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30);

        HBoxCtrl menuArea = new HBoxCtrl();
        editorRoot.RootCtrl.Add(menuArea);

        _BuildMenuButtons(menuArea);

        HSpliterCtrl h1 = new HSpliterCtrl();
        h1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(30);
        editorRoot.RootCtrl.Add(h1);

        HBoxCtrl playCtrlArea = new HBoxCtrl();
        h1.Add(playCtrlArea);
        
        //播放控件
        PlayCtrl playCtrl = new PlayCtrl();
        playCtrl.Name = s_playCtrlName;
        playCtrl.onValueChange = OnPlayCtrlValueChange;
        playCtrl.onStop = OnPlayCtrlStop;
        playCtrlArea.Add(playCtrl);

        VSpliterCtrl v1 = new VSpliterCtrl();
        v1.layoutConstraint = LayoutConstraint.GetSpliterConstraint(200);
        v1.Dragable = false;
        VBoxCtrl speListArea = new VBoxCtrl();
        v1.Add(speListArea);
        h1.Add(v1);

        //特效动画子项列表
        ListViewCtrl speListView = new ListViewCtrl();
        speListView.Name = s_speListCtrlName;
        speListView.onAcceptDragObjs = OnSpeListAcceptDragObjs;
        speListView.onDropObjs = OnSpeListDropObjs;
        speListView.onItemSelected = OnSpeListItemSelect;
        speListView.onDoubleClick = OnSpeListItemDoubleClick;
        speListView.onScroll = OnSpeListScroll;
        //speListView.onItemSelectedR = OnSpeListItemRightBtnUp;
        speListView.onItemSelectedRU = OnSpeListItemRightBtnUp;
        speListArea.Add(speListView);

        VSpliterCtrl v2 = new VSpliterCtrl();
        v2.layoutConstraint = LayoutConstraint.GetSpliterConstraint(400,true);
        v2.Dragable = true;
        v1.Add(v2);

        VBoxCtrl timeLineViewArea = new VBoxCtrl();
        v2.Add(timeLineViewArea);

        //时间线控件
        TimeLineViewCtrl timeLineView = new TimeLineViewCtrl();
        timeLineView.Name = s_speTimeLineCtrlName;
        timeLineView.onItemSelected = OnTimeLineItemSelect;
        timeLineViewArea.Add(timeLineView);

        VBoxCtrl tabViewArea = new VBoxCtrl();
        v2.Add(tabViewArea);

        TabViewCtrl tabView = new TabViewCtrl();
        tabView.Name = s_tabViewCtrlName;
        tabViewArea.Add(tabView);
        tabView.onItemSelected = OnTabViewSelect;

        _BuildPropertyTabView(tabView);

        {//注册窗体回调
            editorRoot.onEnable = OnEditorEnable;
            editorRoot.onDisable = OnEditorDisable;
            editorRoot.onUpdate = OnEditorUpdate;
            editorRoot.onDestroy = OnEditorDestroy;
            editorRoot.onGUI = OnEditorGUI;
            editorRoot.onMessage = OnEditorMessage;
        }
    }

    static void _BuildPropertyTabView(EditorControl parent )
    { 
        VBoxCtrl vb0 = new VBoxCtrl(); 
        vb0.Caption = "属性";
        parent.Add(vb0);
         
        ComboBoxCtrl<int> deathTypeComboBox = new ComboBoxCtrl<int>();
        deathTypeComboBox.Name = s_deathTypeComboBoxName;
        deathTypeComboBox.Caption = "死亡类型";
        deathTypeComboBox.onValueChange = OnDeathTypeComboBoxValueChange;
        vb0.Add(deathTypeComboBox);

        VBoxCtrl vb1 = new VBoxCtrl();
        vb1.Caption = "骨骼绑定";
        parent.Add(vb1);

        ComboBoxCtrl<int> bindingTargetComboBox = new ComboBoxCtrl<int>();
        bindingTargetComboBox.Name = s_bindingTargetComboBoxName;
        bindingTargetComboBox.Caption = "绑定骨骼";
        bindingTargetComboBox.onValueChange = OnBindingTargetComboBoxValueChange;
        vb1.Add(bindingTargetComboBox);

        TextBoxCtrl bindingTargetTextBox = new TextBoxCtrl();
        bindingTargetTextBox.Name = s_bindingTargetTextBoxName;
        bindingTargetTextBox.Caption = "绑定骨骼路径";
        bindingTargetTextBox.Enable = false;
        vb1.Add(bindingTargetTextBox);

    }

    static void _BuildMenuButtons(EditorControl parent)
    {
        Rect btnRect = new Rect(0, 0, 60, 20);
        //Rect undoRect = new Rect(0, 0, 120, 20);
        //Rect colorCtrlRect = new Rect(0, 0, 200, 20);
        //Rect returnBtnRect = new Rect(0, 0, 120, 20);

        ButtonCtrl newBtn = new ButtonCtrl();
        newBtn.Name = s_newBtnName;
        newBtn.Caption = "新建";
        newBtn.Size = btnRect;
        newBtn.onClick = OnNewBtnClick;
        parent.Add(newBtn); 
          
        ButtonCtrl saveBtn = new ButtonCtrl();
        saveBtn.Name = s_saveBtnName;
        saveBtn.Caption = "保存";
        saveBtn.Size = btnRect;
        saveBtn.onClick = OnSaveBtnClick;
        parent.Add(saveBtn);


        ButtonCtrl undoBtn = new ButtonCtrl();
        undoBtn.Name = s_undoBtnName;
        undoBtn.Caption = "撤消";
        undoBtn.Size = btnRect;
        undoBtn.onClick =  OnUndoBtnClick;
        parent.Add(undoBtn);

        ButtonCtrl redoBtn = new ButtonCtrl();
        redoBtn.Name = s_redoBtnName;
        redoBtn.Caption = "重做";
        redoBtn.Size = btnRect;
        redoBtn.onClick = OnRedoBtnClick;
        parent.Add(redoBtn);

        TextBoxCtrl openFileTextBox = new TextBoxCtrl();
        openFileTextBox.Name = s_openFileTextBoxName;
        openFileTextBox.Caption = "当前打开文件";
        openFileTextBox.Enable = false;
        openFileTextBox.Text = "空";
        openFileTextBox.Size = new Rect(0, 0, 150, 20);
        parent.Add(openFileTextBox);

        ObjectFieldCtrl previewModelCtrl = new ObjectFieldCtrl();
        previewModelCtrl.Name = s_previewModelCtrlName;
        previewModelCtrl.Caption = "预览模型";
        previewModelCtrl.ObjectType = typeof(GameObject);
        previewModelCtrl.IsAlowSceneObjects = false; 
        previewModelCtrl.onValueChange = OnPreviewModelValueChanged;
        parent.Add(previewModelCtrl);
    }

     static void OnNewBtnClick(EditorControl c)
     {
         SpecialEffectAnimClipEditorModel.GetInstance().NewClip();
     }

     static void OnSaveBtnClick(EditorControl c)
     {
         SpecialEffectAnimClipEditorModel.GetInstance().SaveClip();
     }

    static void OnUndoBtnClick(EditorControl c)
    {
        SpecialEffectAnimClipEditorModel.GetInstance().Undo();
    }

    static void OnRedoBtnClick(EditorControl c)
    {
        SpecialEffectAnimClipEditorModel.GetInstance().Redo();
    }

     static void OnPreviewModelValueChanged(EditorControl c, object value)
     {
         SpecialEffectAnimClipEditorModel.GetInstance().SetPreviewModel(value as UnityEngine.Object);
     }

     static void OnPlayCtrlValueChange(EditorControl c, object value)
     {
         if (SpecialEffectAnimClipEditorModel.GetInstance().IsBindingMode)
         {//对于绑定模式只修改特效动画播放进度
             SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip.CurrentPlayTime = GetPlayCtrl().PlayTime;
         }
         else
         {
             SpecialEffectAnimClipEditorModel.GetInstance().CurrentPlayTime = GetPlayCtrl().PlayTime;
         }
         GetTimeLineViewCtrl().CurrPlayTime = GetPlayCtrl().PlayTime;
     }

     static void OnBindingTargetComboBoxValueChange( EditorControl c , object value )
     {
         var transInfoList = SpecialEffectAnimClipEditorModel.GetInstance().GetTPoseTransformInfos();
         if (transInfoList == null || transInfoList.Count == 0)
             return;

         int boneIndx = (int)value;

         if (boneIndx < 0 || boneIndx >= transInfoList.Count)
             return; 
         
         int i = TransItemIndex(SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect); 
         SpecialEffectAnimClipEditorModel.GetInstance().SetSpeAnimSelectItemBindingPath(i, transInfoList[boneIndx].path);      
     }

     static void OnDeathTypeComboBoxValueChange( EditorControl c , object value )
     {
         int i = TransItemIndex(SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect);

         SpecialEffectAnimClipEditorModel.GetInstance().SetSpeAnimItemDeathType(i, (int)value);          
     }

     static void OnPlayCtrlStop( EditorControl c )
     {
         
     }

    static void OnEditorEnable(EditorRoot root)
    {
        SpecialEffectAnimClipEditorModel.GetInstance().Init();

        SpecialEffectAnimClipEditorModel.GetInstance().onSpeAnimClipNew = OnSpeClipNew;
        SpecialEffectAnimClipEditorModel.GetInstance().onSpeAnimClipOpen = OnSpeClipOpen;
        SpecialEffectAnimClipEditorModel.GetInstance().onSpeAnimClipItemNumChange = OnSpeClipItemNumChange;
        SpecialEffectAnimClipEditorModel.GetInstance().onSpeAnimClipValueChange = OnSpeClipValueChange;
        SpecialEffectAnimClipEditorModel.GetInstance().onSpeAnimClipItemSelect = OnSpeClipItemSelectionChange;
        SpecialEffectAnimClipEditorModel.GetInstance().onSpeAnimClipSetNewAction = OnSpeSetNewPreviewAction;

        RefreshUIState();
    }

    static void OnEditorDisable(EditorRoot root)
    { 

    }

    static void OnEditorGUI(EditorRoot root)
    {

    }

    static void OnEditorUpdate(EditorRoot root)
    {

    }

    static void OnEditorDestroy(EditorRoot root)
    {
        SpecialEffectAnimClipEditorModel.GetInstance().Destory();
    }

    static void OnEditorMessage(ControlMessage msg)
    { 
        
        switch (msg.msg)
        {
            case ControlMessage.Message.TIMELINECTRL_BEGIN_DRAG_TAG:
            case ControlMessage.Message.TIMELINECTRL_DRAG_TAG:
            case ControlMessage.Message.TIMELINECTRL_END_DRAG_TAG:  
                break;
            case ControlMessage.Message.TIMELINECTRL_BEGIN_DRAG_ITEMS:
                OnTimeLineDragItemsBegin(msg.sender, msg.param0 as List<TimeLineViewCtrl.ItemSelectInfo> );
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
    
    static void OnTimeLineDragItemsBegin(EditorControl c, List<TimeLineViewCtrl.ItemSelectInfo> indxList)
    {
        TimeLineViewCtrl timeLineCtrl = c.Root.FindControl<TimeLineViewCtrl>();
        if (timeLineCtrl == null)
            return;
          
        RequestRepaint();
    }

    //时间线在拖拽中
    static void OnTimeLineDragItems(EditorControl c, List<TimeLineViewCtrl.ItemSelectInfo> indxList)
    {
        TimeLineViewCtrl timeLineCtrl = c.Root.FindControl<TimeLineViewCtrl>(); 
        if (timeLineCtrl == null)
            return;
         
        foreach( var selItem in indxList )
        {
           int i = selItem.indx;
           var item = timeLineCtrl.Items[i];
           SpecialEffectAnimClipEditorModel.GetInstance().SetSpeAnimItemTimeLine( TransItemIndex(i), item.startTime, item.length);
        }

        RequestRepaint();
    }

    static void OnTimeLineDragItemsEnd(EditorControl c, List<TimeLineViewCtrl.ItemSelectInfo> indxList)
    {
        TimeLineViewCtrl timeLineCtrl = c.Root.FindControl<TimeLineViewCtrl>(); 
        if (timeLineCtrl == null)
            return;

        foreach (var selItem in indxList)
        {
            int i = selItem.indx;
            var item = timeLineCtrl.Items[i];
            SpecialEffectAnimClipEditorModel.GetInstance().SetSpeAnimItemTimeLine( TransItemIndex(i) , item.startTime, item.length);
        }
        RequestRepaint();
    }

    static void OnTimeLineItemSelect(EditorControl c, int item)
    { 
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = item;
    }

    static bool OnSpeListAcceptDragObjs(EditorControl c, UnityEngine.Object[] objs, string[] paths)
    {
        //不支持多项拖入
        if (objs.Length > 1)
            return false;

        if (SpecialEffectAnimClipEditorModel.GetInstance().IsBindingMode)
        {//骨骼绑定模式不允许拖拽
            return false;
        }


        UnityEngine.Object obj = objs[0]; 

        if ( SpecialEffectAnimClipEditorModel.GetInstance().IsLegalItem(obj) )
            return true;

        GameObject go = obj as GameObject; 
        if (go != null)
        {
            return true;
        } 

        return false;
    }

    static void OnSpeListDropObjs(EditorControl c, UnityEngine.Object[] objs, string[] paths)
    {
        if (objs.Length > 1)
            return;


        if (SpecialEffectAnimClipEditorModel.GetInstance().IsBindingMode)
        {//骨骼绑定模式不允许拖拽
            return;
        }

        //支持拖拽动画片段子项
        UnityEngine.Object obj = objs[0]; 
        if (SpecialEffectAnimClipEditorModel.GetInstance().IsLegalItem(obj))
        {
            SpecialEffectAnimClipEditorModel.GetInstance().AddItem(obj);
            return;
        }

        //支持拖拽动画片段
        GameObject go = obj as GameObject;
        var clip = go.GetComponent<SpecialEffectAnimationClip>();
        if (clip != null)
        {
            SpecialEffectAnimClipEditorModel.GetInstance().OpenClip(obj);
            return;
        }

        //支持拖拽AnimationClip
        string assetPath = AssetDatabase.GetAssetPath(obj);
        AnimationClip animClip = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimationClip)) as AnimationClip;
        if( animClip != null )
        {
            SpecialEffectAnimClipEditorModel.GetInstance().SetPreviewModelAction(animClip);
            return;
        }
    }

    static void OnSpeListItemSelect( EditorControl c , int item )
    {
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = item;
    }

    static void OnSpeListItemDoubleClick( EditorControl c , object value )
    {
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = (int)value;

        int itemIndx = TransItemIndex((int)value);
        if (itemIndx < 0)
        {//略过动画项 
            Selection.activeObject = null;
            return;
        } 
        var clip = SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip;
        if (clip == null)
            return;

        var clipItem = clip.QueryItem(itemIndx);
        Selection.activeObject = clipItem.obj;
    }

    static void OnSpeListScroll(EditorControl c, Vector2 scrollPos)
    {
        //联动时间线视口
        GetTimeLineViewCtrl().ScrollPos = scrollPos;
    }

    static void OnSpeListItemRightBtnUp( EditorControl c , int item )
    {
        if( item == 0 )
        {//跳过预览项动作
            return;
        }

        if( SpecialEffectAnimClipEditorModel.GetInstance().IsBindingMode )
        {//骨骼绑定模式不允许删除项
            return;
        }

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("删除"), false, OnSpeListRemoveItem, item);
        menu.ShowAsContext();
    }

    static void OnSpeListRemoveItem(object userData)
    {
        int itemIndx = (int)userData;
        SpecialEffectAnimClipEditorModel.GetInstance().RemoveItem( TransItemIndex(itemIndx) );
    }



    static void OnTabViewSelect( EditorControl c, int item )
    {
        if( item == 0 )
        {//编辑模式

            GetNewBtnCtrl().Enable = true;
            GetSaveBtnCtrl().Enable = true;
            GetUndoBtnCtrl().Enable = true;
            GetRedoBtnCtrl().Enable = true;
            GetPreviewModelCtrl().Enable = true;
            GetListViewCtrl().Enable = true;
            GetTimeLineViewCtrl().Enable = true;
            GetPlayCtrl().Enable = true;

            SpecialEffectAnimClipEditorModel.GetInstance().ShowRefModelBoneSpheres(false);
             
            SpecialEffectAnimClipEditorModel.GetInstance().IsBindingMode = false;

            SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        }
        else
        {//骨骼绑定模式

            GetNewBtnCtrl().Enable = false;
            GetSaveBtnCtrl().Enable = false;
            GetUndoBtnCtrl().Enable = false;
            GetRedoBtnCtrl().Enable = false;
            GetPreviewModelCtrl().Enable = false;
            GetListViewCtrl().Enable = true;
            GetTimeLineViewCtrl().Enable = false;
            GetPlayCtrl().Enable = true; 
            
            //骨骼绑定模式将模型强制变为TPose
            SpecialEffectAnimClipEditorModel.GetInstance().ShowPreviewModelTPose();

            SpecialEffectAnimClipEditorModel.GetInstance().ShowRefModelBoneSpheres(true);

            SpecialEffectAnimClipEditorModel.GetInstance().IsBindingMode = true;
        }
         
        RequestRepaint();
    }




    static void OnSpeClipOpen()
    {
        RebuildListViewAndTimeLineView();
        UpdatePreviewLength(); 
        RefreshUIState();
    }

    static void OnSpeClipNew()
    {
        OnSpeClipOpen();
    }

    static void OnSpeClipItemNumChange()
    {
        OnSpeClipOpen();
    }

    static void OnSpeSetNewPreviewAction()
    {
        OnSpeClipOpen(); 
    }
    static void OnSpeClipValueChange()
    {
        var clipProxy = SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip;
        TimeLineViewCtrl timeLineView = GetTimeLineViewCtrl();

        //同步时间线
        int i = 0;

        //更新预览动作时间线
        var previewAction = SpecialEffectAnimClipEditorModel.GetInstance().GetPreviewModelAction();
        if( previewAction != null )
        {
            timeLineView.Items[i].startTime = 0.0f;
            timeLineView.Items[i].length = previewAction.Length; 
        }
        else
        {
            timeLineView.Items[i].startTime = 0.0f;
            timeLineView.Items[i].length = 0.0f;
        }
        i++;

        foreach( var item in clipProxy.Clip.itemList )
        {
            timeLineView.Items[i].startTime = item.StartTime;
            timeLineView.Items[i].length = item.Length;
            i++;    
        }
         
        UpdatePreviewLength(); 
        RefreshUIState();
    }
    

    static void OnSpeClipItemSelectionChange()
    {
        RefreshListViewAndTimeLineViewSelectItem();
        RefreshSelItemPropertyTabView();

        RefreshUIState();
    }

    static void UpdatePreviewLength()
    {
        GetPlayCtrl().TotalTime = SpecialEffectAnimClipEditorModel.GetInstance().PreviewLength;
        GetTimeLineViewCtrl().TotalTime = SpecialEffectAnimClipEditorModel.GetInstance().PreviewLength;
    }

    static void RebuildListViewAndTimeLineView()
    {
        var clip = SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip;

        //更新ListView
        ListViewCtrl listView = GetListViewCtrl();
        listView.ClearItems();

        var previewAnimClip = clip.PreviewAnimClip;

        {
            ListCtrlItem item = new ListCtrlItem();
            if (previewAnimClip != null)
            {
                item.name = previewAnimClip.name;
            }
            else
            {
                item.name = "空动作";
            }
            item.image = UnityInternalIconCache.GetInstance().GetCacheIcon("UnityEditor.AnimationWindow");
            listView.AddItem(item);
        }


        foreach (var item in clip.Clip.itemList)
        {
            ListCtrlItem listItem = new ListCtrlItem();
            listItem.name = item.obj.name;
            if( item as SpecialEffectAnimClipAudioItem != null )
            {
                listItem.image = UnityInternalIconCache.GetInstance().GetCacheIcon("SceneViewAudio");
            }
            else if (item as SpecialEffectAnimClipEffectItem != null)
            {
                listItem.image = UnityInternalIconCache.GetInstance().GetCacheIcon("AvatarPivot");
            }
            listView.AddItem(listItem);
        }
         

        //更新TimeLineView
        TimeLineViewCtrl timeLineView = GetTimeLineViewCtrl();
        timeLineView.Items.Clear();

        {
            TimeLineItem item = new TimeLineItem();
            item.startTime = 0.0f;
            if (previewAnimClip != null)
            { 
                item.length = previewAnimClip.length;
            }
            else
            { 
                item.length = 0.0f;
            }
            timeLineView.Items.Add(item); 
        }
        
        foreach (var item in clip.Clip.itemList)
        {
            TimeLineItem timeLineItem = new TimeLineItem();
            timeLineItem.startTime = item.StartTime;
            timeLineItem.length = item.Length;
            timeLineView.Items.Add(timeLineItem);
        }

        UpdatePreviewLength();
    }

    static void RefreshListViewAndTimeLineViewSelectItem()
    {
        ListViewCtrl listView = GetListViewCtrl();
        TimeLineViewCtrl timeLineView = GetTimeLineViewCtrl();
        int sel = SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect;
        listView.SelectItems.Clear();
        listView.SelectItems.Add(sel);
        timeLineView.LastSelectedItem = sel;
    }

    static void RefreshSelItemPropertyTabView()
    {
        if (SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip == null)
            return;

        int itemIndx = TransItemIndex(SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect);
        if (itemIndx < 0)
        {//略过动画项

            GetBindingTargetComboBoxCtrl().Enable = false; 
            GetDeathTypeComboBoxCtrl().Enable = false;
            return;
        }

        GetBindingTargetComboBoxCtrl().Enable = true; 
        GetDeathTypeComboBoxCtrl().Enable = true;
         
        var clip = SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip;
        var clipItem = clip.QueryItem(itemIndx); 

        //在InspectorView高亮选中特效动画项
        if( clipItem as SpecialEffectAnimClipEffectItem != null )
        {
            var effectItem = clipItem as SpecialEffectAnimClipEffectItem;
            Selection.activeObject = effectItem.effInst.gameObject;
        }
        else
        {
            Selection.activeObject = null;
        }

        var transInfoList = SpecialEffectAnimClipEditorModel.GetInstance().GetTPoseTransformInfos();
        var comboBox = GetBindingTargetComboBoxCtrl();
        comboBox.ClearItem();

        string currItemBindPath = clipItem.bindingTargetPath;
        int i = 0;

		//Add by HouXiaoGang for 代码改善 Start
		ComboItem ite = new ComboItem("None",i);
		comboBox.AddItem(ite);
		//Add by HouXiaoGang  End

        foreach (var trans in transInfoList)
        {
            string path = trans.path; 
            ComboItem item = new ComboItem(path.Substring(path.LastIndexOf('/') + 1), i);
            comboBox.AddItem(item);
            i++;
        }

        i = 0;
		
        int currSel = -1; 
        foreach (var trans in transInfoList)
        {
            if( trans.path == currItemBindPath )
            {
                currSel = i;
                break;
            }
            i++;
        }  
        comboBox.CurrValue = currSel;

        GetBindingTargetTextBoxCtrl().Text = currItemBindPath; 


        var deathTypeComboBox = GetDeathTypeComboBoxCtrl();
        deathTypeComboBox.ClearItem();
        deathTypeComboBox.AddItem(new ComboItem("停止", 0));
        deathTypeComboBox.AddItem(new ComboItem("未播放的不播放", 1));
        deathTypeComboBox.CurrValue = clipItem.deathType;

        GUI.FocusControl("");
    }

    static void RefreshUIState()
    {
        var clipProxy =  SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip;
        string dirtyMark = "";
        if( clipProxy == null )
        {
            s_openFileName = "空";
        }
        else
        {
            if( clipProxy.ClipPrefab == null )
            {
                s_openFileName = "未命名新动画片段";
            }
            else
            {
                s_openFileName = clipProxy.ClipPrefab.name;
            }

            if( clipProxy.IsDirty )
            {
                dirtyMark = "*";
            }
        } 
        (s_root.FindControl(s_openFileTextBoxName) as TextBoxCtrl).Text = s_openFileName + dirtyMark; 

        if( SpecialEffectAnimClipEditorModel.GetInstance().CanUndo() )
        {
            GetUndoBtnCtrl().Enable = true;
        }
        else
        {
            GetUndoBtnCtrl().Enable = false;
        }

        if( SpecialEffectAnimClipEditorModel.GetInstance().CanRedo() )
        {
            GetRedoBtnCtrl().Enable = true;
        }
        else
        {
            GetRedoBtnCtrl().Enable = false;
        }
    }

    static SpecialEffectAnimClipItem GetCurrentSelectItem()
    {
        int itemIndx = TransItemIndex(SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect);
        if (itemIndx < 0)
        {
            return null;
        }
        var clip = SpecialEffectAnimClipEditorModel.GetInstance().CurrentClip;
        return clip.QueryItem(itemIndx); 
    }

    static int TransItemIndex( int i )
    {
        return i - 1;
    }

    /// <summary>
    /// 获取控件助手函数
    /// </summary> 

    static ButtonCtrl GetNewBtnCtrl()
    {
        if (s_root == null)
            return null;

        return s_root.FindControl(s_newBtnName) as ButtonCtrl;
    }

    static ButtonCtrl GetSaveBtnCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_saveBtnName) as ButtonCtrl;
    }

    static ButtonCtrl GetUndoBtnCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_undoBtnName) as ButtonCtrl;
    }

    static ButtonCtrl GetRedoBtnCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_redoBtnName) as ButtonCtrl;
    }

    static ObjectFieldCtrl GetPreviewModelCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_previewModelCtrlName) as ObjectFieldCtrl;
    }

    static PlayCtrl GetPlayCtrl()
    {
        if (s_root == null)
            return null;

        return s_root.FindControl(s_playCtrlName) as PlayCtrl;
    }

    static ListViewCtrl GetListViewCtrl()
    {
        if (s_root == null)
            return null;

        return s_root.FindControl(s_speListCtrlName) as ListViewCtrl;
    }

    static TimeLineViewCtrl GetTimeLineViewCtrl()
    {
        if (s_root == null)
            return null;

        return s_root.FindControl(s_speTimeLineCtrlName) as TimeLineViewCtrl;
    }

    static ComboBoxCtrl<int> GetBindingTargetComboBoxCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_bindingTargetComboBoxName) as ComboBoxCtrl<int>;
    }

    static ComboBoxCtrl<int> GetDeathTypeComboBoxCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_deathTypeComboBoxName) as ComboBoxCtrl<int>;
    }


    static TextBoxCtrl GetBindingTargetTextBoxCtrl()
    {
        if (s_root == null)
            return null;
        return s_root.FindControl(s_bindingTargetTextBoxName) as TextBoxCtrl;
    }

    static void RequestRepaint()
    {
        SceneView.RepaintAll();
        s_root.RequestRepaint();
    }
}

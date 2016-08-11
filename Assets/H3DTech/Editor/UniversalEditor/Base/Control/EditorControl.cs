using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class TriggerInfo
{
    public bool isCtrlBehaveChange = false;
    //本帧是否点击
    public bool isClick = false;
    public bool isHover = false;
    public bool isValueChanged = false;
    public bool isScroll = false;

    public bool isDoubleClick = false;
    public bool isOnPress = false;
    public bool isPressDown = false;
    public bool isPressUp = false;

    public bool isRPressDown = false;
    public bool isRPressUP = false;
    public bool isROnPress = false;

    public bool isDraggingObjs = false;
    public bool isDropObjs = false;
     
    public int lastSelectItem = -1;
    //add by liteng for atlas start
    public int lastSelectItemR = -1;
    public int lastSelectItemRU = -1;
    //add by liteng for atlas end
    public Vector2 scrollPos = new Vector2();

    //Add by liteng for MoveAtlas At 2014/1/4 Start
    public bool isCustomDragAccept = false;
    public bool isCustomDragAcceptCtrl = false;
    public bool isCtrlSelectItem = false;
    public int lastCtrlSelectItem = -1;
    //Add by liteng for MoveAtlas End

    public bool isHandleInput = false;

    public bool isPlay = false;
    public bool isPause = false;
    public bool isStop = false;

    public void Reset()
    {
        isCtrlBehaveChange = false;
        isClick = false;
        isHover = false;
        isValueChanged = false;
        isScroll = false;  
        lastSelectItem = -1;
        //add by liteng for atlas start
        lastSelectItemR = -1;
        lastSelectItemRU = -1;
        //add by liteng for atlas end
        scrollPos.Set(0f, 0f);

        isDoubleClick = false;
        isOnPress = false;
        isPressDown = false;
        isPressUp = false;

        isRPressDown = false;
        isRPressUP = false;
        isROnPress = false;

        isDraggingObjs = false;
        isDropObjs = false;

        //Add by liteng for MoveAtlas At 2014/1/4 Start
        isCustomDragAccept = false;
        isCustomDragAcceptCtrl = false;
        isCtrlSelectItem = false;
        lastCtrlSelectItem = -1;
        //Add by liteng for MoveAtlas End

        isHandleInput = false;

        isPlay = false;
        isPause = false;
        isStop = false;
    }
}

public class EditorControl 
{
    public EditorControl()
    {
        this.name = Guid.NewGuid().ToString();
    }

    public EditorRoot Root
    {
        get { return root; }
        set { root = value; }
    }

    public EditorControl Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Caption
    {
        get { return caption; }
        set { caption = value; }
    }

    public bool IsRoot
    {
        get { return parent == null; }
    }

    public Rect Size
    {
        get { return sizeRect; }
        set 
        { 
            sizeRect = value; 
        }
    }

    public Rect LastRect
    {
        get { return lastRect; }
    }

    public Rect LastContentRect
    {
        get { return lastContentRect; }
    }

    //Modify by lteng for 追加共通控件 At 2015/2/26
    virtual public object CurrValue
    {
        get { return currValue;}
        set 
        { 
            currValue = value;
        }
    }

    public float ValueEpsilon
    {
        get { return valueEpsilon; }
        set { valueEpsilon = value; }
    }

    public Vector2 ValueRange
    {
        get { return valueRange; }
        set { valueRange = value; }
    }

    public bool Visiable
    {
        get { return visiable; }
        set 
        {
            if (visiable != value)
            {
                RequestRepaint();
            }
            visiable = value; 
        }
    }

    virtual public bool Enable
    {
        get { return enable; }
        set
        {
            if (enable != value)
            {
                RequestRepaint();
            }

            //if (IsRoot)
            //{
            //    Root.SetEditorRootEnable(value);
            //}
            enable = value;
        }
    }

    //框架内部变更控件enable属性(针对OnGui重入问题)
    public void SetEnable(bool value)
    {
        enable = value;
    }
    //Add by liteng for MoveAtlas At 2014/1/4 Start
    public object DragObject
    {
        get { return dragObject; }
        set { dragObject = value; }
    }

    public object UserDefData
    {
        get { return userDefData; }
        set { userDefData = value; }
    }

    public Vector2 ScrollPos
    {
        get { return scrollPos; }
        set { scrollPos = value; }
    }

    public bool IsShowScrollBar
    {
        set
        {
            isShowScrollBar = value;
        }
        get { return isShowScrollBar; }
    }

    public object ClickObject
    {
        set
        {
            clickObject = value;
        }
        get { return clickObject; }
    }

    public string CtrlID
    {
        set
        {
            ctrlID = value;
        }

        get
        {
            return ctrlID;
        }

    }

    public string DragStartType
    {
        set
        {
            dragStartType = value;
        }

        get
        {
            return dragStartType;
        }

    }

    public string DragAcceptType
    {
        set
        {
            dragAcceptType = value;
        }

        get
        {
            return dragAcceptType;
        }
    }

    public delegate void VoidDelegate( EditorControl c );
    public delegate void BoolDelegate( EditorControl c , bool state);

    //Modify by lteng for 追加共通控件 At 2015/2/26
    public delegate void ObjectDelegate( EditorControl c , object value);

    public delegate void IntDelegate(EditorControl c, int value);
    public delegate void Vec2Delegate(EditorControl c, Vector2 scrollPos);
    public delegate void DragObjsDelegate(EditorControl c, UnityEngine.Object[] objs, string[] paths);
    public delegate bool AcceptDragObjsDelegate(EditorControl c, UnityEngine.Object[] objs, string[] paths); 
	//Add by liteng for MoveAtlas At 2014/1/5 Start
    public delegate object PrepareCustomDrag(EditorControl c);
    public delegate bool TryAcceptCustomDrag(EditorControl c, object dragObject);
    public delegate void AcceptCustomDrag(EditorControl c, object dragObject);
    //Add by liteng for MoveAtlas End

    //记录本帧的操作信息
    public TriggerInfo frameTriggerInfo = new TriggerInfo();

    //布局约束
    public LayoutConstraint layoutConstraint = new LayoutConstraint();

    public VoidDelegate onCtrlBehaveChange = null;
    //消息回调
    public VoidDelegate  onClick = null;
    public VoidDelegate  onHover = null;

    //Modify by lteng for 追加共通控件 At 2015/2/26
    public ObjectDelegate onValueChange = null;
    public ObjectDelegate  onDoubleClick = null;
    public ObjectDelegate onOnPress = null;
    public ObjectDelegate onPressDown = null;
    public ObjectDelegate onPressUp = null;
    public ObjectDelegate onRPressDown = null;
    public ObjectDelegate onRPressUp = null;

    public IntDelegate   onItemSelected = null;
    //Add by liteng for atlas start
    public IntDelegate onItemSelectedR = null;
    public IntDelegate onItemSelectedRU = null;
    //Add by liteng for atlas end
    public Vec2Delegate  onScroll = null;

    public IntDelegate onDragItemBegin = null;
    public IntDelegate onDragItem = null;
    public IntDelegate onDragItemEnd = null;

    //用于外部向编辑器拖拽资源
    public DragObjsDelegate onDragingObjs = null;
    public DragObjsDelegate onDropObjs = null;
    //用于判断是否接收拖拽物体
    public AcceptDragObjsDelegate onAcceptDragObjs = null;

    //Add by liteng for MoveAtlas At 2014/1/4 Start 
    public PrepareCustomDrag onPrepareCustomDrag = null;
    public TryAcceptCustomDrag onTryAcceptCustomDrag = null;
    public AcceptCustomDrag onAcceptCustomDrag = null;
    public AcceptCustomDrag onAcceptCustomDragCtrl = null;
    public IntDelegate onItemCtrlSelected = null;
    //Add by liteng for MoveAtlas End

    public VoidDelegate onPlay = null;
    public VoidDelegate onPause = null;
    public VoidDelegate onStop = null;

    public virtual void Add(EditorControl c) { }

    public virtual bool Insert(EditorControl c, EditorControl insertBefore) { return false; }

    public virtual void Remove(EditorControl c) { }
    //Add by liteng for MoveAtlas At 2014/1/5
    public virtual void RemoveAll() { }
    public virtual EditorControl GetAt(int i) { return null; }
    public virtual int GetChildCount() { return 0; }
    public virtual void Traverse(EditorCtrlVisitor v) 
    {
        if (v.PreVisit(this))
        {
            v.Visit(this);
            v.AfterVisit(this);
        }
    }

  
    public virtual GUIStyle GetStyle() { return null; }
    public virtual GUILayoutOption[] GetOptions() 
    { 
        if( layoutConstraint != null )
             return layoutConstraint.GetOptions();
        return null;
    }

    public virtual bool IsEventTriggered()
    {
        bool bRet = false;

        if(null == frameTriggerInfo)
        {
            return false;
        }

        bRet = frameTriggerInfo.isHandleInput;
        //do
        //{
        //    if (frameTriggerInfo.isClick)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isHover)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isValueChanged)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.lastSelectItem != -1)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.lastSelectItemR != -1)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.lastSelectItemRU != -1)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isScroll)
        //    {
        //        bRet = true;
        //        break;
        //    }


        //    if (frameTriggerInfo.isDraggingObjs)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isDropObjs)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isCustomDragAccept)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isCustomDragAcceptCtrl)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isCtrlSelectItem)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isDoubleClick)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isOnPress)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isPressDown)
        //    {
        //        bRet = true;
        //        break;
        //    }

        //    if (frameTriggerInfo.isPressUp)
        //    {
        //        bRet = true;
        //        break;
        //    }
        //} while (false);

        return bRet;
    }

    public void SetDragObject(object obj)
    {
        if(null == obj)
        {
            return;
        }

        DragAndDrop.SetGenericData(dragStartType, obj);

        dragObject = obj;
    }
    public bool IsCurrentCtrlEnable()
    {
        bool bRet = true;
        bool parentEnable = true;

        if(null == this.parent)
        {
            bRet = this.enable;
        }
        else
        {
            EditorControl parent = this.parent;
            while (parent != null)
            {
                parentEnable = parent.enable;
                if (!parentEnable)
                {
                    break;
                }

                parent = parent.parent;
            }

            bRet = parentEnable && this.enable;
        }


        return bRet;
    }
    public void UpdateLastRect()
    { 
        SpecialEffectEditorUtility.GetLastRect( ref lastRect );
    }

    public void UpdateContentRect()
    {
        SpecialEffectEditorUtility.GetLastRect( ref lastContentRect );
    }

    public Vector2 CalcLocalPos( Vector2 p )
    {
        Vector2 lastXY = new Vector2(lastRect.x, lastRect.y);
        return p - GetParentLastXY(parent, lastXY);
    }

    private Vector2 GetParentLastXY(EditorControl parentCtrl, Vector2 p)
    {
        if(null == parentCtrl)
        {
            return p;
        }

        Vector2 parentXY = new Vector2(0, 0);

        if(parentCtrl.isShowScrollBar)
        {
            parentXY = new Vector2(p.x + parentCtrl.lastRect.x - parentCtrl.scrollPos.x, p.y + parentCtrl.lastRect.y - parentCtrl.scrollPos.y);
        }
        else
        {
            parentXY = p;
        }

        return GetParentLastXY(parentCtrl.parent, parentXY);
    }

    public void RequestRepaint()
    {
        if (null != Root)
        {
            Root.renderer.RequestRepaint();
        }
        else if ( !IsRoot )
        {
            Parent.RequestRepaint();
        }
    }

    //用于控件发送消息
    public void PostMessage( ControlMessage.Message msg , object p0 = null, object p1 = null )
    {
        if( null != Root )
        {
            ControlMessage newMsg = new ControlMessage(this, msg, p0, p1);
            Root.EnqueueMessage(newMsg);
        }
    }

    public string GetCtrlIDPath()
    {
        string IDPath = this.ctrlID;
        EditorControl parentCtrl = this.parent;

        while (parentCtrl != null)
        {
            IDPath = parentCtrl.ctrlID + "/" + IDPath;
            parentCtrl = parentCtrl.parent;
        }

        return IDPath;
    }

    public EditorRoot GetEditorRoot()
    {
        EditorControl parentCtrl = parent;
        if ((root != null) || (null == parentCtrl))
        {
            return root;
        }

        while ((parentCtrl != null) && (null == parentCtrl.root))
        {
            parentCtrl = parentCtrl.parent;
        }

        if (parentCtrl != null)
        {
            return parentCtrl.root;
        }
        else
        {
            return null;
        }
    }

    //控件所属编辑器窗口
    protected EditorRoot root;
    //控件名
    private string name;
    //控件标题
    private string caption;
    //控件的大小
    private Rect sizeRect = new Rect(0, 0, 200, 20);
    //控件最终绘制大小
    private Rect lastRect = new Rect();

    private Rect lastContentRect = new Rect();

    //Modify by lteng for 追加共通控件 At 2015/2/26
    //控件当前值
    protected object currValue = null;

    //控件变更阈值
    private float valueEpsilon = 0.001f;
    //控件值域
    private Vector2 valueRange = new Vector2();
    //父控件
    private EditorControl parent;
    //控件是否为可见状态
    private bool visiable = true;
    //控件是否为有效状态
    protected bool enable = true;

    //Add by liteng for MoveAtlas At 2014/1/4
    private object dragObject = null;

    private object userDefData = null;

    private Vector2 scrollPos = new Vector2(0, 0);

    private bool isShowScrollBar = false;

    private bool isForceUpdate = false;
    private bool isFocusLastFrame = false;

    private object clickObject = null;

    private string ctrlID = "DefaultID";

    private string dragStartType = string.Empty;

    private string dragAcceptType = string.Empty;

    public bool IsForceUpdate
    {
        get { return isForceUpdate; }
        set { isForceUpdate = value; }
    }

    public bool IsFocusLastFrame
    {
        get { return isFocusLastFrame; }
        set { isFocusLastFrame = value; }
    }

    private bool isForceUpdateText = false;

    public bool IsForceUpdateText
    {
        set
        {
            isForceUpdateText = value;
        }

        get
        {
            return isForceUpdateText;
        }
    }
}

public class EditorCtrlComposite : EditorControl
{
    private void UpdateCtrlCountAndID(EditorRoot root, EditorControl rootCtrl)
    {
        if (GetEditorRoot() != null)
        {
            EditorManager.GetInstance().AssignCtrlID(root, rootCtrl);
        }
    }

    public override void Add(EditorControl c) 
    { 
        //若找到相同控件则不加入列表
        if (children.Contains(c))
            return;

        c.Root = this.Root;
        c.Parent = this;
        children.Add(c);

        //if (
        //       (GetEditorRoot() != null)
        //    && (GetEditorRoot().CtrlCounter > 0)
        //    )
        //{
        //    GetEditorRoot().CtrlCounter++;
        //}

        UpdateCtrlCountAndID(GetEditorRoot(), c);

        //控件关系发生变化需重绘
        RequestRepaint();
    }

    public override bool Insert(EditorControl c, EditorControl insertBefore)
    {
        if (c == null || insertBefore == null)
            return false;

        if (!children.Contains(insertBefore))
            return false;
        
        int i = children.IndexOf(insertBefore);
        if( !c.IsRoot )
        {
            c.Parent.Remove(c);
        }
        c.Root = this.Root;
        c.Parent = this;
        children.Insert(i, c);

        UpdateCtrlCountAndID(GetEditorRoot(), c);

        //控件关系发生变化需重绘
        RequestRepaint();
        return true;
    }

    public override void Remove(EditorControl c) 
    {
        if( children.Remove(c) )
        {
            c.Parent = null;
        }

        //控件关系发生变化需重绘
        RequestRepaint();
    }

    //Add by liteng for MoveAtlas At 2014/1/5 Start
    public override void RemoveAll()
    {
        foreach(var item in children)
        {
            item.Parent = null;
        }

        children.Clear();

        //控件关系发生变化需重绘
        RequestRepaint();
    }
    //Add by liteng for MoveAtlas End

    public override EditorControl GetAt(int i) 
    {
        if (i < 0 || i >= children.Count)
            return null;
 
        return children[i];
    }

    public override int GetChildCount() 
    { 
        return children.Count; 
    }

    public override void Traverse(EditorCtrlVisitor v) 
    {
        //若在预访问阶段判断不通过，则不访问此子树
        if (!v.PreVisit(this))
            return;

        v.Visit(this);
        v.AfterVisit(this);

        if (!v.PreVisitChildren(this))
            return;

        int i = 0;
        foreach( var child in children )
        {
            if (v.PreVisitChild(this, i))
            {
                child.Traverse(v);
                v.AfterVisitChild(this, i);
            }
            i++;
        }
        v.AfterVisitChildren(this);
    }

    public override bool IsEventTriggered()
    {
        bool bRet = false;

        bRet = frameTriggerInfo.isHandleInput;
        if (bRet)
        {
            return true;
        }

        foreach (var item in children)
        {
            bRet = item.IsEventTriggered();
            if (bRet)
            {
                break;
            }
        }

        return bRet;
    }
    //子控件
    public List<EditorControl> children = new List<EditorControl>();

}
using UnityEngine;
using System.Collections;
using UnityEditor;


public class FrameInputInfo
{
    public Vector2 currPos = new Vector2();
    public Vector2 posOffset = new Vector2();
    //鼠标滚轮
    public Vector2 delta = new Vector2();

    public bool leftButtonDown = false;
    public bool midButtonDown = false;
    public bool rightButtonDown = false;

    public bool leftBtnPress = false;
    public bool midBtnPress = false;
    public bool rightBtnPress = false;

    public bool leftBtnPressUp = false;
    public bool midBtnPressUp = false;
    public bool rightBtnPressUp = false;

    public bool drag = false;

    //鼠标滚轮滚动
    public bool scroll = false;

    //用于外部向编辑器拖拽物体
    public bool dragingObjs = false;
    public bool dragObjsPerform = false;
    public Object[] dragObjs = null;
    public string[] dragObjsPaths = null;

    //Add by liteng for MoveAtlas At 2015/1/4 Start
    public bool leftBtnDoubleClick = false;
    public bool customDragUpdated = false;
    //Add by liteng for MoveAtlas End
    public bool leftBtnClick = false;
    public bool leftBtnOnPress = false;

    public bool hasInput = false;

    public bool isMouseInWindow = true;

    public void Update(EditorWindow wnd)
    {
        if(null == wnd)
        {
            return;
        }

        _FrameReset();

        Event currEvent = Event.current;
        Vector2 lastMousePos = currPos;
        currPos = currEvent.mousePosition;
        posOffset = currPos - lastMousePos;

        switch (currEvent.type)
        {
            case EventType.MouseDown:
                if (currEvent.button == 0)
                {

                    //若上一帧鼠标键没有按下
                    if (leftButtonDown == false)
                    {
                        leftBtnPress = true;
                        if (0 == btnDownCount)
                        {
                            btnDownTime = System.DateTime.Now.Ticks;
                            btnDownPos = currEvent.mousePosition;
                        }
                        btnDownCount = currEvent.clickCount;

                    }
                    leftButtonDown = true;
                }

                //鼠标右键按下
                if( currEvent.button == 1)
                {
                    if(rightButtonDown == false )
                    {
                        rightBtnPress = true;
                    }
                    rightButtonDown = true;
                }

                //鼠标中键按下
                if( currEvent.button == 2)
                {
                    if (midButtonDown == false)
                    {
                        midBtnPress = true;
                    }
                    midButtonDown = true; 
                }
                break;

            case EventType.MouseUp:

                if (currEvent.button == 0)
                {
                    if( leftButtonDown )
                    {
                        leftBtnPressUp = true;
                        if (System.DateTime.Now.Ticks - btnDownTime < 2800000)
                        {
                            if ((2 == btnDownCount) && IsPressWithOutMoving(currPos, btnDownPos))
                            {
                                leftBtnDoubleClick = true;
                                leftBtnPressUp = false;
                            }
                        }
                        else if (System.DateTime.Now.Ticks - btnDownTime < 10000000)
                        {
                            if ((1 == btnDownCount) && IsPressWithOutMoving(currPos, btnDownPos))
                            {
                                leftBtnClick = true;
                                leftBtnPressUp = false;
                            }
                        }

                        btnDownTime = 0;
                        btnDownCount = 0;
                    }


                    leftButtonDown = false;
                    leftBtnOnPress = false;
                }

                if(currEvent.button == 1)
                {
                    if( rightButtonDown )
                    {
                        rightBtnPressUp = true;
                    }
                    rightButtonDown = false;
                }

                if(currEvent.button == 2)
                {
                    if( midButtonDown )
                    {
                        midBtnPressUp = true;
                    }
                    midButtonDown = false;
                } 
                break;

            case EventType.ScrollWheel:
                scroll = true;
                delta = currEvent.delta;
                break;
                 
            case EventType.MouseDrag:
                drag = true; 
                break; 

            case EventType.DragUpdated:
                dragingObjs = true;
                //Add by liteng for MoveAtlas At 2014/1/4
                customDragUpdated = true;
                dragObjs = DragAndDrop.objectReferences;
                dragObjsPaths = DragAndDrop.paths; 
                break;

            case EventType.DragPerform:
                dragingObjs = true;
                dragObjsPerform = true;
                dragObjs = DragAndDrop.objectReferences;
                dragObjsPaths = DragAndDrop.paths; 
                break;

            case EventType.DragExited:
                dragingObjs = true;
                dragObjs = DragAndDrop.objectReferences;
                dragObjsPaths = DragAndDrop.paths; 
                break;

            default:
                break;
        }

        CheckLBtnOnPress();

        CheckMouseInWindow(wnd.position);
        
        hasInput = checkInput();
    }

    void CheckLBtnOnPress()
    {
        if (
                leftButtonDown
             && (System.DateTime.Now.Ticks - btnDownTime > 10000000)
             && IsPressWithOutMoving(currPos, btnDownPos)
            )
        {
            if (leftBtnPress)
            {
                leftBtnOnPress = true;
                lastPressTime = System.DateTime.Now.Ticks;
            }
            else
            {
                if (System.DateTime.Now.Ticks - lastPressTime > pressTimeThreshold)
                {
                    leftBtnOnPress = true;
                    lastPressTime = System.DateTime.Now.Ticks;
                }
            }
        }
    }

    void CheckMouseInWindow(Rect windPos)
    {
        if (
               (currPos.x < 0)
            || (currPos.y < 0)
            || (currPos.x > windPos.width)
            || (currPos.y > windPos.height)
            )
        {
            isMouseInWindow = false;
            if (
                   (Event.current.type != EventType.MouseDrag)
                && (Event.current.type != EventType.Layout)
                && (Event.current.type != EventType.Repaint)
                )
            {
                if (leftButtonDown)
                {
                    leftButtonDown = false;
                    leftBtnPressUp = true;
                }

                if (midButtonDown)
                {
                    midButtonDown = false;
                    midBtnPressUp = true;
                }

                if(rightButtonDown)
                {
                    rightButtonDown = false;
                    rightBtnPressUp = true;
                }
            }
        }
        else
        {
            isMouseInWindow = true;
        }
    }

    bool checkInput()
    {
        bool bRet = false;

        do
        {
            if (leftBtnPress)
            {
                bRet = true;
                break;
            }

            if (midBtnPress)
            {
                bRet = true;
                break;
            }

            if (rightBtnPress)
            {
                bRet = true;
                break;
            }

            if (leftBtnClick)
            {
                bRet = true;
                break;
            }

            if (leftBtnDoubleClick)
            {
                bRet = true;
                break;
            }

            if (leftBtnOnPress)
            {
                bRet = true;
                break;
            }

            if (drag)
            {
                bRet = true;
                break;
            }

            if (scroll)
            {
                bRet = true;
                break;
            }

            if (dragingObjs)
            {
                bRet = true;
                break;
            }

            if (dragObjsPerform)
            {
                bRet = true;
                break;
            }

            if (customDragUpdated)
            {
                bRet = true;
                break;
            }
        } while (false);

        return bRet;
    }

    bool IsPressWithOutMoving(Vector2 currPos, Vector2 btnDownPos)
    {
        bool bRet = false;

        if(
               (Mathf.Abs((currPos - btnDownPos).x) <= btnPressFiled.x)
            && (Mathf.Abs((currPos - btnDownPos).y) <= btnPressFiled.y)
            )
        {
            bRet = true;
        }

        return bRet;
    }
    bool CheckLDoubleClick()
    {
        bool isDoubleClick = false;

        btnDownCount++;

        if(1 == btnDownCount)
        {
            btnDownTime = System.DateTime.Now.Ticks;
            isDoubleClick = false;
        }
        else if (2 == btnDownCount)
        {
            if ((System.DateTime.Now.Ticks - btnDownTime) < 2000000)
            {
                isDoubleClick = true;
            }
            else
            {
                isDoubleClick = false;
            }
            btnDownTime = 0;
            btnDownCount = 0;
        }
        else
        {
            btnDownTime = 0;
            btnDownCount = 0;
            isDoubleClick = false;
        }

        return isDoubleClick;
    }

    void _PrintDragItems()
    {
        foreach( var obj in 
            DragAndDrop.objectReferences )
        {
            Debug.Log("obj = " + obj.name);
        }

        foreach (var path in
            DragAndDrop.paths)
        {
            Debug.Log("path = " + path);
        }
    }

    void _FrameReset()
    {
        leftBtnPress = false;
        midBtnPress = false;
        rightBtnPress = false;

        leftBtnClick = false;
        leftBtnDoubleClick = false;
        leftBtnOnPress = false;

        leftBtnPressUp = false;
        midBtnPressUp = false;
        rightBtnPressUp = false;

        drag = false;
        scroll = false;

        dragingObjs = false;
        dragObjsPerform = false;
        dragObjs = null;
        dragObjsPaths = null;
        //Add by liteng for MoveAtlas At 2014/1/4
        customDragUpdated = false;

        isMouseInWindow = false;
    }
     

    public static FrameInputInfo GetInstance()
    {
        return currInputInfo;
    }

    public static void SetCurrInputInfo( FrameInputInfo info )
    {
        currInputInfo = info;
    }

    private static FrameInputInfo currInputInfo = null;

    //Add by liteng for MoveAtlas At 2014/1/4 Start
    private int btnDownCount = 0;
    private long btnDownTime = 0;
    //Add by liteng for MoveAtlas End

    private Vector2 btnPressFiled = new Vector2(10, 10);
    private Vector2 btnDownPos = new Vector2();

    private long lastPressTime = 0;
    private long pressTimeThreshold = 2000000;
}

public class EditorRenderer  
{
    private  LayoutUpdateVisitor layoutCalcVisitor = new LayoutUpdateVisitor();
    private  EditorCtrlVisitor renderVisitor = new EditorRenderVisitor();
    private  TriggerVisitor triggerVisitor = new TriggerVisitor();
    private  EditorCtrlVisitor resetTriggerInfoVisitor = new ResetTriggerInfoVisitor();
    private  UpdateVisitor updateVisitor = new UpdateVisitor();
    private  DestroyVisitor destroyVisitor = new DestroyVisitor();
    public void RequestRepaint()
    {
        repaint = true;
    }

    //是否重绘编辑器，在本帧被请求过
    public bool IsRepaintRequested()
    {
        return repaint;
    }
    //上一帧Render是否仍在执行中(对应OnGui重入)
    private bool isLastFrameRendering = false;
    //本帧控件树渲染是否结束(对应渲染中断，如ObjectField弹出窗口)
    private bool isLastRenderVisitorInterrupt = false;
    //在OnGUI中调用
    public void Render( EditorControl root , Rect wndRect )
    {
        //判断上一帧渲染是否被中断
        if (isLastRenderVisitorInterrupt)
        {//如果是

            //手动结束上一帧Render，正常执行本帧的Render
            isLastFrameRendering = false;
        }

        //判断上一帧的Render是否在执行中，如果是则不处理
        if (isLastFrameRendering)
        {//如果是

            //将窗口置灰，屏蔽所有用户操作
            //root.SetEnable(false);
            return;

        }
        else
        {
            //root.SetEnable(root.Root.Enable);
        }

        isLastFrameRendering = true;

        if (repaint)
        { 
            repaint = false;
        }

        layoutCalcVisitor.areaStack.Clear();
        layoutCalcVisitor.areaStack.Push(wndRect);

        //计算布局
        root.Traverse(layoutCalcVisitor);

        isLastRenderVisitorInterrupt = true;

        //渲染控件树
        root.Traverse(renderVisitor);

        isLastRenderVisitorInterrupt = false;

        //if(root.Enable)
        //{
            triggerVisitor.Clear();
            //收集本帧需要被触发的控件
            root.Traverse(triggerVisitor);
            //集中触发需要触发的控件
            foreach (var c in triggerVisitor.triggerControls)
            {
                this.Trigger(c);
            }

            //清除控件节点中本帧记录的操作
            root.Traverse(resetTriggerInfoVisitor);

            isLastFrameRendering = false;

       // }

    }


    //每帧都需调用，用于执行控件每帧都需的更新任务
    public void Update( EditorControl root )
    {
        deltaTime = Time.realtimeSinceStartup - lastFrameTime;
        lastFrameTime = Time.realtimeSinceStartup;

        updateVisitor._InternalUpdate(deltaTime);
        root.Traverse(updateVisitor);  
    }

    public void Destroy( EditorControl root )
    {
        root.Traverse(destroyVisitor);
    }

    private void Trigger( EditorControl c )
    {
        if(c.frameTriggerInfo.isCtrlBehaveChange)
        {
            if(null != c.onCtrlBehaveChange)
            {
                c.onCtrlBehaveChange(c);
            }
        }

        if (c.frameTriggerInfo.isClick)
        {
            if (null != c.onClick)
            {
                c.onClick(c);
            }
        }

        if (c.frameTriggerInfo.isHover)
        {
            if (null != c.onHover)
            {
                c.onHover(c);
            }
        }

        if (c.frameTriggerInfo.isValueChanged)
        {
            if (null != c.onValueChange)
            {
                c.onValueChange(c, c.CurrValue);
            }
        }

        if (c.frameTriggerInfo.lastSelectItem != -1)
        {
            if (null != c.onItemSelected)
            {
                c.onItemSelected(c, c.frameTriggerInfo.lastSelectItem);
            }
        }

        //add by liteng for atlas start
        if (c.frameTriggerInfo.lastSelectItemR != -1)
        {
            if (null != c.onItemSelectedR)
            {
                c.onItemSelectedR(c, c.frameTriggerInfo.lastSelectItemR);
            }
        }

        if (c.frameTriggerInfo.lastSelectItemRU != -1)
        {
            if (null != c.onItemSelectedRU)
            {
                c.onItemSelectedRU(c, c.frameTriggerInfo.lastSelectItemRU);
            }
        }
        //add by liteng end
        if (c.frameTriggerInfo.isScroll)
        {
            if (null != c.onScroll)
            {
                c.onScroll(c, c.frameTriggerInfo.scrollPos);
            }
        }


        if (c.frameTriggerInfo.isDraggingObjs)
        {
            if (null != c.onDragingObjs)
            {
                c.onDragingObjs(c, FrameInputInfo.GetInstance().dragObjs, FrameInputInfo.GetInstance().dragObjsPaths);
            }
        }

        if (c.frameTriggerInfo.isDropObjs)
        {
            if (null != c.onDropObjs)
            {
                c.onDropObjs(c, FrameInputInfo.GetInstance().dragObjs, FrameInputInfo.GetInstance().dragObjsPaths);
            }
        }

        //Add by liteng for MoveAtlas At 2014/1/4 Start 
        if (c.frameTriggerInfo.isCustomDragAccept)
        {
            if (null != c.onAcceptCustomDrag)
            {
                c.onAcceptCustomDrag(c, c.DragObject);
            }
        }

        if (c.frameTriggerInfo.isCustomDragAcceptCtrl)
        {
            if (null != c.onAcceptCustomDragCtrl)
            {
                c.onAcceptCustomDragCtrl(c, c.DragObject);
            }
        }

        if (c.frameTriggerInfo.isCtrlSelectItem)
        {
            if (null != c.onItemCtrlSelected)
            {
                c.onItemCtrlSelected(c, c.frameTriggerInfo.lastCtrlSelectItem);
            }
        }

        if(c.frameTriggerInfo.isDoubleClick)
        {
            if(null != c.onDoubleClick)
            {
                c.onDoubleClick(c, c.ClickObject);
            }
        }

        if(c.frameTriggerInfo.isOnPress)
        {
            if(null != c.onOnPress)
            {
                c.onOnPress(c, c.ClickObject);
            }
        }

        if(c.frameTriggerInfo.isPressDown)
        {
            if(null != c.onPressDown)
            {
                c.onPressDown(c, c.ClickObject);
            }
        }

        if (c.frameTriggerInfo.isPressUp)
        {
            if (null != c.onPressUp)
            {
                c.onPressUp(c, c.ClickObject);
            }
        }

        if(c.frameTriggerInfo.isRPressDown)
        {
            if(null != c.onRPressDown)
            {
                c.onRPressDown(c, c.ClickObject);
            }
        }

        if(c.frameTriggerInfo.isRPressUP)
        {
            if(null != c.onRPressUp)
            {
                c.onRPressUp(c, c.ClickObject);
            }
        }

        if(c.frameTriggerInfo.isPlay)
        {
            if(null != c.onPlay)
            {
                c.onPlay(c);
            }
        }

        if (c.frameTriggerInfo.isPause)
        {
            if (null != c.onPause)
            {
                c.onPause(c);
            }
        }

        if (c.frameTriggerInfo.isStop)
        {
            if (null != c.onStop)
            {
                c.onStop(c);
            }
        }
    }


    private bool repaint = false;
    private float lastFrameTime = 0.0f;
    private float deltaTime = 0.0f;
 
    	 
}

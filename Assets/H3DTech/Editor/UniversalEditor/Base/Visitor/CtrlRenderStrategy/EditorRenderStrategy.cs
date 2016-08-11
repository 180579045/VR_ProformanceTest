using UnityEngine;
using System.Collections;
using UnityEditor;

public enum SCROLLBAR_DISP_STATUS
{
    SHOW_NONE_SCROLLBAR = 0,
    SHOW_HORIZONTAL_SCROLLBAR,
    SHOW_VERTICAL_SCROLLBAR,
    SHOW_BOTH_SCROLLBAR
}
public class EditorRenderStrategy  
{
    public virtual bool PreVisit(EditorControl c) 
    { 
        if(
               (null == c)
            || (false == c.Visiable)
            )
        {
            return false;
        }

        return true;
    }

    public virtual void Visit(EditorControl c) { }

    public virtual void AfterVisit(EditorControl c) { }

    public virtual bool PreVisitChildren(EditorControl c) { return true; }
    public virtual void AfterVisitChildren(EditorControl c) { }
    public virtual bool PreVisitChild(EditorControl c, int ichild) { return true; }
    public virtual void AfterVisitChild(EditorControl c, int ichild) { }

    public Vector2 CalcLocalPos(EditorControl c, Vector2 p)
    {
        Vector2 mousePos = new Vector2();
        EditorControl parent = null;
        parent = c.Parent;

        mousePos = p;

        while (parent != null)
        {
            if (parent.IsShowScrollBar)
            {
                mousePos = new Vector2(parent.LastRect.x - parent.ScrollPos.x, parent.LastRect.y - parent.ScrollPos.y);
                mousePos = p - mousePos;
                p = mousePos;
            }

            parent = parent.Parent;
        }

        return mousePos;
    }

    virtual public void CheckInputEvent(EditorControl c)
    {
        if (
               (null == c)
            )
        {
            return;
        }

        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);

        if (
               FrameInputInfo.GetInstance().hasInput
            && c.LastRect.Contains(localMousePos)
            )
        {
            c.frameTriggerInfo.isHandleInput = true;
        }
    }

    public SCROLLBAR_DISP_STATUS CheckScrollBarDispStatus(Rect ctrlLastRect, Rect contentLastRect)
    {
        SCROLLBAR_DISP_STATUS dispStatus = SCROLLBAR_DISP_STATUS.SHOW_NONE_SCROLLBAR;

        if (
                 (ctrlLastRect.width < contentLastRect.width)
              && (ctrlLastRect.height < contentLastRect.height )  
            )
        {
            dispStatus = SCROLLBAR_DISP_STATUS.SHOW_BOTH_SCROLLBAR;
        }
        else if (ctrlLastRect.width < contentLastRect.width)
        {
            dispStatus = SCROLLBAR_DISP_STATUS.SHOW_HORIZONTAL_SCROLLBAR;
        }
        else if (ctrlLastRect.height < contentLastRect.height)
        {
            dispStatus = SCROLLBAR_DISP_STATUS.SHOW_VERTICAL_SCROLLBAR;
        }

        return dispStatus;
    }

    public Rect GetCtrlLastRectWithOutScrollBar(EditorControl c)
    {
        Rect lastRectWithoutScrollBar = new Rect();
        if (
               (null == c)
            )
        {
            return lastRectWithoutScrollBar;
        }

        SCROLLBAR_DISP_STATUS scrollStatus = CheckScrollBarDispStatus(c.LastRect, c.LastContentRect);

        lastRectWithoutScrollBar = c.LastRect;

        switch(scrollStatus)
        {
            case SCROLLBAR_DISP_STATUS.SHOW_HORIZONTAL_SCROLLBAR:
                if (lastRectWithoutScrollBar.height > scrollBarWidth)
                {
                    lastRectWithoutScrollBar.height -= scrollBarWidth;
                }
                break;

            case SCROLLBAR_DISP_STATUS.SHOW_VERTICAL_SCROLLBAR:
                if (lastRectWithoutScrollBar.width > scrollBarWidth)
                {
                    lastRectWithoutScrollBar.width -= scrollBarWidth;
                }
                break;

            case SCROLLBAR_DISP_STATUS.SHOW_BOTH_SCROLLBAR:
                if (lastRectWithoutScrollBar.height > scrollBarWidth)
                {
                    lastRectWithoutScrollBar.height -= scrollBarWidth;
                }
                if (lastRectWithoutScrollBar.width > scrollBarWidth)
                {
                    lastRectWithoutScrollBar.width -= scrollBarWidth;
                }
                break;

            default:
                break;
        }

        return lastRectWithoutScrollBar;
    }

    protected float scrollBarWidth = 20f;

    protected static EditorControl m_DragBegionCtrl = null;
    protected static bool m_IsNeedRemove = false;
    protected static bool m_IsCusDragPrepare = false;
    protected static bool m_IsCusDragStart = false;
    protected static EditorControl m_TargetCtrl = null;
}

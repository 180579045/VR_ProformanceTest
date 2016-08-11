using UnityEngine;
using System.Collections;
using UnityEditor;

public class LabelCtrlRenderStrategy : EditorRenderStrategy 
{
    public LabelCtrlRenderStrategy()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 16;
        labelStyle.margin = new RectOffset(0, 0, 5, 5);
    }

    public override void Visit(EditorControl c) 
    {
        LabelCtrl label = c as LabelCtrl;
        if (
               (null == label)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!label.Enable);

        Color oldColor =labelStyle.normal.textColor;
        labelStyle.normal.textColor = label.textColor;
        //Add by liteng for MoveAtlas At 2014/1/4
        labelStyle.fontSize = label.fontSize;
        labelStyle.margin = label.margin;

        GUILayout.Label(c.Caption, labelStyle, c.GetOptions());

        labelStyle.normal.textColor = oldColor;

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        HandleMouseInput(c);
    }

    private void HandleMouseInput(EditorControl c)
    {
        LabelCtrl currCtrl = c as LabelCtrl;
        if (
               (null == currCtrl)
            || !currCtrl.IsCurrentCtrlEnable()
            || currCtrl.IsEventTriggered()
            )
        {
            return;
        }

        CheckInputEvent(c);

        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);
        localMousePos += currCtrl.ScrollPos;

        if (
                   FrameInputInfo.GetInstance().leftBtnDoubleClick
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isDoubleClick = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnClick
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isClick = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnOnPress
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isOnPress = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPress
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isPressDown = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPressUp
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isPressUp = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
    }

    private bool _IsExtensible(EditorControl c)
    {
        if (c.layoutConstraint.expandWidth == true &&
            c.layoutConstraint.expandHeight == true)
        {
            return true;
        }
        return false;
    }

    GUIStyle labelStyle;
    	 
}

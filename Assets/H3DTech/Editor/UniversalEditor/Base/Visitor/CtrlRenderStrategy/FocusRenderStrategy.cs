using UnityEngine;
using System.Collections;
using UnityEditor;

public class FocusRenderStrategy : EditorRenderStrategy
{
    public override bool PreVisit(EditorControl c)
    {
        if (
               (null == c)
            || (false == c.Visiable)
            )
        {
            return false;
        }
        string focusName = GUI.GetNameOfFocusedControl();

        if (c.IsForceUpdate)
        {
            c.IsForceUpdateText = true;
            if (
                   (string.IsNullOrEmpty(focusName))
                )
            {
                GUI.FocusControl("");
            }
        }

        return true;
    }

    public override void AfterVisit(EditorControl c)
    {
        string focusName = GUI.GetNameOfFocusedControl();
        if(null == c)
        {
            return;
        }
        
        if (!c.Enable)
        {
            c.IsFocusLastFrame = false;
        }

        if (
               c.IsFocusLastFrame
            && (Event.current.type == EventType.Repaint)
            )
        {
            GUI.FocusControl(c.CtrlID);
            c.IsFocusLastFrame = false;
        }

        if (
                !c.Enable
             || (c.IsForceUpdateText
                && (Event.current.type == EventType.Repaint))
             )
        {
            if (focusName.Equals(c.CtrlID))
            {
                c.IsFocusLastFrame = true;
                GUI.FocusControl("");
            }

            c.IsForceUpdateText = false;
        }
    }


}
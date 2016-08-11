using UnityEngine;
using System.Collections;
using UnityEditor;

public class HSpliterRenderStrategy : EditorRenderStrategy
{
    public override bool PreVisit(EditorControl c) 
    {
        GUILayout.BeginVertical();
        return true;
    }
    public override void Visit(EditorControl c) 
    {
        HSpliterCtrl spliter = c as HSpliterCtrl; 
        GUILayout.Box("", c.GetStyle(), c.GetOptions()); 

        c.UpdateLastRect();

        if (spliter.Dragable && spliter.IsCurrentCtrlEnable())
        {
            if (
               FrameInputInfo.GetInstance().leftBtnPress &&
               c.LastRect.Contains(FrameInputInfo.GetInstance().currPos)
             )
            {
                spliter.IsDragging = true;
            }
            else
            {

                if (FrameInputInfo.GetInstance().leftButtonDown == false)
                {
                    spliter.IsDragging = false;
                }

                if (spliter.IsDragging)
                {
                    if (null != c.layoutConstraint)
                    {
                        Vector2 mouseDelta = FrameInputInfo.GetInstance().posOffset;


                        if (Mathf.Abs(mouseDelta.y) > Mathf.Epsilon)
                        {
                            if (c.layoutConstraint.spliterOffsetInv)
                            {
                                c.layoutConstraint.spliterOffset -= mouseDelta.y;
                            }
                            else
                            {
                                c.layoutConstraint.spliterOffset += mouseDelta.y;
                            }

                            if (c.layoutConstraint.spliterOffset < spliter.MinOffset)
                            {
                                c.layoutConstraint.spliterOffset = spliter.MinOffset;
                            }
                            if (c.layoutConstraint.spliterOffset > spliter.MaxOffset)
                            {
                                c.layoutConstraint.spliterOffset = spliter.MaxOffset;
                            }

                            c.RequestRepaint();
                        }
                    } 
                }
            }
            EditorGUIUtility.AddCursorRect(c.LastRect, MouseCursor.ResizeVertical);
        }
         
    }
    public override void AfterVisit(EditorControl c) 
    {
        GUILayout.EndVertical();
    }

    public override bool PreVisitChild(EditorControl c, int ichild)
    {
        HSpliterCtrl hSpliter = c as HSpliterCtrl;
        if (null == hSpliter)
        {
            return true;
        }

        EditorGUI.BeginDisabledGroup(!hSpliter.Enable);

        return true;
    }

    public override void AfterVisitChild(EditorControl c, int ichild)
    {
        HSpliterCtrl hSpliter = c as HSpliterCtrl;
        if (null == hSpliter)
        {
            return;
        }

        EditorGUI.EndDisabledGroup();
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

public class VSpliterRenderStrategy : EditorRenderStrategy
{ 
    public override bool PreVisit(EditorControl c)
    {
        GUILayout.BeginHorizontal();
        return true;
    }
    public override void Visit(EditorControl c)
    {
        VSpliterCtrl spliter = c as VSpliterCtrl; 
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


                        if (Mathf.Abs(mouseDelta.x) > Mathf.Epsilon)
                        {
                            if (c.layoutConstraint.spliterOffsetInv)
                            {
                                c.layoutConstraint.spliterOffset -= mouseDelta.x;
                            }
                            else
                            {
                                c.layoutConstraint.spliterOffset += mouseDelta.x;
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
            EditorGUIUtility.AddCursorRect(c.LastRect, MouseCursor.ResizeHorizontal);
        }
    }
    public override void AfterVisit(EditorControl c)
    {
        GUILayout.EndHorizontal();
    }

    public override bool PreVisitChild(EditorControl c, int ichild)
    {
        VSpliterCtrl vSpliter = c as VSpliterCtrl;
        if (null == vSpliter)
        {
            return true;
        }

        EditorGUI.BeginDisabledGroup(!vSpliter.Enable);

        return true;
    }

    public override void AfterVisitChild(EditorControl c, int ichild)
    {
        VSpliterCtrl vSpliter = c as VSpliterCtrl;
        if (null == vSpliter)
        {
            return;
        }

        EditorGUI.EndDisabledGroup();
    }
}

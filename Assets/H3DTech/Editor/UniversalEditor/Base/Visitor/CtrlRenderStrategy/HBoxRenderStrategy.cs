using UnityEngine;
using System.Collections;
using UnityEditor;

public class HBoxRenderStrategy : EditorRenderStrategy
{
    public override bool PreVisit(EditorControl c)
    {
        HBoxCtrl hBox = c as HBoxCtrl;
        if (
               (null == hBox)
            || (!hBox.Visiable)
            )
        {
            return false;
        }

        return true;
    }

    public override void AfterVisit(EditorControl c)
    {
        
    }
    public override bool PreVisitChildren(EditorControl c) 
    {
        //GUILayoutOption[] v = new GUILayoutOption[]
        //{
        //    GUILayout.Height(c.Size.height),
        //    GUILayout.ExpandWidth(true)
        //};

        //GUILayoutOption[] h = new GUILayoutOption[]
        //{
        //    GUILayout.Width(c.Size.width),
        //    GUILayout.ExpandHeight(true)
        //};
        HBoxCtrl hBox = c as HBoxCtrl;
        if(null == hBox)
        {
            return true;
        }

        //GUILayoutOption[] layoutOption = null;

        EditorGUI.BeginDisabledGroup(!c.Enable);

        if (_IsExtensible(c))
        {
            GUILayout.BeginHorizontal(
                new GUILayoutOption[]{ 
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true)
                });
            //layoutOption = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };

        }
        else if( c.Size.width == 0 && c.Size.height == 0)
        {
            //Debug.Log("H");
            GUILayout.BeginHorizontal();
            //layoutOption = null;

        } else {
            //GUILayout.BeginVertical(v);
            //GUILayout.BeginHorizontal(h); 
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(c.Size.width), GUILayout.Height(c.Size.height) });
            //layoutOption = new GUILayoutOption[] { GUILayout.Width(c.Size.width), GUILayout.Height(c.Size.height) };

        }

        if (hBox.IsShowScrollBar)
        {
            Vector2 newScrollPos = EditorGUILayout.BeginScrollView(hBox.ScrollPos, hBox.GetOptions());

            if (!newScrollPos.Equals(hBox.ScrollPos))
            {
                c.frameTriggerInfo.isScroll = true;
                c.frameTriggerInfo.scrollPos = newScrollPos;
            }
            hBox.ScrollPos = newScrollPos;
            GUILayout.BeginHorizontal();

        }

        return true;
    }
    public override void AfterVisitChildren(EditorControl c) 
    {
        HBoxCtrl hBox = c as HBoxCtrl;
        if(null == hBox)
        {
            EditorGUI.EndDisabledGroup();
            return;
        }

        if (hBox.IsShowScrollBar)
        {
            GUILayout.EndHorizontal();
            hBox.UpdateContentRect();
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.EndHorizontal();
            hBox.UpdateContentRect();
        }

        EditorGUI.EndDisabledGroup();
        hBox.UpdateLastRect();

        HandleMouseInput(c);
        //GUILayout.EndVertical(); 
    }

    private bool _IsExtensible( EditorControl c )
    {
        if (c.layoutConstraint.expandWidth == true &&
            c.layoutConstraint.expandHeight == true)
        {
            return true;
        } 
        return false;
    }

    private void HandleMouseInput(EditorControl c)
    {
        HBoxCtrl hBox = c as HBoxCtrl;
        if (
               (null == hBox)
            || !hBox.IsCurrentCtrlEnable()
            || hBox.IsEventTriggered()
            )
        {
            return;
        }

        CheckInputEvent(c);

        Rect contentRect = GetCtrlLastRectWithOutScrollBar(hBox);
        
        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);

        if  (
                   FrameInputInfo.GetInstance().leftBtnDoubleClick
                && contentRect.Contains(localMousePos)
            )
        {
            hBox.frameTriggerInfo.isDoubleClick = true;
            hBox.ClickObject = null;

            hBox.RequestRepaint();
        }
        else if(
                   FrameInputInfo.GetInstance().leftBtnClick
                && contentRect.Contains(localMousePos)             
            )
        {
            hBox.frameTriggerInfo.isClick = true;
            hBox.ClickObject = null;

            hBox.RequestRepaint();
        }
        else if(
                   FrameInputInfo.GetInstance().leftBtnOnPress
                && contentRect.Contains(localMousePos)             
            )
        {
            hBox.frameTriggerInfo.isOnPress = true;
            hBox.ClickObject = null;

            hBox.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPress
                && contentRect.Contains(localMousePos)
            )
        {
            hBox.frameTriggerInfo.isPressDown = true;
            hBox.ClickObject = null;

            hBox.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPressUp
                && contentRect.Contains(localMousePos)
            )
        {
            hBox.frameTriggerInfo.isPressUp = true;
            hBox.ClickObject = null;

            hBox.RequestRepaint();
        }
    }


}

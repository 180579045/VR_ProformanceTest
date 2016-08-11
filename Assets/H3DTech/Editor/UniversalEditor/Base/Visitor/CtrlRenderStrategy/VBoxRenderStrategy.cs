using UnityEngine;
using System.Collections;
using UnityEditor;

public class VBoxRenderStrategy : EditorRenderStrategy
{
    public override bool PreVisit(EditorControl c)
    {
        VBoxCtrl vBox = c as VBoxCtrl;
        if (
               (null == vBox)
            || (!vBox.Visiable)
            )
        {
            return false;
        }

        return base.PreVisit(c);
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
        VBoxCtrl vBox = c as VBoxCtrl;
        if(null == vBox)
        {
            return true;
        }
        EditorGUI.BeginDisabledGroup(!c.Enable);

        //GUILayoutOption[] layoutOption = null;

        GUILayoutOption[] newlayoutOption = null;

        if (vBox.IsShowScrollBar)
        {
            Rect parentRect = vBox.Size;
            if (
                   (0 == vBox.Size.width)
                || (0 == vBox.Size.height)
                )
            {
                parentRect = GetLayoutParentRect(vBox.Parent, vBox);
                //parentRect = GetLayoutParentRect(vBox);
            }

            if (parentRect.height > 30f)
            {
                parentRect = new Rect(0, 0, parentRect.width, parentRect.height - 30f);
            }

            newlayoutOption = new GUILayoutOption[] { GUILayout.Width(parentRect.width), GUILayout.Height(parentRect.height) };

            Vector2 newScrollPos = EditorGUILayout.BeginScrollView(vBox.ScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none, newlayoutOption);
            if (!newScrollPos.Equals(vBox.ScrollPos))
            {
                c.frameTriggerInfo.isScroll = true;
                c.frameTriggerInfo.scrollPos = newScrollPos;
            }

            vBox.ScrollPos = newScrollPos;
        }

        if (_IsExtensible(c))
        {
            GUILayout.BeginVertical(
                new GUILayoutOption[]{ 
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true)
                });
            //layoutOption = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };

        }else if( c.Size.width == 0 && c.Size.height == 0){
            //Debug.Log("V");
            GUILayout.BeginVertical();
            //layoutOption = null;

        }else{
            //GUILayout.BeginHorizontal(h);
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(c.Size.width), GUILayout.Height(c.Size.height) });
            //layoutOption = new GUILayoutOption[] { GUILayout.Width(c.Size.width), GUILayout.Height(c.Size.height) };
        }

        return true;
    }

    public Rect GetLayoutParentRect( EditorControl parent, EditorControl currCtrl)
    {

        EditorControl p = currCtrl.Parent;
        while (p != null && !(p is SpliterCtrl))
        {
            currCtrl = p;
            p = p.Parent;
        }

        if (currCtrl != null)
        {
            //currCtrl.Size = new Rect(0, 0, currCtrl.LastRect.width, currCtrl.LastRect.height);
            return currCtrl.Size;
        }

        return new Rect();
        //Rect parentRect = new Rect();
        //currCtrl.Size = new Rect();

        //if (null == parent)
        //{
        //    return parentRect;
        //}

        //parentRect = parent.Size;
        //if (!parent.IsShowScrollBar)
        //{
        //    currCtrl.Size = new Rect(0, 0, parent.Size.width, parent.Size.height - (currCtrl.LastRect.y - parentRect.y));
        //}
        //else
        //{
        //    currCtrl.Size = new Rect(0, 0, parent.Size.width, parent.Size.height - (currCtrl.LastRect.y));
        //}

        //if (!(parent is VBoxCtrl) && !(parent is HBoxCtrl))
        //{
        //    parentRect = GetLayoutParentRect(parent.Parent, currCtrl);
        //}
        //return parentRect;
    }

    public override void AfterVisitChildren(EditorControl c)
    {
        VBoxCtrl vBox = c as VBoxCtrl;
        if(null == vBox)
        {
            return;
        }

        GUILayout.EndVertical();

        if (vBox.IsShowScrollBar)
        {
            vBox.UpdateContentRect();

            EditorGUILayout.EndScrollView();
        }
        else
        {
            vBox.UpdateContentRect();
        }

        EditorGUI.EndDisabledGroup();

        vBox.UpdateLastRect();

        HandleMouseInput(vBox);


      //GUILayout.EndHorizontal(); 
    }

    private void HandleMouseInput(EditorControl c)
    {
        VBoxCtrl vBox = c as VBoxCtrl;
        if (
               (null == vBox)
            || !vBox.IsCurrentCtrlEnable()
            || vBox.IsEventTriggered()
            )
        {
            return;
        }

        CheckInputEvent(c);

        Rect contentRect = GetCtrlLastRectWithOutScrollBar(vBox);

        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);

        if (
                   FrameInputInfo.GetInstance().leftBtnDoubleClick
                && contentRect.Contains(localMousePos)
            )
        {
            vBox.frameTriggerInfo.isDoubleClick = true;
            vBox.ClickObject = null;

            vBox.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnClick
                && contentRect.Contains(localMousePos)
            )
        {
            vBox.frameTriggerInfo.isClick = true;
            vBox.ClickObject = null;

            vBox.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnOnPress
                && contentRect.Contains(localMousePos)
            )
        {
            vBox.frameTriggerInfo.isOnPress = true;
            vBox.ClickObject = null;

            vBox.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPress
                && contentRect.Contains(localMousePos)
            )
        {
            vBox.frameTriggerInfo.isPressDown = true;
            vBox.ClickObject = null;

            vBox.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPressUp
                && contentRect.Contains(localMousePos)
            )
        {
            vBox.frameTriggerInfo.isPressUp = true;
            vBox.ClickObject = null;

            vBox.RequestRepaint();
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

}

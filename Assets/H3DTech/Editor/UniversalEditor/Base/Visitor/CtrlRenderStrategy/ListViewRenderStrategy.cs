using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ListViewRenderStrategy : EditorRenderStrategy
{ 

    public ListViewRenderStrategy()
    {
        itemStyle = new GUIStyle();
        itemStyle.padding.top = -1;
        itemStyle.padding.bottom = 1;
        itemStyle.normal.textColor = itemTextColor;
    }

    public override void Visit(EditorControl c)
    {
        ListViewCtrl list = c as ListViewCtrl;

        if (
               (null == list)
            || (!list.Visiable)
            )
        {
            return;
        }

		//Modify by liteng for MoveAtlas At 2014/1/4 Start
        if(!list.IsTextureView)
        {
            VisitByListView(c);
        }
        else
        {
            VisitByTextureView(c);
        }
		//Modify by liteng for MoveAtlas End
    }

	//Add by liteng for MoveAtlas At 2014/1/4 Start
    private void VisitByListView(EditorControl c)
    {
        ListViewCtrl list = c as ListViewCtrl;

        if (list == null)
            return;


        EditorGUI.BeginDisabledGroup(!list.Enable);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(c.GetStyle(), c.GetOptions());
        Vector2 newScrollPos = EditorGUILayout.BeginScrollView(list.ScrollPos, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);

        if (!newScrollPos.Equals(list.ScrollPos))
        {
            c.frameTriggerInfo.isScroll = true;
            c.frameTriggerInfo.scrollPos = newScrollPos;
        }
        list.ScrollPos = newScrollPos;

        int count = 0;
        foreach (var item in list.Items)
        {
            foreach (var index in list.SelectItems)
            {
                if (index == count)
                {
                    GUI.color = item.onSelectColor;
                    GUI.Box(list.Items[index].lastRect, GUIContent.none);
                    GUI.color = Color.white;
                    break;
                }

            }
            //Modify by liteng for MoveAtlas End
            GUIContent itemContent = new GUIContent();
            itemContent.text = item.name;
            if (item.image != null)
            {
                itemContent.image = item.image;
            }
            //add by liteng for atlas begin
            if (!string.IsNullOrEmpty(item.tooltip))
            {
                itemContent.tooltip = item.tooltip;
            }

            //add by liteng end
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(itemContent, itemStyle, new GUILayoutOption[] { GUILayout.MaxWidth(list.LastRect.width - scrollBarWidth) });

            SpecialEffectEditorUtility.GetLastRect(ref item.lastRect);

            EditorGUILayout.EndHorizontal();

            //Modify by liteng for MoveAtlas At 2015/1/4
            HandleMouseAction(list, count);

            count++;
        }
        HandleDragAction(list);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        //为了顶住右侧边框，使ScrollView显示完全
        //GUILayout.Space(10f);

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private void VisitByTextureView(EditorControl c)
    {
        ListViewCtrl list = c as ListViewCtrl;
        Rect tempRect = new Rect();
        int listIndex = 0;
        if (list == null)
            return;

        if(0 == list.TextureSizeLevel)
        {
            list.TextureSizeLevel = 1;
        }

        EditorGUILayout.BeginVertical(c.GetStyle(), c.GetOptions());

        Vector2 newScrollPos = EditorGUILayout.BeginScrollView(list.ScrollPos, false, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUIStyle.none);
        if (!newScrollPos.Equals(list.ScrollPos))
        {
            c.frameTriggerInfo.isScroll = true;
            c.frameTriggerInfo.scrollPos = newScrollPos;
        }
        list.ScrollPos = newScrollPos;
  
        int curColumn = 0;

        int columns = Mathf.FloorToInt((float)c.LastRect.width / (float)(list.TextureSizeLevel * m_Padding + 20f));
        if (columns < 1)
        {
            columns = 1;
        }

        while (listIndex < list.Items.Count)
        {
            EditorGUILayout.BeginHorizontal();

            for (; listIndex < list.Items.Count; listIndex++)
            {
                if (curColumn >= columns)
                {
                    curColumn = 0;
                    break;
                }

                foreach (var index in list.SelectItems)
                {
                    if (index == listIndex)
                    {
                        GUI.color = list.Items[index].onSelectTexColor;
                        GUI.Box(list.Items[index].lastRect, GUIContent.none);
                        break;
                    }

                }
                GUIContent itemContent = new GUIContent();
                if (!string.IsNullOrEmpty(list.Items[listIndex].tooltip))
                {
                    itemContent.tooltip = list.Items[listIndex].tooltip;
                }
                if (list.Items[listIndex].image != null)
                {
                    itemContent.image = list.Items[listIndex].image;
                }
           
                EditorGUILayout.BeginVertical();
                GUILayout.Box(itemContent, new GUILayoutOption[] { GUILayout.Width(list.TextureSizeLevel * m_Padding), GUILayout.Height(list.TextureSizeLevel * m_Padding) });
                SpecialEffectEditorUtility.GetLastRect(ref list.Items[listIndex].lastRect);

                EditorGUILayout.LabelField(list.Items[listIndex].name, itemStyle, GUILayout.MaxWidth(list.TextureSizeLevel * m_Padding));
                SpecialEffectEditorUtility.GetLastRect(ref tempRect);
                list.Items[listIndex].lastRect.height += tempRect.height;

                GUI.color = Color.white;
                EditorGUILayout.EndVertical();

                HandleMouseAction(list, listIndex);             
                curColumn++;
            }
            EditorGUILayout.EndHorizontal();
        }

        HandleDragAction(list);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        c.UpdateLastRect();
        CheckInputEvent(c);

    }

    private void HandleMouseAction(ListViewCtrl list, int curIndex)
    {
        if(
               (null == list)
            || (!list.IsCurrentCtrlEnable())
            )
        {
            return;
        }

        if (!list.LastRect.Contains(CalcLocalPos(list, FrameInputInfo.GetInstance().currPos)))
        {
            return;
        }

        bool bIsFind = false;
        Vector2 localMousePos = list.CalcLocalPos(FrameInputInfo.GetInstance().currPos);
        localMousePos += list.ScrollPos;

        //Ctrl + LButtonPress
        if  (
                FrameInputInfo.GetInstance().leftBtnPress && Event.current.control &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
            )
        {
            foreach (var index in list.SelectItems)
            {
                if (curIndex == index)
                {
                    bIsFind = true;
                    break;
                }
            } 

            if (!bIsFind)
            {
                list.SelectItems.Add(curIndex);
                list.LastSelectItem = curIndex;
                list.frameTriggerInfo.isCtrlSelectItem = true;
                list.frameTriggerInfo.lastCtrlSelectItem = list.LastSelectItem;
            }
            else
            {
                m_TargetCtrl = list;
                m_IsNeedRemove = true;               
            }

            CustomDragPrepare(list);

            list.RequestRepaint();
        }
        //Ctrl + LButtonPressUp
        else if (
                FrameInputInfo.GetInstance().leftBtnPressUp && Event.current.control &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
            )
        {
            foreach (var index in list.SelectItems)
            {
                if (curIndex == index)
                {
                    bIsFind = true;
                    break;
                }
            }

            if (m_IsNeedRemove && (list.Name == m_TargetCtrl.Name))
            {
                list.SelectItems.Remove(curIndex);
                UpdateLastSelectItemAfterRemove(list, curIndex);
                list.frameTriggerInfo.isCtrlSelectItem = true;
                list.frameTriggerInfo.lastCtrlSelectItem = list.LastSelectItem;
                m_IsNeedRemove = false;
                m_TargetCtrl = null;
            }

            list.RequestRepaint();
        }
        //LButtonPress
        else if (
                FrameInputInfo.GetInstance().leftBtnPress &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
            )
        {
            foreach (var index in list.SelectItems)
            {
                if (curIndex == index)
                {
                    bIsFind = true;
                    break;
                }
            }

            if(!bIsFind)
            {
                list.SelectItems.Clear();
                list.SelectItems.Add(curIndex);
                list.LastSelectItem = curIndex;
                list.frameTriggerInfo.lastSelectItem = curIndex;
            }
            else
            {
                m_TargetCtrl = list;
                m_IsNeedRemove = true;
            }

            CustomDragPrepare(list);

            list.RequestRepaint();
        }
        //LButtonPressUp
        else if (
                FrameInputInfo.GetInstance().leftBtnPressUp &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
            )
        {
            if (m_IsNeedRemove && (list.Name == m_TargetCtrl.Name))
            {
                list.SelectItems.Clear();
                list.SelectItems.Add(curIndex);
                list.LastSelectItem = curIndex;
                list.frameTriggerInfo.lastSelectItem = curIndex; 
                
                m_IsNeedRemove = false;
                m_TargetCtrl = null;
            }

            list.RequestRepaint();
        }
        //LButtonDoubleClick
        else if (
                FrameInputInfo.GetInstance().leftBtnDoubleClick &&
                list.Items[curIndex].lastRect.Contains(localMousePos)         
            )
        {
            list.SelectItems.Clear();
            list.SelectItems.Add(curIndex);
            list.LastSelectItem = curIndex;
            list.frameTriggerInfo.lastSelectItem = curIndex;
            list.frameTriggerInfo.isDoubleClick = true;
            list.ClickObject = curIndex;

            list.RequestRepaint();
        }
        //LButtonOnPress
        else if (
                FrameInputInfo.GetInstance().leftBtnOnPress &&
                list.Items[curIndex].lastRect.Contains(localMousePos)         
            )
        {
            list.frameTriggerInfo.isOnPress = true;
            list.ClickObject = curIndex;

            list.RequestRepaint();
        }
        //RButtonDown
        else if (
                FrameInputInfo.GetInstance().rightBtnPress &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
           )
        {
            foreach (var index in list.SelectItems)
            {
                if (curIndex == index)
                {
                    bIsFind = true;

                    break;
                }
            }

            if (!bIsFind)
            {
                list.SelectItems.Clear();
                list.SelectItems.Add(curIndex);
            }
            list.LastSelectItem = curIndex;
            list.frameTriggerInfo.lastSelectItemR = curIndex;

            list.RequestRepaint();
        }
        //RButtonUP
        else if (
                FrameInputInfo.GetInstance().rightBtnPressUp &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
            )
        {
            list.frameTriggerInfo.lastSelectItemRU = curIndex;

            list.RequestRepaint();
        }
        //Drag
        else if (
                FrameInputInfo.GetInstance().drag &&
                list.Items[curIndex].lastRect.Contains(localMousePos)
            )
        {
            CustomDragStart(list);

            list.RequestRepaint();
        }
        else
        {
            //do nothing
        }

    }

    private void HandleDragAction(ListViewCtrl list)
    {
        if (
               (null == list)
            || !list.IsCurrentCtrlEnable()
            )
        {
            return;
        }

        if (
            (list.LastRect.Contains(FrameInputInfo.GetInstance().currPos)
             && FrameInputInfo.GetInstance().dragingObjs && !m_IsCusDragStart)
            )
        {
            bool accept = false;

            if (list.onAcceptDragObjs != null)
            {
                accept = list.onAcceptDragObjs(
                                        list,
                                        FrameInputInfo.GetInstance().dragObjs,
                                        FrameInputInfo.GetInstance().dragObjsPaths);

                if (accept)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (!FrameInputInfo.GetInstance().dragObjsPerform)
                    {
                        list.frameTriggerInfo.isDraggingObjs = true;
                    }
                    else
                    {
                        list.frameTriggerInfo.isDropObjs = true;
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }
        //DragUpdated
        else if (
                (list.LastRect.Contains(FrameInputInfo.GetInstance().currPos)) &&
                (FrameInputInfo.GetInstance().customDragUpdated)
            )
        {
            CustomDragUpdated(list);

            list.RequestRepaint();
        }
        //DragAccept ctrl
        else if (
                    FrameInputInfo.GetInstance().dragObjsPerform && Event.current.control &&
                    list.LastRect.Contains(FrameInputInfo.GetInstance().currPos)
            )
        {
            CustomDragAccept(list, true);

            list.RequestRepaint();
        }
        else if(
                    FrameInputInfo.GetInstance().dragObjsPerform &&
                    list.LastRect.Contains(FrameInputInfo.GetInstance().currPos)         
            )
        {
            CustomDragAccept(list, false);

            list.RequestRepaint();
        }


    }

    private void UpdateLastSelectItemAfterRemove(ListViewCtrl list, int removeIndex)
    {
        if(list.SelectItems.Count > 0)
        {
            list.LastSelectItem = list.SelectItems[0];
            list.frameTriggerInfo.lastSelectItem = list.SelectItems[0];
        }
        else
        {
            list.LastSelectItem = -1;
            list.frameTriggerInfo.lastSelectItem = -1;
        }
    }
    private void PrepareDrag(EditorControl list)
    {
        ListViewCtrl list2 = list as ListViewCtrl;

        if (null == list)
        {
            return;
        }

        DragAndDrop.PrepareStartDrag();
        DragAndDrop.objectReferences = new Object[] { null };   // 必须在这里初始化objectReferences，否则会遇到Unity drag & drop的bug
        DragAndDrop.SetGenericData("CustomDragFromList", list2.SelectItems);
    }
    private void CustomDragPrepare(EditorControl c)
    {
        object dragObject = null;
        ListViewCtrl list = c as ListViewCtrl;
    
        if (null == list)
        {
            return;
        }

        if (list.onPrepareCustomDrag != null)
        {
            dragObject = list.onPrepareCustomDrag(list);

            if (dragObject != null)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new Object[] { null };   // 必须在这里初始化objectReferences，否则会遇到Unity drag & drop的bug
                DragAndDrop.SetGenericData(list.DragStartType, dragObject);
                m_IsCusDragPrepare = true;    
            }
        }
    }


    private void CustomDragStart(EditorControl c)
    {
        ListViewCtrl list = c as ListViewCtrl;

        if (null == list)
        {
            return;
        }
        
        if(m_IsCusDragPrepare)
        {
            DragAndDrop.StartDrag(list.DragStartType);
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

            m_DragBegionCtrl = list;

            m_IsCusDragStart = true;

            m_IsCusDragPrepare = false;
        }

    }

    private void CustomDragUpdated(EditorControl c)
    {
        object dragObject = null;
        ListViewCtrl list = c as ListViewCtrl;

        if (null == list)
        {
            return;
        }

        if (
               (m_DragBegionCtrl != null)
            && (m_DragBegionCtrl.Name == list.Name)
            )
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }
        else
        {
            dragObject = DragAndDrop.GetGenericData(list.DragAcceptType);

            if(list.onTryAcceptCustomDrag != null)
            {
                if(list.onTryAcceptCustomDrag(list, dragObject))
                {
                    if (Event.current.control)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

    }
    
    private void CustomDragAccept(EditorControl c, bool isCtrl)
    {
        object dragObject = null;
        ListViewCtrl list = c as ListViewCtrl;

        if (null == list)
        {
            return;
        }

        DragAndDrop.AcceptDrag();

        dragObject = DragAndDrop.GetGenericData(c.DragAcceptType);
        c.DragObject = dragObject;
        if (isCtrl)
        {
            c.frameTriggerInfo.isCustomDragAcceptCtrl = true;
        }
        else
        {
            c.frameTriggerInfo.isCustomDragAccept = true;
        }

        m_IsCusDragStart = false;

        DragAndDrop.PrepareStartDrag();
    }
	//Add by liteng for MoveAtlas End

    GUIStyle itemStyle;
    Color itemTextColor =
       new Color(179f / 255f, 179f / 255f, 179f / 255f); // dark style下foldout文字颜色

    //Add by liteng for MoveAtlas At 2014/1/4 Start
    float m_Padding = 60.0f;

}

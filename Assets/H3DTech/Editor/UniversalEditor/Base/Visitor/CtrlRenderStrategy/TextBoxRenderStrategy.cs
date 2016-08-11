using UnityEngine;
using System.Collections;
using UnityEditor;


public class TextBoxRenderStrategy : FocusRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        TextBoxCtrl textBox = c as TextBoxCtrl;
        currCtrl = c as TextBoxCtrl;
        if (
               (null == textBox)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!textBox.Enable);

        textBoxContent.text = textBox.Caption;
        if (textBox.Icon != null)
        {
            textBoxContent.image = textBox.Icon;
        }

        lastText = textBox.Text;

        EditorGUILayout.BeginHorizontal();
        Vector2 labelSize = EditorStyles.label.CalcSize(textBoxContent);
        EditorGUILayout.LabelField(textBoxContent, new GUILayoutOption[] {GUILayout.Width(labelSize.x)});
      
        GUI.SetNextControlName(textBox.CtrlID);
        textBox.Text = EditorGUILayout.TextField(textBox.Text, textBox.GetOptions());
        textBox.IsForceUpdate = false;

        EditorGUILayout.EndHorizontal();

        if (
               (lastText != null)
            && !lastText.Equals(textBox.Text)
            )
        {
            textBox.frameTriggerInfo.isValueChanged = true;
            lastText = textBox.Text;

            textBox.RequestRepaint();
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        HandleDragAction(c);

        CheckInputEvent(c);
    }

    private void HandleDragAction(EditorControl c)
    {

        if (
               (null == currCtrl)
            || !currCtrl.IsCurrentCtrlEnable()
            )
        {
            return;
        }


        //Vector2 currMousePos = FrameInputInfo.GetInstance().currPos;
        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);

        //DragUpdated
        if (
                (currCtrl.LastRect.Contains(localMousePos)) &&
                (FrameInputInfo.GetInstance().customDragUpdated)
            )
        {
            CustomDragUpdated(currCtrl);

            currCtrl.RequestRepaint();
        }
        else if (
                    FrameInputInfo.GetInstance().dragObjsPerform &&
                    currCtrl.LastRect.Contains(localMousePos)
            )
        {
            CustomDragAccept(currCtrl, false);

            currCtrl.RequestRepaint();
        }
    }

    private void CustomDragUpdated(EditorControl c)
    {
        object dragObject = null;

        if (null == currCtrl)
        {
            return;
        }

        if (
               (m_DragBegionCtrl != null)
            && (m_DragBegionCtrl.Name == currCtrl.Name)
            )
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }
        else
        {
            dragObject = DragAndDrop.GetGenericData(currCtrl.DragAcceptType);

            if (currCtrl.onTryAcceptCustomDrag != null)
            {
                if (currCtrl.onTryAcceptCustomDrag(currCtrl, dragObject))
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

        if (null == currCtrl)
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
    GUIContent textBoxContent = new GUIContent();
    private TextBoxCtrl currCtrl;

    //bool isForceUpdateText = false;
    string lastText = string.Empty;
}

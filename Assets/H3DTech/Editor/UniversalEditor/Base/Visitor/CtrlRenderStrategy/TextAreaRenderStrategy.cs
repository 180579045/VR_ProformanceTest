using UnityEngine;
using UnityEditor;

public class TextAreaRenderStrategy : FocusRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as TextAreaCtrl;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastText = (string)currCtrl.CurrValue;
  
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, null, GUI.skin.verticalScrollbar, GUIStyle.none, currCtrl.GetOptions());
        GUI.SetNextControlName(currCtrl.CtrlID);
        currCtrl.CurrValue = EditorGUILayout.TextArea((string)currCtrl.CurrValue, GUILayout.Width(currCtrl.Size.width - 25));
        currCtrl.IsForceUpdate = false;
        EditorGUILayout.EndScrollView();

        if (lastText != (string)currCtrl.CurrValue)
        {
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private TextAreaCtrl currCtrl;
    private string lastText = string.Empty;
    private Vector2 scrollPos = new Vector2(0, 0);
}
using UnityEngine;
using UnityEditor;

public class TagFieldRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as TagFieldCtrl;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelectTag = (string)currCtrl.CurrValue;

        if (!string.IsNullOrEmpty(currCtrl.Caption))
        {
            currCtrl.CurrValue = EditorGUILayout.TagField(currCtrl.Caption, (string)currCtrl.CurrValue, currCtrl.GetOptions());
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.TagField((string)currCtrl.CurrValue, currCtrl.GetOptions());
        }

        if (lastSelectTag != (string)currCtrl.CurrValue)
        {
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private TagFieldCtrl currCtrl;
    private string lastSelectTag = string.Empty;
}
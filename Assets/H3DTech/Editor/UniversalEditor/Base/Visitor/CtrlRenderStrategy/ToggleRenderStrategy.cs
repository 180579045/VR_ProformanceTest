using UnityEngine;
using UnityEditor;

public class ToggleRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as ToggleCtrl;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelect = (bool)currCtrl.CurrValue;

        if(currCtrl.IsToggleLeft)
        {
            currCtrl.CurrValue = EditorGUILayout.ToggleLeft(currCtrl.Caption, (bool)currCtrl.CurrValue);
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.Toggle(currCtrl.Caption, (bool)currCtrl.CurrValue);
        }

        if (lastSelect != (bool)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private ToggleCtrl currCtrl;
    private bool lastSelect = false;

}
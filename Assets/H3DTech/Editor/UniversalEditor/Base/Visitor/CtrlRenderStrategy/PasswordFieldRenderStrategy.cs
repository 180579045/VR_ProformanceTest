using UnityEditor;
using UnityEngine;

public class PasswordFieldRenderStrategy : FocusRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as PasswordFieldCtrl;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastPassword = (string)currCtrl.CurrValue;

        if (string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.PasswordField((string)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }
        else
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.PasswordField(currCtrl.Caption, (string)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }

        if (lastPassword != (string)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private PasswordFieldCtrl currCtrl;
    private string lastPassword = string.Empty;
}
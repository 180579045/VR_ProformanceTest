using UnityEditor;
using System.Collections;
using UnityEngine;

public class FloatFieldRenderStrategy : FocusRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as DataFieldCtrl<float>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (float)currCtrl.CurrValue;

        if (string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.FloatField((float)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }
        else
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.FloatField(currCtrl.Caption, (float)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }

        if (Mathf.Abs((float)currCtrl.CurrValue - lastValve) > currCtrl.ValueEpsilon)
        {
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private DataFieldCtrl<float> currCtrl;
    private float lastValve = 0;
}
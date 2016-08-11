using UnityEngine;
using UnityEditor;
using System;

public class MaskFieldRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as MaskFieldCtrl<int>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelectItem = (int)currCtrl.CurrValue;

        if (string.IsNullOrEmpty(currCtrl.Caption))
        {
            currCtrl.CurrValue = EditorGUILayout.MaskField((int)currCtrl.CurrValue, currCtrl.DispStr, currCtrl.GetOptions());
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.MaskField(currCtrl.Caption, (int)currCtrl.CurrValue, currCtrl.DispStr, currCtrl.GetOptions());
        }

        if (lastSelectItem != (int)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

    }

    private MaskFieldCtrl<int> currCtrl;

    private int lastSelectItem = 0;
}
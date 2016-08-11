using UnityEngine;
using UnityEditor;
using System;

public class EnumMaskFieldRenderStrategy : EditorRenderStrategy
{  
    public override void Visit(EditorControl c)
    {
        currCtrl = c as MaskFieldCtrl<Enum>;
        if(
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelectItem = (Enum)currCtrl.CurrValue;

        if(string.IsNullOrEmpty(currCtrl.Caption))
        {
            currCtrl.CurrValue = EditorGUILayout.EnumMaskField((Enum)currCtrl.CurrValue, currCtrl.GetOptions());
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.EnumMaskField(currCtrl.Caption, (Enum)currCtrl.CurrValue, currCtrl.GetOptions());
        }

        if (!lastSelectItem.Equals((Enum)currCtrl.CurrValue))
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private MaskFieldCtrl<Enum> currCtrl;

    private Enum lastSelectItem = null;
}
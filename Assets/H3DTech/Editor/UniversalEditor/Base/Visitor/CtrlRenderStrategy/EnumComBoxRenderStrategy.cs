using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class EnumComboBoxRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as ComboBoxCtrl<Enum>;

        if (
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
            currCtrl.CurrValue = EditorGUILayout.EnumPopup((Enum)currCtrl.CurrValue,
                                                            new GUILayoutOption[] { GUILayout.Width(currCtrl.Size.width), GUILayout.Height(currCtrl.Size.height) });
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.EnumPopup(currCtrl.Caption, (Enum)currCtrl.CurrValue,
                                                            new GUILayoutOption[] { GUILayout.Width(currCtrl.Size.width), GUILayout.Height(currCtrl.Size.height) });
        }

        if (!lastSelectItem.Equals((Enum)currCtrl.CurrValue))
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    ComboBoxCtrl<Enum> currCtrl;
    private Enum lastSelectItem = null;

}

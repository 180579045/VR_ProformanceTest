using UnityEngine;
using System.Collections;
using UnityEditor;

public class ComboBoxRenderStrategy : EditorRenderStrategy 
{
    public override void Visit(EditorControl c) 
    {
        currCtrl = c as ComboBoxCtrl<int>;

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
            currCtrl.CurrValue = EditorGUILayout.IntPopup(
                (int)currCtrl.CurrValue,
                currCtrl.DisplayOptions,
                currCtrl.OptionValues, new GUILayoutOption[] { GUILayout.Width(currCtrl.Size.width), GUILayout.Height(currCtrl.Size.height) });
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.IntPopup(
                currCtrl.Caption,
                (int)currCtrl.CurrValue,
                currCtrl.DisplayOptions,
                currCtrl.OptionValues, new GUILayoutOption[] { GUILayout.Width(currCtrl.Size.width), GUILayout.Height(currCtrl.Size.height) });

        }

        if (lastSelectItem != (int)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    ComboBoxCtrl<int> currCtrl;
    private int lastSelectItem = 0;

}

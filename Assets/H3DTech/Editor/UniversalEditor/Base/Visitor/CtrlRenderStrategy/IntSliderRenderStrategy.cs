using UnityEngine;
using System.Collections;
using UnityEditor;

public class IntSliderRenderStrategy : FocusRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as SliderCtrl<int>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (int)currCtrl.CurrValue;

        if(!string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.IntSlider(currCtrl.Caption, (int)currCtrl.CurrValue, (int)currCtrl.ValueRange.x, (int)currCtrl.ValueRange.y, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }
        else
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.IntSlider((int)currCtrl.CurrValue, (int)currCtrl.ValueRange.x, (int)currCtrl.ValueRange.y, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }

        if (lastValve != (int)currCtrl.CurrValue)
        {
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

    }

    private SliderCtrl<int> currCtrl;
    private int lastValve = 0;

}

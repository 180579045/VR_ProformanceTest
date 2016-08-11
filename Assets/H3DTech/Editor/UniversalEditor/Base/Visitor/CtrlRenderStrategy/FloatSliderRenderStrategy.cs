using UnityEngine;
using System.Collections;
using UnityEditor;

public class FloatSliderRenderStrategy : FocusRenderStrategy 
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as SliderCtrl<float>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (float)currCtrl.CurrValue;

        if(!string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.Slider(currCtrl.Caption, (float)currCtrl.CurrValue, currCtrl.ValueRange.x, currCtrl.ValueRange.y, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }
        else
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.Slider((float)currCtrl.CurrValue, currCtrl.ValueRange.x, currCtrl.ValueRange.y, currCtrl.GetOptions());
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

    private SliderCtrl<float> currCtrl;
    private float lastValve = 0;
}

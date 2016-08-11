using UnityEngine;
using System.Collections;
using UnityEditor;

public class MinMaxSliderRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as SliderCtrl<Vector2>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (Vector2)currCtrl.CurrValue;
        tempValue = (Vector2)currCtrl.CurrValue;

        if (!string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUIContent dispContent = new GUIContent();
            dispContent.text = currCtrl.Caption;
            EditorGUILayout.MinMaxSlider(dispContent, ref tempValue.x, ref tempValue.y, currCtrl.ValueRange.x, currCtrl.ValueRange.y, currCtrl.GetOptions());
        }
        else
        {
            EditorGUILayout.MinMaxSlider(ref tempValue.x, ref tempValue.y, currCtrl.ValueRange.x, currCtrl.ValueRange.y, currCtrl.GetOptions());
        }

        currCtrl.CurrValue = tempValue;

        if ((Vector2)currCtrl.CurrValue != lastValve)
        {
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private SliderCtrl<Vector2> currCtrl;
    private Vector2 lastValve = new Vector2(0, 0);
    private Vector2 tempValue = new Vector2(0, 0);
}

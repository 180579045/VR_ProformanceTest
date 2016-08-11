using UnityEngine;
using UnityEditor;

public class LayerFieldRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as LayerFieldCtrl;
        if (
               (null == currCtrl)
            || (!currCtrl.Visiable)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelectIndex = (int)currCtrl.CurrValue;

        if(!string.IsNullOrEmpty(currCtrl.Caption))
        {
            currCtrl.CurrValue = EditorGUILayout.LayerField(currCtrl.Caption, (int)currCtrl.CurrValue, currCtrl.GetOptions());
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.LayerField((int)currCtrl.CurrValue, currCtrl.GetOptions());
        }

        if (lastSelectIndex != (int)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private LayerFieldCtrl currCtrl;
    private int lastSelectIndex = 0;
}
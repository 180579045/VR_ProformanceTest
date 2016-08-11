using UnityEngine;
using UnityEditor;

public class SelectionGridRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as SelectionGridCtrl;
        if(
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelectIndex = (int)currCtrl.CurrValue;

        currCtrl.CurrValue = GUILayout.SelectionGrid((int)currCtrl.CurrValue, currCtrl.DispStr, currCtrl.XCount, c.GetOptions());

        if (lastSelectIndex != (int)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

    }

    private SelectionGridCtrl currCtrl;
    private int lastSelectIndex = 0;
}
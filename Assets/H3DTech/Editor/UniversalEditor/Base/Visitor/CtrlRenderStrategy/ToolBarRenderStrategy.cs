using UnityEngine;
using UnityEditor;

public class ToolBarRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as ToolBarCtrl;

        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastSelectIndex = (int)currCtrl.CurrValue;

        currCtrl.CurrValue = GUILayout.Toolbar((int)currCtrl.CurrValue, currCtrl.DispStr, currCtrl.GetOptions());

        if (lastSelectIndex != (int)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private ToolBarCtrl currCtrl;
    private int lastSelectIndex = 0;
}
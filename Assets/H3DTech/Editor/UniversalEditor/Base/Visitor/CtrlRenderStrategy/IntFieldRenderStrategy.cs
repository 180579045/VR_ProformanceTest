using UnityEditor;
using UnityEngine;
public class IntFieldRenderStrategy : FocusRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as DataFieldCtrl<int>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (int)currCtrl.CurrValue;
 
        if (string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.IntField((int)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }
        else
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.IntField(currCtrl.Caption, (int)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;     
        }

        if (lastValve != (int)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

    }
    
    private DataFieldCtrl<int> currCtrl;
    private int lastValve = 0;
}
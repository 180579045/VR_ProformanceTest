using UnityEditor;
using UnityEngine;

public class Vector3FieldRenderStrategy : EditorRenderStrategy
{
    //public override void AfterVisit(EditorControl c)
    //{
    //    if (
    //            !c.Enable
    //         || (IsForceUpdateText
    //             && (Event.current.type == EventType.Repaint))
    //        )
    //    {

    //        GUI.FocusControl("");

    //        IsForceUpdateText = false;
    //    }
    //}

    public override void Visit(EditorControl c)
    {
        currCtrl = c as DataFieldCtrl<Vector3>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);


        lastValve = (Vector3)currCtrl.CurrValue;

        GUI.SetNextControlName(currCtrl.CtrlID);
        currCtrl.CurrValue = EditorGUILayout.Vector3Field(currCtrl.Caption, (Vector3)currCtrl.CurrValue, currCtrl.GetOptions());
        currCtrl.IsForceUpdate = false;

        if (lastValve != (Vector3)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        currCtrl.UpdateLastRect();

        CheckInputEvent(c);
    }

    private DataFieldCtrl<Vector3> currCtrl;
    private Vector3 lastValve = new Vector3(0, 0, 0);
}
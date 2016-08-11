using UnityEditor;
using UnityEngine;

public class Vector2FieldRenderStrategy : EditorRenderStrategy
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
        currCtrl = c as DataFieldCtrl<Vector2>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (Vector2)currCtrl.CurrValue;

        GUI.SetNextControlName(currCtrl.CtrlID);
        currCtrl.CurrValue = EditorGUILayout.Vector2Field(currCtrl.Caption, (Vector2)currCtrl.CurrValue, currCtrl.GetOptions());
        currCtrl.IsForceUpdate = false;

        if (lastValve != (Vector2)currCtrl.CurrValue)
        {
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        currCtrl.UpdateLastRect();

        CheckInputEvent(c);
    }

    private DataFieldCtrl<Vector2> currCtrl;
    private Vector2 lastValve = new Vector2(0, 0);
}
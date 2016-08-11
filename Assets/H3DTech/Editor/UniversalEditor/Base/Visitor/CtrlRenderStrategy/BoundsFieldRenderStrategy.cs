using UnityEditor;
using UnityEngine;

public class BoundsFieldRenderStrategy : EditorRenderStrategy
{
    //public override bool PreVisit(EditorControl c)
    //{
    //    if (c.IsForceUpdate)
    //    {
    //        GUI.FocusControl("");
    //    }

    //    return true;
    //}

    //public override void AfterVisit(EditorControl c)
    //{
    //    if (
    //            !c.Enable
    //         || (IsForceUpdateText
    //             && (Event.current.type == EventType.Repaint))
    //         )
    //    {

    //        GUI.FocusControl("");

    //        IsForceUpdateText = false;
    //    }

    //}

    public override void Visit(EditorControl c)
    {
        currCtrl = c as DataFieldCtrl<Bounds>;
        if (
               (null == currCtrl)
            || (!currCtrl.Visiable)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (Bounds)currCtrl.CurrValue;

        if (string.IsNullOrEmpty(currCtrl.Caption))
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.BoundsField((Bounds)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }
        else
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            currCtrl.CurrValue = EditorGUILayout.BoundsField(currCtrl.Caption, (Bounds)currCtrl.CurrValue, currCtrl.GetOptions());
            currCtrl.IsForceUpdate = false;
        }

        if (lastValve != (Bounds)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }


    private DataFieldCtrl<Bounds> currCtrl;
    private Bounds lastValve = new Bounds(Vector3.zero, new Vector3(0, 0, 0));
}
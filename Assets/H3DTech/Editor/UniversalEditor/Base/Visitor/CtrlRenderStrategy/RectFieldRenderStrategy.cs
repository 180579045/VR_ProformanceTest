using UnityEditor;
using UnityEngine;

public class RectFieldRenderStrategy : EditorRenderStrategy
{

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
        currCtrl = c as DataFieldCtrl<Rect>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (Rect)currCtrl.CurrValue;

        GUI.SetNextControlName(currCtrl.CtrlID);
        currCtrl.CurrValue = EditorGUILayout.RectField(currCtrl.Caption, (Rect)currCtrl.CurrValue, currCtrl.GetOptions());
        currCtrl.IsForceUpdate = false;

        if (lastValve != (Rect)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }


        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

    }

    private DataFieldCtrl<Rect> currCtrl;
    private Rect lastValve = new Rect(0, 0, 0, 0);
}
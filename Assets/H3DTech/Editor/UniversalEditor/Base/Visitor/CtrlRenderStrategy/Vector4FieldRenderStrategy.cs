using UnityEditor;
using UnityEngine;

public class Vector4FieldRenderStrategy : EditorRenderStrategy
{
    //public override void AfterVisit(EditorControl c)
    //{
    //    if (
    //            c.IsUnfocusCtrl
    //         || (IsForceUpdateText
    //             && (Event.current.type == EventType.Repaint))
    //         )
    //    {
            
    //        GUI.FocusControl("");

    //        IsForceUpdateText = false;

    //        c.IsUnfocusCtrl = false;
    //    }
    //}

    public override void Visit(EditorControl c)
    {
        currCtrl = c as DataFieldCtrl<Vector4>;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastValve = (Vector4)currCtrl.CurrValue;

        GUI.SetNextControlName(currCtrl.CtrlID);
        currCtrl.CurrValue = EditorGUILayout.Vector4Field(currCtrl.Caption, (Vector4)currCtrl.CurrValue, currCtrl.GetOptions());
        currCtrl.IsForceUpdate = false;


        if (lastValve != (Vector4)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private DataFieldCtrl<Vector4> currCtrl;
    private Vector4 lastValve = new Vector4(0, 0, 0, 0);
}
using UnityEngine;
using UnityEditor;
using System;

public class ObjectFieldRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as ObjectFieldCtrl;
        if (
               (null == currCtrl)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        lastItem = (UnityEngine.Object)currCtrl.CurrValue;

        if (!string.IsNullOrEmpty(currCtrl.Caption))
        {
            currCtrl.CurrValue = EditorGUILayout.ObjectField(currCtrl.Caption, (UnityEngine.Object)currCtrl.CurrValue, currCtrl.ObjectType, currCtrl.IsAlowSceneObjects, currCtrl.GetOptions());
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.ObjectField((UnityEngine.Object)currCtrl.CurrValue, currCtrl.ObjectType, currCtrl.IsAlowSceneObjects, currCtrl.GetOptions());
        }

        if (lastItem != (UnityEngine.Object)currCtrl.CurrValue)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private ObjectFieldCtrl currCtrl;
    private UnityEngine.Object lastItem = null;
}
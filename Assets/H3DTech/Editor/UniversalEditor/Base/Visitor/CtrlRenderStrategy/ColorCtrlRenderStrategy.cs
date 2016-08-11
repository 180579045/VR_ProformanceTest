using UnityEngine;
using System.Collections;
using UnityEditor;

public class ColorCtrlRenderStrategy : EditorRenderStrategy
{
    private ColorCtrl currCtrl = null;

    public override void Visit(EditorControl c) 
    {
        currCtrl = c as ColorCtrl;
        if (currCtrl == null)
            return;


        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(currCtrl.Caption , new GUILayoutOption[]{GUILayout.MaxWidth(100f)});
        Color newColor = EditorGUILayout.ColorField(
            currCtrl.currColor , 
            new GUILayoutOption[]{ 
                GUILayout.MaxWidth(c.Size.width) , 
                GUILayout.MaxHeight(c.Size.height) });

        if( !newColor.Equals(currCtrl.currColor) )
        {
            currCtrl.currColor = newColor;
            currCtrl.CurrValue = newColor;
            currCtrl.frameTriggerInfo.isValueChanged = true;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }
}

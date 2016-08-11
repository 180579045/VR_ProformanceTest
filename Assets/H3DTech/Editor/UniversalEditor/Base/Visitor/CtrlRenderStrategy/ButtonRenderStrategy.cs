using UnityEngine;
using System.Collections;
using UnityEditor;

public class ButtonRenderStrategy : EditorRenderStrategy
{
    public ButtonRenderStrategy()
    { 
        
    }

    public override void Visit(EditorControl c) 
    {
        currCtrl = c as ButtonCtrl;
        if (currCtrl == null)
            return;


        Color oldColor = GUI.color;
        GUI.color = currCtrl.BtnColor;

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        //Moidfy by liteng for 追加共同控件 at 2015/2/26 Start
        if(currCtrl.IsRepeat)
        {
            c.frameTriggerInfo.isClick = GUILayout.RepeatButton(c.Caption, c.GetOptions());
            //currCtrl.RequestRepaint();
        }
        else
        { 
            c.frameTriggerInfo.isClick = GUILayout.Button(c.Caption, c.GetOptions()); 
        }
        //Modify by liteng for 追加共同控件 End

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

        GUI.color = oldColor;
  
    }


    private ButtonCtrl currCtrl;
}

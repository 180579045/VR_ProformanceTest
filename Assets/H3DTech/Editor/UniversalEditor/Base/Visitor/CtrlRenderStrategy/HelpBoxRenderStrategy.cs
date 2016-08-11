using UnityEditor;

public class HelpBoxRenderStrategy : EditorRenderStrategy
{

    public override void Visit(EditorControl c)
    {
        currCtrl = c as HelpBoxCtrl;
        if (
               (null == currCtrl)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        EditorGUILayout.HelpBox(currCtrl.Caption, currCtrl.MsgType);

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    private HelpBoxCtrl currCtrl;
}
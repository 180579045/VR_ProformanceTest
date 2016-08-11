using UnityEngine;
using UnityEditor;

public class SpaceRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        SpaceCtrl currCtrl = c as SpaceCtrl;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        if ((float)currCtrl.CurrValue < 0f)
        {
            return;
        }

        GUILayout.Space((float)currCtrl.CurrValue);
    }
}
using UnityEngine;
using System.Collections;

public class HBoxCtrl : EditorCtrlComposite 
{
    public HBoxCtrl(bool isShowScroll = false)
    {
        this.IsShowScrollBar = isShowScroll;
        Size = new Rect();
    }

    public override GUILayoutOption[] GetOptions()
    {
        return new GUILayoutOption[]{
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true)
        };
    }

 
}

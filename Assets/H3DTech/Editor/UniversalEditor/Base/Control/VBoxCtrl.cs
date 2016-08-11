using UnityEngine;
using System.Collections;

public class VBoxCtrl : EditorCtrlComposite
{
    public VBoxCtrl(bool isShowScroll = false)
    {
        this.Size = new Rect();
        this.IsShowScrollBar = isShowScroll;
    }

    public override GUILayoutOption[] GetOptions()
    {
        return new GUILayoutOption[]{
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true)
        };
    }

}

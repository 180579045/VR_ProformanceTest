using UnityEngine;
using System.Collections;

public class ButtonCtrl : EditorControl 
{
    public ButtonCtrl()
    {
        this.Size = new Rect(0, 0, 80, 20);
    }

    public Color BtnColor
    {
        get { return color; }
        set { color = value; }
    }

    //Add by liteng for 追加共同控件 at 2015/2/26 Start
    public bool IsRepeat
    {
        get { return isRepeat; }
        set { isRepeat = value; }
    }
    //Modify by liteng for 代码改善 End

    public override GUILayoutOption[] GetOptions() 
    { 
        if( layoutConstraint.expandWidth == true )
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(Size.height) };  
        }
        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) }; 
    }

    private Color color = Color.white;

    //Add by liteng for 追加共同控件 at 2015/2/26
    private bool isRepeat = false;
}

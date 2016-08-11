using UnityEngine;
using System.Collections;

public class LabelCtrl : EditorControl 
{
    public Color textColor = Color.white;
    //Add by liteng for MoveAtlas At 2014/1/4
    public int fontSize = 16;
    public RectOffset margin = new RectOffset(0, 0, 5, 5);
    public override GUILayoutOption[] GetOptions()
    {
        if (layoutConstraint.expandWidth == true)
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20f) };
        }

        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) };
    }
}

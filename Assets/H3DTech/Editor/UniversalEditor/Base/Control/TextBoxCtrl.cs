using UnityEngine;
using System.Collections;

public class TextBoxCtrl : EditorControl 
{
    public TextBoxCtrl()
    {
        this.Size = new Rect();
    }
    public string Text
    {
        get
        {
            return (string)CurrValue;
        }
        set
        {
            if ((string)currValue != (string)value)
            {
                currValue = (string)value;
                IsForceUpdate = true;
            }
        }
    }

    public Texture Icon
    {
        get { return image; }
        set { image = value; }
    }

    private Texture image = null;

    public override GUILayoutOption[] GetOptions()
    {
        if (
            (layoutConstraint.expandWidth == true)
            || ((0 == this.Size.width))
            || ((0 == this.Size.height))
            )
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20f) };
        }

        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) };
    }

    //public bool isForceUpdate = false;
    //public bool isFocusLastFrame = false;

}

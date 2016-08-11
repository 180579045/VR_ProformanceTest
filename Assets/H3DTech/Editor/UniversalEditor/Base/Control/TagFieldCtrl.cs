using UnityEngine;
using System;

public class TagFieldCtrl : EditorControl
{
    public TagFieldCtrl()
    {
        this.CurrValue = string.Empty;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public TagFieldCtrl(string value)
    {
        this.CurrValue = value;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public override object CurrValue
    {
        get { return currValue; }
        set
        {
            try
            {
                currValue = (string)value;
            }
            catch (InvalidCastException)
            {
                currValue = string.Empty;
                Debug.Log("为TagFieldCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }
    public override GUILayoutOption[] GetOptions()
    {
        if (layoutConstraint.expandWidth == true)
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(Size.height) };
        }
        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) };
    }
}
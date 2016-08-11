using UnityEngine;
using System;

public class PasswordFieldCtrl : EditorControl
{
    public PasswordFieldCtrl()
    {
        this.CurrValue = string.Empty;
    }

    public PasswordFieldCtrl(string value)
    {
        this.CurrValue = value;
    }

    public override object CurrValue
    {
        get { return currValue; }
        set
        {
            try
            {
                if ((string)currValue != (string)value)
                {
                    currValue = (string)value;
                    IsForceUpdate = true;
                }
            }
            catch (InvalidCastException)
            {
                currValue = string.Empty;
                Debug.Log("为PasswordFieldCtrl控件的CurrValue赋值了错误类型数据!");
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
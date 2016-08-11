using UnityEngine;
using System;

public class LayerFieldCtrl : EditorControl
{
    public LayerFieldCtrl()
    {
        this.CurrValue = 0;
    }

    public LayerFieldCtrl(int value)
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
                currValue = (int)value;
            }
            catch (InvalidCastException)
            {
                currValue = 0;
                Debug.Log("为LayerFieldCtrl控件的CurrValue赋值了错误类型数据!");
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
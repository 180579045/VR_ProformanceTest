using UnityEngine;
using System;

public class ToolBarCtrl : CollectionCtrlBase<string>
{
    public string[] DispStr
    {
        get { return ItemTbl.ToArray(); }
    }

    public ToolBarCtrl()
    {
        this.CurrValue = 0;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public ToolBarCtrl(int value)
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
                currValue = (int)value;
            }
            catch (InvalidCastException)
            {
                currValue = 0;
                Debug.Log("为ToolBarCtrl控件的CurrValue赋值了错误类型数据!");
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
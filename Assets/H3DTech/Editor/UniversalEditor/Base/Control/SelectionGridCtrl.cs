using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class SelectionGridCtrl : CollectionCtrlBase<string>
{
    public string[] DispStr
    {
        get { return ItemTbl.ToArray(); }
    }

    public int XCount
    {
        get { return xCount; }
        set { xCount = value; }
    }

    public SelectionGridCtrl()
    {
        this.CurrValue = 0;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public SelectionGridCtrl(int value)
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
                Debug.Log("为SelectionGridCtrl控件的CurrValue赋值了错误类型数据!");
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

    private int xCount = 3;
}
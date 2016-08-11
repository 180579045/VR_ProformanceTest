using UnityEngine;
using System;

public class ToggleCtrl : EditorControl
{
    private bool isToggleLeft = false;

    public bool IsToggleLeft
    {
        get { return isToggleLeft; }
        set { isToggleLeft = value; }
    }

    public ToggleCtrl()
    {
        this.CurrValue = false;
    }

    public ToggleCtrl(bool value)
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
                currValue = (bool)value;
            }
            catch (InvalidCastException)
            {
                currValue = false;
                Debug.Log("为ToggleCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }
}
using UnityEngine;
using System;

public class MaskFieldCtrl<T> : CollectionCtrlBase<string>
{
    public string[] DispStr
    {
        get { return ItemTbl.ToArray(); }
    }

    public MaskFieldCtrl(T currValue)
    {
        this.CurrValue = currValue;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public MaskFieldCtrl()
    {
        this.CurrValue = default(T);
        this.Size = new Rect(0, 0, 300, 20);
    }

    public override object CurrValue
    {
        get { return currValue; }
        set
        {
            try
            {
                currValue = (T)value;
            }
            catch (InvalidCastException)
            {
                currValue = default(T);

                Debug.Log("为MaskFieldCtrl控件的CurrValue赋值了错误类型数据!");
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
using UnityEngine;
using System;
public class DataFieldCtrl<T> : EditorControl
{
    public DataFieldCtrl(T initValue)
    {
        this.CurrValue = initValue;
        this.Size = new Rect(0, 0, 0, 0);
    }

    public DataFieldCtrl()
    {
        this.CurrValue = default(T);
        this.Size = new Rect(0, 0, 0, 0);
    }

    public override object CurrValue
    {
        get { return currValue; }
        set 
        {
            try
            {
                if (
                    (null == currValue)
                    && (value != null)
                    )
                {
                    currValue = (T)value;
                    IsForceUpdate = true;
                }
                else if (currValue != null)
                {
                    if (currValue.GetType().IsValueType)
                    {
                        if (!currValue.Equals(value))
                        {
                            currValue = (T)value;
                            IsForceUpdate = true;
                        }
                    }
                    else
                    {
                        if (!object.ReferenceEquals(currValue, value))
                        {
                            currValue = (T)value;
                            IsForceUpdate = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                currValue = default(T);
                Debug.Log("为DataFieldCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }

    public override GUILayoutOption[] GetOptions()
    {
        GUILayoutOption[] layoutOption = null;
        if (layoutConstraint.expandWidth == true)
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(Size.height) };
        }

        if(
               (0 == Size.width)
            || (0 == Size.height)
            )
        {
            layoutOption = new GUILayoutOption[] { GUILayout.MaxWidth(200f) };

        }
        else
        {
            layoutOption = new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) };
        }

        return layoutOption;
    }
}
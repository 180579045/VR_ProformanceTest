using UnityEngine;
using System;

public class SliderCtrl<T> : EditorControl
{

    public SliderCtrl(T value)
    {
        this.CurrValue = value;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public SliderCtrl()
    {
        this.CurrValue = default(T);
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
            catch (InvalidCastException)
            {
                currValue = default(T);
                Debug.Log("为SliderCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }

    public override GUILayoutOption[] GetOptions()
    {
        //return new GUILayoutOption[]{
        //    GUILayout.Height(30f),
        //    GUILayout.MinWidth(500f),
        //    GUILayout.ExpandWidth(true)
        //};
        if (layoutConstraint.expandWidth == true)
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(Size.height) };
        }
        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) }; 
    }
} 

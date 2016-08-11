using UnityEngine;
using System;

public class SpaceCtrl : EditorControl
{
    public override object CurrValue
    {
        get { return currValue; }
        set
        {
            try
            {
                currValue = value;
            }
            catch (Exception)
            {
                currValue = 0f;
                Debug.Log("为SpaceCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ComboItem
{
    public ComboItem( string n , int o ) 
    {
        name = n;
        option = o;
    }

    public string name = "";
    public int option = 0;
}

public class ComboBoxCtrl<T> : CollectionCtrlBase<ComboItem> 
{
    public ComboBoxCtrl(T currValue) 
    {
        this.CurrValue = currValue;
        this.Size = new Rect(0, 0, 300, 20);
    }

    public ComboBoxCtrl()
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
            catch (Exception)
            {
                currValue = default(T);
                Debug.Log("为ComboBoxCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }

    public string[] DisplayOptions 
    { 
        get 
        { 
            List<string> dipStr = new List<string>();

            foreach(var item in ItemTbl)
            {
                dipStr.Add(item.name);
            }

            return dipStr.ToArray(); 
        } 
    }

    public int[] OptionValues
    {
        get
        {
            List<int> values = new List<int>();

            foreach (var item in ItemTbl)
            {
                values.Add(item.option);
            }

            return values.ToArray();
        }
    }

    //List<ComboItem> options = new List<ComboItem>();
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollectionCtrlBase<T> : EditorControl
{
    public List<T> ItemTbl
    {
        get { return itemTbl; }
    }

    public void AddItem(T item)
    {
        if (null == item)
        {
            return;
        }

        if (!itemTbl.Contains(item))
        {
            itemTbl.Add(item);
        }
    }

    public void AddItem(List<T> tbl)
    {
        if (null == tbl)
        {
            return;
        }

        itemTbl = itemTbl.Union(tbl).ToList<T>();
    }

    public void RemoveItem(T item)
    {
        if (null == item)
        {
            return;
        }

        itemTbl.Remove(item);
    }

    public void RemoveItem(int index)
    {
        itemTbl.RemoveAt(index);
    }

    public void ClearItem()
    {
        itemTbl.Clear();
    }

    private List<T> itemTbl = new List<T>();

}

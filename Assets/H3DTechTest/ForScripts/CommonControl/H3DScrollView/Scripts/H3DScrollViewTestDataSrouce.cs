using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class H3DScrollViewTestDataSrouce : IH3DScrollViewDataSource
{
    int mItemDataCount = 0;
    List<object> dataList = new List<object>();

    public int itemDataCount
    {
        set
        {
            mItemDataCount = value;
            dataList.Clear();
            for( int i = 0 ; i < mItemDataCount ; i++ )
            {
                dataList.Add(i+1);
            }
        }
    }

    public int GetItemDataCount()
    {
        return mItemDataCount;
    }

    
    public bool TryGetItemData(int i, out object data)
    {
        if( i < 0 || i >= dataList.Count )
        {
            data = null;
            return false;
        }
        data = dataList[i];
        return true;
    }
    
    public void ReleaseItemData(object data)
    { 
    }


    	 
}

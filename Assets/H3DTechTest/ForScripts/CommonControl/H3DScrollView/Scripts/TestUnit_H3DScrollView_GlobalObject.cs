using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class TestUnit_H3DScrollView_GlobalObject : MonoBehaviour 
{

    public H3DScrollView scrollView;

    List<object> dataList = new List<object>();

    void Start()
    {
        for( int i = 0 ; i < 10 ; i++ )
        {
            dataList.Add(i);
        }

        scrollView.dataList = dataList;
         
    }


}

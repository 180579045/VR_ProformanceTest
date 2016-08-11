using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestUnit_H3DScrollView_SetDataListStaticData : MonoBehaviour 
{

    public H3DScrollView singleColVertScrollView;
    public H3DScrollView mutiColVertScrollView;
    public H3DScrollView singleColHoriScrollView;
    public H3DScrollView mutiColHoriScrollView;
    public int dataCount;
    public bool isDataSource = false;

    Vector2 scrollPos = Vector2.zero;

	void Start () 
    {
        UpdateData();
	}
	
	// Update is called once per frame
	void Update () 
    { 
	}

    void OnGUI()
    {
        string dataCountText = dataCount.ToString();

        GUILayout.BeginHorizontal();
        GUILayout.Label("数据量：",new GUILayoutOption[]{GUILayout.MaxWidth(70f)});
        dataCountText = GUILayout.TextField(dataCountText, new GUILayoutOption[] { GUILayout.MaxWidth(50f) });
        int c = 0;
        if( int.TryParse( dataCountText,out c ) )
        {
            if( c != dataCount )
            {
                c = Mathf.Max(c, 0);
                dataCount = c;
                UpdateData();
            }
            
        }
        GUILayout.EndHorizontal();

        Vector2 newScrollPos = Vector2.zero;
        GUILayout.BeginHorizontal();
        GUILayout.Label("ScrollPos横向定位：", new GUILayoutOption[] { GUILayout.MaxWidth(70f) });
        newScrollPos.x = GUILayout.HorizontalSlider(scrollPos.x, 0.0f, 1.0f, new GUILayoutOption[] { GUILayout.MaxWidth(70f) });
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ScrollPos纵向定位：", new GUILayoutOption[] { GUILayout.MaxWidth(70f) });
        newScrollPos.y = GUILayout.HorizontalSlider(scrollPos.y, 0.0f, 1.0f, new GUILayoutOption[] { GUILayout.MaxWidth(70f) });
        GUILayout.EndHorizontal();

        if( 
            newScrollPos.x != scrollPos.x ||
            newScrollPos.y != scrollPos.y 
           )
        {
            scrollPos = newScrollPos;
            UpdateScrollPos();

        }
    }

    void UpdateScrollPos()
    {
        singleColVertScrollView.scrollPos = scrollPos;
        mutiColVertScrollView.scrollPos = scrollPos;
        singleColHoriScrollView.scrollPos = scrollPos;
        mutiColHoriScrollView.scrollPos = scrollPos;
    }

    void UpdateData()
    {
        if (isDataSource)
        {
            H3DScrollViewTestDataSrouce dataSrc = new H3DScrollViewTestDataSrouce();
            dataSrc.itemDataCount = dataCount;

            singleColVertScrollView.dataSource = dataSrc;
            mutiColVertScrollView.dataSource = dataSrc;
            singleColHoriScrollView.dataSource = dataSrc;
            mutiColHoriScrollView.dataSource = dataSrc;
        }
        else
        {
            List<object> dataList = new List<object>();
            for (int i = 0; i < dataCount; i++)
            {
                dataList.Add(i + 1);
            }

            singleColVertScrollView.dataList = dataList;
            mutiColVertScrollView.dataList = dataList;
            singleColHoriScrollView.dataList = dataList;
            mutiColHoriScrollView.dataList = dataList;
        }
    }
}

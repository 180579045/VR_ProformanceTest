using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NikkiScrollViewTestDataSrouce : MonoBehaviour
{
    public enum H3DNikkDateState
    {
        Loading,         
        SetDate,
        ShowNow,
        none
    }
    public enum ResourceLoadResult
    {
        Failed = 0,
        Loading,
        Ok
    }

    public H3DScrollView mSingleColVertScrollView;
    public bool mIsDataSourceList = true;
    public H3DNikkDateState mH3DNikkDateState = H3DNikkDateState.Loading;
    public ResourceLoadResult mResourceLoadResult = ResourceLoadResult.Failed;
    public List<object> mNikiDataObjList = new List<object>();
    public List<NikiiDataSource> mNikiDataList = new List<NikiiDataSource>();
    public GameObject mLoadingObj;
    public GameObject mScrollBar;
    [System.NonSerialized]
    protected int dataCount;
    // Use this for initialization
    void Awake()
    {
        string pathTemp;
        dataCount = 100;
        mLoadingObj.SetActive(true);
        mScrollBar.SetActive(false);
        for (int i = 0; i < dataCount; ++i)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                pathTemp = "file://" + Application.dataPath + "/H3DTechExample/ForScripts/CommonControl/H3DScrollView/Data/Dress/" + "niki(" + i + ").assetbundle";               
            }
            else if (Application.platform == RuntimePlatform.Android)
                pathTemp = "file://" + "/mnt/sdcard/Android/data/H3DDateNiki/Dress/" + "niki(" + i + ").assetbundle";
            else
                pathTemp = "";
             StartCoroutine(Load(pathTemp,i));            
         }
    }   
    // Update is called once per frame
    void Update()
    {
         
        switch (mH3DNikkDateState)
        {
            case H3DNikkDateState.Loading:
                {
                    if (mNikiDataList.Count == dataCount)
                    {
                        mH3DNikkDateState = H3DNikkDateState.SetDate;
                        mLoadingObj.SetActive(false);
                        mScrollBar.SetActive(true);
                    }
                }
                break;
            case H3DNikkDateState.SetDate:
                {
                    if (mIsDataSourceList)
                    {
                        List<object> dataList = new List<object>();
                        for (int i = 0; i < dataCount; i++)
                        {
                            dataList.Add(mNikiDataList[i]);
                        }
                        mSingleColVertScrollView.dataList = dataList;
                    }
                    else
                    {
                        H3DNikkiScrollViewDataSrouce dataSrc = new H3DNikkiScrollViewDataSrouce();
                        dataSrc.itemDataCount = dataCount;
                        dataSrc.dataList.Clear();
                        for (int i = 0; i < dataCount; i++)
                        {
                            dataSrc.dataList.Add(mNikiDataList[i]);
                        }                        
                        mSingleColVertScrollView.dataSource = dataSrc;
 
                    }
                    mH3DNikkDateState = H3DNikkDateState.ShowNow;
                }
                break;
            case H3DNikkDateState.ShowNow:
                break;
            default:
                break;
        }

    }
    public IEnumerator Load(string path,int num)
    {
        WWW www = new WWW(path);      
        yield return www;
        UnityEngine.Object[] objs = null;
        if (www.isDone)
        {
            if (www.error == null)
            {
                mResourceLoadResult = ResourceLoadResult.Ok;
                AssetBundle bundle = www.assetBundle;
                objs = UnpackBundle<UnityEngine.Object>(bundle);                
                NikiiDataSource mNikiiDataSource = new NikiiDataSource();
                mNikiiDataSource.mNikiDateNum=num;
                mNikiiDataSource.mNikiDateTex=objs[0] as Texture2D;
                mNikiDataList.Add(mNikiiDataSource);                
                bundle.Unload(false);
                Debug.Log("资源" + path + "读取成功！");
            }
            else
            {
                mResourceLoadResult = ResourceLoadResult.Failed;
                Debug.LogWarning("资源" + path + "读取失败！error = " + www.error);
            }
        }
        else
        {
            mResourceLoadResult = ResourceLoadResult.Failed;
            Debug.LogWarning("资源" + path + "读取失败！error = " + www.error);
        }
    }
    public static T[] UnpackBundle<T>(AssetBundle bundle) where T : UnityEngine.Object
    {
        if (bundle == null)
        {
            return null;
        }

        List<T> resList = new List<T>();
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
        UnityEngine.Object[] objs = bundle.LoadAll();
#else
        UnityEngine.Object[] objs = bundle.LoadAllAssets();
#endif
        foreach (var obj in objs)
        {
            T res = obj as T;
            if (res != null)
            {
                resList.Add(res);
            }
        }
        return resList.ToArray();
    }
}

public class NikiiDataSource
{
    public Texture2D mNikiDateTex;
    public int mNikiDateNum;
}

public class H3DNikkiScrollViewDataSrouce : IH3DScrollViewDataSource
{
    int mItemDataCount = 0;
    public List<object> dataList = new List<object>();
    public int itemDataCount
    {
        set
        {
            mItemDataCount = value;                   
        }
    }

    public int GetItemDataCount()
    {
        return mItemDataCount;
    }


    public bool TryGetItemData(int i, out object data)
    {
        if (i < 0 || i >= dataList.Count)
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
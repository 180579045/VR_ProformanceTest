using UnityEngine;
using System.Collections;

public class H3DWrapContent : MonoBehaviour {


    Transform mTrans;
    UIPanel mPanel;
    H3DScrollView mScrollView;
    BetterList<Transform> mChildren = new BetterList<Transform>();

    //Vector3 mPanelStartPos = Vector3.zero;
    int mStartItemIndx = 0;
    H3DScrollView.H3DScrollViewMovementType mMoveType;
    int mTotalItemCount = 0;
    int mColumnLimit = 1;
    //Vector2 mScrollPos = Vector2.zero;
    int mItemWidth = 100;
    int mItemHeight = 100;
    //用来记录缓冲条目当前填充数据项索引
    int[] mSlots;
    int[] mSlotFlags;

    Vector3 tempVec3 = Vector3.zero;
     
	void Start () 
    {
        RecalcLayout();

        mScrollView.onDataDirty = OnDataChange;
        mScrollView.onScrollPosDirty = OnScrollPosChange;
        mScrollView.onForceUpdateData = ForceUpdateData;

        for(int i = 0 ; i < mChildren.size ; i++ )
        {
            mChildren[i].gameObject.SetActive(false);
        }

        UpdateContent(); 
	}
 
 
    void OnPanelMove( UIPanel panel )
    {
        if (Application.isPlaying)
        {
            UpdateContent();
        }
    }

    void OnDataChange()
    {
        mTotalItemCount = mScrollView.dataCount;
        for (int i = 0; i < mSlots.Length; i++)
        {
            mSlots[i] = -1;
        }
    }

    void OnScrollPosChange( Vector2 pos )
    {
        int cacheItemCount = mChildren.size;
        if (cacheItemCount <= 0)
        {
            return;
        }

        //将corners变换至本地坐标系
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }

        if (mMoveType == H3DScrollView.H3DScrollViewMovementType.Vertical)
        {
            mStartItemIndx = ((int) Mathf.Abs(corners[2].y)) / mItemHeight;
            mStartItemIndx = Mathf.Max(mStartItemIndx, 0);
            mStartItemIndx *= mColumnLimit;
            if( mStartItemIndx + cacheItemCount > mTotalItemCount )
            {
                mStartItemIndx = mTotalItemCount - cacheItemCount;
            } 
            mStartItemIndx = Mathf.Max(mStartItemIndx, 0);

            for( int i = mStartItemIndx ; i < mStartItemIndx + cacheItemCount ; i++ )
            {
                int slotIndx = i % cacheItemCount;
                if( i >= mTotalItemCount )
                {
                    mChildren[slotIndx].gameObject.SetActive(false);
                    continue;
                }

                tempVec3.Set(( i % mColumnLimit ) * mItemWidth  , -( i / mColumnLimit ) * mItemHeight , 0f );
                mChildren[slotIndx].localPosition = tempVec3;
            }


        }else if( mMoveType == H3DScrollView.H3DScrollViewMovementType.Horizontal )
        {
            mStartItemIndx = ((int) Mathf.Abs(corners[0].x)) / mItemWidth;
            mStartItemIndx = Mathf.Max(mStartItemIndx, 0);
            mStartItemIndx *= mColumnLimit;
            if (mStartItemIndx + cacheItemCount > mTotalItemCount)
            {
                mStartItemIndx = mTotalItemCount - cacheItemCount;
            } 
            mStartItemIndx = Mathf.Max(mStartItemIndx, 0);

            for (int i = mStartItemIndx; i < mStartItemIndx + cacheItemCount; i++)
            {
                int slotIndx = i % cacheItemCount;
                if (i >= mTotalItemCount)
                {
                    mChildren[slotIndx].gameObject.SetActive(false);
                    continue;
                }

                tempVec3.Set((i / mColumnLimit) * mItemWidth, -( i % mColumnLimit ) * mItemHeight, 0f);
                mChildren[slotIndx].localPosition = tempVec3;
            }
        }

        UpdateContent();
    }

    void ForceUpdateData()
    {
        UpdateContent(true);
    }


    void UpdateContent( bool forceUpdateData = false)
    {
        int cacheItemCount = mChildren.size;

        if( cacheItemCount <= 0 )
        {
            return;
        }

        //Vector2 childSize = mChildren[0].GetComponent<UIWidget>().localSize;
         
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }
        //corners为本地坐标
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);

        Vector2 itemsSpan = new Vector2(mItemWidth * ( cacheItemCount / mColumnLimit ) , mItemHeight * ( cacheItemCount / mColumnLimit ) ); 
        Vector2 extents = itemsSpan * 0.5f;

        
        if( mMoveType == H3DScrollView.H3DScrollViewMovementType.Vertical )
        {
            
            for( int i = 0 ; i < mChildren.size ; i++ )
            {
                Transform child = mChildren[i];  

                if (child.localPosition.y > center.y + extents.y)
                {
                    //已到底部，无需向下置换
                    if( mStartItemIndx + cacheItemCount >= mTotalItemCount )
                    {
                        continue;
                    }
                    mStartItemIndx++;

                    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y - itemsSpan.y , child.localPosition.z );
                }
                else if (child.localPosition.y < center.y - extents.y)
                {
                    //当前已是顶部，无需向上置换
                    if( mStartItemIndx <= 0 )
                    {
                        continue;
                    }
                    mStartItemIndx--;

                    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y + itemsSpan.y, child.localPosition.z); 
                }  
            }

            FillData(forceUpdateData);

        }else if( mMoveType == H3DScrollView.H3DScrollViewMovementType.Horizontal )
        {
            for (int i = 0; i < mChildren.size; i++)
            {
                Transform child = mChildren[i];

                if (child.localPosition.x < center.x - extents.x )
                {
                    //已到底部，无需向下置换
                    if (mStartItemIndx + cacheItemCount >= mTotalItemCount)
                    {
                        continue;
                    }
                    mStartItemIndx++;

                    child.localPosition = new Vector3(child.localPosition.x + itemsSpan.x , child.localPosition.y , child.localPosition.z);
                }
                else if (child.localPosition.x > center.x + extents.x)
                {
                    //当前已是顶部，无需向上置换
                    if (mStartItemIndx <= 0)
                    {
                        continue;
                    }
                    mStartItemIndx--;

                    child.localPosition = new Vector3(child.localPosition.x - itemsSpan.x , child.localPosition.y , child.localPosition.z);
                }
            }

            FillData(forceUpdateData);
        }


        //后续需要想如何降低此调用频率
        mPanel.SetDirty();
    }

 
    public void FillData( bool force )
    {
        int cacheItemCount = mChildren.size;

        if (cacheItemCount <= 0)
        {
            return;
        }

        ClearSlotFlags();

        int lastItemIndx = mTotalItemCount - 1;
        if (mTotalItemCount - mStartItemIndx > cacheItemCount)
        {
            lastItemIndx = mStartItemIndx + cacheItemCount - 1;
        }

        for (int i = mStartItemIndx; i <= lastItemIndx; i++)
        {
            int slotIndx = i % cacheItemCount;
            if (i != mSlots[slotIndx] || force )
            {
                mSlotFlags[slotIndx] = 1;
                mSlots[slotIndx] = i;
            }
        }
                

        for (int i = 0; i < mSlots.Length; i++)
        {
            if (mSlotFlags[i] == 1)
            { 

                if (!mChildren[i].gameObject.activeInHierarchy)
                {
                    mChildren[i].gameObject.SetActive(true);
                }

                H3DScrollViewItem scrollViewItem = mChildren[i].GetComponent<H3DScrollViewItem>();
                if (scrollViewItem != null)
                {
                    object itemData = null;
                    if (mScrollView.TryGetItemData(mSlots[i], out itemData))
                    {
                        scrollViewItem.SetItemData(itemData);
                    }
                    else
                    {
                        scrollViewItem.SetItemData(null);
                    }
                }
            }
        }  

    }


    [ContextMenu("排列布局")] 
    public void RecalcLayout()
    {
        Init();
        if( mChildren.size > 0 )
        {
            int widgetWidth=0;
            int widgetHeight=0;
            if (mChildren[0].GetComponent<UIWidget>() != null)
            {
                widgetWidth = mChildren[0].GetComponent<UIWidget>().width;
                widgetHeight = mChildren[0].GetComponent<UIWidget>().height;

            }
            else if (mChildren[0].GetComponent<BoxCollider>() != null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("由于子物体上没有 UIWidget 属性，所以获得此物体的BoxCollider，建议添加 UIWidget 属性");
#endif
                widgetWidth = (int)mChildren[0].GetComponent<BoxCollider>().size.x;
                widgetHeight = (int)mChildren[0].GetComponent<BoxCollider>().size.y;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("由于子物体上没有 UIWidget 属性，也没有BoxCollider，建议添加 UIWidget 属性或者必须添加BoxCollider属性");
#endif
            }
            Vector2 clipSoftness = mPanel.clipSoftness;
            clipSoftness *= 1.5f;

            int i = 0;
            foreach (var c in mChildren)
            { 
                switch (mMoveType)
                {
                    case H3DScrollView.H3DScrollViewMovementType.Vertical:
                        c.localPosition = new Vector3( ( i % mColumnLimit ) * mItemWidth  , -( i / mColumnLimit ) * mItemHeight , 0f);        
                        break;
                    case H3DScrollView.H3DScrollViewMovementType.Horizontal:
                        c.localPosition = new Vector3( ( i / mColumnLimit ) * mItemWidth ,  -( i % mColumnLimit ) * mItemHeight , 0f);
                        break;
                    default:
                        break;
                }
                i++; 
            }


            //将剪裁区域变换至Panel本地空间
            Vector3[] corners = mPanel.worldCorners;
            for (int j = 0; j < 4; ++j)
            {
                Vector3 v = corners[j];
                v = mPanel.transform.InverseTransformPoint(v);
                corners[j] = v;
            }

            switch (mMoveType)
            {
                case H3DScrollView.H3DScrollViewMovementType.Vertical:
                    mTrans.localPosition = new Vector3(mTrans.localPosition.x, corners[2].y - widgetHeight / 2f - clipSoftness.y, 0f);
                    break;
                case H3DScrollView.H3DScrollViewMovementType.Horizontal:
                    mTrans.localPosition = new Vector3(corners[0].x + widgetWidth / 2f + clipSoftness.x , mTrans.localPosition.y, 0f);
                    break;
                default:
                    break;
            } 
        }  
    }

    //获取所有WrapContent所需对象
    void Init()
    {
        mTrans = gameObject.transform;
        mPanel = NGUITools.FindInParents<UIPanel>(mTrans);
        mScrollView = mPanel.gameObject.GetComponent<H3DScrollView>();

        mPanel.onClipMove = OnPanelMove;

        mStartItemIndx = 0;
        mMoveType = mScrollView.movementType;
        mTotalItemCount = 0;
        mTotalItemCount = 0;
        mColumnLimit = mScrollView.columnLimit;
        //mScrollPos = Vector2.zero;
        mItemWidth = mScrollView.itemWidth;
        mItemHeight = mScrollView.itemHeight;

        //mPanelStartPos = mPanel.transform.localPosition;

        //为子项目绑定H3DDragScrollView脚本
        mChildren.Clear();
        Transform[] widgets = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            widgets[i] = transform.GetChild(i);
        }
        //UIWidget[] widgets = gameObject.GetComponentsInChildren<UIWidget>();

        foreach(Transform w in widgets)
        {
            if (w.parent != mTrans)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                if (w.gameObject.GetComponent<H3DDragScrollView>() == null)
                {
                    w.gameObject.AddComponent<H3DDragScrollView>();
                }
            }
            mChildren.Add(w);          
           
        }

        mSlots = new int[mChildren.size]; 
        for( int i = 0 ; i < mSlots.Length ; i++ )
        {
            mSlots[i] = -1;
        }

        mSlotFlags = new int[mChildren.size];
        for( int i = 0 ; i < mSlotFlags.Length ; i++ )
        {
            mSlotFlags[i] = 0;
        }

    }

  

    void ClearSlotFlags()
    {
        for( int i = 0 ; i < mSlotFlags.Length ; i++ )
        {
            mSlotFlags[i] = 0;
        }
    }
}

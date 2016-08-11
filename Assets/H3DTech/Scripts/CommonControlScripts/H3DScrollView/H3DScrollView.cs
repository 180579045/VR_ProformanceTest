using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
public interface IH3DScrollViewDataSource
{
    //获得显示数据总数（即使数据没有载入）
    int GetItemDataCount();

    //尝试获得第i个数据（索引从0开始）
    //此函数会在条目进入Warmup区域时被调用，
    //直到实际返回数据或条目离开Warmup区域为止。
    bool TryGetItemData(int i, out object data);
    //尝试释放条目数据
    //此函数会在条目离开Warmup区域时被调用，
    //此函数给实现者一个释放无用条目数据的机会。
    void ReleaseItemData(object data);
}

public interface IH3DScrollViewItem
{
    //为ScrollView条目设置数据，数据由产品具体条目控制脚本解释
    //并更新条目显示状态。当data传入null时条目需要将显示状态变成
    //默认状态
    void SetItemData(object data);
}

 
public class H3DScrollView : MonoBehaviour 
{
    public enum H3DScrollViewMovementType
    {
        None,
        Horizontal,
        Vertical,
        Unrestricted
    }
    

    public enum H3DScrollViewDragEffect
    {
        None,
        Momentum,
        MomentumAndSpring
    }

    public delegate void VoidDelegate();

    public delegate void Vector2Delegate(Vector2 pos);


    public H3DScrollViewMovementType mMovementType = H3DScrollViewMovementType.Vertical;

    public H3DScrollViewDragEffect mDragEffect = H3DScrollViewDragEffect.MomentumAndSpring;

    public int mColumnLimit = 1;

    public int mWarmUpZoneItemCount = 0;

    public float mMomentumAmount = 35f;

    public int mItemWidth = 100;

    public int mItemHeight = 100;

    public Transform mHorizonScrollBar;

    public Transform mVerticalScrollBar;
    /*
     * 数据源
     */


    [System.NonSerialized]
    IH3DScrollViewDataSource mDataSource = null;

    [System.NonSerialized]
    List<object> mDataList = new List<object>();


    /*
     * 状态数据
     */

    [System.NonSerialized]
    protected Vector2 mScrollPos = Vector2.zero;

    [System.NonSerialized]
    protected GameObject mCachedGameObject;

    [System.NonSerialized]
    protected Transform mTrans;

    [System.NonSerialized]
    protected UIPanel mPanel;

    

    [System.NonSerialized]
    protected Bounds mWidgetsAABB;

    [System.NonSerialized]
    protected bool mIsAABBDirty = true;

    [System.NonSerialized]
    protected Vector3 mLastHitPos = Vector3.zero;

    [System.NonSerialized]
    protected Plane mPlane;
        
    [System.NonSerialized]
    protected bool mPressed = false;

    //当前动量
    [System.NonSerialized]
    protected Vector3 mCurrMomentum = Vector3.zero;

    [System.NonSerialized]
    protected Vector2 mTotalItemLength = Vector2.zero;

    [System.NonSerialized]
    protected Vector3 mPanelStartLocalPos = Vector3.zero;

    [System.NonSerialized]
    protected Vector2 mPanelStartClipOffset = Vector2.zero;

    [System.NonSerialized]
    protected bool mIsDataDirty = true;

    [System.NonSerialized]
    protected bool mIsScrollPosDirty = true;

    [System.NonSerialized]
    public VoidDelegate onDataDirty;

    [System.NonSerialized]
    public VoidDelegate onForceUpdateData;

    [System.NonSerialized]
    public Vector2Delegate onScrollPosDirty;

    [System.NonSerialized]
    protected Vector2 tempVec2 = Vector2.zero;

    [System.NonSerialized]
    protected Vector3 tempVec3 = Vector3.zero;

    [System.NonSerialized]
    protected UIProgressBar mCachedHoriScrollBar;//水平scrollbar

    [System.NonSerialized]
    protected UIProgressBar mCachedVertScrollBar;//竖直scrollbar

    [System.NonSerialized]
    protected bool mIgnoreCallbacks;//是否忽略ScrollBar回调
    
    public IH3DScrollViewDataSource dataSource
    {
        get { return mDataSource; }
        set 
        { 
            mDataSource = value;
            mScrollPos = Vector2.zero;
            mIsDataDirty = true;
            mIsScrollPosDirty = true;
            //重设数据源后AABB需要更新
            mIsAABBDirty = true;
        }
    }

    //对于无需动态加载的数据，产品可以直接通过此函数
    //设置数据列表给控件
    public List<object> dataList
    {
        get { return mDataList; }
        set 
        { 
            mDataList = value;
            mScrollPos = Vector2.zero;
            mIsDataDirty = true;
            mIsScrollPosDirty = true; 
            //重设数据源后AABB需要更新
            mIsAABBDirty = true;
        }
    }

    public int dataCount
    {
        get
        { 
            if( mDataSource != null )
            {
                return mDataSource.GetItemDataCount();
            }
            return mDataList.Count;
        }
    }
     

    //设置条目暖身区域，滚动至此区域的条目会尝试加载数据
    //区域大小由count所指定的条目数决定。
    public int warmUpZoneItemCount
    {
        get { return mWarmUpZoneItemCount; }
        set { mWarmUpZoneItemCount = value; }
    }

    //设置拖拽动量
    public float momentumAmount
    {
        get { return mMomentumAmount; }
        set { mMomentumAmount = value; }
    }

    public H3DScrollViewMovementType movementType
    {
        get { return mMovementType; }
        set { mMovementType = value; }
    }

    //拖拽效果
    public H3DScrollViewDragEffect dragEffect
    {
        get { return mDragEffect; }
        set { mDragEffect = value; }
    }
        
    public int itemWidth
    {
        get { return mItemWidth; }
        set { mItemWidth = value; }
    }

    public int itemHeight
    {
        get { return mItemHeight; }
        set { mItemHeight = value; }
    }

    public Transform horizonScrollBar
    {
        get { return horizonScrollBar; }
    }

    //列数限制
    public int columnLimit
    {
        get { return mColumnLimit; }
        set { mColumnLimit = value; }
    }

    //设置滚动位置 pos中的x与y值域均为[0,1] 
    public Vector2 scrollPos
    {
        get { return mScrollPos; }
        set 
        { 
            mScrollPos = value;

            mScrollPos.x = Mathf.Max(mScrollPos.x, 0f);
            mScrollPos.x = Mathf.Min(mScrollPos.x, 1f);

            mScrollPos.y = Mathf.Max(mScrollPos.y, 0f);
            mScrollPos.y = Mathf.Min(mScrollPos.y, 1f);

            // 存在滑动条的时候设置
            if (mHorizonScrollBar != null || mVerticalScrollBar != null)      
            {
                mIsScrollPosDirty = true;
            } 
        }
    }


    //所有子Widgets的包围盒。在Panel本地坐标系下。
    protected Bounds widgetsAABB
    {
        get
        {
            if( mIsAABBDirty )
            {
                mIsAABBDirty = false;
                mWidgetsAABB = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
            }
            return mWidgetsAABB;
        }
    }

    
    protected Vector2 totalItemLength
    {
        get
        {
            int rowCount = dataCount / mColumnLimit;
            rowCount += ( dataCount - rowCount * mColumnLimit ) > 0 ? 1 : 0;
            mTotalItemLength.x = mItemWidth * rowCount;
            mTotalItemLength.y = mItemHeight * rowCount;
            return mTotalItemLength;
        }
    }

    void Awake()
    {
        mCachedGameObject = this.gameObject;
        mTrans = transform;
        mPanel = mCachedGameObject.GetComponent<UIPanel>();
        
        if( mMovementType == H3DScrollViewMovementType.Vertical )
        { 
            if( mVerticalScrollBar != null )
            {
                mCachedVertScrollBar = mVerticalScrollBar.GetComponent<UIProgressBar>();
            }

        }else if( mMovementType == H3DScrollViewMovementType.Horizontal ){

            if( mHorizonScrollBar != null )
            {
                mCachedHoriScrollBar = mHorizonScrollBar.GetComponent<UIProgressBar>();
            } 
        }
         
    }

	void Start () 
    {
        mPanelStartLocalPos = mTrans.localPosition;
        mPanelStartClipOffset = mPanel.clipOffset;
          
        //注册垂直滚动条回调
        if (mCachedVertScrollBar)
        {
            EventDelegate.Add(mCachedVertScrollBar.onChange, OnScrollBarValueChange);
        }

        //注册水平滚动条回调
        if (mCachedHoriScrollBar)
        {
            EventDelegate.Add(mCachedHoriScrollBar.onChange, OnScrollBarValueChange);
        }

        
	}
	 
	void Update () 
    {
	    if( mIsDataDirty )
        {
            mIsDataDirty = false;

            if( onDataDirty != null )
            {
                onDataDirty();
            }

            UpdateScrollBar();
        }

        if( mIsScrollPosDirty )
        {
            mIsScrollPosDirty = false;

            if( onScrollPosDirty != null )
            {  
                Vector2 itemsLen = totalItemLength;
                //Vector2 clipSoftness = mPanel.clipSoftness;

                float clipWidth = mPanel.finalClipRegion.z;
                float clipHeight = mPanel.finalClipRegion.w;

                float bottom = Mathf.Max(itemsLen.y - clipHeight, 0f);
                float right = Mathf.Max(itemsLen.x - clipWidth, 0f);

                float horizonStart = Mathf.Lerp(0f, right, mScrollPos.x);
                float verticalStart = Mathf.Lerp(0f, bottom, mScrollPos.y);

                if( movementType == H3DScrollViewMovementType.Horizontal )
                {
                    tempVec3.Set(-horizonStart, 0f, 0f);
                    tempVec2.Set(horizonStart, 0f);
                }else if( movementType == H3DScrollViewMovementType.Vertical )
                {
                    tempVec3.Set(0f, verticalStart,0f);
                    tempVec2.Set(0f, -verticalStart);
                }

                var onClipMove = mPanel.onClipMove;
                mPanel.onClipMove = null;

                mTrans.localPosition = mPanelStartLocalPos + tempVec3;
                mPanel.clipOffset = mPanelStartClipOffset + tempVec2;

                mPanel.onClipMove = onClipMove;

                onScrollPosDirty(mScrollPos);
            }
        }
	}

    void OnScrollBarValueChange()
    {
        if( !mIgnoreCallbacks )
        {
            mIgnoreCallbacks = true;

            Vector4 clip = mPanel.finalClipRegion;
            float clipViewSize = 0f;
            float contentSize = 0f;

            switch (movementType)
            {
                case H3DScrollViewMovementType.Vertical:
                    if (mCachedVertScrollBar)
                    {
                        clipViewSize = clip.w;
                        contentSize = totalItemLength.y;

                        
                        UIScrollBar scrollBar = mCachedVertScrollBar as UIScrollBar;

                        if( scrollBar )
                        {
                            scrollBar.barSize = clipViewSize / contentSize;
                        }

                        Vector2 pos = scrollPos;
                        pos.y = mCachedVertScrollBar.value;
                        scrollPos = pos; 
                    }
                    break;
                case H3DScrollViewMovementType.Horizontal:
                    if(mCachedHoriScrollBar)
                    {
                        clipViewSize = clip.z;
                        contentSize = totalItemLength.x;

                        UIScrollBar scrollBar = mCachedHoriScrollBar as UIScrollBar;

                        if (scrollBar)
                        {
                            scrollBar.barSize = clipViewSize / contentSize;
                        }

                        Vector2 pos = scrollPos;
                        pos.x = mCachedHoriScrollBar.value;
                        scrollPos = pos; 
                    }
                    break;
                default:
                    break; 
            }

            mIgnoreCallbacks = false;
        }
    }

    void UpdateScrollBar()
    {
        if( !mIgnoreCallbacks )
        {
            mIgnoreCallbacks = true;

            Vector4 clip = mPanel.finalClipRegion;
            float clipViewSize = 0f;
            float clipVisiableSize = 0f;
            float contentSize = 0f;

            float scrollBarValue = 0f;
            float scrollBarSize = 0f;

            float clipPos = 0f;

            float clipMoveRegion = 0f;

            switch( movementType )
            {
                case H3DScrollViewMovementType.Vertical:
                    clipViewSize = clip.w;
                    clipPos = -(mPanel.clipOffset.y - mPanelStartClipOffset.y);
                    contentSize = totalItemLength.y;
                    
                    clipMoveRegion = Mathf.Max(contentSize - clipViewSize,0f);

                    clipVisiableSize = Mathf.Max(clipPos > clipMoveRegion ? clipViewSize - (clipPos - clipMoveRegion) : clipViewSize + clipPos, 0f);
                    clipVisiableSize = Mathf.Min(clipVisiableSize, clipViewSize);

                    scrollBarSize = Mathf.Clamp01(clipVisiableSize / contentSize); 
                    scrollBarValue = Mathf.Clamp01(clipPos/clipMoveRegion);


                    if( mCachedVertScrollBar )
                    {
                        mCachedVertScrollBar.value = scrollBarValue;

                        UIScrollBar scrollBar = mCachedVertScrollBar as UIScrollBar;

                        if( scrollBar )
                        {
                            scrollBar.barSize = scrollBarSize;
                        }
                    }

                    else
                    {
                        Vector2 pos = scrollPos;
                        pos.y = scrollBarValue;    
                        scrollPos = pos;
                    }

                    break;
                case H3DScrollViewMovementType.Horizontal:
                    clipViewSize = clip.z;
                    clipPos = mPanel.clipOffset.x - mPanelStartClipOffset.x;
                    contentSize = totalItemLength.x;

                    clipMoveRegion = Mathf.Max(contentSize - clipViewSize,0f);

                    clipVisiableSize = Mathf.Max(clipPos > clipMoveRegion ? clipViewSize - (clipPos - clipMoveRegion) : clipViewSize + clipPos, 0f);
                    clipVisiableSize = Mathf.Min(clipVisiableSize, clipViewSize);

                    scrollBarSize = Mathf.Clamp01(clipVisiableSize / contentSize); 
                    scrollBarValue = Mathf.Clamp01(clipPos/clipMoveRegion);

                    if (mCachedHoriScrollBar)
                    {
                        mCachedHoriScrollBar.value = scrollBarValue;

                        UIScrollBar scrollBar = mCachedHoriScrollBar as UIScrollBar;

                        if (scrollBar)
                        {
                            scrollBar.barSize = scrollBarSize;
                        }
                    }

                    else
                    {
                        Vector2 pos = scrollPos;
                        pos.x = scrollBarValue;
                        scrollPos = pos;
                    }

                    break;
                default:
                    break;
            }

            mIgnoreCallbacks = false;
        }
    }
 
   
 

    void MovePanel( Vector3 localOffset )
    {
        Vector2 co = mPanel.clipOffset;
        co.x -= localOffset.x;
        co.y -= localOffset.y;
        mPanel.clipOffset = co;

        mTrans.localPosition += localOffset;

        mIsAABBDirty = true;
    }

    void LateUpdate()
    {
        if (!Application.isPlaying) return;
        float delta = RealTime.deltaTime;

        if (!mPressed)
        {
            if (mCurrMomentum.magnitude > 0.0001f)
            {
                Vector3 offset = NGUIMath.SpringDampen(ref mCurrMomentum, 9f, delta);
                MovePanel(offset); 
                UpdateScrollBar();
                RestrictWithinBounds();
                return;
            }
            else
            {
                mCurrMomentum = Vector3.zero;
                UpdateScrollBar();
            }
        }
        
        //对动量进行阻尼降低
        NGUIMath.SpringDampen(ref mCurrMomentum, 9f, delta); 
    }

    public void ForceUpdateData()
    {
        if( onForceUpdateData != null )
        {
            onForceUpdateData();
        }
    }

    public bool TryGetItemData( int i , out object data)
    {
        data = null;
        if( mDataSource != null )
        {
            return mDataSource.TryGetItemData(i, out data);
        }

        if( i >= 0 && i < mDataList.Count )
        {
            data = mDataList[i];
            return true;
        }
        return false;
    }

    public void Press( bool isDown )
    {
        if( isDown )
        {             
           mPressed = true; 
           //清当前动量为0
           mCurrMomentum = Vector3.zero;

           mLastHitPos = UICamera.lastHit.point; 
           mPlane = new Plane(mTrans.rotation * Vector3.back , mLastHitPos);           
           DisableSpring(); 
        }
        else
        {
           mPressed = false; 
           RestrictWithinBounds();  
        }
    }
    public void Drag()
    {        
        Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);                
	    float dist = 0f;
        if (mPlane.Raycast(ray, out dist))
        {
            Vector3 currHitPos = ray.GetPoint(dist); 
            Vector3 offset = currHitPos - mLastHitPos;
            Vector3 localOffset = mTrans.InverseTransformPoint(currHitPos) - mTrans.InverseTransformPoint(mLastHitPos);
            mLastHitPos = currHitPos;

            if (offset.x != 0f || offset.y != 0f || offset.z != 0f)
            {
                offset = mTrans.InverseTransformDirection(offset);

                switch (movementType)
                {
                    case H3DScrollViewMovementType.Horizontal:
                        localOffset.y = 0f;
                        localOffset.z = 0f;
                        break;
                    case H3DScrollViewMovementType.Vertical:
                        localOffset.x = 0f;
                        localOffset.z = 0f;
                        break;
                    case H3DScrollViewMovementType.Unrestricted:
                        localOffset.z = 0f;
                        break;
                    default:
                        break;
                }
                offset = mTrans.TransformDirection(offset);
            }

            //调整当前动量
            mCurrMomentum = Vector3.Lerp(mCurrMomentum, mCurrMomentum + localOffset * (0.01f * mMomentumAmount), 0.67f);
            MovePanel(localOffset);
            UpdateScrollBar();
        }
    }

    public void DisableSpring()
    {
        SpringPanel sp = GetComponent<SpringPanel>();
        if (sp != null) sp.enabled = false;
    }

    public void RestrictWithinBounds()
    { 
     
        Bounds b = widgetsAABB; 
        Vector3 constraint = mPanel.CalculateConstrainOffset(b.min, b.max);
        Vector4 clipRegion = mPanel.finalClipRegion;
        
        if( movementType == H3DScrollViewMovementType.Vertical )
        {
            if( b.size.y < clipRegion.w - mPanel.clipSoftness.y * 2f )
            {
                constraint.y = mPanelStartLocalPos.y - mTrans.localPosition.y;
            }
            constraint.x = 0f;
        }

        if( movementType == H3DScrollViewMovementType.Horizontal )
        {
            if (b.size.x < clipRegion.z - mPanel.clipSoftness.x * 2f)
            {
                constraint.x = mPanelStartLocalPos.x - mTrans.localPosition.x;
            }
            constraint.y = 0f;
        }
         

        if (constraint.sqrMagnitude > 1f)
        {
            // Spring back into place
            Vector3 pos = mTrans.localPosition + constraint;
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            SpringPanel.Begin(mPanel.gameObject, pos, 13f); 
        }
    }

     

#if UNITY_EDITOR

    public void RecalcLayout()
    {
        H3DWrapContent wrapContent = GetComponentInChildren<H3DWrapContent>();
        if( wrapContent != null )
        {
            wrapContent.RecalcLayout();
        }
    }

    [MenuItem("H3D/通用控件/通用ScrollView")]
    static void CreateH3DScrollView()
    {  
        GameObject go = new GameObject("New Scroll View"); 
        UIPanel panel = go.AddComponent<UIPanel>(); 
        panel.clipping = UIDrawCall.Clipping.SoftClip;
        go.AddComponent<H3DScrollView>(); 
        
        GameObject attachPointGo = new GameObject("Item AttachPoint");
        attachPointGo.AddComponent<H3DWrapContent>();

        if (Selection.activeGameObject != null)
        {
            go.transform.parent = Selection.activeGameObject.transform;
            go.layer = go.transform.parent.gameObject.layer;
        }
        else
            go.transform.parent = null;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        attachPointGo.transform.parent = go.transform;
        attachPointGo.transform.localPosition = Vector3.zero;
        attachPointGo.transform.localRotation = Quaternion.identity;
        attachPointGo.transform.localScale = Vector3.one;
        attachPointGo.layer = go.layer; 
    } 
#endif

  
}

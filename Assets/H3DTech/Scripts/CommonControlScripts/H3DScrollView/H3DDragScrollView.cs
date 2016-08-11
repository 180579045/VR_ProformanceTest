using UnityEngine;
using System.Collections;

public class H3DDragScrollView : MonoBehaviour 
{
     
    //所属ScrollView
    H3DScrollView mScrollView;
     
    Transform mTrans;
     

    void Awake()
    {
        mTrans = transform;
        var p = mTrans.parent;
        while( p != null )
        {
            mScrollView = p.gameObject.GetComponent<H3DScrollView>();
            if( mScrollView != null )
            {
                break;
            }
            p = p.parent;
        }
         
    }

	void Start () 
    {
	
	}
	 
	void Update () 
    { 
	}

    void OnPress( bool isDown )
    { 
        if( mScrollView )
        {
            mScrollView.Press(isDown);
        }
    }

    void OnDrag(Vector2 delta)
    {
        if (mScrollView)
        {
            mScrollView.Drag();
        }
    }

    void OnScroll(float delta)
    {
         
    }
}

using UnityEngine;
using System.Collections;

public class NikkiScrollViewShowDate : H3DScrollViewItem
{
    public UITexture mUISprite;
    public UILabel mlabel; 

    public override void SetItemData(object data)
    {

        mUISprite.mainTexture = ((NikiiDataSource)data).mNikiDateTex;
        mlabel.text = ((NikiiDataSource)data).mNikiDateNum.ToString();
    }
}

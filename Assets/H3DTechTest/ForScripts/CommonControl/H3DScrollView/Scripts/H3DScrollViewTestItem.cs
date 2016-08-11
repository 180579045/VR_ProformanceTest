using UnityEngine;
using System.Collections;

public class H3DScrollViewTestItem : H3DScrollViewItem {

    public UILabel label;


    public override void SetItemData(object data)
    {
        int i = (int)data;
        label.text = i.ToString();
    }

}

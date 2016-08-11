using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListCtrlItem
{
    //列表项名
    public string name;
    //add by liteng for atlas
    public string tooltip;
    //列表项图标
    public Texture image;
    //项中的用户数据
    public object userObj;
    //正常情况下颜色显示
    public Color color = Color.green;
    //列表项被选择时的颜色
    public Color onSelectColor = Color.blue; 
	//Add by liteng for MoveAtlas At 2014/1/4
    public Color onSelectTexColor = new Color(0.5f, 0.5f, 0.8f);
    //最近一次绘制矩形区域
    public Rect lastRect = new Rect(); 
}

public class ListViewCtrl : EditorControl 
{
    public ListViewCtrl()
    {
        this.Size = new Rect(0, 0, 0, 0);
        this.DragStartType = "CustomListDrag";
        this.DragAcceptType = "CustomListDrag";
    }

    public List<ListCtrlItem> Items
    {
        get { return items; }
    }

    public int LastSelectItem
    {
        get { return lastSelectItem; }
        set 
        { 
            lastSelectItem = value;
            if (lastSelectItem >= items.Count)
                lastSelectItem = items.Count - 1; 
        }
    }
	//Add by liteng for MoveAtlas At 2014/1/4 Start
    public List<int> SelectItems
    {
        get { return m_SelectItems; }
        set { m_SelectItems = value; }
    }

    public bool IsTextureView
    {
        get { return m_bIsTextureView; }
        set { m_bIsTextureView = value; }
    }

    public int TextureSizeLevel
    {
        get { return m_TextureSizeLevel; }
        set { m_TextureSizeLevel = value; }
    }
	//Add by liteng for MoveAtlas End

    public override GUIStyle GetStyle()
    {
        return SpecialEffectEditorStyle.PanelBox;
    }

    public override GUILayoutOption[] GetOptions()
    {
        if (
               (0 == Size.width)
            || (0 == Size.height)
            )
        {
            return new GUILayoutOption[] {   
            GUILayout.ExpandHeight(true), 
            GUILayout.ExpandWidth(true) };

        }
        else
        {
            return new GUILayoutOption[] {   
            GUILayout.Width(Size.width), GUILayout.Height(Size.height),
            GUILayout.ExpandHeight(true), 
            GUILayout.ExpandWidth(true) };
        }
    }

    public void AddItem(ListCtrlItem item)
    {
        items.Add(item);
        lastSelectItem = -1;
        //Add by liteng for MoveAtlas At 2014/1/4
        m_SelectItems.Clear();
    }

    public ListCtrlItem GetItemAt(int i)
    {
        return items[i];
    }

    public int IndexOfItem(ListCtrlItem item)
    {
        return items.IndexOf(item);
    }

    public void RemoveItem(ListCtrlItem item)
    {
        items.Remove(item);
        lastSelectItem = -1;
        //Add by liteng for MoveAtlas At 2014/1/4
        m_SelectItems.Clear();  
    }

    public void ClearItems()
    {
        items.Clear();
        lastSelectItem = -1;
        //Add by liteng for MoveAtlas At 2014/1/4
        m_SelectItems.Clear();
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    //Add by liteng for MoveAtlas At 2014/1/5 Start
    public void ClearSelectItems()
    {
        if(null == m_SelectItems)
        {
            return;
        }

        m_SelectItems.Clear();
        lastSelectItem = -1;
    }
    //Add by liteng for MoveAtlas End

    //private Vector2 scrollPos = new Vector2(0,0); 
    private List<ListCtrlItem> items = new List<ListCtrlItem>();
    private int lastSelectItem = -1;

    //Add by liteng for MoveAtlas At 2014/1/5 Start
    private List<int> m_SelectItems = new List<int>();
    private bool m_bIsTextureView = false;
    private int m_TextureSizeLevel = 1;
    //Add by liteng for MoveAtlas End

}

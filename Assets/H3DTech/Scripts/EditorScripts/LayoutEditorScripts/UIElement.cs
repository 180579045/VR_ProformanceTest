using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum EUIOpFlag
{
    UOF_Invalid = 0,
    UOF_Shape = 1 << 0,
    UOF_Attribute = 1 << 1,
    UOP_Lock = 1 << 2,
    //UOF_WidthUpdated = 1 << 1,
    //UOF_HeightUpdated = 1 << 2,
    //UOF_Scale = 1 << 1,
}
public class UIElement : MonoBehaviour
{
    [SerializeField]
    bool _Hide;
    [SerializeField]
    bool _Removed;
    [SerializeField]
    bool _Freeze;
    [SerializeField]
    public bool IsLock;

    UIWidget.Pivot m_pivot = UIWidget.Pivot.BottomLeft;
    Vector3[] m_Corners = new Vector3[4];

    public string FullPathName;
    public int HierarchyPos = 0;

    [System.Serializable]
    public struct SubPrefabPair
    {
        public string sGameObjPath;
        public string sPrefabPath;
    }
    public List<SubPrefabPair> SubPrefabInfos = new List<SubPrefabPair>();
    
    private UIElement m_rootUI = null;
    private int m_OpFlag = 0;
    private GameObject m_SubPrefabParent = null;

    public delegate void Func_SyncUITreeLockForSubPrefab(List<UIElement> vUI);
    public Func_SyncUITreeLockForSubPrefab funcSyncUITreeLockForSubPrefab = null;

    public bool Freeze
    {
        set
        {
            _Freeze = value;
        }
        get
        {
            return GetParentUIFreeze(this) ? true : _Freeze;
        }
    }

    public bool Lock
    {
        get
        {
            return IsLock;
        }
        set
        {
            if (CheckFlag(EUIOpFlag.UOP_Lock))
                return;

            IsLock = value;

            AddOpFlag(EUIOpFlag.UOP_Lock);
            SyncPrefabObjLock(IsLock);
        }
    }
    public string Name
    {
        get { return gameObject.name; }
        set { gameObject.name = value; }
    }
    public bool Hide
    {
        get { return _Hide; }
        set
        {
            _Hide = value;
            gameObject.SetActive(Visible);
        }
    }
    public bool Removed
    {
        get { return _Removed; }
        set
        {
            _Removed = value;
            gameObject.SetActive(Visible);
        }
    }

    public bool CanEdit
    {
        get
        {
            if(GetComponent<BoxCollider>() != null)
                return true;

            return IsUI;
        }
    }
    public bool IsUI
    {
        get { return (IsLock ? !CanRemove() : GetWidget() != null); }
    }
    public int ChildrenCount
    {
        get
        {
            return transform.childCount;
        }
    }

    // 为了实现锁定控件，对UIWidget接口的包装
    public UIWidget.Pivot pivot
    {
        get
        {
            if (IsLock)
            {
                return m_pivot;
            }
            else
            {
                UIWidget w = GetWidget();

                return (w != null ? w.pivot : UIWidget.Pivot.BottomLeft);
            }
        }
        set
        {
            if (IsLock)
            {
                m_pivot = value;
            }
            else
            {
                UIWidget w = GetWidget();

                if (w != null)
                {
                    w.pivot = value;
                }
            }
        }
    }
    public float width
    {
        get
        {
            if (IsLock)
            {
                Vector3[] world_Corners = worldCorners;

                return world_Corners[2].x - world_Corners[0].x;
            }
            else
            {
                UIWidget w = GetWidget();

                return (w != null ? w.width : 0);
            }
        }
        set
        {
            if (IsLock)
            {
                //
            }
            else
            {
                UIWidget w = GetWidget();

                if (w != null)
                {
                    if (value < w.minWidth)
                    {
                        value = w.minWidth;
                    }
                    w.width = (int)(value + 0.5f);
                }
            }
        }
    }
    public float height
    {
        get
        {
            if (IsLock)
            {
                Vector3[] world_Corners = worldCorners;

                return world_Corners[2].y - world_Corners[0].y;
            }
            else
            {
                UIWidget w = GetWidget();

                return (w != null ? w.height : 0);
            }
        }
        set
        {
            if (IsLock)
            {
                //
            }
            else
            {
                UIWidget w = GetWidget();

                if (w != null)
                {
                    if (value < w.minHeight)
                    {
                        value = w.minHeight;
                    }
                    w.height = (int)(value + 0.5f);
                }
            }
        }
    }
    public Vector3[] worldCorners
    {
        get
        {
            if (IsLock)
            {
                if (m_Corners == null)
                {
                    m_Corners = new Vector3[4];
                    m_Corners[0] = new Vector3();
                    m_Corners[1] = new Vector3();
                    m_Corners[2] = new Vector3();
                    m_Corners[3] = new Vector3();
                }

                float x0 = float.MaxValue;
                float y0 = float.MaxValue;
                float x1 = float.MinValue;
                float y1 = float.MinValue;
                UIWidget[] widgets = GetComponentsInChildren<UIWidget>(true);

                for (int i = 0; i < widgets.Length; ++i)
                {
                    ExpandRect(widgets[i].worldCorners, ref x0, ref y0, ref x1, ref y1);
                }

                m_Corners[0].x = x0;
                m_Corners[0].y = y0;
                m_Corners[1].x = x0;
                m_Corners[1].y = y1;
                m_Corners[2].x = x1;
                m_Corners[2].y = y1;
                m_Corners[3].x = x1;
                m_Corners[3].y = y0;

                return m_Corners;
            }
            else
            {
                UIWidget w = GetWidget();
                if(w != null)
                {
                    return w.worldCorners;
                }
                else
                {
                    BoxCollider box = GetComponent<BoxCollider>();
                    if(box != null)
                    {
                        Vector3[] corner = new Vector3[4];
                        Vector3 pos = this.gameObject.transform.position;
                        pos += box.center;

                        corner[0] = pos - box.size/2;
                        corner[2] = pos + box.size/2;
                        corner[1] = pos;
                        corner[1].x -= box.size.x/2;
                        corner[1].y += box.size.y/2;
                        corner[3] = pos;
                        corner[3].x += box.size.x/2;
                        corner[3].y -= box.size.y/2;
                        return corner;
                        
                    }
                    else
                    {
                        return m_Corners;
                    }
                }
            }
        }
    }
    public Vector3 Pos
    {
        get
        {
            if (IsLock)
            {
                Vector3[] world_Corners = worldCorners;
                float x = 0;
                float y = 0;

                switch (pivot)
                {
                    case UIWidget.Pivot.TopLeft:
                    case UIWidget.Pivot.Left:
                    case UIWidget.Pivot.BottomLeft:
                        x = world_Corners[0].x;
                        break;
                    case UIWidget.Pivot.Top:
                    case UIWidget.Pivot.Center:
                    case UIWidget.Pivot.Bottom:
                        x = (world_Corners[0].x + world_Corners[2].x) / 2;
                        break;
                    case UIWidget.Pivot.TopRight:
                    case UIWidget.Pivot.Right:
                    case UIWidget.Pivot.BottomRight:
                        x = world_Corners[2].x;
                        break;
                }
                switch (pivot)
                {
                    case UIWidget.Pivot.BottomLeft:
                    case UIWidget.Pivot.Bottom:
                    case UIWidget.Pivot.BottomRight:
                        y = world_Corners[0].y;
                        break;
                    case UIWidget.Pivot.Left:
                    case UIWidget.Pivot.Center:
                    case UIWidget.Pivot.Right:
                        y = (world_Corners[0].y + world_Corners[2].y) / 2;
                        break;
                    case UIWidget.Pivot.TopLeft:
                    case UIWidget.Pivot.Top:
                    case UIWidget.Pivot.TopRight:
                        y = world_Corners[2].y;
                        break;
                }

                return new Vector3(x, y);
            }
            else
            {
                return transform.position;
            }
        }
        set
        {
            if (IsLock)
            {
                Vector3 move_delta = value - Pos;
                transform.position += move_delta;
            }
            else
            {
                transform.position = value;
            }
        }
    }
    public Vector3 LocalPos
    {
        get
        {
            if (IsLock)
            {
                Transform parent = transform.parent;

                if (parent != null)
                {
                    return Pos - parent.position;
                }
                return Pos;
            }
            else
            {
                return transform.localPosition;
            }
        }
        set
        {
            if (IsLock)
            {
                Vector3 move_delta = value - LocalPos;
                transform.localPosition += move_delta;
            }
            else
            {
                transform.localPosition = value;
            }
        }
    }

    //public Vector3 BottomLeftPos
    //{
    //    get
    //    {
    //        UIWidget w = GetComponent<UIWidget>();
    //        if(w == null)
    //        {
    //            return transform.position;
    //        }
    //        else
    //        {
    //            Vector3 vPos = transform.position;
    //            int width = w.width;
    //            int height = w.height;
    //            switch(w.pivot)
    //            {
    //                case UIWidget.Pivot.Left:
    //                case UIWidget.Pivot.BottomLeft:
    //                case UIWidget.Pivot.TopLeft:
    //                    break;
    //                case UIWidget.Pivot.Center:
    //                case UIWidget.Pivot.Bottom:
    //                case UIWidget.Pivot.Top:
    //                    vPos.x = vPos.x - (float)width/2;
    //                    break;
    //                case UIWidget.Pivot.Right:
    //                case UIWidget.Pivot.BottomRight:
    //                case UIWidget.Pivot.TopRight:
    //                    vPos.x = vPos.x - (float)width;
    //                    break;
    //            }

    //            switch(w.pivot)
    //            {
    //                case UIWidget.Pivot.Left:
    //                case UIWidget.Pivot.Center:
    //                case UIWidget.Pivot.Right:
    //                    vPos.y = vPos.y - (float)height / 2;
    //                    break;
    //                case UIWidget.Pivot.TopLeft:
    //                case UIWidget.Pivot.Top:
    //                case UIWidget.Pivot.TopRight:
    //                    vPos.y = vPos.y - (float)height;
    //                    break;
    //            }
    //            return vPos;
    //        }
    //    }
    //}

#region prefabstuff
    public void SetRootUIElement(UIElement root)
    {
        m_rootUI = root;
    }

    public void AddOpFlag(EUIOpFlag eFlag)
    {
        m_OpFlag |= (int)eFlag;
    }
    public void RemoveFlag(EUIOpFlag eFlag)
    {
        m_OpFlag &= ~(int)eFlag;
    }
    public bool CheckFlag(EUIOpFlag eFlag)
    {
        return (m_OpFlag & (int)eFlag) != 0;
    }
    public void ClearOpFlag()
    {
        m_OpFlag = 0;
    }
    public void TryFindSubPrefabParent()
    {
        m_SubPrefabParent = null;

        Transform tfP = transform.parent;
        while (tfP != null)
        {
            if (tfP.GetComponent<UIPrefabNode>() != null)
            {
                if(tfP.childCount == 1)
                {
                    GameObject pg = tfP.GetChild(0).gameObject;
                    //if(pg == this.gameObject)
                    //    return;

                    m_SubPrefabParent = pg;
                }
                else
                {
                    Debug.Log("Logic Error: UIPrefabNode has more than one child");
                }
                return;
            }
            tfP = tfP.parent;
        }
    }

    public GameObject FindObject(string sObjPath)
    {
        string path = sObjPath.Trim().Replace('\\', '/');
        path = path.Substring(path.IndexOf('/') + 1);
        string[] paths = path.Split('/');
        Transform res_trans = m_rootUI.transform;

        for (int i = 0; i < paths.Length; ++i)
        {
            res_trans = res_trans.FindChild(paths[i]);
            if (res_trans == null)
            {
                Debug.Log("Logic Error: Not Find Object in Currnet Layout: " + sObjPath);
                return null;
            }
        }

        return res_trans.gameObject;
    }

    public string GetRelPathToSubPrefab()
    {
        if (m_SubPrefabParent == null)
            return "";

        string path = FullPathName;
        string SPPPath = m_SubPrefabParent.GetComponent<UIElement>().FullPathName;
        string s = path.Substring(SPPPath.Length);
        return s;
    }
    public static GameObject[] GetSubChildren(GameObject go)
    {
        GameObject[] children = new GameObject[go.transform.childCount];

        for (int i = 0; i < go.transform.childCount; ++i)
        {
            children[i] = go.transform.GetChild(i).gameObject;
        }
        return children;
    }
    public static string GetGameObjectFullPathName(GameObject obj, GameObject root)
    {
        if (obj == null || root == null)
            return "";

        string sName = obj.name;
        Transform parentTrans = obj.transform.parent;
        while (parentTrans != null)
        {
            sName = parentTrans.gameObject.name + "/" + sName;
            if (parentTrans == root.transform)
                break;
            parentTrans = parentTrans.parent;
        }

        //int index = sName.IndexOf('/');
        //if(index > 0)
        //{
        //    string sFirst = sName.Substring(0, index);
        //    string sLeft = sName.Substring(index);
        //    if (sFirst != "UIRootTempPanel")
        //    {
        //        sFirst = sFirst + "(Clone)";
        //        sName = sFirst + sLeft;
        //    }
        //}
        return sName;
    }

    public List<string> FindAllObjPathWithSamePrefab(string sObjPath)
    {
        List<string> allObjPath = new List<string>();
        string sPrefab = "";
        foreach (UIElement.SubPrefabPair pp in SubPrefabInfos)
        {
            if (pp.sGameObjPath == sObjPath)
            {
                sPrefab = pp.sPrefabPath;
                break;
            }
        }

        if (sPrefab.Length > 0)
        {
            foreach (UIElement.SubPrefabPair pp in SubPrefabInfos)
            {
                if (sPrefab == pp.sPrefabPath && pp.sGameObjPath != sObjPath)
                {
                    allObjPath.Add(pp.sGameObjPath);
                }
            }
        }

        return allObjPath;
    }
#endregion

    //public delegate void UIOpFunc(UIElement ui, Vector3 param);

    public void Move(Vector3 move_delta)
    {
        if (!IsLock)
        {
            if(IsEditBoxCollider())
            {
                BoxCollider box = GetComponent<BoxCollider>();
                if (box != null)
                {
                    box.center += move_delta;
                    return;
                }
            }
        }
        
        transform.position += GetRealMoveDelta(GetParentUI(), this, move_delta);
    }

    public void SyncPrefabObjLock(bool bLock)//这里有UIPrefabNode脚本的同类prefab也要同步lock
    {
        List<UIElement> vUISync = new List<UIElement>();

        if(m_SubPrefabParent == null)
        {
            UIPrefabNode uipn = GetComponent<UIPrefabNode>();
            if (uipn == null)
                return;
            if(transform.childCount == 1)
            {
                GameObject childObj = transform.GetChild(0).gameObject;
                string sFullPath = GetGameObjectFullPathName(childObj, m_rootUI.gameObject);
                List<string> sAllObj = m_rootUI.FindAllObjPathWithSamePrefab(sFullPath);
                foreach (string s in sAllObj)
                {
                    string sObj = s;
                    int n = s.LastIndexOf('/');
                    if(n >= 0)
                        sObj = s.Substring(0, n);

                    GameObject obj = FindObject(sObj);
                    if (obj != null)
                    {
                        UIElement ui = obj.GetComponent<UIElement>();
                        ui.Lock = bLock;
                        vUISync.Add(ui);
                    }
                }
            }
            else
            {
                Debug.Log("Logic Error: UIPrefabNode has more than one child");
            }
        }
        else
        {
            if (m_rootUI == null)
            {
                Debug.Log("Logic Error: rootUI is empty");
                return;
            }
            string sFullPath = GetGameObjectFullPathName(m_SubPrefabParent, m_rootUI.gameObject);
            List<string> sAllObj = m_rootUI.FindAllObjPathWithSamePrefab(sFullPath);
            string sRelPathToSubPrefab = GetRelPathToSubPrefab();
            foreach (string s in sAllObj)
            {
                string sObj = s + sRelPathToSubPrefab;
                GameObject obj = FindObject(sObj);
                if (obj != null)
                {
                    UIElement ui = obj.GetComponent<UIElement>();
                    ui.Lock = bLock;
                    vUISync.Add(ui);
                }
            }
        }

        if(vUISync.Count > 0)
        {
            if (funcSyncUITreeLockForSubPrefab != null)
                funcSyncUITreeLockForSubPrefab(vUISync);
        }
    }

    //Sync Input Operation
    public void SyncPrefabUI(int eOpFlag)
    {
        if (CheckFlag((EUIOpFlag)eOpFlag))
            return;

        if (m_SubPrefabParent == null)
            return;

        if (m_rootUI == null)
        {
            Debug.Log("Logic Error: rootUI is empty");
            return;
        }

        bool bSubPPThis = false;
        if (m_SubPrefabParent == gameObject)
            bSubPPThis = true;

        string sFullPath = GetGameObjectFullPathName(m_SubPrefabParent, m_rootUI.gameObject);
        List<string> sAllObj = m_rootUI.FindAllObjPathWithSamePrefab(sFullPath);
        string sRelPathToSubPrefab = GetRelPathToSubPrefab();
        foreach (string s in sAllObj)
        {
            string sObj = s + sRelPathToSubPrefab;
            GameObject obj = FindObject(sObj);
            if (obj != null)
            {
                UIElement ui = obj.GetComponent<UIElement>();

                if ((eOpFlag & (int)EUIOpFlag.UOF_Shape) != 0)
                    ui.UIOpSync_Shape(this, bSubPPThis);
                
                if ((eOpFlag & (int)EUIOpFlag.UOF_Attribute) != 0)
                    ui.UIOpSync_Attribute(this, bSubPPThis);
            }
        }
    }
    public void UIOpSync_Shape(UIElement ui, bool bSubPP)
    {
        if (CheckFlag(EUIOpFlag.UOF_Shape))
            return;

        AddOpFlag(EUIOpFlag.UOF_Shape);

        Vector3 pos = Vector3.zero;
        if (bSubPP)
            pos = transform.localPosition;

        transform.localPosition = ui.transform.localPosition;

        UIWidget w = GetComponent<UIWidget>();
        if(w != null)
        {
            UIWidget w_ui = ui.GetComponent<UIWidget>();
            if(w_ui != null)
            {
                w.width = w_ui.width;
                w.height = w_ui.height;
            }
        }

        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            BoxCollider box_ui = ui.GetComponent<BoxCollider>();
            if(box_ui != null)
            {
                box.center = box_ui.center;
                box.size = box_ui.size;
            }
        }

        if (bSubPP)
            transform.localPosition = pos;
    }

    public void UIOpSync_Attribute(UIElement ui, bool bSubPP)
    {
        if (CheckFlag(EUIOpFlag.UOF_Attribute))
            return;

        AddOpFlag(EUIOpFlag.UOF_Attribute);

        if (!bSubPP)
        {
            transform.localPosition = ui.transform.localPosition;
        }

        UIWidget w = GetComponent<UIWidget>();
        UIWidget w_ui = ui.GetComponent<UIWidget>();
        if (w == null || w_ui == null)
            return;

        if((w is UISprite) && (w_ui is UISprite))
        {
            UIOpSync_Attribute_Common(w, w_ui);
            UIOpSync_Attribute_Sprite((w as UISprite), (w_ui as UISprite));
        }
        else if ((w is UILabel) && (w_ui is UILabel))
        {
            UIOpSync_Attribute_Common(w, w_ui);
            UIOpSync_Attribute_Label((w as UILabel), (w_ui as UILabel));
        }
        else if ((w is UITexture) && (w_ui is UITexture))
        {
            UIOpSync_Attribute_Common(w, w_ui);
            UIOpSync_Attribute_Texture((w as UITexture), (w_ui as UITexture));
        }
        else
        {
            Debug.Log("UI Style Not Matched");
        }
    }
    private static void UIOpSync_Attribute_Common(UIWidget ui, UIWidget uiFrom)
    {
        ui.depth = uiFrom.depth;
        ui.width = uiFrom.width;
        ui.height = uiFrom.height;
    }
    private static void UIOpSync_Attribute_Sprite(UISprite ui, UISprite uiFrom)
    {
        ui.atlas        = uiFrom.atlas;
        ui.spriteName   = uiFrom.spriteName;
        ui.type         = uiFrom.type;
        ui.MarkAsChanged();
    }
    private static void UIOpSync_Attribute_Label(UILabel ui, UILabel uiFrom)
    {
        ui.text = uiFrom.text;
        ui.effectStyle = uiFrom.effectStyle;
        ui.effectDistance = uiFrom.effectDistance;
        ui.maxLineCount = uiFrom.maxLineCount;
        ui.trueTypeFont = uiFrom.trueTypeFont;
        ui.fontSize = uiFrom.fontSize;
        ui.fontStyle = uiFrom.fontStyle;
        ui.effectColor = uiFrom.effectColor;
        ui.color = uiFrom.color;
    }
    private static void UIOpSync_Attribute_Texture(UITexture ui, UITexture uiFrom)
    {
        //ui.mainTexture = uiFrom.mainTexture;
        //ui.material = uiFrom.material;
    }


    public bool SetWidthDelta(int scale, float delta)
    {
        bool res = false;
        if (IsLock)
        {
        }
        else
        {
            UIWidget w = GetWidget();
            if (w != null)
            {
                UIWidget.Pivot lastPivot = w.pivot;
                w.pivot = UIWidget.Pivot.Left;
                
                int new_width = GetRealWidth(GetParentUI(), w, scale, delta);
                if (new_width != w.width)
                {
                    res = true;
                    if (scale < 0)
                    {
                        w.pivot = UIWidget.Pivot.Right;
                        //Move(new Vector3(w.width - new_width, 0));
                    }
                    //NGUIMath.AdjustWidget(w, 0f, 0f, new_width - w.width, 0f);
                    w.width = new_width;
                    //Vector3 vBL = w.worldCorners[0];
                    //Vector3 vPos = w.transform.localPosition;
                }
                w.pivot = lastPivot;
            }
            else
            {
                BoxCollider box = GetComponent<BoxCollider>();
                if(box != null)
                {
                    float fMove = delta;
                    if (scale != 0 && Mathf.Abs(fMove) > float.Epsilon)
                    {
                        Vector3 center = box.center;
                        center.x += fMove / 2;
                        box.center = center;

                        Vector3 size = box.size;
                        if(scale < 0)
                        {
                            size.x -= fMove;
                        }
                        else
                        {
                            size.x += fMove;
                        }
                        box.size = size;

                        res = true;
                    }
                }
            }
        }
        return res;
    }
    public bool SetHeightDelta(int scale, float delta)
    {
        bool res = false;
        if (IsLock)
        {
        }
        else
        {
            UIWidget w = GetWidget();
            if (w != null)
            {
                UIWidget.Pivot lastPivot = w.pivot;
                w.pivot = UIWidget.Pivot.Bottom;

                int new_height = GetRealHeight(GetParentUI(), w, scale, delta);
                if (new_height != w.height)
                {
                    res = true;
                    if (scale < 0)
                    {
                        w.pivot = UIWidget.Pivot.Top;
                        //Move(new Vector3(0, w.height - new_height));
                    }
                    w.height = new_height;
                }

                w.pivot = lastPivot;
            }
            else
            {
                BoxCollider box = GetComponent<BoxCollider>();
                if (box != null)
                {
                    float fMove = delta;
                    if (scale != 0 && Mathf.Abs(fMove) > float.Epsilon)
                    {
                        Vector3 center = box.center;
                        center.y += fMove / 2;
                        box.center = center;

                        Vector3 size = box.size;
                        if (scale < 0)
                        {
                            size.y -= fMove;
                        }
                        else
                        {
                            size.y += fMove;
                        }
                        box.size = size;

                        res = true;
                    }
                }
            }
        }
        return res;
    }
    //

    // 节点身上的UI元素（UIWidget），可能返回空
    public UIWidget GetWidget()
    {
        return (IsLock ? null : GetComponent<UIWidget>());
    }
    public UIElement GetChild(int index)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            return null;
        }
        return transform.GetChild(index).GetComponent<UIElement>();
    }

    // 增加子节点, 返回值为新增节点。锁定的节点不能增加子节点
    public UIElement AddChildNode()
    {
        if (IsLock)
        {
            return null;
        }

        GameObject new_go = new GameObject();
        new_go.transform.parent = transform;

        return new_go.AddComponent<UIElement>();
    }
    public bool SetParent(UIElement ui_element)
    {
        if (ui_element == null || ui_element.IsLock)
        {
            return false;
        }
        transform.parent = ui_element.transform;
        return true;
    }
    public bool RemoveNode()
    {
        if (!CanRemove())
        {
            return false;
        }
        Object.DestroyImmediate(gameObject);

        return true;
    }
    // 是否可以删除该节点，含有UI元素（UIWidget）的节点不能删除
    public bool CanRemove()
    {
        return (GetComponentsInChildren<UIWidget>(true).Length == 0);
    }

    public bool VisbleGolbal
    {
        get { return Visible && gameObject.activeInHierarchy; }
    }

    bool Visible
    {
        get { return (!_Hide && !_Removed); }
    }

    void ExpandRect(Vector3[] corners, ref float x0, ref float y0, ref float x1, ref float y1)
    {
        if (corners[0].x < x0)
        {
            x0 = corners[0].x;
        }
        if (corners[0].y < y0)
        {
            y0 = corners[0].y;
        }
        if (corners[2].x > x1)
        {
            x1 = corners[2].x;
        }
        if (corners[2].y > y1)
        {
            y1 = corners[2].y;
        }
    }
    UIElement GetParentUI()
    {
        UIElement ui_element = (transform.parent != null ? transform.parent.GetComponent<UIElement>() : null);

        while (ui_element != null)
        {
            if (ui_element.IsUI)
            {
                break;
            }
            ui_element = (ui_element.transform.parent != null ? ui_element.transform.parent.GetComponent<UIElement>() : null);
        }

        return ui_element;
    }

    UIElement GetParent()
    {
        UIElement ui_element = (transform.parent != null ? transform.parent.GetComponent<UIElement>() : null);

        //while (ui_element != null)
        //{
        //    if (ui_element.IsUI)
        //    {
        //        break;
        //    }
        //    ui_element = (ui_element.transform.parent != null ? ui_element.transform.parent.GetComponent<UIElement>() : null);
        //}

        return ui_element;
    }

    bool GetParentUIFreeze(UIElement element)
    {
        UIElement parent = GetParent();
        if (null == parent)
        {
            return false;
        }

        return parent.Freeze;
    }

    int GetRealWidth(UIElement parent_ui, UIWidget ui, int scale, float move_delta)
    {
        float delta = scale * move_delta;

        //if (delta > 0 && parent_ui != null)
        //{
        //    delta = Mathf.Min(delta, Mathf.Max(0, (scale > 0 ? parent_ui.Pos.x + parent_ui.width - (ui.transform.position.x + ui.width) : ui.transform.position.x - parent_ui.Pos.x)));
        //}

        float new_width = ui.width + delta;

        if (new_width < ui.minWidth)
        {
            new_width = ui.minWidth;
        }

        return (int)(new_width + 0.5f);
    }
    int GetRealHeight(UIElement parent_ui, UIWidget ui, int scale, float move_delta)
    {
        float delta = scale * move_delta;

        //if (delta > 0 && parent_ui != null)
        //{
        //    delta = Mathf.Min(delta, Mathf.Max(0, (scale > 0 ? parent_ui.Pos.y + parent_ui.height - (ui.transform.position.y + ui.height) : ui.transform.position.y - parent_ui.Pos.y)));
        //}

        float new_height = ui.height + delta;

        if (new_height < ui.minHeight)
        {
            new_height = ui.minHeight;
        }

        return (int)(new_height + 0.5f);
    }
    Vector3 GetRealMoveDelta(UIElement parent_ui, UIElement ui, Vector3 move_delta)
    {
        return move_delta;
        //if (parent_ui == null)
        //{
        //    return move_delta;
        //}

        //float x_delta = parent_ui.Pos.x - ui.Pos.x + (move_delta.x > 0 ? parent_ui.width - ui.width : 0);
        //float y_delta = parent_ui.Pos.y - ui.Pos.y + (move_delta.y > 0 ? parent_ui.height - ui.height : 0);

        //x_delta = (move_delta.x > 0 ? Mathf.Min(move_delta.x, Mathf.Max(x_delta, 0)) : Mathf.Max(move_delta.x, Mathf.Min(x_delta, 0)));
        //y_delta = (move_delta.y > 0 ? Mathf.Min(move_delta.y, Mathf.Max(y_delta, 0)) : Mathf.Max(move_delta.y, Mathf.Min(y_delta, 0)));

        //return new Vector3(x_delta, y_delta);
    }

    public bool IsEditBoxCollider()
    {
        if (GetComponent<UIWidget>() != null)
            return false;

        if (GetComponent<BoxCollider>() != null)
            return true;

        return false;
    }

    public void UpdateAllSubPrefabLocalPos()
    {
        UIPrefabNode[] uipns = this.GetComponentsInChildren<UIPrefabNode>();
        foreach (UIPrefabNode uiPrefabNode in uipns)
        {
            if (uiPrefabNode.transform.childCount == 1)
            {
                GameObject goSubPrefabParent = uiPrefabNode.transform.GetChild(0).gameObject;

                UIWidget.Pivot piLast = UIWidget.Pivot.BottomLeft;
                UIWidget uiW = goSubPrefabParent.GetComponent<UIWidget>();
                //Vector3 uiWorldPos = goSubPrefabParent.transform.position;

                uiPrefabNode.prefabLocalPos = goSubPrefabParent.transform.localPosition;

                if (uiW != null && uiPrefabNode.SavePivot && uiW.pivot != uiPrefabNode.origPivot)
                {
                    piLast = uiW.pivot;
                    Vector3 before = uiW.worldCorners[0];
                    uiW.rawPivot = uiPrefabNode.origPivot;
                    Vector3 after = uiW.worldCorners[0];
                    float deltaX = before.x - after.x;
                    float deltaY = before.y - after.y;
                    uiW.rawPivot = piLast;
                    uiPrefabNode.prefabLocalPos.x += deltaX;
                    uiPrefabNode.prefabLocalPos.y += deltaY;
                }
            }
        }
    }
}

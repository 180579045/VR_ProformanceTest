using UnityEngine;
using System.Linq;

public enum MOUSEINAXIS
{
    MOUSEIN_BASE = 0,
    MOUSEIN_X,
    MOUSEIN_XNEG,
    MOUSEIN_Y,
    MOUSEIN_YNEG,
    MOUSEIN_Z,
    MOUSEIN_ZNEG,
    MOUSEIN_NONE = -1,

}

public class AxisObject : CompositeGeoometryObject
{
    public AxisObject(string objectID)
        : base(objectID)
    {
        
    }

    public AxisObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {
    }

    #region public接口
    public override Material DefaultMat
    {
        get
        {
            return m_DefaultMat;
        }
        set
        {
            m_DefaultMat = value;
        }
    }

    public override void SetDispMaterial(Material mat)
    {
        return;
    }

    public override bool IsMouseInObject(Camera cam, Vector2 mousePos, out object objInfo)
    {
        bool bRet = false;
        objInfo = null;

        if (Event.current.type == EventType.MouseDown)
        {
            int test = 1;
            test++;
        }

        for (int index = 0; index < m_Children.Count; index++)
        {
            if (m_Children[index] != null)
            {
                object info = null;

                if (m_Children[index].IsMouseInObject(cam, mousePos, out info))
                {
                    Material yellowMat = new Material(Shader.Find("Diffuse"));
                    
                    yellowMat.color = Color.yellow;

                    m_Children[index].SetDispMaterial(yellowMat);
                    objInfo = new object();
                    objInfo = (MOUSEINAXIS)(MOUSEINAXIS.MOUSEIN_BASE + index);
                    bRet = true;
                }
                else
                {
                    m_Children[index].SetDispMaterial(m_Children[index].DefaultMat);
                }
            }

        }

        return bRet;
    }

    public override bool IsMouseInObject(Ray ray, out object objInfo)
    {
        bool bRet = false;
        objInfo = null;

        if (Event.current.type == EventType.MouseDown)
        {
            int test = 1;
            test++;
        }

        for (int index = 0; index < m_Children.Count; index++)
        {
            if (m_Children[index] != null)
            {
                object info = null;

                if (m_Children[index].IsMouseInObject(ray, out info))
                {
                    Material yellowMat = new Material(Shader.Find("Diffuse"));

                    yellowMat.color = Color.yellow;

                    m_Children[index].SetDispMaterial(yellowMat);
                    objInfo = new object();
                    objInfo = (MOUSEINAXIS)(MOUSEINAXIS.MOUSEIN_BASE + index);
                    bRet = true;
                }
                else
                {
                    m_Children[index].SetDispMaterial(m_Children[index].DefaultMat);
                }
            }

        }

        return bRet;
    }

    public void SetXAxisVisiable(bool visiable)
    {
        if(
            (null == m_Children)
            )
        {
            return;
        }
        SetAllAxisVisiable();

        m_Children[(int)AXISTAPER_INDEX.AXISTRAPER_X + 1].SetVisiable(visiable);
        m_Children[(int)AXISTAPER_INDEX.AXISTRAPER_X_NEG + 1].SetVisiable(visiable);
    }

    public void SetYAxisVisiable(bool visiable)
    {
        if (
            (null == m_Children)
            )
        {
            return;
        }
        SetAllAxisVisiable();

        m_Children[(int)AXISTAPER_INDEX.AXISTRAPER_Y + 1].SetVisiable(visiable);
        m_Children[(int)AXISTAPER_INDEX.AXISTRAPER_Y_NEG + 1].SetVisiable(visiable);
    }

    public void SetZAxisVisiable(bool visiable)
    {
        if (
            (null == m_Children)
            )
        {
            return;
        }
        SetAllAxisVisiable();

        m_Children[(int)AXISTAPER_INDEX.AXISTRAPER_Z + 1].SetVisiable(visiable);
        m_Children[(int)AXISTAPER_INDEX.AXISTRAPER_Z_NEG + 1].SetVisiable(visiable);
    }

    public void SetAllAxisVisiable()
    {
        foreach (var item in m_Children)
        {
            if (item != null)
            {
                item.SetVisiable(true);
            }
        }
    }
    #endregion

    #region Base虚函数
    protected override void FixChildrenObj(string objectID, Quaternion roration, Vector3 pos, Vector3 size, int layer)
    {
        if(string.IsNullOrEmpty(objectID))
        {
            return;
        }

        GeometryObject baseCube = new CubeObject(objectID + m_PostfixStr[0]);
        baseCube.SetParent(this);
        baseCube.LocalPosition = new Vector3(0f, 0f, 0f);
        baseCube.LocalRotation = new Quaternion();

        GeometryObject[] axisTaper = new GeometryObject[6];

        Material redMat = new Material(Shader.Find("Diffuse"));
        redMat.color = Color.red;
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X] = new TaperObject(objectID + m_PostfixStr[(int)AXISTAPER_INDEX.AXISTRAPER_X + 1]);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X].SetParent(this);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X].LocalPosition = new Vector3(1.5f, 0f, 0f);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X].LocalRotation = Quaternion.AngleAxis(90, new Vector3(0f, 0f, 1f));
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X].DefaultMat = redMat;

        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X_NEG] = new TaperObject(objectID + m_PostfixStr[(int)AXISTAPER_INDEX.AXISTRAPER_X_NEG + 1]);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X_NEG].SetParent(this);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X_NEG].LocalPosition = new Vector3(-1.5f, 0f, 0f);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_X_NEG].LocalRotation = Quaternion.AngleAxis(-90, new Vector3(0f, 0f, 1f));

        Material greenMat = new Material(Shader.Find("Diffuse"));
        greenMat.color = Color.green;
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y] = new TaperObject(objectID + m_PostfixStr[(int)AXISTAPER_INDEX.AXISTRAPER_Y + 1]);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y].SetParent(this);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y].LocalPosition = new Vector3(0f, 1.5f, 0f);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y].LocalRotation = Quaternion.AngleAxis(180, new Vector3(0f, 0f, 1f));
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y].DefaultMat = greenMat;

        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y_NEG] = new TaperObject(objectID + m_PostfixStr[(int)AXISTAPER_INDEX.AXISTRAPER_Y_NEG + 1]);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y_NEG].SetParent(this);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y_NEG].LocalPosition = new Vector3(0f, -1.5f, 0f);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Y_NEG].LocalRotation = Quaternion.AngleAxis(0, new Vector3(0f, 0f, 1f));

        Material bluewMat = new Material(Shader.Find("Diffuse"));
        bluewMat.color = Color.blue;
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z] = new TaperObject(objectID + m_PostfixStr[(int)AXISTAPER_INDEX.AXISTRAPER_Z + 1]);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z].SetParent(this);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z].LocalPosition = new Vector3(0f, 0f, 1.5f);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z].LocalRotation = Quaternion.AngleAxis(-90, new Vector3(1f, 0f, 0f));
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z].DefaultMat = bluewMat;

        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z_NEG] = new TaperObject(objectID + m_PostfixStr[(int)AXISTAPER_INDEX.AXISTRAPER_Z_NEG + 1]);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z_NEG].SetParent(this);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z_NEG].LocalPosition = new Vector3(0f, 0f, -1.5f);
        axisTaper[(int)AXISTAPER_INDEX.AXISTRAPER_Z_NEG].LocalRotation = Quaternion.AngleAxis(90, new Vector3(1f, 0f, 0f));

        //TextObject axisText_x = new TextObject(objectID + "x_Text");
        //axisText_x.SetParent(this);
        //axisText_x.LocalPosition = new Vector3(2.7f, 0f, 0f);
        //axisText_x.Rotation = Quaternion.identity;
        //axisText_x.Text = "x";

        //TextObject axisText_y = new TextObject(objectID + "y_Text");
        //axisText_y.SetParent(this);
        //axisText_y.LocalPosition = new Vector3(0f, 2.7f, 0f);
        //axisText_y.Rotation = Quaternion.identity;
        //axisText_y.Text = "y";

        //TextObject axisText_z = new TextObject(objectID + "z_Text");
        //axisText_z.SetParent(this);
        //axisText_z.LocalPosition = new Vector3(0f, 0f, 2.7f);
        //axisText_z.Rotation = Quaternion.identity;
        //axisText_z.Text = "z";

        m_Children.Add(baseCube);

        m_Children.AddRange(axisTaper.ToList<GeometryObject>());

        //m_Children.Add(axisText_x);
        //m_Children.Add(axisText_y);
        //m_Children.Add(axisText_z);
    }
    #endregion

    #region private私有函数

    #endregion
    private string[] m_PostfixStr = new string[7] { "_BaseCube", "_X", "_XNEG", "_Y", "_YNEG", "_Z", "_ZNEG", };

    private enum AXISTAPER_INDEX
    {
        AXISTRAPER_X = 0,
        AXISTRAPER_X_NEG,
        AXISTRAPER_Y,
        AXISTRAPER_Y_NEG,
        AXISTRAPER_Z,
        AXISTRAPER_Z_NEG,
    }
}
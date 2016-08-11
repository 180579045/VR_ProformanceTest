using UnityEngine;
using System.Collections.Generic;
public enum OPERATESTATUS
{
    LMOUSE_DOWN = 0,
    LMOUSE_UP,
    LMOUSE_CLICK,
    MOUSE_DRAG
}


public class GeometryObject
{
    public GeometryObject(string objectID)
    {
        m_Param = null;
        InitGameObject(
                        objectID
                        ,new Quaternion()
                        ,new Vector3()
                        ,new Vector3(1f, 1f, 1f)
                        ,new Material(Shader.Find("Diffuse"))
                        ,0
                        );
    }

    public GeometryObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer, List<object> param = null)
    {
        m_Param = param;

        InitGameObject(
                        objectID
                        ,roration
                        ,pos
                        ,size
                        ,mat
                        ,layer
                        );
    }

    #region public接口
    public string ObjectID
    {
        get
        {
            return m_ObjectID;
        }
    }

    public Quaternion Rotation
    {
        set
        {
            if (null == m_GameObject)
            {
                return;
            }

            m_GameObject.transform.rotation = value;
        }
        get
        {
            if (null == m_GameObject)
            {
                return new Quaternion();
            }

            return m_GameObject.transform.rotation;
        }
    }

    public Quaternion LocalRotation
    {
        set
        {
            if (null == m_GameObject)
            {
                return;
            }

            m_GameObject.transform.localRotation = value;
        }
        get
        {
            if (null == m_GameObject)
            {
                return new Quaternion();
            }

            return m_GameObject.transform.localRotation;
        }
    }

    public Vector3 Position
    {
        set
        {
            if (null == m_GameObject)
            {
                return;
            }

            m_GameObject.transform.position = value;
        }
        get
        {
            if (null == m_GameObject)
            {
                return new Vector3();
            }

            return m_GameObject.transform.position;
        }
    }

    public Vector3 LocalPosition
    {
        set
        {
            if (null == m_GameObject)
            {
                return;
            }

            m_GameObject.transform.localPosition = value;
        }
        get
        {
            if (null == m_GameObject)
            {
                return new Vector3();
            }

            return m_GameObject.transform.localPosition;
        }
    }

    public Vector3 Scale
    {
        set
        {
            if (null == m_GameObject)
            {
                return;
            }

            m_GameObject.transform.localScale = value;
        }
        get
        {
            if (null == m_GameObject)
            {
                return new Vector3();
            }

            return m_GameObject.transform.localScale;
        }
    }

    public virtual string Text
    {
        set
        {
            m_Text = value;
        }

        get
        {
            return m_Text;
        }
    }

    public virtual Material DefaultMat
    {
        set
        {
            m_DefaultMat = value;
            SetDispMaterial(m_DefaultMat);
        }

        get
        {
            return m_DefaultMat;
        }
    }

    public virtual long DrawCounter
    {
        set
        {
            m_DrawCounter = value;
        }

        get
        {
            return m_DrawCounter;
        }
    }

    public virtual int Layer
    {
        set
        {
            if(m_GameObject != null)
            {
                m_GameObject.layer = value;
            }
        }

        get
        {
            int layer = 0;
            if (m_GameObject != null)
            {
                layer = m_GameObject.layer;
            }

            return layer;
        }
    }

    public virtual void SetVisiable(bool visiable)
    {
        m_GameObject.SetActive(visiable);
    }

    public virtual void SetParent(GeometryObject parent)
    {
        if(null == parent)
        {
            return;
        }

        m_GameObject.transform.parent = parent.m_GameObject.transform;

        DrawCounter = parent.m_DrawCounter;
    }

    public virtual void SetDispMaterial(Material mat)
    {
        if (null == m_GameObject)
        {
            return;
        }

        MeshRenderer meshRender = m_GameObject.GetComponent<MeshRenderer>();
        if (null == meshRender)
        {
            return;
        }

        meshRender.sharedMaterial = mat;
    }

    public virtual void Release()
    {
        if (m_GameObject != null)
        {
            GameObject.DestroyImmediate(m_GameObject);
            m_GameObject = null;
        }
    }

    public virtual bool IsMouseInObject(Camera cam, Vector2 mousePos, out object objInfo)
    {
        bool bRet = false;
        objInfo = null;

        if (null == cam)
        {
            return false;
        }

        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit info;


        if (Physics.Raycast(ray, out info, float.PositiveInfinity))
        {
            if(info.collider.name == ObjectID)
            {
                bRet = true;
            }
        }

        return bRet;
    }

    public virtual bool IsMouseInObject(Ray ray, out object objInfo)
    {
        bool bRet = false;
        objInfo = null;

        RaycastHit info;

        if (Event.current.type == EventType.MouseDown)
        {
            int test = 1;
            test++;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        }
        if (Physics.Raycast(ray, out info, float.PositiveInfinity))
        {
            if (info.collider.name == ObjectID)
            {
                bRet = true;
            }
        }

        return bRet;
    }
    #endregion

    #region Virtual函数
    protected virtual void InitGameObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        if(string.IsNullOrEmpty(objectID))
        {
            return;
        }

        CreateGameObject();

        FixGameObject(objectID, roration, pos, size, layer);

        FixComponent(mat);

    }

    protected virtual void CreateGameObject()
    {
        GameObject.DestroyImmediate(m_GameObject);
        m_GameObject = null;

        m_GameObject = new GameObject();
    }

    protected virtual void FixGameObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, int layer)
    {
        if (
               (null == m_GameObject)
            && string.IsNullOrEmpty(objectID)
            )
        {
            return;
        }

        m_GameObject.name = objectID.ToString();
        m_GameObject.layer = layer;
        m_GameObject.transform.rotation = roration;
        m_GameObject.transform.position = pos;
        m_GameObject.transform.localScale = size;

        m_ObjectID = objectID;
        m_DrawCounter = 0;

        if (!EditorHelper.IsDebugMode())
        {
            m_GameObject.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    protected virtual void FixComponent(Material mat)
    {
        if(null == m_GameObject)
        {
            return;
        }

        if (null == mat)
        {
            mat = new Material(Shader.Find("Diffuse"));
        }

        MeshRenderer meshRender = m_GameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = m_GameObject.AddComponent<MeshFilter>();

        meshRender.material = mat;
        meshFilter.mesh = FixMesh();
        m_DefaultMat = mat;
    }

    protected virtual Mesh FixMesh()
    {
        return null;
    }

    protected virtual void ReleaseMesh()
    {
        return;
    }
    #endregion

    #region private函数

    #endregion

    protected GameObject m_GameObject = null;
    protected string m_ObjectID = string.Empty;
    protected string m_Text = string.Empty;
    protected long m_DrawCounter = 0;
    protected Material m_DefaultMat = null;
    protected List<object> m_Param = null;
}
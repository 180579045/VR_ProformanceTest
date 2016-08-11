using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositeGeoometryObject : GeometryObject
{
    public CompositeGeoometryObject(string objectID)
        : base(objectID)
    {
        
    }

    public CompositeGeoometryObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {
    }

    #region public接口
    public override void Release()
    {
        foreach(var item in m_Children)
        {
            if(item != null)
            {
                item.Release();
            }
        }

        base.Release();
    }

    public override Material DefaultMat
    {
        get
        {
            return m_DefaultMat;
        }
        set
        {
            foreach(var item in m_Children)
            {
                if(item != null)
                {
                    item.DefaultMat = value;
                }
            }

            m_DefaultMat = value;
        }
    }

    public override long DrawCounter
    {
        get
        {
            return m_DrawCounter;
        }
        set
        {
            m_DrawCounter = value;

            foreach(var item in m_Children)
            {
                if(item != null)
                {
                    item.DrawCounter = value;
                }
            }
        }
    }

    public override int Layer
    {
        get
        {
            int layer = 0;
            if(m_GameObject != null)
            {
                layer = m_GameObject.layer;
            }

            return layer;
        }
        set
        {
            if(m_GameObject != null)
            {
                m_GameObject.layer = value;
            }

            foreach(var item in m_Children)
            {
                if(item != null)
                {
                    item.Layer = value;
                }
            }
        }
    }
    public override void SetDispMaterial(Material mat)
    {
        foreach(var item in m_Children)
        {
            if(item != null)
            {
                item.SetDispMaterial(mat);
            }
        }
    }

    public override bool IsMouseInObject(Camera cam, Vector2 mousePos, out object info)
    {
        bool bRet = false;
        info = null;

        foreach(var item in m_Children)
        {
            if (item != null)
            {
                bRet = item.IsMouseInObject(cam, mousePos, out info);
                if(bRet)
                {
                    break;
                }
            }
        }

        return bRet;
    }

    public override bool IsMouseInObject(Ray ray, out object objInfo)
    {
        bool bRet = false;
        objInfo = null;

        foreach (var item in m_Children)
        {
            if (item != null)
            {
                bRet = item.IsMouseInObject(ray, out objInfo);
                if (bRet)
                {
                    break;
                }
            }
        }

        return bRet;
    }
    #endregion
    protected override void InitGameObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        if(string.IsNullOrEmpty(objectID))
        {
            return;
        }

        CreateGameObject();

        FixGameObject(objectID, roration, pos, size, layer);

        FixChildrenObj(objectID, roration, pos, size, layer);

        FixComponent(mat);
    }

    protected virtual void FixChildrenObj(string objectID, Quaternion roration, Vector3 pos, Vector3 size, int layer)
    {
        return;
    }

    protected override void FixComponent(Material mat)
    {
        return;
    }

    protected List<GeometryObject> m_Children = new List<GeometryObject>();
}
using UnityEngine;
using System.Collections.Generic;

public enum GEOMETRYTYPE
{
    GEOMETRY_TYPE_DEFAULT = -1,
    GEOMETRY_TYPE_CUBE = 0,
    GEOMETRY_TYPE_TAPER,
    GEOMETRY_TYPE_CYLINDER,
    GEOMETRY_TYPE_AXIS,
    GEOMETRY_TYPE_TEXT,
    GEOMETRY_TYPE_RING,
    GEOMETRY_TYPE_SECTOR,
    GEOMETRY_TYPE_SPHERE,
}

public class GeometryMesh
{
    public long DrawCounter
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
    public Mesh RealMesh
    {
        get
        {
            return m_Mesh;
        }
    }

    public GEOMETRYTYPE Type
    {
        get
        {
            return m_Type;
        }
    }

    public GeometryMesh(List<object> paramTbl = null)
    {
        param = paramTbl;
        BuildMesh();
    }

    public virtual void BuildMesh()
    {
        return;
    }

    public virtual bool IsMeshEqual(List<object> paramTbl)
    {
        return true;
    }

    public void ReleaseMesh()
    {
        if(m_Mesh != null)
        {
            m_Mesh.Clear();
        }
    }

    protected GEOMETRYTYPE m_Type = GEOMETRYTYPE.GEOMETRY_TYPE_DEFAULT;
    protected Mesh m_Mesh = new Mesh();
    protected long m_DrawCounter = 0;
    public List<object> param = null;
}
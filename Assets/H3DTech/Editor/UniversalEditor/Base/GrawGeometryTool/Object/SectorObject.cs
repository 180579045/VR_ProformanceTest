using UnityEngine;
using System.Collections.Generic;

public class SectorObject : GeometryObject
{
    public SectorObject(string objectID)
        : base(objectID)
    {

    }

    public SectorObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer, List<object> param)
        : base(objectID, roration, pos, size, mat, layer, param)
    {        
    }

    protected override void FixComponent(Material mat)
    {
        base.FixComponent(mat);


    }

    protected override Mesh FixMesh()
    {
        Mesh mesh = null;
        GeometryMeshManager.GetInstance().GetMesh(GEOMETRYTYPE.GEOMETRY_TYPE_SECTOR, out mesh, m_Param);


        return mesh;
    }

    protected override void ReleaseMesh()
    {
        List<System.Object> param = new List<object>();
        param.Add(m_Param);

        GeometryMeshManager.GetInstance().ReleaseMesh(GEOMETRYTYPE.GEOMETRY_TYPE_SECTOR, m_Param);
    }
}
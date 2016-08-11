using UnityEngine;

public class TaperObject : GeometryObject
{
    public TaperObject(string objectID)
        : base(objectID)
    {

    }

    public TaperObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {

    }

    protected override void FixComponent(Material mat)
    {
        base.FixComponent(mat);
       
        CapsuleCollider capCollider = m_GameObject.AddComponent<CapsuleCollider>();
       
        capCollider.center = new Vector3();
        capCollider.radius = 0.5f;
        capCollider.height = 2f;
    }

    protected override Mesh FixMesh()
    {
        Mesh mesh = null;
        GeometryMeshManager.GetInstance().GetMesh(GEOMETRYTYPE.GEOMETRY_TYPE_TAPER, out mesh);

        return mesh;
    }

    protected override void ReleaseMesh()
    {
        GeometryMeshManager.GetInstance().ReleaseMesh(GEOMETRYTYPE.GEOMETRY_TYPE_TAPER);
    }
}
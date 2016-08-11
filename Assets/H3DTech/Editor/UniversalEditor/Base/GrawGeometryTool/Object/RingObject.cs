using UnityEngine;

public class RingObject : GeometryObject
{
    public RingObject(string objectID)
        : base(objectID)
    {

    }

    public RingObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {

    }


    protected override Mesh FixMesh()
    {
        Mesh mesh = null;
        GeometryMeshManager.GetInstance().GetMesh(GEOMETRYTYPE.GEOMETRY_TYPE_RING, out mesh);

        return mesh;
    }

    protected override void ReleaseMesh()
    {
        GeometryMeshManager.GetInstance().ReleaseMesh(GEOMETRYTYPE.GEOMETRY_TYPE_RING);
    }

}
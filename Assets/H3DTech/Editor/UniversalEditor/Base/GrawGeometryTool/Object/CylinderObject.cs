using UnityEngine;

public class CylinderObject : GeometryObject
{
    public CylinderObject(string objectID)
        : base(objectID)
    {

    }

    public CylinderObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {

    }

    protected override void InitGameObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GameObject.DestroyImmediate(m_GameObject);
        m_GameObject = null;

        m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        FixGameObject(objectID, roration, pos, size, layer);
    }

}
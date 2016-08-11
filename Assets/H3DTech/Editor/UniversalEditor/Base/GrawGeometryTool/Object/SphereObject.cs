using UnityEngine;

public class SphereObject : GeometryObject
{
    public SphereObject(string objectID)
        : base(objectID)
    {

    }

    public SphereObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {

    }

    protected override void InitGameObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GameObject.DestroyImmediate(m_GameObject);
        m_GameObject = null;

        m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        FixGameObject(objectID, roration, pos, size, layer);

        DefaultMat = new Material(Shader.Find("Diffuse"));
    }
}
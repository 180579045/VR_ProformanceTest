using UnityEngine;

public class CubeObject : GeometryObject
{
    public CubeObject(string objectID)
        : base(objectID)
    {

    }

    public CubeObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {

    }

    protected override void InitGameObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GameObject.DestroyImmediate(m_GameObject);
        m_GameObject = null;

        m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

        FixGameObject(objectID, roration, pos, size, layer);

        DefaultMat = new Material(Shader.Find("Diffuse"));
    }
}
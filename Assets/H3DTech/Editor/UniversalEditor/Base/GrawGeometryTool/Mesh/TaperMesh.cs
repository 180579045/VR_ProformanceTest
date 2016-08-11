using UnityEngine;

public class TaperMesh : GeometryMesh
{
    public override void BuildMesh()
    {
        m_Mesh = new Mesh();

        int n = 20;
        int vertNum = n * 3 * 2;

//        Vector3 orgPoint = new Vector3();

        Vector3[] verts = new Vector3[vertNum];
        Vector3[] norms = new Vector3[vertNum];
        Vector2[] uvs = new Vector2[vertNum];

        int[] indices = new int[vertNum];

        verts[0].Set(0f, -1f, 0f);
        verts[1].Set(-0.5f, -1f, 0f);
        
        //底面
        for (int index = 2; index < 3 * n;)
        {
            if(index > 2)
            {
                verts[index - 2].Set(0f, -1f, 0f);
                verts[index - 1] = verts[index - 3];
            }

            Vector3 v1 = verts[index - 1] - verts[index - 2];
            Vector3 v2 = Quaternion.AngleAxis(-360 / n, new Vector3(0, 1, 0)) * v1;
            verts[index] = v2 + verts[index - 2];

            Vector3 side1 = verts[index] - verts[index - 2];
            Vector3 side2 = verts[index - 1] - verts[index - 2];
            Vector3 normal = Vector3.Cross(side1, side2);
            normal = normal / normal.magnitude;
            norms[index] = normal;
            norms[index - 1] = normal;
            norms[index - 2] = normal;

            index += 3;
        }

        for (int index = 3 * n; index < 3 * n * 2;)
        {
            verts[index].Set(0f, 1f, 0f);
            verts[index + 1] = verts[index - 3 * n + 2];
            verts[index + 2] = verts[index - 3 * n + 1];

            Vector3 side1 = verts[index + 1] - verts[index];
            Vector3 side2 = verts[index + 2] - verts[index];
            Vector3 normal = Vector3.Cross(side1, side2);
            normal = normal / normal.magnitude;
            norms[index] = normal;
            norms[index + 1] = normal;
            norms[index + 2] = normal;
       
            index += 3;
        }

        for (int index = 0; index < vertNum; index++)
        {
            //norms[index] = (verts[index] - orgPoint).normalized + verts[index];
            indices[index] = index;
        }

        for (int index = 0; index < vertNum; index++)
        {
            uvs[index] = new Vector2(0, 0);
        }

        m_Mesh.vertices = verts;
        m_Mesh.normals = norms;
        m_Mesh.uv = uvs;
        m_Mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        //GameObject.CreatePrimitive(PrimitiveType)
    }


}
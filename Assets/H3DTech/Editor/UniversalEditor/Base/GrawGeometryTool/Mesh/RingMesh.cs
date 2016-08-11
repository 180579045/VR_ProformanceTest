using UnityEngine;

public class RingMesh : GeometryMesh
{
    public override void BuildMesh()
    {
        m_Mesh = new Mesh();

        int n = 36;
        int vertNum = n * 2;

        //        Vector3 orgPoint = new Vector3();

        Vector3[] verts = new Vector3[vertNum];
        Vector3[] norms = new Vector3[vertNum];
        Vector2[] uvs = new Vector2[vertNum];

        int[] indices = new int[vertNum];


        for(int index = 0; index < vertNum;)
        {
            if(0 == index)
            {
                verts[index].Set(1f, 0f, 0f);
                verts[index + 1] = Quaternion.AngleAxis(360 / n, new Vector3(0, 1, 0)) * verts[index];
                index += 2;
                continue;
            }
            verts[index] = verts[index - 1];
            verts[index + 1] = Quaternion.AngleAxis(360 / n, new Vector3(0, 1, 0)) * verts[index];

            index += 2;

        }

        for (int index = 0; index < vertNum; index++)
        {
            //norms[index] = (verts[index] - orgPoint).normalized + verts[index];
            norms[index] = Vector3.up;
            indices[index] = index;
        }

        for (int index = 0; index < vertNum; index++)
        {
            uvs[index] = new Vector2(0, 0);
        }

        m_Mesh.name = "RingMesh";
        m_Mesh.vertices = verts;
        m_Mesh.normals = norms;
        m_Mesh.uv = uvs;
        m_Mesh.SetIndices(indices, MeshTopology.Lines, 0);
    }


}
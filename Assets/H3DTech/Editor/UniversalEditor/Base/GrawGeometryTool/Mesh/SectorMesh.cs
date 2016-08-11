using UnityEngine;
using System.Collections.Generic;
public class SectorMesh : GeometryMesh
{
    public SectorMesh(List<System.Object> paramTbl)
        : base(paramTbl)
    {

    }

    public override bool IsMeshEqual(List<System.Object> paramTbl)
    {
        bool bRet = false;

        if ((float)paramTbl[0] == (float)param[0])
        {
            bRet = true;
        }
        else
        {
            bRet = false;
        }


        return bRet;
    }

    public override void BuildMesh()
    {
        m_Mesh = new Mesh();

        float centerAngle = (float)param[0];
        
        if(centerAngle == 0)
        {
            centerAngle = 90f;
        }
        int vertNum = 36 * 3;


        //        Vector3 orgPoint = new Vector3();

        Vector3[] verts = new Vector3[vertNum];
        Vector3[] norms = new Vector3[vertNum];
        Vector2[] uvs = new Vector2[vertNum];

        int[] indices = new int[vertNum];

        verts[0].Set(0f, 0f, 0f);
        verts[1].Set(-1f, 0f, 0f);
        verts[1] = Quaternion.AngleAxis((90f - centerAngle / 2), new Vector3(0, 1, 0)) * verts[1];
        for (int index = 2; index < vertNum; )
        {
            if (index > 2)
            {
                verts[index - 2].Set(0f, 0f, 0f);
                verts[index - 1] = verts[index - 3];
            }

            Vector3 v1 = verts[index - 1] - verts[index - 2];
            Vector3 v2 = Quaternion.AngleAxis((centerAngle / 36), new Vector3(0, 1, 0)) * v1;
            verts[index] = v2 + verts[index - 2];

            index += 3;
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

        m_Mesh.name = "SectorMesh" ;
        m_Mesh.vertices = verts;
        m_Mesh.normals = norms;
        m_Mesh.uv = uvs;
        m_Mesh.SetIndices(indices, MeshTopology.Triangles, 0);
    }


}
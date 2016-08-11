using System.Collections.Generic;
using UnityEngine;


public class GeometryMeshManager
{
    public void GetMesh(GEOMETRYTYPE meshType, out Mesh realMesh, List<System.Object> param = null)
    {
        realMesh = null;
        GeometryMesh geometryMesh = null;

        if (!IsMeshExist(meshType, out geometryMesh, param))
        {
            CreateNewMesh(meshType, out geometryMesh, param);
        }

        if (geometryMesh != null)
        {
            realMesh = geometryMesh.RealMesh;
            geometryMesh.DrawCounter++;
        }

        return;
    }

    public void ReleaseMesh(GEOMETRYTYPE meshType, List<System.Object> param = null)
    {
        GeometryMesh geometryMesh = null;

        if (IsMeshExist(meshType, out geometryMesh, param = null))
        {
            if(
                   (geometryMesh != null)
                && (geometryMesh.DrawCounter > 0)
                )
            {
                geometryMesh.DrawCounter--;
            }
        }

        return;
    }

    public void ReleaseUseLessMesh()
    {
        if(null == m_MeshTbl)
        {
            return;
        }

        for (int index = m_MeshTbl.Count - 1; index > 0; index--)
        {
            if (
                (m_MeshTbl[index] != null)
                && (0 == m_MeshTbl[index].DrawCounter)
                )
            {
                m_MeshTbl[index].ReleaseMesh();
                m_MeshTbl.RemoveAt(index);
            }
        }
    }

    private void CreateNewMesh(GEOMETRYTYPE meshType, out GeometryMesh geometryMesh, List<System.Object> param = null)
    {
        geometryMesh = null;

        if (null == m_MeshTbl)
        {
            return;
        }

        do
        {
            if (IsMeshExist(meshType, out geometryMesh, param))
            {
                break;
            }

            switch (meshType)
            {
                case GEOMETRYTYPE.GEOMETRY_TYPE_TAPER:
                    geometryMesh = new TaperMesh();
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_RING:
                    geometryMesh = new RingMesh();
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_SECTOR:
                    geometryMesh = new SectorMesh(param);
                    break;

                default:
                    break;
            }

            if (geometryMesh != null)
            {
                m_MeshTbl.Add(geometryMesh);
            }

        } while (false);

        return;
    }

    private bool IsMeshExist(GEOMETRYTYPE meshType, out GeometryMesh mesh, List<System.Object> param = null)
    {
        bool bRet = false;
        mesh = null;

        if(null == m_MeshTbl)
        {
            return false;
        }

        foreach(var item in m_MeshTbl)
        {
            if(null == item)
            {
                continue;
            }

            if (
                  (item.Type == meshType)
                && item.IsMeshEqual(param)
                )
            {
                mesh = item;
                break;
            }
        }

        return bRet;
    }

    private List<GeometryMesh> m_MeshTbl = new List<GeometryMesh>();

    static private GeometryMeshManager m_Instance = null;

    public static GeometryMeshManager GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GeometryMeshManager();
        }
        return m_Instance;
    }

    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            GeometryMeshManager.DestoryInstance();
        }
    }
}
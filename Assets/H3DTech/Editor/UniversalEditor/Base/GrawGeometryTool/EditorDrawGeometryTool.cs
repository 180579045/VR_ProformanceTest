using UnityEngine;
using System.Collections.Generic;

public class EditorDrawGeometryTool
{
    public void ReleaseAllGeometryObj()
    {
        foreach(var item in m_ObjectTbl)
        {
            if(item != null)
            {
                item.Release();
            }
        }

        m_ObjectTbl.Clear();
    }
    public void DrawBegin()
    {
        if (m_CanDrawObject)
        {
            //Debug.LogError("连续执行DrawBegin");
            return;
        }

        m_CanDrawObject = true;

        InitObjDrawCounter();
    }

    public void DrawEnd()
    {
        if (!m_CanDrawObject)
        {
            //Debug.LogError("连续执行DrawEnd");
            return;
        }

        m_CanDrawObject = false;

        ReleaseUselessGeometryObj();
    }

    public void DrawCube(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_CUBE
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );

    }

    public void DrawTaper(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;
        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_TAPER
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );
    }


    public void DrawAxis(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_AXIS
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );

        AxisObject axisObj = obj as AxisObject;
        if (axisObj != null)
        {
            axisObj.SetAllAxisVisiable();
        }
    }

    public void DrawAxisWithoutX(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_AXIS
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );

        AxisObject axisObj = obj as AxisObject;
        if(axisObj != null)
        {
            axisObj.SetXAxisVisiable(false);
        }
    }

    public void DrawAxisWithoutY(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_AXIS
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );

        AxisObject axisObj = obj as AxisObject;
        if (axisObj != null)
        {
            axisObj.SetYAxisVisiable(false);
        }
    }

    public void DrawAxisWithoutZ(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_AXIS
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );

        AxisObject axisObj = obj as AxisObject;
        if (axisObj != null)
        {
            axisObj.SetZAxisVisiable(false);
        }
    }
    public void DrawText(string id, string text, Quaternion roration, Vector3 pos, Vector3 size, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_TEXT
                            , roration
                            , pos
                            , size
                            , null
                            , layer
                            , out obj
                            );

        if (obj != null)
        {
            obj.Text = text;
        }
    }

    public void DrawRing(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_RING
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );

    }

    public void DrawSector(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer, float centerAngle)
    {
        GeometryObject obj = null;

        List<object> param = new List<object>();
        param.Add(centerAngle);


        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_SECTOR
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            , param
                            );
    }

    public void DrawSphere(string id, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
    {
        GeometryObject obj = null;

        DrawGeometryObject(
                            id
                            , GEOMETRYTYPE.GEOMETRY_TYPE_SPHERE
                            , roration
                            , pos
                            , size
                            , mat
                            , layer
                            , out obj
                            );
    }
    public bool GetGeometryObjWithMouse(Camera cam, Vector2 mousePos, out string id, out object info)
    {
        bool bRet = false;
        id = string.Empty;
        info = null;

        foreach(var item in m_ObjectTbl)
        {
            if(item != null)
            {
                bRet = item.IsMouseInObject(cam, mousePos, out info);
                if (bRet)
                {
                    id = item.ObjectID;
                    break;
                }
            }
        }

        return bRet;
    }

    public bool GetGeometryObjWithMouse(Ray ray, out string id, out object info)
    {
        bool bRet = false;
        id = string.Empty;
        info = null;

        foreach (var item in m_ObjectTbl)
        {
            if (item != null)
            {
                bRet = item.IsMouseInObject(ray, out info);
                if (bRet)
                {
                    id = item.ObjectID;
                    break;
                }
            }
        }

        return bRet;
    }

    #region private函数

    //private void DrawGeometryObjectWithCamera(string id, GEOMETRYTYPE type, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer, Camera cam)
    //{
    //    GeometryObject obj = null;

    //    if (
    //           (null == m_ObjectTbl)
    //        && string.IsNullOrEmpty(id)
    //        )
    //    {
    //        return;
    //    }

    //    if (!m_CanDrawObject)
    //    {
    //        Debug.LogError("未执行DrawBegin");
    //        return;
    //    }

    //    CreateGeometryObj(id, type, roration, pos, size, mat, layer, out obj);

    //    if(obj != null)
    //    {
    //        obj.UpdateCustomStatus(cam);
    //    }
    //}

    private void DrawGeometryObject(string id, GEOMETRYTYPE type, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer, out GeometryObject obj, List<object> param = null)
    {
        obj = null;

        if (
               (null == m_ObjectTbl)
            && string.IsNullOrEmpty(id)
            )
        {
            return;
        }

        if (!m_CanDrawObject)
        {
            Debug.LogError("未执行DrawBegin");
            return;
        }

        CreateGeometryObj(id, type, roration, pos, size, mat, layer, out obj, param);
    }


    private void CreateGeometryObj(string id, GEOMETRYTYPE type, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer, out GeometryObject obj, List<object> param = null)
    {
        obj = null;

        if(string.IsNullOrEmpty(id))
        {
            return;
        }

        if (!IsObjExist(id, out obj))
        {
            switch (type)
            {
                case GEOMETRYTYPE.GEOMETRY_TYPE_CUBE:
                    obj = new CubeObject(id, roration, pos, size, mat, layer);
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_TAPER:
                    obj = new TaperObject(id, roration, pos, size, mat, layer);
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_AXIS:
                    obj = new AxisObject(id, roration, pos, size, mat, layer);
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_TEXT:
                    obj = new TextObject(id, roration, pos, size, mat, layer);
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_RING:
                    obj = new RingObject(id, roration, pos, size, mat, layer);
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_SECTOR:
                    obj = new SectorObject(id, roration, pos, size, mat, layer, param);
                    break;

                case GEOMETRYTYPE.GEOMETRY_TYPE_SPHERE:
                    obj = new SphereObject(id, roration, pos, size, mat, layer);
                    break;

                default:
                    break;
            }


            if (obj != null)
            {
                obj.DrawCounter++;
                m_ObjectTbl.Add(obj);
            }
        }
        else
        {
            if (obj != null)
            {
                obj.Rotation = roration;
                obj.Position = pos;
                obj.Scale = size;
                obj.DefaultMat = mat;
                obj.Layer = layer;

                obj.DrawCounter++;
            }
        }
    }

    private void InitObjDrawCounter()
    {
        if (null == m_ObjectTbl)
        {
            return;
        }

        foreach (var item in m_ObjectTbl)
        {
            if (item != null)
            {
                item.DrawCounter = 0;
            }
        }
    }

    private void ReleaseUselessGeometryObj()
    {
        if (null == m_ObjectTbl)
        {
            return;
        }

        for (int index = m_ObjectTbl.Count - 1; index >= 0; index--)
        {
            if (null == m_ObjectTbl[index])
            {
                continue;
            }

            if (0 == m_ObjectTbl[index].DrawCounter)
            {
                m_ObjectTbl[index].Release();
                m_ObjectTbl.RemoveAt(index);
            }
        }

    }

    private void ReleaseUselessGeometryMesh()
    {
        GeometryMeshManager.GetInstance().ReleaseUseLessMesh();
    }

    private bool IsObjExist(string id, out GeometryObject geometryObject)
    {
        bool bRet = false;
        geometryObject = null;

        if (
               (null == m_ObjectTbl)
            && string.IsNullOrEmpty(id)
            )
        {
            return false;
        }

        foreach (var item in m_ObjectTbl)
        {
            if (id == item.ObjectID)
            {
                bRet = true;
                geometryObject = item;
                break;
            }
        }

        return bRet;
    }
    #endregion

    private List<GeometryObject> m_ObjectTbl = new List<GeometryObject>();
    private bool m_CanDrawObject = false;
}
using UnityEngine;
using System.Collections;
using UnityEditor;

public class MainViewRenderStrategy : EditorRenderStrategy
{
     

	public override void Visit(EditorControl c)
    {
        bool isHandle = false;
        currCtrl = c as MainViewCtrl;
        if (
               (null == currCtrl)
            )
        {
            return;
        }

        currCtrl.Init();

        //_RenderScene();

        if(currCtrl.IsShowAxis)
        {
            isHandle = InitAxis();
        }
  
        Rect mainViewRect = new Rect(c.LastRect);
        Rect assistantViewRect = new Rect(c.LastRect.x + c.LastRect.width - assTextureSize, c.LastRect.y, assTextureSize, assTextureSize);

        if ((c.LastRect.x + c.LastRect.width) < assTextureSize)
        {
            assistantViewRect.x = c.LastRect.x;
            assistantViewRect.width = c.LastRect.width;
        }

        if ((c.LastRect.height) < assTextureSize)
        {
            assistantViewRect.height = c.LastRect.height;
        }

        EditorGUILayout.BeginHorizontal(c.GetStyle(), c.GetOptions());
            //GUI.DrawTexture(mainViewRect, currCtrl.mainViewTexture, ScaleMode.StretchToFill, false);
            GUI.DrawTextureWithTexCoords(mainViewRect, currCtrl.mainViewTexture, currCtrl.mainViewUVRect, false);
            if(currCtrl.IsShowAxis)
            {
                //GUI.DrawTexture(assistantViewRect, currCtrl.assistantViewTexture, ScaleMode.StretchToFill, true);
                GUI.DrawTextureWithTexCoords(assistantViewRect, currCtrl.assistantViewTexture, currCtrl.assistantViewUVRect, true);

            }
        EditorGUILayout.EndHorizontal();
        c.UpdateLastRect();

        if(!isHandle)
        {
            _HandleInput();
        }

        //主视图大小有变
        if( 
                c.LastRect.width != mainViewRect.width ||
                c.LastRect.height != mainViewRect.height
          )
        {
            currCtrl.Resize(c.LastRect); 
            c.RequestRepaint();
        }

    }

    private Vector3 UpdatePos(Vector3 inputPos)
    {
        Vector3 outputPos = new Vector3();
        Vector3 orgPos = currCtrl.assistantCam.WorldToScreenPoint(currCtrl.assCentter);

        float inputLen = new Vector2(inputPos.x - orgPos.x, inputPos.y - orgPos.y).magnitude;

        if (inputLen > 0)
        {
            outputPos.x = (inputLen + 16f) * ((inputPos.x - orgPos.x) / inputLen);
        }
        outputPos.x += orgPos.x;

        if(inputLen > 0)
        {
            outputPos.y = (inputLen + 16f) * ((inputPos.y - orgPos.y) / inputLen);
        }
        outputPos.y += orgPos.y;

        outputPos.z = 7f;

        return outputPos;
    }

    bool InitAxis()
    {
        bool bRet = false;

        if(null == currCtrl)
        {
            return false;
        }
 
        DrawAxis();

        bRet = HandleAxis();

        return bRet;
    }

    void DrawAxis()
    {
        if(
            (null == currCtrl)
            || (null == currCtrl.GetEditorRoot())
            || (null == currCtrl.GetEditorRoot().GetgeometryTool())
            )
        {
            return;
        }

        Vector3 axisPos = MainViewCtrl.s_assistantViewOrigion;
        Vector3 axisXTextPos = new Vector3(axisPos.x + 2.5f, axisPos.y, axisPos.z);
        Vector3 axisYTextPos = new Vector3(axisPos.x, axisPos.y + 2.5f, axisPos.z);
        Vector3 axisZTextPos = new Vector3(axisPos.x, axisPos.y, axisPos.z + 2.5f);
        Vector3 screenXPos = currCtrl.assistantCam.WorldToScreenPoint(axisXTextPos);
        Vector3 screenYPos = currCtrl.assistantCam.WorldToScreenPoint(axisYTextPos);
        Vector3 screenZPos = currCtrl.assistantCam.WorldToScreenPoint(axisZTextPos);
     
        Vector3 axisSize = new Vector3(1f, 1f, 1f);
        Vector3 axisStrSize = new Vector3(1f, 1f, 1f);

        Quaternion q0 = Quaternion.identity;
        Material mat = new Material(Shader.Find("Diffuse"));

        float assYAngle = Vector3.Angle(currCtrl.assistantCam.transform.up, Vector3.up);

        screenXPos = UpdatePos(screenXPos);
        axisXTextPos = currCtrl.assistantCam.ScreenToWorldPoint(screenXPos);

        screenYPos = UpdatePos(screenYPos);
        axisYTextPos = currCtrl.assistantCam.ScreenToWorldPoint(screenYPos);

        screenZPos = UpdatePos(screenZPos);
        axisZTextPos = currCtrl.assistantCam.ScreenToWorldPoint(screenZPos);

        if (
               (assYAngle >= -180f)
            && (assYAngle <= 180f)
            )
        {
            axisXTextPos.x = currCtrl.assistantCam.transform.up.x + axisXTextPos.x;
            axisXTextPos.y = currCtrl.assistantCam.transform.up.y + axisXTextPos.y;
            axisXTextPos.z = currCtrl.assistantCam.transform.up.z + axisXTextPos.z;

            axisYTextPos.x = currCtrl.assistantCam.transform.up.x + axisYTextPos.x;
            axisYTextPos.y = currCtrl.assistantCam.transform.up.y + axisYTextPos.y;
            axisYTextPos.z = currCtrl.assistantCam.transform.up.z + axisYTextPos.z;

            axisZTextPos.x = currCtrl.assistantCam.transform.up.x + axisZTextPos.x;
            axisZTextPos.y = currCtrl.assistantCam.transform.up.y + axisZTextPos.y;
            axisZTextPos.z = currCtrl.assistantCam.transform.up.z + axisZTextPos.z;
        }
        else
        {
            axisXTextPos.x = -currCtrl.assistantCam.transform.up.x + axisXTextPos.x;
            axisXTextPos.y = -currCtrl.assistantCam.transform.up.y + axisXTextPos.y;
            axisXTextPos.z = -currCtrl.assistantCam.transform.up.z + axisXTextPos.z;

            axisYTextPos.x = -currCtrl.assistantCam.transform.up.x + axisYTextPos.x;
            axisYTextPos.y = -currCtrl.assistantCam.transform.up.y + axisYTextPos.y;
            axisYTextPos.z = -currCtrl.assistantCam.transform.up.z + axisYTextPos.z;

            axisZTextPos.x = -currCtrl.assistantCam.transform.up.x + axisZTextPos.x;
            axisZTextPos.y = -currCtrl.assistantCam.transform.up.y + axisZTextPos.y;
            axisZTextPos.z = -currCtrl.assistantCam.transform.up.z + axisZTextPos.z;
        }

        float xAngle = Vector3.Angle(currCtrl.assistantCam.transform.forward, Vector3.right);
        float yAngle = Vector3.Angle(currCtrl.assistantCam.transform.forward, Vector3.up);
        float zAngle = Vector3.Angle(currCtrl.assistantCam.transform.forward, Vector3.forward);
      
        if(
               ((xAngle < angleOffset) && (xAngle > -angleOffset))
            || ((xAngle < (180f + angleOffset)) && (xAngle > (180f - angleOffset)))
            )
        {
            currCtrl.GetEditorRoot().GetgeometryTool().DrawAxisWithoutX(MainViewCtrl.axisName, q0, axisPos, axisSize, mat, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_Y", "y", currCtrl.assistantCam.transform.rotation, axisYTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_Z", "z", currCtrl.assistantCam.transform.rotation, axisZTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
        }
        else if(
                 ((yAngle < angleOffset) && (yAngle > -angleOffset))
            || ((yAngle < (180f + angleOffset)) && (yAngle > (180f - angleOffset)))          
            )
        {
            currCtrl.GetEditorRoot().GetgeometryTool().DrawAxisWithoutY(MainViewCtrl.axisName, q0, axisPos, axisSize, mat, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_X", "x", currCtrl.assistantCam.transform.rotation, axisXTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_Z", "z", currCtrl.assistantCam.transform.rotation, axisZTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
        }
        else if(
                 ((zAngle < angleOffset) && (zAngle > -angleOffset))
            || ((zAngle < (180f + angleOffset)) && (zAngle > (180f - angleOffset)))
            )
        {
            currCtrl.GetEditorRoot().GetgeometryTool().DrawAxisWithoutZ(MainViewCtrl.axisName, q0, axisPos, axisSize, mat, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_X", "x", currCtrl.assistantCam.transform.rotation, axisXTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_Y", "y", currCtrl.assistantCam.transform.rotation, axisYTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
        }
        else
        {
            currCtrl.GetEditorRoot().GetgeometryTool().DrawAxis(MainViewCtrl.axisName, q0, axisPos, axisSize, mat, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_X", "x", currCtrl.assistantCam.transform.rotation, axisXTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_Y", "y", currCtrl.assistantCam.transform.rotation, axisYTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
            currCtrl.GetEditorRoot().GetgeometryTool().DrawText(MainViewCtrl.axisName + "_Z", "z", currCtrl.assistantCam.transform.rotation, axisZTextPos, axisStrSize, MainViewCtrl.s_AssLayer);
        }
    }

    bool HandleAxis()
    {
        bool bRet = false;

        if (
            (null == currCtrl)
            || (null == currCtrl.GetEditorRoot())
            || (null == currCtrl.GetEditorRoot().GetgeometryTool())
            )
        {
            return false;
        }

        bool isMouseIn = false;
        string mouseInObjID = string.Empty;
        object paramObj = null;

        Vector2 localMousePos = currCtrl.CalcLocalPos(FrameInputInfo.GetInstance().currPos);
        localMousePos.x = localMousePos.x - (currCtrl.LastRect.width - 128f);
        Vector2 cameraMousePos = new Vector2(localMousePos.x, 128f - localMousePos.y);
        isMouseIn = currCtrl.GetEditorRoot().GetgeometryTool().GetGeometryObjWithMouse(currCtrl.assistantCam, cameraMousePos, out mouseInObjID, out paramObj);

        if (
            FrameInputInfo.GetInstance().leftBtnPress
            && isMouseIn
            && (paramObj != null)
            && (mouseInObjID.StartsWith(currCtrl.mainViewRoot.name))
            )
        {

            if ((MOUSEINAXIS)paramObj == MOUSEINAXIS.MOUSEIN_X)
            {
                RoroateAll(new Vector3(0f, -90f, 0f));
            }
            else if ((MOUSEINAXIS)paramObj == MOUSEINAXIS.MOUSEIN_XNEG)
            {
                RoroateAll(new Vector3(0f, 90f, 0f));
            }
            else if ((MOUSEINAXIS)paramObj == MOUSEINAXIS.MOUSEIN_Y)
            {
                RoroateAll(new Vector3(90f, 0f, 0f));
            }
            else if ((MOUSEINAXIS)paramObj == MOUSEINAXIS.MOUSEIN_YNEG)
            {
                RoroateAll(new Vector3(-90f, 0f, 0f));
            }
            else if ((MOUSEINAXIS)paramObj == MOUSEINAXIS.MOUSEIN_Z)
            {
                RoroateAll(new Vector3(0f, 180f, 0f));
            }
            else if ((MOUSEINAXIS)paramObj == MOUSEINAXIS.MOUSEIN_ZNEG)
            {
                RoroateAll(new Vector3(0f, 0f, 0f));
            }
            bRet = true;
        }

        return bRet;
    }

    void RoroateAll(Vector3 angle)
    {
        Transform camTrans = currCtrl.camObj.transform;
        Transform assistantCamTrans = currCtrl.assistantCamObj.transform;

        camTrans.localPosition = new Vector3(currCtrl.center.x, currCtrl.center.y, currCtrl.center.z - currCtrl.radius);
        camTrans.localRotation = Quaternion.identity;

        assistantCamTrans.localPosition = new Vector3(currCtrl.assCentter.x, currCtrl.assCentter.y, currCtrl.assCentter.z - currCtrl.assRadius);
        assistantCamTrans.localRotation = Quaternion.identity;

        Vector3 localPos = (camTrans.localPosition - currCtrl.center).normalized * currCtrl.radius;
        Vector3 localPosAss = (assistantCamTrans.localPosition - currCtrl.assCentter).normalized * currCtrl.assRadius;

        Quaternion q0 = Quaternion.Euler(angle);

        camTrans.localPosition = q0 * localPos;
        camTrans.Rotate(angle, Space.Self);
        camTrans.localPosition += currCtrl.center;

        assistantCamTrans.localPosition = q0 * localPosAss;
        assistantCamTrans.Rotate(angle, Space.Self);
        assistantCamTrans.localPosition += currCtrl.assCentter;

    }
    void _HandleInput()
    {
        if (currCtrl == null)
            return;

        Vector2 currMousePos = FrameInputInfo.GetInstance().currPos;
        //Vector2 localMousePos = currCtrl.CalcLocalPos(currMousePos);
        Vector2 mouseMoveDelta = FrameInputInfo.GetInstance().posOffset;
        Vector2 wheelScrollDelta = FrameInputInfo.GetInstance().delta;

        //2D视图
        bool is2DView = currCtrl.Is2DView;

        CheckInputEvent(currCtrl);

        //若当前鼠标不在视口区域内直接忽略
        if( !currCtrl.LastRect.Contains( currMousePos ))
        {
            currCtrl.rotateDragging = false;
            currCtrl.moveDragging = false;
            return;
        }

        if( FrameInputInfo.GetInstance().dragingObjs  )
        {
            bool accept = false;

            if (currCtrl.onAcceptDragObjs != null)
            {
                accept = currCtrl.onAcceptDragObjs(
                                        currCtrl,
                                        FrameInputInfo.GetInstance().dragObjs,
                                        FrameInputInfo.GetInstance().dragObjsPaths);

                if (accept)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (!FrameInputInfo.GetInstance().dragObjsPerform)
                    {
                        currCtrl.frameTriggerInfo.isDraggingObjs = true;
                    }
                    else
                    {
                        currCtrl.frameTriggerInfo.isDropObjs = true;
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        //根据输入分析控件的推拽状态
        _UpdateDraggingState();
         
        if( Mathf.Abs( mouseMoveDelta.x ) < Mathf.Epsilon &&
            Mathf.Abs( mouseMoveDelta.y ) < Mathf.Epsilon &&
            !FrameInputInfo.GetInstance().scroll 
            ) 
        {//无明显移动直接返回
            return;
        }

        //Transform rootTrans = currCtrl.mainViewRoot.transform;
        Transform camTrans = currCtrl.camObj.transform;

        Camera cam = currCtrl.mainCam;

        if( currCtrl.rotateDragging )
        {
            if (!is2DView)
            {//只有3D视图才响应旋转拖拽
                float angleAroundUp = mouseMoveDelta.x * 0.1f;
                float angleAroundRight = mouseMoveDelta.y * 0.1f;
           
                Vector3 localPos = (camTrans.localPosition - currCtrl.center).normalized * currCtrl.radius;


                //Quaternion q0 = Quaternion.AngleAxis(angleAroundUp, camTrans.up);

                //camTrans.localPosition = q0 * localPos;
                //camTrans.Rotate(Vector3.up, angleAroundUp, Space.Self);

                //Quaternion q1 = Quaternion.AngleAxis(angleAroundRight, camTrans.right);

                //camTrans.Rotate(Vector3.right, angleAroundRight, Space.Self);
                //camTrans.localPosition = q1 * camTrans.localPosition;

                //camTrans.localPosition += currCtrl.center;

                Quaternion q0 = Quaternion.AngleAxis(angleAroundUp, Vector3.up);

                camTrans.localPosition = q0 * localPos;
                camTrans.Rotate(Vector3.up, angleAroundUp, Space.World);

                Quaternion q1 = Quaternion.AngleAxis(angleAroundRight, camTrans.right);

                camTrans.Rotate(camTrans.right, angleAroundRight, Space.World);
                camTrans.localPosition = q1 * camTrans.localPosition;
 
                camTrans.localPosition += currCtrl.center;


                if(currCtrl.IsShowAxis)
                {
                    Transform assCamTrans = currCtrl.assistantCamObj.transform; 
                    Vector3 localPosAss = (assCamTrans.localPosition - currCtrl.assCentter).normalized * currCtrl.assRadius;

                    //Quaternion q3 = Quaternion.AngleAxis(angleAroundUp, assCamTrans.up);
 
                    //assCamTrans.localPosition = q3 * localPosAss;
                    //assCamTrans.Rotate(Vector3.up, angleAroundUp, Space.Self);

                    //Quaternion q2 = Quaternion.AngleAxis(angleAroundRight, assCamTrans.right);

                    //assCamTrans.Rotate(Vector3.right, angleAroundRight, Space.Self);
                    //assCamTrans.localPosition = q2 * assCamTrans.localPosition;

                    Quaternion q3 = Quaternion.AngleAxis(angleAroundUp, Vector3.up);

                    assCamTrans.localPosition = q3 * localPosAss;
                    assCamTrans.Rotate(Vector3.up, angleAroundUp, Space.World);

                    Quaternion q2 = Quaternion.AngleAxis(angleAroundRight, assCamTrans.right);
                    assCamTrans.Rotate(assCamTrans.right, angleAroundRight, Space.World);
                    assCamTrans.localPosition = q2 * assCamTrans.localPosition;

                    assCamTrans.localPosition += currCtrl.assCentter;
                }

                currCtrl.RequestRepaint();
            }
        }

        if( currCtrl.moveDragging )
        {
            float moveX = -mouseMoveDelta.x * 0.01f;
            float moveY = mouseMoveDelta.y * 0.01f;

            if (!is2DView)
            {//3D视图
                Vector3 localPos = camTrans.localPosition - currCtrl.center;
                currCtrl.center += camTrans.up * moveY + camTrans.right * moveX;
                camTrans.localPosition = localPos + currCtrl.center;
            }
  
            currCtrl.RequestRepaint();
        }

        float zoom = 0.0f;
        if( currCtrl.zoomWheelScroll )
        {
            zoom = wheelScrollDelta.y * 0.2f;
        }else if( currCtrl.zoomDragging )
        {
            zoom = mouseMoveDelta.y * 0.01f; 
        }

        if (currCtrl.zoomWheelScroll || currCtrl.zoomDragging)
        {
            if (!is2DView)
            {
                currCtrl.radius += zoom;
                currCtrl.radius = Mathf.Clamp(currCtrl.radius, currCtrl.minRadius, currCtrl.maxRadius);

                Vector3 localPos = (camTrans.localPosition - currCtrl.center).normalized * currCtrl.radius;
                camTrans.localPosition = localPos + currCtrl.center;
            }
            else
            {
                if (currCtrl.zoomDragging)
                {//缩放拖拽在2D视图中是移动
                    float offsetX = (-mouseMoveDelta.x / cam.pixelWidth) * cam.orthographicSize * 2.0f;
                    float offsetY = (mouseMoveDelta.y / cam.pixelHeight) * cam.orthographicSize * 2.0f;
                    camTrans.localPosition += new Vector3(offsetX, offsetY);
                }
                else
                { 
                    if (zoom < 0.0f)
                    {
                        cam.orthographicSize *= 0.9f;
                    }
                    else
                    {
                        cam.orthographicSize *= 1.1f;
                    }
                }
            }
            currCtrl.RequestRepaint();
        }

        
    }

    void _UpdateDraggingState()
    {
        if (FrameInputInfo.GetInstance().leftButtonDown)
        {
            if (FrameInputInfo.GetInstance().leftBtnPress)
            {
                currCtrl.rotateDragging = true;
            }
        }
        else
        {
            currCtrl.rotateDragging = false;
        }

        if (FrameInputInfo.GetInstance().midButtonDown)
        {
            if (FrameInputInfo.GetInstance().midBtnPress)
            {
                currCtrl.moveDragging = true;
            }
        }
        else
        {
            currCtrl.moveDragging = false;
        }

        if (FrameInputInfo.GetInstance().rightButtonDown)
        {
            if (FrameInputInfo.GetInstance().rightBtnPress)
            {
                currCtrl.zoomDragging = true;
            }
        }
        else
        {
            currCtrl.zoomDragging = false;
        }

        if( FrameInputInfo.GetInstance().scroll )
        {
            currCtrl.zoomWheelScroll = true;
        }
        else
        {
            currCtrl.zoomWheelScroll = false;
        }
    }
 
    void _RenderScene()
    {
        currCtrl.mainCam.Render();
        if (currCtrl.IsShowAxis)
        {
            currCtrl.assistantCam.Render();
        }
    }


    MainViewCtrl currCtrl = null;
    private float angleOffset = 15f;
    private float assTextureSize = 128f;
}

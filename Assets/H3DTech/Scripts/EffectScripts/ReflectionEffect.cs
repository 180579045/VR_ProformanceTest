using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReflectionEffect : MonoBehaviour 
{
    
    //反射纹理大小
    [SerializeField]
    protected int textureSize = 256;

    //反射层掩码，用于指定哪些层物体可以被反射
    [SerializeField]
    protected LayerMask reflectionLayerMask;

    [SerializeField]
    protected Dictionary<Camera,Camera> reflCamTable = new Dictionary<Camera,Camera>();

    [SerializeField]
    protected Dictionary<Camera, RenderTexture> reflRTTable = new Dictionary<Camera,RenderTexture>();

    Renderer r;

	void Start () 
    {
        r = GetComponent<Renderer>();
	}
	 
	void Update () 
    {
	    
	}

    void OnDisable()
    {
        foreach (var rt in reflRTTable.Values)
        {
            DestroyImmediate(rt);
        }
        reflRTTable.Clear();

        foreach (var cam in reflCamTable.Values)
        {
            DestroyImmediate(cam.gameObject);
        }
        reflCamTable.Clear();
    }

    void OnWillRenderObject()
    {
        if (!enabled || !r || !r.sharedMaterial || !r.enabled)
            return;


        Camera cam = Camera.current;
        cam = Camera.main;

        if (!cam)
            return;

        Camera currReflCam;
        RenderTexture currReflRT;

        _CreateReflectionObjects(cam,out currReflCam, out currReflRT);

        _SyncCameraParams(cam, currReflCam);

        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        float d = -Vector3.Dot(normal,pos);
        Vector4 reflPlane = new Vector4(normal.x, normal.y, normal.z, d);

        //计算反射矩阵
        Matrix4x4 reflMat = Matrix4x4.zero;
        _CalcReflectionMatrix(reflPlane, ref reflMat);

        Vector3 camPos = cam.transform.position;
        
        Vector4 clipPlane = _CameraSpacePlane(currReflCam, pos, normal, 1.0f);
        currReflCam.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane); 
        currReflCam.worldToCameraMatrix = cam.worldToCameraMatrix * reflMat;

        currReflCam.cullingMask = ~(1 << this.gameObject.layer) & reflectionLayerMask.value; 
        currReflCam.targetTexture = currReflRT;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
        GL.SetRevertBackfacing(true);
#else
        GL.invertCulling = true;
#endif

        Vector3 euler = cam.transform.eulerAngles;
        currReflCam.transform.position = reflMat.MultiplyPoint(camPos);
        currReflCam.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

        currReflCam.Render();

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
        GL.SetRevertBackfacing(false);
#else
        GL.invertCulling = false;
#endif

        r.sharedMaterial.SetTexture("_ReflectionTex", currReflRT);
    }

    Vector4 _CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    { 
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(pos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }


    void _CalcReflectionMatrix( Vector4 p , ref Matrix4x4 m )
    {
        m.m00 = 1.0f - 2.0f * p.x * p.x;
        m.m01 = -2.0f * p.x * p.y;
        m.m02 = -2.0f * p.x * p.z;
        m.m03 = -2.0f * p.w * p.x;

        m.m10 = -2.0f * p.y * p.x;
        m.m11 = 1.0f - 2.0f * p.y * p.y;
        m.m12 = -2.0f * p.y * p.z;
        m.m13 = -2.0f * p.w * p.y;

        m.m20 = -2.0f * p.z * p.x;
        m.m21 = -2.0f * p.z * p.y;
        m.m22 = 1.0f - 2.0f * p.z * p.z;
        m.m23 = -2.0f * p.w * p.z;

        m.m30 = 0.0f;
        m.m31 = 0.0f;
        m.m32 = 0.0f;
        m.m33 = 1.0f;
    }
    

    void _CreateReflectionObjects( Camera cam , out Camera reflectionCam , out RenderTexture reflRT )
    {

        bool result = reflRTTable.TryGetValue(cam, out reflRT );
        if( !result || reflRT.width != textureSize )
        {
            if( reflRT != null )
            {
                DestroyImmediate(reflRT);
                reflRTTable.Remove(cam);
            }

            reflRT = new RenderTexture(textureSize, textureSize, 16);
            reflRT.name = "_ReflectionRT" + cam.GetInstanceID();
            reflRT.isPowerOfTwo = true;
            reflRT.hideFlags = HideFlags.DontSave;

            reflRTTable.Add(cam, reflRT);
        }

        result = reflCamTable.TryGetValue(cam, out reflectionCam);
        if( !result )
        {
            GameObject newCamGo = new GameObject("ReflectionCamera - " + cam.GetInstanceID() , typeof(Camera) );
            reflectionCam = newCamGo.GetComponent<Camera>();
            reflectionCam.hideFlags = HideFlags.DontSave;
            newCamGo.transform.position = transform.position;
            newCamGo.transform.rotation = transform.rotation;
            newCamGo.hideFlags = HideFlags.HideAndDontSave;
            reflectionCam.enabled = false;
            reflCamTable.Add(cam, reflectionCam); 
        }
    }

    void _SyncCameraParams( Camera src , Camera dest )
    {
        if (dest == null)
            return; 
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
  
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }
}

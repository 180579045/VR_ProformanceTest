using UnityEngine;
using System.Collections;




public class DistortionCamera : MonoBehaviour 
{
    [HideInInspector]
    //此字段已废弃使用distortionLayer
    public string distortionLayerName = "";

    //扰动物体层
    public LayerMask distortionLayer;
    //用于绘制前景物体，前景物体不受到扰动效果影响。前景物体不考虑扰动物体层
    //的深度，前景物体永远遮挡背景物体
    public LayerMask foregroundLayer;
    public Texture screenTex = null;

    //主相机
    Camera mMainCam;
    //前景相机
    Camera mForegroundCam;
    //扰动背景捕获相机
    Camera mCaptureCam;
    //用于剔除的相机
    Camera mCullingCam;

    RenderTexture mCaptureRT;
    RenderTexture mTemp;
    Rect mScreenRect;
    Material mBuffFlipMat;
    int mRTID;
    int mScreenTexID;
    int mScreenTexFactorID;
    int mCullingMask = -1;

    bool mEnableFullScreenColorEffect = false;

    int mCamDefaultMask = -1;
    CameraClearFlags mCamDefaultClearFlags;
    Color mCamDefaultClearColor;

    bool mDistortionNeeded = false;
    bool mEnableForegroundCamera = false;
 
    const float CaptureCamDepthOffset = -1.0f;
    const float ForegroundCamDepthOffset = 1.0f;
    const float DistortionCullingCamDepthOffset = -50.0f;

    public bool DistortionNeeded
    {
        get { return mDistortionNeeded; }
        set
        {
            if( mDistortionNeeded == value )
            {
                return;
            } 
            mDistortionNeeded = value;

            if( mDistortionNeeded && enabled )
            {
                EnableEffect();
            } else {//在非扰动模式下只使用主相机绘制
                DisableEffect();
            }
        }
    }

    public bool EnableForegroundCamera
    {
        get { return mEnableForegroundCamera; }
        set
        {

            mEnableForegroundCamera = value;
        }
    }

    void OnEnable()
    { 
        if (DistortionNeeded)
        {
            EnableEffect();
        }
    }

    void OnDisable()
    {
        DisableEffect();
    }

    void EnableEffect()
    { 
        mMainCam.clearFlags = CameraClearFlags.SolidColor;
        mMainCam.backgroundColor = new Color(0, 0, 0, 0);
        mMainCam.cullingMask = mCullingMask;
        mCaptureCam.gameObject.SetActive(true);

        mCaptureCam.Render();
    }

    void DisableEffect()
    {
        mMainCam.clearFlags = mCamDefaultClearFlags;
        mMainCam.backgroundColor = mCamDefaultClearColor;
        mMainCam.cullingMask = mCamDefaultMask; 
        mCaptureCam.gameObject.SetActive(false); 
    }

    public void EnableFullScreenColorEffect( bool e )
    {
        mEnableFullScreenColorEffect = e;
    }

    bool CanDistortion()
    {
        return mMainCam != null && mCaptureCam != null;
    }

    void Init()
    {
        mCaptureCam.backgroundColor = mMainCam.backgroundColor;
        mCaptureCam.clearFlags = mCamDefaultClearFlags;
        mCaptureCam.cullingMask = ~mCullingMask & mCamDefaultMask;
        mCaptureCam.depth = mMainCam.depth + CaptureCamDepthOffset;
        mCaptureCam.gameObject.SetActive(false);

        mForegroundCam.backgroundColor = mMainCam.backgroundColor;
        mForegroundCam.clearFlags = CameraClearFlags.Depth;
        mForegroundCam.cullingMask = foregroundLayer.value;
        mForegroundCam.depth = mMainCam.depth + ForegroundCamDepthOffset;
        mForegroundCam.gameObject.SetActive(false);

        mCullingCam.backgroundColor = Color.black;
        mCullingCam.clearFlags = CameraClearFlags.SolidColor;
        mCullingCam.cullingMask = mCullingMask;
        mCullingCam.depth = mMainCam.depth + DistortionCullingCamDepthOffset;
        mCullingCam.gameObject.SetActive(true);

        UpdateCameraParams();
    }

    void Awake()
    {
        //自动创建扰动相机材质球
        mBuffFlipMat = new Material(Shader.Find("Unlit/Transparent"));

        mCullingMask = _CullingMask();
         
        mMainCam = GetComponent<Camera>();  
        if (mCaptureCam == null)
        {
            GameObject newCam = new GameObject();
            newCam.name = "CaptureCamera";
            mCaptureCam = newCam.AddComponent<Camera>();
            mCaptureCam.transform.parent = mMainCam.transform;
            mCaptureCam.transform.localPosition = Vector3.zero;
            mCaptureCam.transform.localRotation = Quaternion.identity;
            mCaptureCam.transform.localScale = Vector3.one;

            //skybox
            newCam.AddComponent<Skybox>();
        }

        GameObject foregroundCamGo = new GameObject();
        foregroundCamGo.name = "ForegroundCamera";
        mForegroundCam = foregroundCamGo.AddComponent<Camera>();
        mForegroundCam.transform.parent = mMainCam.transform;
        mForegroundCam.transform.localPosition = Vector3.zero;
        mForegroundCam.transform.localRotation = Quaternion.identity;
        mForegroundCam.transform.localScale = Vector3.one;

        GameObject cullingCamGo = new GameObject();
        cullingCamGo.name = "Culling";
        
        mCullingCam = cullingCamGo.AddComponent<Camera>();
        cullingCamGo.AddComponent<DistortionCullingCamera>().mDistortionCam = this;
        mCullingCam.transform.parent = mMainCam.transform;
        mCullingCam.transform.localPosition = Vector3.zero;
        mCullingCam.transform.localRotation = Quaternion.identity;
        mCullingCam.transform.localScale = Vector3.one;

     
    }

	// Use this for initialization
	void Start () 
    {
        mCamDefaultMask = mMainCam.cullingMask;
        mCamDefaultClearFlags = mMainCam.clearFlags;
        mCamDefaultClearColor = mMainCam.backgroundColor;

        if (!CanDistortion())
        {
            Debug.Log("无法渲染扰动效果！");
            return;
        } 

        mRTID = Shader.PropertyToID("_CaptureRT");
        mScreenTexID = Shader.PropertyToID("_FullScreenTex");
        mScreenTexFactorID = Shader.PropertyToID("_ScreenTexFactor");

        Init(); 

        //为全屏捕捉相机创建RT
        mCaptureRT = new RenderTexture( Screen.width , Screen.height , 16 );
        mCaptureCam.targetTexture = mCaptureRT;  
	}

    int _CullingMask()
    {
        if( distortionLayerName != "" )
        {
           return LayerMask.GetMask(new string[] { distortionLayerName });
        } 
        return distortionLayer.value;
    }

    //先绘制完此帧，延迟一帧进行效果关闭操作
    bool mReqDisableEffect = false;

    void OnPreRender()
    {

    }

    void OnPostRender()
    { 
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (DistortionNeeded)
        {
            Graphics.Blit(mCaptureRT, dest);
            Graphics.Blit(src, dest, mBuffFlipMat);

            if (mCaptureRT.width != Screen.width ||
                mCaptureRT.height != Screen.height
             )
            {
                mCaptureCam.targetTexture = null;
                RenderTexture.Destroy(mCaptureRT);

                mCaptureRT = new RenderTexture(Screen.width, Screen.height, 24);
                mMainCam.aspect = mCaptureRT.width / mCaptureRT.height;
                mCaptureCam.aspect = mMainCam.aspect;
                mCaptureCam.fieldOfView = mMainCam.fieldOfView;
                mCaptureCam.targetTexture = mCaptureRT;
            }

            if (mReqDisableEffect)
            {
                DistortionNeeded = DistortionObject.DistortionNeeded;
                mReqDisableEffect = false;
            } 
        }
        else
        {
            Graphics.Blit(src, dest); 
        }


        DistortionObject.DistortionNeeded = false;
    }

    public void TriggerDistortionEffect()
    {
        if (DistortionNeeded != DistortionObject.DistortionNeeded)
        {
            if (!DistortionObject.DistortionNeeded)
            {
                mReqDisableEffect = true;
            }
            else
            {
                DistortionNeeded = DistortionObject.DistortionNeeded;
            }
        }

        if (DistortionNeeded)
        {
            Shader.SetGlobalTexture(mRTID, mCaptureRT);

            if (screenTex != null)
            {
                Shader.SetGlobalTexture(mScreenTexID, screenTex);
            }
            else
            {
                Shader.SetGlobalTexture(mScreenTexID, null);
            }

            if (mEnableFullScreenColorEffect)
            {
                Shader.SetGlobalFloat(mScreenTexFactorID, 1.0f);
            }
            else
            {
                Shader.SetGlobalFloat(mScreenTexFactorID, 0.0f);
            }
        }
    }

    void UpdateCameraParams()
    {
        mCaptureCam.aspect = mMainCam.aspect;
        mCaptureCam.fieldOfView = mMainCam.fieldOfView;
        mCaptureCam.worldToCameraMatrix = mMainCam.worldToCameraMatrix;
        mCaptureCam.farClipPlane = mMainCam.farClipPlane;
        mCaptureCam.nearClipPlane = mMainCam.nearClipPlane;

        mForegroundCam.aspect = mMainCam.aspect;
        mForegroundCam.fieldOfView = mMainCam.fieldOfView;
        mForegroundCam.worldToCameraMatrix = mMainCam.worldToCameraMatrix;
        mForegroundCam.farClipPlane = mMainCam.farClipPlane;
        mForegroundCam.nearClipPlane = mMainCam.nearClipPlane;

        mCullingCam.aspect = mMainCam.aspect;
        mCullingCam.fieldOfView = mMainCam.fieldOfView;
        mCullingCam.worldToCameraMatrix = mMainCam.worldToCameraMatrix;
        mCullingCam.farClipPlane = mMainCam.farClipPlane;
        mCullingCam.nearClipPlane = mMainCam.nearClipPlane;

        if (mMainCam.GetComponent<Skybox>() != null)
        {
            mCaptureCam.GetComponent<Skybox>().material = mMainCam.GetComponent<Skybox>().material;
        }
    }

    void Update()
    {
        _UpdateCameraCullingMask();
        UpdateCameraParams();
    }

    void LateUpdate()
    {
      
    }

    void _UpdateCameraCullingMask()
    {
        if( EnableForegroundCamera )
        {//启用
            
            if (!mForegroundCam.gameObject.activeInHierarchy)
            {
                if( DistortionNeeded )
                {

                    mMainCam.cullingMask = mCullingMask & ~foregroundLayer.value;
                    mCaptureCam.cullingMask = ~mCullingMask & mCamDefaultMask & ~foregroundLayer.value;
                }
                else
                {
                    mMainCam.cullingMask = mCamDefaultMask & ~foregroundLayer.value;
                }
                mForegroundCam.cullingMask = foregroundLayer.value; 
                mForegroundCam.gameObject.SetActive(true);
            }
                       
        }
        else
        {//禁用
            if( mForegroundCam.gameObject.activeInHierarchy )
            {
                if( DistortionNeeded )
                {
                    mMainCam.cullingMask = mCullingMask;
                    
                }
                else
                {
                    mMainCam.cullingMask = mCamDefaultMask;
                }
                mCaptureCam.cullingMask = ~mCullingMask & mCamDefaultMask;
                mForegroundCam.cullingMask = foregroundLayer.value;
                mForegroundCam.gameObject.SetActive(false);
            }
        }
    }
}

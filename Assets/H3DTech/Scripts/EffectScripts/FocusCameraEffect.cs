using UnityEngine;
using System.Collections;
using System;

public class FocusCameraEffect : MonoBehaviour 
{

    public float m_cur_brightness = 1.0f;
    public float m_target_brightness;
    public float m_brightness_offset;
    public string foreGroundLayerName = "ForeGround";

    Camera mMainCam;
    Camera mForegroundCam;
    int mMainCamCullingMask;
    int mCullingMask; 
     
    Material mBackgroundEffMat;
    Shader mFocusCamEffShader;
     
    int mDarkFactorID = -1;
    bool m_need_disable = false;

    bool mIsStarted = false;

    void Awake()
    {
        mFocusCamEffShader = Shader.Find("H3D/InGame/ScreenEffect/FocusCameraEffect");
        mDarkFactorID = Shader.PropertyToID("_Factor");
        mBackgroundEffMat = new Material(mFocusCamEffShader);

        mCullingMask = LayerMask.GetMask(new string[] { foreGroundLayerName }); 

        mMainCam = GetComponent<Camera>();

        //创建前景相机
        GameObject newCam = new GameObject();
        newCam.name = "ForeGroundCamera";
        mForegroundCam = newCam.AddComponent<Camera>();
        mForegroundCam.transform.parent = mMainCam.transform;
        mForegroundCam.transform.localPosition = Vector3.zero;
        mForegroundCam.transform.localRotation = Quaternion.identity;
        mForegroundCam.transform.localScale = Vector3.one;

        mForegroundCam.gameObject.SetActive(false);
    }

    void Start()
    {
        mIsStarted = true;
        //将MainCam的剔除掩码保存
        mMainCamCullingMask = mMainCam.cullingMask; 
    }


    void OnEnable()
    {
        if (mIsStarted)
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
        mCullingMask = LayerMask.GetMask(new string[] { foreGroundLayerName }); 
        mForegroundCam.gameObject.SetActive(true);

        mMainCam.cullingMask = ~mCullingMask & mMainCamCullingMask;  

        mForegroundCam.aspect = mMainCam.aspect;
        mForegroundCam.backgroundColor = mMainCam.backgroundColor;
        mForegroundCam.fieldOfView = mMainCam.fieldOfView;
        //清Depth前景物体不考虑背景物体的深度
        mForegroundCam.clearFlags = CameraClearFlags.Depth;
        mForegroundCam.cullingMask = mCullingMask & mMainCamCullingMask;
        mForegroundCam.depth = mMainCam.depth + 1;
    }

    void DisableEffect()
    {
        mForegroundCam.gameObject.SetActive(false);
        mMainCam.cullingMask = mMainCamCullingMask;
        m_cur_brightness = 1;
    }

	
	// Update is called once per frame
	void Update () 
    {
        //同步相机参数 
        mForegroundCam.fieldOfView = mMainCam.fieldOfView;
        mForegroundCam.aspect = mMainCam.aspect;

        if (Mathf.Abs(m_target_brightness - m_cur_brightness) > 0.001f)
        {
            m_cur_brightness += m_brightness_offset;
        }
        else 
        {
            if (m_need_disable)
            {
                base.enabled = false;
            } 
        }
        mBackgroundEffMat.SetFloat(mDarkFactorID, m_cur_brightness);
	}

    

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mBackgroundEffMat);
    }

    public void SetTargetBrightness(float target_brightness , int duration, bool need_disable)
    {
        m_target_brightness = target_brightness;
        if (duration == 0)
        {
            m_cur_brightness = m_target_brightness;
        }
        else 
        {
            m_brightness_offset = (m_target_brightness - m_cur_brightness) / duration;
        }
        m_need_disable = need_disable;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
public class TrailSection
{
    public Vector3 Pos = Vector3.zero;
    public float UpdateTime = float.PositiveInfinity;
}

[ExecuteInEditMode]
public class H3DTrailRender : MonoBehaviour
{
    [SerializeField]
#if UNITY_5_1 || UNITY_5_0
    public UnityEngine.Rendering.ShadowCastingMode m_CastShadows = UnityEngine.Rendering.ShadowCastingMode.On;
#else
    public bool m_CastShadows = true;
#endif

    [SerializeField]
    public bool m_ReceiveShadows = true;
    [SerializeField]
    public float m_Time = 5f;
    [SerializeField]
    public float m_StartWidth = 1f;
    [SerializeField]
    public float m_EndWidth = 1f;
    [SerializeField]
    public float m_MinVerDis = 0.1f;
    [SerializeField]
    public bool m_IsAutoDestruct = false;
    [SerializeField]
    public bool m_IsAlwaysFaceToCam = true;
    [SerializeField]
    public float m_RotationAngle = 0f;
    [SerializeField]
    public Material m_TrailMaterial = null;
    [SerializeField]
    public Color[] m_Colors = new Color[colorNum] { Color.white, Color.white, Color.white, Color.white, Color.white};

    private Mesh newMesh = null;
    private MeshFilter meshF = null;

    private const int colorNum = 5;
    private GameObject trailGO = null;
    private bool isFadeOuting = false;
    private bool isStart = false;
    private float startTime = float.PositiveInfinity;
    private float currTime = 0f;
    private Vector3 lastStartPos = new Vector3();
    private Vector3 originalPos = new Vector3();

    private FrameMemAllocator<Vector3> vertexAllocator = new FrameMemAllocator<Vector3>();
    private FrameMemAllocator<Color> colorAllocator = new FrameMemAllocator<Color>();
    private FrameMemAllocator<Vector2> uvAllocator = new FrameMemAllocator<Vector2>();
    private FrameMemAllocator<int> triangleAllocator = new FrameMemAllocator<int>();
    private TrailSectionAllocator sectionAllocator = new TrailSectionAllocator();

    Quaternion q0 = new Quaternion();
    Vector3 moveVec = new Vector3();
    Vector3 pointDir = new Vector3();
    Vector3 sectionNoraml = new Vector3();

    TrailSection tempSection = new TrailSection();


    public void Clear()
    {
        if(null == trailGO)
        {
            return;
        }

        InitFadeOutState();

        MeshFilter meshF = trailGO.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if(meshF != null)
        {
            if(Application.isPlaying)
            {
                if (meshF.mesh != null)
                {
                    meshF.mesh.Clear();
                    meshF.sharedMesh.Clear();
                }
            }
            else
            {
                if (meshF.sharedMesh != null)
                {
                    meshF.sharedMesh.Clear();
                }
            }
        }

        sectionAllocator.Clear();
    }

    private void Awake()
    {
        InitTrailGO();

        ResetAnimationPos();
    }

    protected virtual void Update()
    {
        currTime = Time.time;

        if(IsCommonError())
        {
            return;
        }

        CheckCurrLayer();

        FadeOutSection();

        FixSection();

        FixVertex();
    }

    public void ResetAnimationPos()
    {
        if(IsCommonError())
        {
            return;
        }

        Animation anim = gameObject.GetComponent<Animation>();
        if(anim != null)
        {
            anim[anim.clip.name].time = 0f;
            anim.Sample();
            originalPos = gameObject.transform.position;
        }

        Animator animator = gameObject.GetComponent<Animator>();
        if(animator != null)
        {
            animator.Update(0);
            originalPos = gameObject.transform.position;       
        }
    }

    void OnEnable()
    {
        InitTrailGO();

    }

    void OnDestroy()
    {
        if (trailGO != null)
        {
            DestroyImmediate(trailGO);
            trailGO = null;
        }
    }
    void OnDisable()
    {
        Clear();
    }

    public void InitTrailGO()
    {
        if(null == gameObject)
        {
            return;
        }

        newMesh = new Mesh();

        if (null == trailGO)
        {
            trailGO = new GameObject("H3DTrail");
            //trailGO.transform.parent = gameObject.transform;
            //trailGO.transform.localPosition = Vector3.zero;
            trailGO.transform.position = Vector3.zero;
            trailGO.transform.rotation = Quaternion.identity;
            trailGO.transform.localScale = Vector3.one;

            trailGO.AddComponent<MeshFilter>();
            trailGO.AddComponent<MeshRenderer>();


        }

        MeshRenderer renderTrail = trailGO.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        if (renderTrail != null)
        {
            renderTrail.material = m_TrailMaterial;
        }

        meshF = trailGO.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (meshF != null)
        {
            meshF.mesh = newMesh;
        }

        originalPos = gameObject.transform.position;

        trailGO.hideFlags = HideFlags.HideAndDontSave;
    }

    public void CheckCurrLayer()
    {
        if (
            (null == gameObject)
            || (null == trailGO)
            )
        {
            return;
        }

        if(gameObject.layer != trailGO.layer)
        {
            trailGO.layer = gameObject.layer;
        }
    }

    public void FadeOutSection()
    {
        if(sectionAllocator.DataCount < 2)
        {
            return;
        }

        if ((currTime - startTime) > m_Time)
        {
            if(!isFadeOuting)
            {
                isFadeOuting = true;
            }

            if (isFadeOuting)
            {
                for (long index = sectionAllocator.DataCount - 1; index >= 0; index--)
                {

                    if ((currTime - m_Time) >= sectionAllocator.GetData(index).UpdateTime)
                    {
                        sectionAllocator.RemoveAt(index);
                    }
                }

                if (1 == sectionAllocator.DataCount)
                {
                    sectionAllocator.Clear();
                }

                if (0 == sectionAllocator.DataCount)
                {
                    if (m_IsAutoDestruct)
                    {
                        DestructGO();
                    }
                    else
                    {
                        InitFadeOutState();
                    }
                }
            }
        }
    }

    public void FixSection()
    {
        if(IsCommonError())
        {
            return;
        }

        //InitTrailGOTrans();
        if(m_Time < 0)
        {
            Clear();
        }

        Vector3 nowPos = gameObject.transform.position;

        if (0 == sectionAllocator.DataCount)
        {
            float pointDis = (nowPos - originalPos).magnitude;
            if(!isStart)
            {
                if (pointDis > 0)
                {
                    isStart = true;
                    startTime = currTime;
                }
            }

            if (pointDis >= m_MinVerDis)
            {
                tempSection.Pos = nowPos;
                tempSection.UpdateTime = currTime;
                sectionAllocator.Add(tempSection);

                tempSection.Pos = originalPos;
                tempSection.UpdateTime = startTime;
                sectionAllocator.Add(tempSection);

                lastStartPos = nowPos;
            }
        }
        else if (sectionAllocator.DataCount >= 2)
        {
            float pointDis = (nowPos - lastStartPos).magnitude;
            if (pointDis >= m_MinVerDis)
            {
                tempSection.Pos = nowPos;
                tempSection.UpdateTime = currTime;

                sectionAllocator.Insert(0, tempSection);
                lastStartPos = nowPos;
            }
            else
            {
                tempSection.Pos = nowPos;
                tempSection.UpdateTime = currTime;

                sectionAllocator.Replace(0, tempSection);
            }
        }
    }

    public void FixVertex()
    {
        if(IsCommonError())
        {
            return;
        }

#if UNITY_EDITOR         
        if (Application.isPlaying)
        {
            meshF.mesh.Clear();
        }
        else
        {
            meshF.sharedMesh.Clear();
        }
#else
        meshF.mesh.Clear();
#endif

        long size = sectionAllocator.DataCount;
        if (size < 2)
        {
            return;
        }

        long verCount = size * 2;
        Vector3[] vertices = vertexAllocator.AllocMem(verCount);
        Color[] colors = colorAllocator.AllocMem(verCount);
        Vector2[] uv = uvAllocator.AllocMem(verCount);
        int[] triangles = triangleAllocator.AllocMem((size - 1) * 2 * 3);
  
        Color startColor = m_Colors[0];
        Color endColor = m_Colors[1];

        float mod = (float)1 / (float)(size - 1);
        float traiDis = GetTrailDis();
        for (long index = 0; index < size; index++)
        {
            TrailSection indexSection = sectionAllocator.GetData(index);
            float sectionWidth = GetTrailWidth(index, traiDis);
            pointDir = GetTrailDir(index);
            float colorSection = (colorNum - 1) * index * mod;
            if (
                  (colorSection >= 0)
                  && (colorSection < 1)
                  )
            {
                startColor = m_Colors[0];
                endColor = m_Colors[1];
            }
            else if (
                 (colorSection >= 1)
                && (colorSection < 2)
                )
            {
                startColor = m_Colors[1];
                endColor = m_Colors[2];
            }
            else if (
                 (colorSection >= 2)
                && (colorSection < 3)
                )
            {
                startColor = m_Colors[2];
                endColor = m_Colors[3];
            }
            else
            {
                startColor = m_Colors[3];
                endColor = m_Colors[4];
            }

            float colorMod = colorSection - (int)(colorSection);
            if (colorSection >= 4f)
            {
                colorMod = 1;
            }

            vertices[index * 2] = indexSection.Pos + (pointDir * sectionWidth);
            vertices[index * 2 + 1] = indexSection.Pos + (-pointDir * sectionWidth);

            //vertices[index * 2] = indexSection.Pos + (pointDir * sectionWidth) - gameObject.transform.position;
            //vertices[index * 2 + 1] = indexSection.Pos + (-pointDir * sectionWidth) - gameObject.transform.position;

            uv[index * 2].x = index * mod;
            uv[index * 2].y = 1;
            uv[index * 2 + 1].x = uv[index * 2].x;
            uv[index * 2 + 1].y = 0;

            //colors[index * 2] = Color.Lerp(startColor, endColor, colorMod);
            //colors[index * 2 + 1] = colors[index * 2];
            colors[index * 2] = startColor * (1 - colorMod) + endColor * (colorMod);
            colors[index * 2 + 1] = colors[index * 2];
        }

        for (int i = 0; i < triangles.Length / 6; i++)
        {
            if (i < size - 1)
            {
                triangles[i * 6 + 0] = i * 2;
                triangles[i * 6 + 1] = i * 2 + 1;
                triangles[i * 6 + 2] = i * 2 + 2;

                triangles[i * 6 + 3] = i * 2 + 2;
                triangles[i * 6 + 4] = i * 2 + 1;
                triangles[i * 6 + 5] = i * 2 + 3;
            }
            else
            {
                triangles[i * 6 + 0] = triangles[((size - 1) * 2) * 3 - 1];
                triangles[i * 6 + 1] = triangles[((size - 1) * 2) * 3 - 1];
                triangles[i * 6 + 2] = triangles[((size - 1) * 2) * 3 - 1];
                triangles[i * 6 + 3] = triangles[((size - 1) * 2) * 3 - 1];
                triangles[i * 6 + 4] = triangles[((size - 1) * 2) * 3 - 1];
                triangles[i * 6 + 5] = triangles[((size - 1) * 2) * 3 - 1];
            }
        }

        //尽量不要Set,先看内部缓冲区是否有空间
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            meshF.mesh.MarkDynamic();
            meshF.mesh.vertices = vertices;
            meshF.mesh.colors = colors;
            meshF.mesh.uv = uv;
            meshF.mesh.triangles = triangles;

        }
        else
        {
            meshF.sharedMesh.MarkDynamic();
            meshF.sharedMesh.vertices = vertices;
            meshF.sharedMesh.colors = colors;
            meshF.sharedMesh.uv = uv;
            meshF.sharedMesh.triangles = triangles;
        }
#else 
        meshF.mesh.MarkDynamic();
        meshF.mesh.vertices = vertices;
        meshF.mesh.colors = colors;
        meshF.mesh.uv = uv;
        meshF.mesh.triangles = triangles;
#endif
    }

    private void InitTrailGOTrans()
    {
        trailGO.transform.position = Vector3.zero;
        trailGO.transform.rotation = Quaternion.identity;
        trailGO.transform.localScale = Vector3.one;
    }
    private float GetTrailWidth(long index, float trailDis)
    {
        if(
            (null == sectionAllocator)
            || (index < 0)
            || (index >= sectionAllocator.DataCount)
            )
        {
            return float.PositiveInfinity;
        }

        float sectionWidth = 0f;
        long size = sectionAllocator.DataCount;

        if (0 == index)
        {
            sectionWidth = m_StartWidth * 0.5f;
        }
        else if ((size - 1) == index)
        {
            sectionWidth = m_EndWidth * 0.5f;
        }
        else
        {
            if (m_StartWidth != m_EndWidth)
            {
                float currDis = GetCurrentDis(index);
                float lerpMod = currDis / trailDis;
                lerpMod = (float)Math.Round((double)lerpMod, 2);
                // sectionWidth = Mathf.Lerp(m_StartWidth, m_EndWidth, 1 - lerpMod);
                sectionWidth = (m_StartWidth * (lerpMod) + m_EndWidth * (1 - lerpMod));
                sectionWidth = (float)Math.Round((double)sectionWidth, 2);
    
                sectionWidth *= 0.5f;
            }
            else
            {
                sectionWidth = m_StartWidth * 0.5f;
            }
        }

        return sectionWidth;
    }
    private Vector3 GetTrailDir(long index)
    {
        Camera mainCam = null;
        Vector3 dirVec = Vector3.zero;
        Vector3 defaultRight = Vector3.zero;
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            if (Camera.allCameras.Length > 0)
            {
                mainCam = Camera.allCameras[0];
                dirVec = mainCam.transform.forward;
                defaultRight = mainCam.transform.right;
            }
        }
        else
        {
            if (SceneView.lastActiveSceneView != null)
            {
                mainCam = SceneView.lastActiveSceneView.camera;
            }
            dirVec = Vector3.forward;
            defaultRight = Vector3.right;
        }
#else
        mainCam = Camera.main;
#endif
        if(null == mainCam)
        {
            return Vector3.zero;
        }

        long size = sectionAllocator.DataCount;

        if (index < size - 1)
        {
            moveVec = sectionAllocator.GetData(index).Pos - sectionAllocator.GetData(index + 1).Pos;
        }
        else
        {
            moveVec = sectionAllocator.GetData(index - 1).Pos - sectionAllocator.GetData(index).Pos;
        }

        if(m_IsAlwaysFaceToCam)
        {
            Vector3 camToSection = sectionAllocator.GetData(index).Pos - mainCam.transform.position;
            sectionNoraml = Vector3.Project(camToSection, moveVec) - camToSection;
            pointDir = Vector3.Cross(moveVec, sectionNoraml).normalized;
            if (pointDir == Vector3.zero)
            {
                pointDir = defaultRight;
            }
        }
        else
        {
            pointDir = -Vector3.Cross(moveVec, dirVec).normalized;
            if (pointDir == Vector3.zero)
            {
                pointDir = defaultRight;
            }
        }

        if (!m_IsAlwaysFaceToCam)
        {
            q0 = Quaternion.AngleAxis(m_RotationAngle, moveVec);
            pointDir = q0 * pointDir;
        }

        return pointDir;
    }

    private float GetTrailDis()
    {
        float dis = 0f;

        for(int index = 1; index < sectionAllocator.DataCount; index++)
        {
            dis += Vector3.Distance(sectionAllocator.GetData(index).Pos, sectionAllocator.GetData(index - 1).Pos); 
        }

        return dis;
    }

    private float GetCurrentDis(long index)
    {
        float dis = 0f;
        if(index == sectionAllocator.DataCount - 1)
        {
            return 0f;
        }

        if(0 == index)
        {
            return GetTrailDis();
        }

        for(long i = sectionAllocator.DataCount - 1; i > index; i--)
        {
            dis += Vector3.Distance(sectionAllocator.GetData(i).Pos, sectionAllocator.GetData(i - 1).Pos);
        }

        return dis;
    }
    public void UpdateTrailGOMat()
    {
        if (trailGO != null)
        {
            MeshRenderer renderTrail = trailGO.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            if (renderTrail != null)
            {
                renderTrail.material = m_TrailMaterial;
#if UNITY_5_1 || UNITY_5_0
                renderTrail.shadowCastingMode = m_CastShadows;
#else
                renderTrail.castShadows = m_CastShadows;
#endif
                renderTrail.receiveShadows = m_ReceiveShadows;
            }
        }
    }


    public void DestructGO()
    {
        Clear();
        if (trailGO != null)
        {
            Destroy(trailGO);
            trailGO = null;
        }

        if (gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void DestructTrail()
    {
        if (trailGO != null)
        {
            DestroyImmediate(trailGO);
            trailGO = null;
        }
    }
    public bool IsCommonError()
    {
        bool bRet = false;

        if(
            (null == gameObject)
            || (null == trailGO)
            || (!this.enabled)
            )
        
        {
            bRet = true;
        }

        return bRet;
    }

    private void InitFadeOutState()
    {
        isStart = false;
        isFadeOuting = false;
        if (gameObject != null)
        {
            originalPos = gameObject.transform.position;
        }
    }
}
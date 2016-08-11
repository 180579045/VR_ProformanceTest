using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]   
public class SpecialEffect : MonoBehaviour 
{

    public enum PlayStyle
    { 
        Once,
        Loop
    }
     
    //特效的子元素
    [HideInInspector] 
    public List<SpecialEffectElement> elems = new List<SpecialEffectElement>();
     
    //特效总结束时间
    [HideInInspector]
    public float totalTime; 

    //播放方式
    [HideInInspector] 
    public PlayStyle style = PlayStyle.Once;
     

    //是否在Awake时播放，默认关闭
    [HideInInspector]
    public bool playOnAwake = false;

    [HideInInspector]
    public bool supportPhysics = false;

    //绑定目标路径
    [HideInInspector]
    public string bindingTargetPath = "";

    [HideInInspector]
    public string BindingTargetPath
    {
        get
        {
            return bindingTargetPath;
        }
        set
        {
            bindingTargetPath = value;
        }
    }

    [HideInInspector] 
    public int TotalFrames
    {
        get
        {
            return Mathf.FloorToInt(totalTime / frameInterval);
        }
         
    }

    [HideInInspector]
    public float CurrPlayTime
    {
        get { return currPlayTime; }
        set
        {
            currPlayTime = value; 
            if (currPlayTime > totalTime)
            { 
                if (style == PlayStyle.Loop)
                {
                    currPlayTime = Mathf.Repeat(currPlayTime, totalTime); 
                } 
            }
            else if (currPlayTime < 0)
                currPlayTime = 0;

            float oldScale = GetSpeedScale();

            //当前播放时间意义是播放进度，故设置时应将
            //速度缩放归1
            SetSpeedScale(1.0f);
            UpdatePlayStatus();
            _SetCurrPlayTime(currPlayTime);
            Pause();
            SetSpeedScale(oldScale);

        }
    }

    
    //获取与设置当前播放帧，设置完后会暂停 
    [HideInInspector] 
    public int CurrFrame
    {
        get
        {
            return frame;
        }

        set
        {  
            frame = value;

            if (frame >= TotalFrames)
            {
                if (style == PlayStyle.Loop)
                {
                    frame %= TotalFrames;
                } 
            }else if (frame < 0)
                frame = 0;

            currPlayTime = ((float)frame) * frameInterval; 

            UpdatePlayStatus(); 
            _SetCurrPlayTime(currPlayTime);
            Pause();
        }
    }
    [HideInInspector] 
    public bool CanShow
    {
        set
        {
            canShow = value;
            foreach(var item in elems)
            {
                if(item != null)
                {
                    item.CanShow = value;
                }
            }
        }

        get
        {
            return canShow;
        }
    }
    [HideInInspector] 
    public float NormailizedTime
    {
        set
        {
            if (
                  (value < 0f)
                || (value > 1f)
                )
            {
                return;
            }

            CurrPlayTime = totalTime * value;
        }

        get
        {
            if (Mathf.Abs(totalTime - 0f) < Mathf.Epsilon)
            {
                return 0f;
            }

            return CurrPlayTime / totalTime;
        }
    }

    [HideInInspector]
    [System.NonSerialized]
    //播放状态，目前只支持播放与停止 
    private bool isPlaying = false;

    [HideInInspector]
    [System.NonSerialized]
    private float currPlayTime = 0f;

    [HideInInspector]
    [System.NonSerialized]
    private float speedScale = 1f;

    [HideInInspector]
    [System.NonSerialized]
    private bool canShow = true;

    [HideInInspector]
    [System.NonSerialized]
    private int frame = 0;
    
    //是否在启动时已经播放过了（用于PlayOnAwake选项）
    [HideInInspector]
    [System.NonSerialized]
    private bool isAwakePlayed = false;

    //在Start函数调用之前调用Play函数
    [HideInInspector]
    [System.NonSerialized]
    private bool isPlayBeforeStart = false;

    [HideInInspector]
    [System.NonSerialized]
    private bool isStarted = false;

    [HideInInspector]
    [System.NonSerialized]
    private const float frameInterval = 1f / 30f;


    //将特效绑定到BindingTargetPath路径指定的物体上。
    //若路径指定物体不存在，则返回false
    public bool BindTarget( GameObject go )
    {
        if (go == null)
            return false;

        //若绑定路径为空，则直接绑定到此GameObject上
        if (BindingTargetPath.Equals(""))
        {
            _ApplyBindTarget(go.transform);
            return true;
        }

        //略过根节点名
        string path = BindingTargetPath.Substring(BindingTargetPath.IndexOf('/') + 1);

        Transform findTrans = go.transform.Find(path);

        //若没有找到路径所指定对象，不绑定，并返回false
        if( findTrans == null )
        {
            
#if !UNITY_EDITOR
            //在客户端环境下，在没有找到绑定物体的情况下
            //需要将特效绑定到物体根节点上 
            _ApplyBindTarget(go.transform);
#endif
            return false;
        } 
        _ApplyBindTarget(findTrans);
        return true;
    }

    //将当前特效根节点绑定到指定Transform上
    private void _ApplyBindTarget( Transform trans )
    {
        Vector3 localPos = this.transform.localPosition;
        Quaternion localRotate = this.transform.localRotation;
        Vector3 localScale = this.transform.localScale;

        this.transform.parent = trans;

        this.transform.localPosition = localPos;
        this.transform.localRotation = localRotate;
        this.transform.localScale = localScale;

        //保留Prefab姿态
        //this.transform.localPosition = Vector3.zero;
        //this.transform.localRotation = Quaternion.identity;
    }

    //只在编辑器中调用
    public void PlayInEditModeInit()
    {
#if UNITY_EDITOR
        foreach (var e in elems)
        {
            //先启用当前elem所属的GameObject
            //这样避免在起始状态禁用的情况下
            //调用初始化函数失败
            e.gameObject.SetActive(true);
            e.SendMessage("_Init");
        }

        Stop(); 

#endif
    }

    void Awake()
    {   
    }
     

    void OnEnable()
    {
    }

    void OnDisable()
    {
        isAwakePlayed = false;
    }

	void Start () 
    {

        isStarted = true;
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            Stop();
            if( ( isPlayBeforeStart || playOnAwake ) && !isAwakePlayed )
            {
                Play();
                isAwakePlayed = true;
            }
        }

	}

    public bool IsPlaying()
    {
        return isPlaying;
    }

    //播放特效
    public void Play()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif
            if (!isStarted)
            {
                isPlayBeforeStart = true;
                return;
            }
#if UNITY_EDITOR
        }
#endif
        isPlaying = true; 
    }

    //暂停特效
    public void Pause()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif
            if (!isStarted)
            {
                isPlayBeforeStart = false;
                return;
            }
#if UNITY_EDITOR
        }
#endif

        isPlaying = false;
        _PauseAllElems();
    }

    //停止特效
    public void Stop()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif
            if (!isStarted)
            {
                isPlayBeforeStart = false;
                return;
            }
#if UNITY_EDITOR
        }
#endif 
        isPlaying = false;

        _ResetCurrPlayTime();
        _ShutDownAllElems();
    }
	 
	void Update () 
    {
        if (supportPhysics)
        {
            return;
        }

        UpdateSpeStatus();
	}

    void FixedUpdate()
    {
        if (!supportPhysics)
        {
            return;
        }

        UpdateSpeStatus();
    }

    private void UpdateSpeStatus()
    {
        if (!IsPlaying())
            return;

        float frameElapseTime = 0.0f;
        frameElapseTime = Time.deltaTime;
        currPlayTime += frameElapseTime * speedScale;

        UpdatePlayStatus();
    }

    private void UpdatePlayStatus()
    {
        //若从开始播放的时间流失时间已经超过播放总时间
        if (currPlayTime > totalTime)
        {
            _ResetCurrPlayTime();
            if (style == PlayStyle.Once)
            {
                Stop();
                return;
            }
            _ResetAllElems();
        }

        foreach (var e in elems)
        {
            if(e != null)
            {
                e.UpdateState(currPlayTime);
            }
        }

        foreach (var e in elems)
        {
            if(e != null)
            {
                e.UpdatePlayingState(currPlayTime);
            }
        }
    }

    public override int GetHashCode()
    {
        return bindingTargetPath.GetHashCode();
    }

    public override bool Equals(object o)
    {
        if (o == null)
            return false;

        //自反
        if (o == this)
            return true;

        if (GetType() != o.GetType())
            return false;

        SpecialEffect otherSpe = o as SpecialEffect;

        if( totalTime != otherSpe.totalTime )
            return false;

        if( style != otherSpe.style )
            return false;

        if( playOnAwake != otherSpe.playOnAwake )
            return false;

       

        if( !bindingTargetPath.Equals(otherSpe.bindingTargetPath) )
            return false;


         //元素数不想等
        if( elems.Count != otherSpe.elems.Count )
            return false;

        //元素已经通过SpecialEffectEditorUtility按广度
        //有先遍历添加，所以如果层级关系相同必然顺序一致
        for (int i = 0; i < elems.Count; i++ )
        {
            SpecialEffectElement e0 = elems[i];
            SpecialEffectElement e1 = otherSpe.elems[i];

            if (!e0.Equals(e1))
                return false;
        }


        return true;
    }


    //此函数将一个特效属性值与子元素属性值拷贝给自身。
    //此函数只允许编辑器调用，外界不许调用
    //前置条件：脚本所挂接的gameObject除特效之外的其他属性必需相等
    public bool _CopyValues( SpecialEffect o )
    {
#if UNITY_EDITOR
        if (o == null)
            return false;

        if (o == this)
            return true;

        if (GetType() != o.GetType())
            return false;

        if (elems.Count != o.elems.Count)
            return false;

        //检查底下的元素类型是否完全一样
        for (int i = 0; i < elems.Count; i++ )
        {
            if (elems[i].GetType() != o.elems[i].GetType())
                return false;
        }

        totalTime = o.totalTime;
        style = o.style;
        speedScale = o.speedScale;

        playOnAwake = o.playOnAwake;
        bindingTargetPath = o.bindingTargetPath;

        for (int i = 0; i < elems.Count; i++)
        {
            if (!elems[i]._CopyValues(o.elems[i]))
            {//此种情况禁止出现
                Debug.LogError("注意!"+"子元素拷贝失败！出现不同步的特效！拷贝操作从\""+o.gameObject.name+"\"到\""+gameObject.name+"\"");
                return false;
            }
        }

        return true;
#else
        //客户端版，此函数永远运行失败
        return false;
#endif
    }

 
    //重置起始时间戳，用于重放特效
    void _ResetCurrPlayTime()
    { 
        currPlayTime = 0f; 
    }

 

    void _PauseAllElems()
    {
        foreach( var e in elems )
        {
            if(e != null)
            {
                e.Pause();
            }
        }
    }

    void _ResetAllElems()
    {
        foreach( var e in elems )
        {
            if(e != null)
            {
                e.Reset();
            }
        }
    }

    void _ShutDownAllElems()
    {
        foreach( var e in elems )
        {
            if (null == e)
            {
                continue;
            }
            e.Stop();
            e.SetEnable(false);
        }
    }

    void _SetCurrPlayTime( float t )
    {
        foreach( var e in elems )
        {
            if(e != null)
            {
                e.SetCurrPlayTime(t);
            }
        }
    }

    public void SetSpeedScale(float scale)
    {
        if (Mathf.Abs(speedScale - scale) < Mathf.Epsilon)
        {
            return;
        }

        if (scale < -Mathf.Epsilon)          
        {
            return;
        }

        foreach(var item in elems)
        {
            item.SetSpeedScale(scale);
        }

        speedScale = scale;
    }

    public float GetSpeedScale()
    {
        return speedScale;
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SpecialEffectAnimClipItem 
{
    public SpecialEffectAnimationClip SpeAnimClip
    {
        get { return speAnimClip; }
        set { speAnimClip = value; }
    }

    public float StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }

    public float EndTime
    {
        get { return startTime + length; }
        set
        {
            startTime = value - length;
            startTime = (startTime < 0.0f ? 0.0f : startTime);
        }
    }

    public float Length
    {
        get { return length; }
        set
        {
            length = value;
            length = length < 0.0f ? 0.0f : length;
        }
    }

    public float SpeedScale
    {
        get { return speedScale; }
        set
        {
            speedScale = value;
            _UpdateSpeedScale(speedScale);
        }
    }

    public virtual void Init()
    {

    }
     

    public virtual void Attach(GameObject go)
    {

    }

    public virtual void Detach()
    {

    }  
 
    public virtual void Stop()
    {

    }

    public virtual void Update(float currTime , ISpecialEffectContext context  ,bool force = false)
    { 
    }

#if UNITY_EDITOR

    public virtual void Init_Editor()
    {

    }

    public virtual void Destory_Editor()
    {

    }

#endif
     
    protected virtual void _UpdateSpeedScale( float scale )
    {

    }

    protected bool _IsInPlayTimeInterval(float currTime)
    {
        float localTime = _Trans2LocalTime(currTime);  
        return (localTime >= 0.0f && localTime <= Length);
    }

    protected float _Trans2LocalTime(float currTime)
    {
        return currTime - StartTime;
    }

    //Item描述
    public string desc;

    //相对于绑定骨骼本地位置偏移
    public Vector3 localOffsetPos = Vector3.zero;

    //相对于绑定骨骼本地旋转
    public Quaternion localRotation = Quaternion.identity;

    //相对于绑定骨骼本地缩放
    public Vector3 localScale = Vector3.one;

    //绑定骨骼
    public string bindingTargetPath;

    //死亡类型
    public int deathType;

    //此Item起始时间
    public float startTime;

    //此Item持续时间
    public float length;

    //Item持有资源对象
    public UnityEngine.Object obj;

    //速度缩放
    [HideInInspector]
    [NonSerialized]
    protected float speedScale = 1.0f;

    [HideInInspector]
    [NonSerialized]
    //从属的特效动画
    protected SpecialEffectAnimationClip speAnimClip;

}

[Serializable]
public class SpecialEffectAnimClipEffectItem : SpecialEffectAnimClipItem
{
    public override void Init()
    {
        effPrefab = obj as GameObject;

        if (effPrefab == null)
        {
            return;
        }

        if (effInst == null)
        {
            effInst = (GameObject.Instantiate(effPrefab) as GameObject).GetComponent<SpecialEffect>();
            effInst.Stop();

            effInst.transform.localPosition = localOffsetPos;
            effInst.transform.localRotation = localRotation;
            effInst.transform.localScale = localScale;

            _ApplyBindTarget(speAnimClip.transform); 

            
#if UNITY_EDITOR
            //对于编辑器非播放模式，进行初始化
            if (!Application.isPlaying)
            { 
                effInst.PlayInEditModeInit(); 
            }
#endif
            effInst.gameObject.SetActive(false);
        }

        if( effInst != null )
        {
            length = effInst.totalTime;

        }

    }

#if UNITY_EDITOR

    public override void Init_Editor()
    {
        this.Init();
    }

    public override void Destory_Editor()
    {
        if( effInst != null )
        {
            Detach();
            GameObject.DestroyImmediate(effInst.gameObject);
            effInst = null; 
        }

    } 
#endif

    public override void Stop()
    {
        if( effInst != null )
        { 
            effInst.Stop();
            effInst.gameObject.SetActive(false);
            isPlaying = false;
        }
    }

    public override void Update(float currTime, ISpecialEffectContext context, bool force = false)
    {
        //未初始化成功则不更新
        if (effInst == null)
            return;

        if (speAnimClip.IsKilled && !force )
        {//若特效被杀死
            if (!_OnKilled())
            {
                return;
            }
        }

        float localTime = _Trans2LocalTime(currTime);

        if (_IsInPlayTimeInterval(currTime))
        {//在播放区间内对特效进行启用播放

            if (!effInst.gameObject.activeInHierarchy)
            {//在播放区间内特效一定是启动的
                effInst.gameObject.SetActive(true);
            }

            if( force )
            {//强制同步当前播放时间
                effInst.CurrPlayTime = localTime;
            }

            if (isPlaying != speAnimClip.IsPlaying)
            {//同步播放状态
                if ( speAnimClip.IsPlaying )
                {
                    effInst.CurrPlayTime = localTime;
                    effInst.Play();
                    isPlaying = true;
                }
                else
                {
                    effInst.Pause();
                    isPlaying = false;
                }
            }

        }else{ //对于区间外的特效进行停止处理 
            if (isPlaying)
            {
                effInst.Stop();
                isPlaying = false;
            } 

            if (effInst.gameObject.activeInHierarchy)
            {
                effInst.gameObject.SetActive(false);
            }
        }


    }


    public override void Attach(GameObject go)
    {
        Transform findTrans = go.transform.Find(bindingTargetPath);

        //若没有找到路径所指定对象
        if (string.IsNullOrEmpty( bindingTargetPath ) ||  findTrans == null)
        {
            _ApplyBindTarget(speAnimClip.transform);
            return;
        }
        _ApplyBindTarget(findTrans);
    }

    public override void Detach()
    {
        _ApplyBindTarget(speAnimClip.transform);
    }


    protected override void _UpdateSpeedScale(float scale)
    {
        if (effInst != null)
        {
            effInst.SetSpeedScale(scale);
        }
    }

    private bool _OnKilled()
    {
        if (deathType == 0)
        {//停止
            if (isPlaying)
            {
                effInst.Stop();
                isPlaying = false;
            }

            if (effInst.gameObject.activeInHierarchy)
            {
                effInst.gameObject.SetActive(false);
            }
            return false;
        }
        else if (deathType == 1)
        {//未播放的不播放
            if (effInst.gameObject.activeInHierarchy)
            {
                return true;
            }
            return false;
        }
        return true;
    }

    private void _ApplyBindTarget(Transform trans)
    {
        Vector3 localPos = effInst.transform.localPosition;
        Quaternion localRotate = effInst.transform.localRotation;
        Vector3 localScale = effInst.transform.localScale;

        effInst.transform.parent = trans;

        effInst.transform.localPosition = localPos;
        effInst.transform.localRotation = localRotate;
        effInst.transform.localScale = localScale;
    }

    [HideInInspector]
    [NonSerialized]
    public GameObject effPrefab;

    [HideInInspector]
    [NonSerialized]
    public SpecialEffect effInst;

    //指示是否已经调用过SpecialEffect的Play
    [HideInInspector]
    [NonSerialized]
    protected bool isPlaying = false;
}

 
[Serializable]
public class SpecialEffectAnimClipAudioItem : SpecialEffectAnimClipItem
{
    public override void Init()
    {
        audioClip = obj as AudioClip;

       if( audioClip != null )
       {
           length = audioClip.length;
       }

    }

#if UNITY_EDITOR

    public override void Init_Editor()
    {
        this.Init();
    }

    public override void Destory_Editor()
    {
        
    }

#endif

    public override void Stop()
    {
       ISpecialEffectContext context = SpeAnimClip.Context; 
       if (context == null || audioClip == null)
           return;
         
       context.Stop(SpeAnimClip.gameObject, audioClip);
       isPlaying = false; 
    }

    public override void Update(float currTime, ISpecialEffectContext context, bool force = false)
    {
        if (context == null || audioClip == null)
            return;

        if (speAnimClip.IsKilled && !force)
        {//若特效被杀死
            if (!_OnKilled())
            {
                return;
            }
        }
        
        float localTime = _Trans2LocalTime(currTime); 

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif

            if (_IsInPlayTimeInterval(currTime))
            {//在播放区间内对特效进行启用播放 
                if (isPlaying != SpeAnimClip.IsPlaying)
                {
                    if (SpeAnimClip.IsPlaying)
                    {
                        context.Play(SpeAnimClip.gameObject, audioClip, localTime);
                        isPlaying = true;
                    }
                    else
                    {
                        context.Pause(SpeAnimClip.gameObject, audioClip);
                        isPlaying = false;
                    }
                }
            }
            else
            { //对于区间外的特效进行停止处理  
                if (isPlaying)
                {
                    context.Stop(SpeAnimClip.gameObject, audioClip);
                    isPlaying = false;
                }
            }


#if UNITY_EDITOR
        }
        else
        {//非播放模式下  

            if (localTime > 0.0f && localTime <= Length)
            {//在播放区间内对特效进行启用播放 
                if( !isAudioPlayed )
                {
                    context.Play(SpeAnimClip.gameObject, audioClip, localTime);
                    isAudioPlayed = true;
                }
            }
            else
            {
                if (isAudioPlayed)
                {
                    context.Stop(SpeAnimClip.gameObject, audioClip);
                    isAudioPlayed = false;
                }
            }

        }
#endif
        
    }

 
    protected override void _UpdateSpeedScale(float scale)
    {
        ISpecialEffectContext context = SpeAnimClip.Context; 
        if (context == null || audioClip == null)
            return;

        context.SetSpeedScale(SpeAnimClip.gameObject, audioClip , scale); 
    }

    bool _OnKilled()
    {
        if (deathType == 0)
        {//停止
            if (isPlaying)
            {
                Stop();
            } 
            return false;
        }
        else if (deathType == 1)
        {//未播放的不播放 
            if ( isPlaying )
            {
                return true;
            }
            return false;
        }
        return true;
    }

    [HideInInspector]
    [NonSerialized]
    //声音片段引用
    public AudioClip audioClip;

    [HideInInspector]
    [NonSerialized]
    bool isPlaying = false;


    
#if UNITY_EDITOR
    [HideInInspector]
    [NonSerialized]
    bool isAudioPlayed = false;
#endif
}


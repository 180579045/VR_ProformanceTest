using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public interface ISpecialEffectContext
{
    void SetSpeedScale(GameObject go, AudioClip clip, float speedScale);

    void Play(GameObject go , AudioClip clip , float delaySecs );

    void Stop(GameObject go, AudioClip clip);

    void Pause(GameObject go, AudioClip clip);
}


public class SpecialEffectAnimationClip : MonoBehaviour
{

    public delegate void NotifyCallback(SpecialEffectAnimationClip clip);

    void Awake()
    {
        //初始化，让所有子项实例化自己
        _Init(); 
    }
    
    void Start()
    {

    }

    void OnEnable()
    {
        if( autoPlay )
        {
            Play();
        }
    }

    void OnDisable()
    { 
    }

    public ISpecialEffectContext Context
    {
        get { return context; }
        set { context = value; }
    }

    public float TotalTime
    {
        get { return _CalcTotalTime(); } 
    }

    public int TotalFrame
    {
        get { return Mathf.FloorToInt(_CalcTotalTime() / frameTime); }
    }

    public float CurrPlayTime
    {
        get { return currPlayTime; }
        set 
        { 
            currPlayTime = value;
            Pause();
            float oldScale = SpeedScale;
            SpeedScale = 1.0f;
            _Update(currPlayTime, true);
            SpeedScale = oldScale;
        }
    }
     
    public float NormailizedTime
    { 
        get
        {
            if( Mathf.Abs(TotalTime - 0f) < Mathf.Epsilon ) 
                return 0.0f; 
            return CurrPlayTime / TotalTime;
        } 
        set
        {
            if ( value < 0.0f|| value > 1.0f ) return; 
            CurrPlayTime = TotalTime * value;
        }
    }

    public int CurrPlayFrame
    {
        get { return Mathf.FloorToInt( currPlayTime / frameTime ) ; }
        set { CurrPlayTime = ((float)value) * frameTime; }
    }

    public float SpeedScale
    {
        get 
        { 
            return speedScale; 
        }
        set 
        {  
            speedScale = Mathf.Max( value , 0.0f );
            _UpdateSpeedScale(speedScale);
        }
    }

    public bool SupportPhysics
    {
        set
        {
            supportPhysics = value;
            SetSpeSupportPhysics(supportPhysics );
        }

        get
        {
            return supportPhysics;
        }
    }
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    public bool IsKilled
    {
        get { return isKilled; }
    }


    public void Play()
    {
        isPlaying = true;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Stop()
    {
        isKilled = false;
        isPlaying = false;
        currPlayTime = 0.0f;

        _OnStop();
    }

    public void Kill()
    {
        isKilled = true;
    }

    public void Attach( GameObject go )
    {   
        foreach( var item in itemList )
        {
            item.Attach(go);
        }
    }

    public void SetSpeSupportPhysics(bool support)
    {
        for (int index = 0; index < itemList.Count; index++)
        {
            if(
                (itemList[index] != null)
                && (itemList[index] is SpecialEffectAnimClipEffectItem)
                )
            {
                SpecialEffectAnimClipEffectItem animClipItem = itemList[index] as SpecialEffectAnimClipEffectItem;
                if(animClipItem != null)
                {
                    animClipItem.effInst.supportPhysics = support;
                }
            }
        }
    }

    public void Detach()
    {   
        foreach (var item in itemList)
        { 
            item.Detach();
        }  
    }

    void Update()
    { 
        if(supportPhysics)
        {
            return;
        }

        UpdateAniClipStatus();
    }

    void FixedUpdate()
    {
        if (!supportPhysics)
        {
            return;
        }

        UpdateAniClipStatus();
    }

    void UpdateAniClipStatus()
    {
        float frameElapseTime = 0.0f;
        frameElapseTime = Time.deltaTime * speedScale;
        if (!IsPlaying)
        {
            frameElapseTime = 0.0f;
        }
        currPlayTime += frameElapseTime;

        if (CurrPlayTime > TotalTime)
        {//播放完毕状态重置
            Stop();
        }
        _Update(currPlayTime);
    }

    void _Init()
    {
        //将持久化特效项列表誊写至ItemList
        UpdateItemList();

        foreach (var item in itemList)
        {
            item.SpeAnimClip = this;
            item.Init();
        }

        List<SpecialEffectAnimClipItem> rmList = new List<SpecialEffectAnimClipItem>();

        foreach(var item in itemList)
        {
            //特效动画项所持有的
            if( item.obj == null )
            {
                rmList.Add(item);
            }
        }

        foreach( var item in rmList )
        {
            if( item is SpecialEffectAnimClipEffectItem )
            {
                effectList.Remove(item as SpecialEffectAnimClipEffectItem);
            }
            else if (item is SpecialEffectAnimClipAudioItem)
            {
                audioList.Remove(item as SpecialEffectAnimClipAudioItem);
            }
             
            itemList.Remove(item); 
        }
    }

    void _Update( float time , bool force = false)
    {  
        foreach (var item in itemList)
        {
            item.Update(time, context, force);
        }  
    }

    float _CalcTotalTime()
    {
        float total = 0.0f;   
        foreach (var item in itemList)
        {
            total = Mathf.Max(total, item.EndTime);
        }  
        return total;
    }
        
    void _OnStop()
    {   
        foreach (var item in itemList)
        {
            item.Stop();
        }  
    }

    void _UpdateSpeedScale( float scale )
    { 
        foreach( var item in itemList )
        {
            item.SpeedScale = scale;
        } 
    }

#if UNITY_EDITOR

    public void Init_Editor()
    {
        UpdateItemList();

        foreach( var item in itemList )
        {
            item.SpeAnimClip = this;
            item.Init_Editor();
        }

        List<SpecialEffectAnimClipItem> rmList = new List<SpecialEffectAnimClipItem>(); 
        foreach (var item in itemList)
        {
            //特效动画项所持有的
            if (item.obj == null)
            {
                rmList.Add(item);
            }
        }

        foreach (var item in rmList)
        {
            if (item is SpecialEffectAnimClipEffectItem)
            {
                //Debug.LogWarning("不存在特效项" + item. + "所持有特效！此特效项已被忽略！");
                effectList.Remove(item as SpecialEffectAnimClipEffectItem);
            }
            else if (item is SpecialEffectAnimClipAudioItem)
            {
                //Debug.LogWarning("不存在音效项" + item.name + "所持有AudioClip！此音效项已被忽略！");
                audioList.Remove(item as SpecialEffectAnimClipAudioItem);
            } 
            itemList.Remove(item);
        }
    }

    public void Destory_Editor()
    {
        foreach (var item in itemList)
        { 
            item.Destory_Editor();
        } 
    }

    //为Clip增加子项
    //type: 0为spe , 1为audio
    public int AddItem( UnityEngine.Object obj )
    {  
        if (obj as AudioClip != null)
        {
            SpecialEffectAnimClipAudioItem audioItem = new SpecialEffectAnimClipAudioItem();
            audioItem.SpeAnimClip = this; 
            audioItem.obj = obj;
            audioItem.Init_Editor();
            itemList.Add(audioItem);
            UpdateSerializeItemList();
            return itemList.Count - 1;
        }

        var go = obj as GameObject;
        if (go != null)
        {
            var spe = go.GetComponent<SpecialEffect>();
            if (spe != null)
            {
                SpecialEffectAnimClipEffectItem effItem = new SpecialEffectAnimClipEffectItem();
                effItem.SpeAnimClip = this;
                effItem.obj = go; 
                effItem.Init_Editor();
                itemList.Add(effItem);
                UpdateSerializeItemList();
                return itemList.Count - 1;
            }  
        }  
        return -1;
    }

    public void InsertItem( UnityEngine.Object obj , int i )
    {
        if( obj as AudioClip != null )
        {
            SpecialEffectAnimClipAudioItem audioItem = new SpecialEffectAnimClipAudioItem();
            audioItem.SpeAnimClip = this; 
            audioItem.obj = obj;
            audioItem.Init_Editor(); 
            itemList.Insert(i, audioItem);

            UpdateSerializeItemList();
            return;
        }

       var go = obj as GameObject;
       if( go != null )
       {
           var spe = go.GetComponent<SpecialEffect>();
           if( spe != null )
           {
               SpecialEffectAnimClipEffectItem effItem = new SpecialEffectAnimClipEffectItem();
               effItem.SpeAnimClip = this;
               effItem.obj = go; 
               effItem.Init_Editor();
               itemList.Insert(i, effItem);
           }

           UpdateSerializeItemList();
       } 
    }

    public void InsertItem( SpecialEffectAnimClipItem item , int i )
    {
        if (item == null)
            return;

        item.Init_Editor();
        itemList.Insert(i, item);

        UpdateSerializeItemList();
    }




    public UnityEngine.Object GetItemObj( int i )
    {
        var item = itemList[i];
        if( item != null )
        {
            return item.obj;
        } 
        return null;
    }

    public void DeleteItem( int i )
    {
        itemList[i].Destory_Editor();
        itemList.RemoveAt(i);

        UpdateSerializeItemList();
    }
    
 

    public SpecialEffectAnimClipItem QueryItem( int i)
    {
        return itemList[i];
    }

    //将ItemList列表中的项同步只effectList与audioList
    //实际持久化的是effectList与audioList，特化列表项
    //才可持久化特化参数
    public void UpdateSerializeItemList()
    {
        audioList.Clear();
        effectList.Clear();
         
        foreach( var item in itemList )
        {
            if( item as SpecialEffectAnimClipAudioItem != null )
            {
                audioList.Add(item as SpecialEffectAnimClipAudioItem);
            }
            else if (item as SpecialEffectAnimClipEffectItem != null)
            {
                effectList.Add(item as SpecialEffectAnimClipEffectItem);
            }
        }

    }


#endif

    public void UpdateItemList()
    {
        itemList.Clear();
        foreach (var eff in effectList)
        {
            itemList.Add(eff);
        }

        foreach (var audio in audioList)
        {
            itemList.Add(audio);
        }

    }



    //在Enable时自动播放
    public bool autoPlay = false;


    public string previewAnimClipPath = "";


    public List<SpecialEffectAnimClipAudioItem> audioList = new List<SpecialEffectAnimClipAudioItem>(); 
    public List<SpecialEffectAnimClipEffectItem> effectList = new List<SpecialEffectAnimClipEffectItem>();


    //此列表仅用于运行时及编辑期
    [HideInInspector]
    [NonSerialized]
    public List<SpecialEffectAnimClipItem> itemList = new List<SpecialEffectAnimClipItem>();

    /// <summary>
    /// 回调函数
    /// </summary>
    public NotifyCallback onStop;

    

    /// <summary>
    /// 运行时数据
    /// </summary>

    //当前播放状态
    [HideInInspector]
    [NonSerialized]
    protected bool isPlaying = false;

    [HideInInspector]
    [NonSerialized]
    protected bool isKilled = false;

    //当前播放进度时间
    [HideInInspector]
    [NonSerialized]
    protected float currPlayTime = 0.0f;

    //速度缩放
    [HideInInspector]
    [NonSerialized]
    protected float speedScale = 1.0f;


    //每帧时间，用于将时间换算为帧
    [HideInInspector]
    [NonSerialized]
    protected float frameTime = 1.0f / 30.0f;

    //特效播放环境
    [HideInInspector]
    [NonSerialized]
    protected ISpecialEffectContext context = null;
 
    //是否支持物理计算
    [HideInInspector]
    [NonSerialized]
    protected bool supportPhysics = false;

}

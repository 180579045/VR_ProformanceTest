using UnityEngine;
using System.Collections;

[ExecuteInEditMode]   
public class SpecialEffectAnimation : SpecialEffectElement{

    [HideInInspector]
    [System.NonSerialized]
    private Animation anim;

    [HideInInspector]
    [System.NonSerialized]
    private string animClipName;

    [HideInInspector]
    [System.NonSerialized]
    //是否已经开始播放
    private bool isPlayed = false;

    protected override void _Init()
    {
        anim = GetComponent<Animation>();
        animClipName = anim.clip.name;
        if (anim == null)
        {
            #if UNITY_EDITOR
            Debug.Log("未成功获得动画脚本！");
            #endif
        }
    }

    protected override void _PlayImpl()
    {
        if (anim == null)
            return;
        
      /*  if (anim[animClipName] == null)
        {
            #if UNITY_EDITOR
            Debug.LogError("脚本中没有animation clip动画或者此动画是animator的动画，animation的动画必须是Legacy的属性");
            #endif
            return;
        }*/
        UpdateSpeed();

        if (!isPlayed)
        {
            anim.Play(animClipName);
            isPlayed = true;
        }
    }

    protected override void _PauseImpl()
    {
        if (anim == null)
            return;

        if (isPlayed)
        {
            anim[animClipName].speed = 0f;
        }
    }

    protected override void _ResetImpl()
    {
        if (anim == null)
            return; 

        anim.Rewind(animClipName);
        anim.Stop(animClipName);
        isPlayed = false;
    }

    private bool _IsAnimClipFinish()
    {
        AnimationState clipState = anim[animClipName];
        return clipState.enabled == false && (clipState.speed > 0.0f);
    }

    protected override void _SetCurrPlayTime(float t)
    {
        if (anim == null)
            return;
         
        float ltime = _CalcLocalTime(t); 
        if( ltime < 0 )
            return;

        if ((anim.clip.wrapMode != WrapMode.Loop) && (ltime > anim[animClipName].length))
        {
            ltime = anim[animClipName].length;
        }
        else if ((anim.clip.wrapMode == WrapMode.Loop) && (ltime > anim[animClipName].length))
        {
            ltime %= anim[animClipName].length;
        }

        anim.Play(animClipName);
        anim[animClipName].time = ltime; 

#if UNITY_EDITOR
        //在Unity Edit 模式下需调用
        if( !Application.isPlaying )
            anim.Sample();
#endif
    }

    public override void UpdateSpeed()
    {
        anim[animClipName].speed = 1f * SpeedScale;
    }
}

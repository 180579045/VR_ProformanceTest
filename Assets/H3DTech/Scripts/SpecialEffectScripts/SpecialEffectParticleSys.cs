using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
[ExecuteInEditMode]   
public class SpecialEffectParticleSys : SpecialEffectElement 
{
    [HideInInspector]
    [System.NonSerialized]
    protected ParticleSystem particleSys;

    [HideInInspector]
    [System.NonSerialized]
    private float startSpeed = 0f;

    //用于记录上一次CurrPlayTime更新时间，防止
    //粒子在运行时每次都从头模拟
    [HideInInspector]
    [System.NonSerialized]
    float prevPlayTime = 0.0f;

    protected override void _Init()
    {
        particleSys = GetComponent<ParticleSystem>();
         
        if (particleSys == null)
        {
            Debug.Log("未成功获取粒子系统，检查是否插件绑定错误！");
        }
        else
        {
            startSpeed = particleSys.playbackSpeed;
        }
    }

    protected override void _PlayImpl()
    {
        if (particleSys == null)
            return;

        UpdateSpeed();
        particleSys.Play(true);
    }

    protected override void _PauseImpl()
    {
        if (particleSys == null)
            return; 

        particleSys.Pause(true); 
    }
 
    protected override void _ResetImpl()
    {
        if (particleSys == null)
            return; 
         
        particleSys.Clear(true);
        particleSys.Simulate(0, true, true);
    }

    protected override void _SetCurrPlayTime(float t)
    {
        if (particleSys == null)
            return;

        float ltime = _CalcLocalTime(t);
        
        if( ltime < prevPlayTime )
        {
            particleSys.Clear(true);
            particleSys.Simulate(ltime, true, true);
        }
        else
        {//对于步进式调用CurrPlayTime，不再从头模拟ParticleSystem
            float dt = ltime - prevPlayTime;
            particleSys.Simulate(dt,true);
        } 
    }

    public override void UpdateSpeed()
    {
        particleSys.playbackSpeed = startSpeed * SpeedScale;
    }

    protected override void _CustomOperate(float elapseTime)
    {
       // _SetCurrPlayTime(elapseTime);

        float ltime = _CalcLocalTime(elapseTime);

        particleSys.Clear(true);
        particleSys.Simulate(ltime, true, true);

        if(!IsPlaying())
        {
            if (particleSys == null)
                return;

            particleSys.Play();
            //particleSys.Pause(true); 
        }
    }
}

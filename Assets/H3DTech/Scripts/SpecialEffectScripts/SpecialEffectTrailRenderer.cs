using UnityEngine;
using System.Collections;

public class SpecialEffectTrailRenderer : SpecialEffectElement
{

    [HideInInspector]
    [System.NonSerialized]
    protected H3DTrailRender trailRenderer;



    //用于记录上一次CurrPlayTime更新时间，防止
    //粒子在运行时每次都从头模拟
    [HideInInspector]
    [System.NonSerialized]
    float prevPlayTime = 0.0f;

    protected override void _Init()
    {
        trailRenderer = GetComponent<H3DTrailRender>();

        if (trailRenderer == null)
        {
            Debug.Log("未成功获取TrailRenderer，检查是否插件绑定错误！");
        } 
    }

    protected override void _PlayImpl()
    {
        if (trailRenderer == null)
            return;
         
    }

    protected override void _PauseImpl()
    {
        if (trailRenderer == null)
            return; 
    }

    protected override void _ResetImpl()
    {
        if (trailRenderer == null)
            return;

        trailRenderer.Clear();
    }

    protected override void _SetCurrPlayTime(float t)
    {
        if (trailRenderer == null)
            return;

        float ltime = _CalcLocalTime(t);

        if (ltime < prevPlayTime)
        {
            trailRenderer.Clear();
        }

        prevPlayTime = ltime;
    }

 
}

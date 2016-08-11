using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalLODSystem 
{ 
    public GlobalLODSystem()
    {
        currLOD = 0;
        strategyFactory = CreateLodSetStrategyFactory(); 
        UpdateGlobalLODConfig();
    }

    public int CurrLOD
    {
        get
        {
            return currLOD;
        } 
        set
        {
            currLOD = value; 
            UpdateGlobalLODConfig();
            UpdateAllLODObjects();
        }
    }
    
    public List<LodSubSystem> SubSysList
    {
        get
        {
            return subSysList;
        }
    }

    public void RegisterSubSystem( LodSubSystem sys )
    {
        if( !subSysList.Contains(sys) )
        {
            sys.CurrLOD = currLOD;
            subSysList.Add(sys);
        }
    }

    public void UnregisterSubSystem( LodSubSystem sys )
    {
        if( subSysList.Contains(sys) )
        {
            subSysList.Remove(sys);
        }
    }

    public LodSetStrategy GetLodSetStrategy( int type )
    {
        return strategyFactory.Create(type);
    }

    protected virtual void UpdateGlobalLODConfig()
    {
        if( currLOD == 0 )
        {
            QualitySettings.pixelLightCount = 1; 
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            Shader.globalMaximumLOD = 400;
        } else if( currLOD == 1) {
            QualitySettings.pixelLightCount = 0; 
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            Shader.globalMaximumLOD = 300;
        } else if (currLOD == 2) {
            QualitySettings.pixelLightCount = 0; 
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            Shader.globalMaximumLOD = 200;
        }

        //粒子射线跟踪数量限制
        QualitySettings.particleRaycastBudget = 16;

        QualitySettings.softVegetation = false;

        //关闭反射Probe
        QualitySettings.realtimeReflectionProbes = false;

        //关闭抗锯齿
        QualitySettings.antiAliasing = 0;

        //骨骼混合参考骨骼数
        QualitySettings.blendWeights = BlendWeights.TwoBones;
    }

    protected virtual void UpdateAllLODObjects()
    {
        for( int i = 0 ; i < subSysList.Count ; i++ )
        {
            subSysList[i].CurrLOD = currLOD;
        }
    } 

    protected virtual LodSetStrategyFactory CreateLodSetStrategyFactory()
    {
        return new DefaultLODSetStrategyFactory();
    }
    
    //当前LOD级别
    int currLOD = 0;

    //子系统列表
    List<LodSubSystem> subSysList = new List<LodSubSystem>();

    //策略工厂
    LodSetStrategyFactory strategyFactory;

    static GlobalLODSystem GetInstance()
    {
        if( _instance == null )
        {
            _instance = new GlobalLODSystem();
        }
        return _instance;
    }

    static GlobalLODSystem _instance;

}

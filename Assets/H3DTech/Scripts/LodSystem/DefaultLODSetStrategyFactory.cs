using UnityEngine;
using System.Collections;

public class DefaultLODSetStrategyFactory : LodSetStrategyFactory 
{ 
    enum StrategyType
    {
        SceneStatic = 1,
        SceneDynamic,
        Player,
        NPC,
        Light
    };

    public override LodSetStrategy Create(int type)
    {
        if (type == (int)StrategyType.SceneStatic)
        {
            return sceneStaticLODStrategy;
        }
        else if (type == (int)StrategyType.SceneDynamic)
        {
            return sceneDynamicLODStrategy;
        }
        else if (type == (int)StrategyType.Player)
        {
            return playerLODStrategy;
        }
        else if (type == (int)StrategyType.NPC)
        {
            return npcLODStrategy;
        }
        else if (type == (int)StrategyType.Light)
        {
            return lightLODStrategy;
        }

        return null;
    }

    DefaultSceneStaticObjectsLodSetStrategy sceneStaticLODStrategy = new DefaultSceneStaticObjectsLodSetStrategy();
    DefaultSceneDynamicObjectsLodSetStrategy sceneDynamicLODStrategy = new DefaultSceneDynamicObjectsLodSetStrategy();
    DefaultPlayerLodSetStrategy playerLODStrategy = new DefaultPlayerLodSetStrategy();
    DefaultNPCLodSetStrategy npcLODStrategy = new DefaultNPCLodSetStrategy();
    DefaultLightLodSetStrategy lightLODStrategy = new DefaultLightLodSetStrategy();
}

using UnityEngine;
using System.Collections;

public class DefaultSceneStaticObjectsLodSetStrategy : LodSetStrategy 
{ 
    public override void SetLOD(GameObject go, int lod)
    { 
        Renderer[] rendererList = go.GetComponentsInChildren<Renderer>(); 
        for( int i = 0 ; i < rendererList.Length ; i++ )
        {
            SetLOD(rendererList[i], lod);
        }
        rendererList = go.GetComponents<Renderer>();
        for( int i = 0 ; i < rendererList.Length ; i++ )
        {
            SetLOD(rendererList[i], lod);
        }
    }

    void SetLOD( Renderer r , int lod  )
    {
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //场景静态物体禁用LightProbe
        r.useLightProbes = false;
        r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        
        if (lod == 0)
        {
            r.receiveShadows = true;    
        }
        else if (lod == 1)
        {
            r.receiveShadows = false;    
        }
        else if (lod == 2)
        {
            r.receiveShadows = false;    
        }
    }
}

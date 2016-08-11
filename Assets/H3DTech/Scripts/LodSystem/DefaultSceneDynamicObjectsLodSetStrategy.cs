using UnityEngine;
using System.Collections;

public class DefaultSceneDynamicObjectsLodSetStrategy : LodSetStrategy 
{ 
    public override void SetLOD(GameObject go, int lod)
    {
        Renderer[] rendererList = go.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rendererList.Length; i++)
        {
            SetLOD(rendererList[i], lod);
        }
        rendererList = go.GetComponents<Renderer>();
        for (int i = 0; i < rendererList.Length; i++)
        {
            SetLOD(rendererList[i], lod);
        }
    }

    void SetLOD(Renderer r, int lod)
    { 
        r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off; 
        if (lod == 0)
        {
            r.receiveShadows = true; 
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            r.useLightProbes = true;
        }
        else if (lod == 1)
        {
            r.receiveShadows = false;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.useLightProbes = false;
        }
        else if (lod == 2)
        {
            r.receiveShadows = false;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.useLightProbes = false;
        }
    }
}

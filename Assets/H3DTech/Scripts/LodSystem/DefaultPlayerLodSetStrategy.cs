using UnityEngine;
using System.Collections;

public class DefaultPlayerLodSetStrategy : LodSetStrategy 
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
        r.receiveShadows = false;

        if (lod == 0)
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            r.useLightProbes = true;
        }
        else if (lod == 1)
        { 
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.useLightProbes = false;
        }
        else if (lod == 2)
        { 
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.useLightProbes = false;
        }
    }
}

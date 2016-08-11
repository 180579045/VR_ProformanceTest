using UnityEngine;
using System.Collections;

public class DistortionObject : MonoBehaviour 
{

    void OnWillRenderObject()
    {
        DistortionObject.DistortionNeeded = true;
    }

    public static bool DistortionNeeded
    {
        get { return s_distortionNeeded; }
        set { s_distortionNeeded = value; }
    }

    protected static bool s_distortionNeeded = false;
}

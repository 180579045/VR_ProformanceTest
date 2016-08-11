using UnityEngine;
using System.Collections;

public class DistortionCullingCamera : MonoBehaviour {

    public DistortionCamera mDistortionCam;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnPreRender()
    {
        mDistortionCam.TriggerDistortionEffect();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

    }


    void OnPostRender()
    {
        
    }

}

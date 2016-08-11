using UnityEngine;
using System.Collections;

public class SpecialEffectAnimClipDebugObject : MonoBehaviour 
{
    SpecialEffectDefaultContext context;

	// Use this for initialization
	void Start () 
    {
        GameObject go = new GameObject("SpecialEffectContext");
        go.AddComponent<AudioSource>();
        context = go.AddComponent<SpecialEffectDefaultContext>();

        if( clip != null )
        {
            clip.Context = context;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (clip == null)
        {
            GUILayout.Label("请设置特效动画");
            return;
        }

        if (GUILayout.Button("播放"))
        {
            clip.Play();
        }

        if (GUILayout.Button("暂停"))
        {
            clip.Pause();
        }

        if (GUILayout.Button("停止"))
        {
            clip.Stop();
        }

        if (GUILayout.Button("Kill"))
        {
            clip.Kill();
        }

        if (GUILayout.Button("SpeedDown"))
        {
            speed -= 0.1f;
            clip.SpeedScale = speed;
        }

        if (GUILayout.Button("SpeedUp"))
        {
            speed += 0.1f;
            clip.SpeedScale = speed; 
        }

        if (clipAsset != null)
        {
            if (GUILayout.Button("实例化并Play"))
            {
                GameObject go = GameObject.Instantiate(clipAsset) as GameObject;
                go.GetComponent<SpecialEffectAnimationClip>().Play();
                go.GetComponent<SpecialEffectAnimationClip>().Context = context;
            }
        }

        if( refModelPrefab != null && clipAsset != null )
        {
            if (GUILayout.Button("实例化并绑定模型"))
            {
                GameObject refModelGo = GameObject.Instantiate(refModelPrefab) as GameObject;
                GameObject go = GameObject.Instantiate(clipAsset) as GameObject;
                go.GetComponent<SpecialEffectAnimationClip>().Context = context;
                go.GetComponent<SpecialEffectAnimationClip>().Attach(refModelGo);
                go.GetComponent<SpecialEffectAnimationClip>().Play();
                clip = go.GetComponent<SpecialEffectAnimationClip>();
            }
        }

        GUILayout.BeginHorizontal();
        currFrame = (int)GUILayout.HorizontalSlider(currFrame, 0, clip.TotalFrame, GUILayout.MaxWidth(200));
        GUILayout.TextField(currFrame.ToString());
        GUILayout.EndHorizontal();
        if (GUILayout.Button("应用当前帧"))
        {
            clip.CurrPlayFrame = currFrame;
        }

        GUILayout.BeginHorizontal();
        currPlayTime = (float)GUILayout.HorizontalSlider(currPlayTime, 0.0f, clip.TotalTime, GUILayout.MaxWidth(200));
        GUILayout.TextField(currPlayTime.ToString());
        GUILayout.EndHorizontal();
        if (GUILayout.Button("应用当前设置时间"))
        {
            clip.CurrPlayTime = currPlayTime;
        }

        GUILayout.BeginHorizontal();
        normailizedTime = (float)GUILayout.HorizontalSlider(normailizedTime, 0.0f, 1.0f, GUILayout.MaxWidth(200));
        GUILayout.TextField(normailizedTime.ToString());
        GUILayout.EndHorizontal();
        if (GUILayout.Button("应用NormailizedTime"))
        {
            clip.NormailizedTime = normailizedTime;
        }
    }

    public SpecialEffectAnimationClip clip;

    public GameObject clipAsset;

    public GameObject refModelPrefab;

    int currFrame;

    float normailizedTime;

    float currPlayTime;

    float speed = 1.0f;
}

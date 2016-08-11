using UnityEngine;
using System.Collections; 

public class SpecialEffectDebugObject : MonoBehaviour {


    public SpecialEffect speTarget;
    private SpecialEffect testSpePool;
    private int currFrame = 0;

    private float speed = 1f;

    public float normailizedTime = 0f;

    void OnEnable()
    {
        if (null == testSpePool)
        {
            testSpePool = GameObject.Instantiate(speTarget) as SpecialEffect;
        }
    }

    void OnGUI()
    {

#if UNITY_EDITOR
        if (speTarget == null)
            return;
         
        if( GUILayout.Button("播放特效") )
        {
            speTarget.Play();
        }

        if( GUILayout.Button("暂停特效") )
        {
            speTarget.Pause();
        }

        if( GUILayout.Button("停止特效") )
        {
            speTarget.Stop();
        }

        if (GUILayout.Button("SpeedDown"))
        {
            speed -= 0.1f;
            speTarget.SetSpeedScale(speed);
        }

        if (GUILayout.Button("SpeedUp"))
        {
            speed += 0.1f;
            speTarget.SetSpeedScale(speed);
        }

        if (GUILayout.Button("SpeedDownToZero"))
        {
            speed = 0f;
            speTarget.SetSpeedScale(speed);
        }

        if (GUILayout.Button("ResetSpeed"))
        {
            speed = 1.0f;
            speTarget.SetSpeedScale(speed);
        }

        GUILayout.BeginHorizontal();
        currFrame = (int)GUILayout.HorizontalSlider(currFrame, 0, speTarget.TotalFrames - 1, GUILayout.MaxWidth(200));
        GUILayout.TextField(currFrame.ToString());
        GUILayout.EndHorizontal();
        if (GUILayout.Button("应用当前帧"))
        {
            speTarget.CurrFrame = currFrame;
        }

        GUILayout.BeginHorizontal();
        normailizedTime = (float)GUILayout.HorizontalSlider(normailizedTime, 0.0f, 1.0f, GUILayout.MaxWidth(200));
        GUILayout.TextField(normailizedTime.ToString());
        GUILayout.EndHorizontal();
        if (GUILayout.Button("应用NormailizedTime"))
        {
            speTarget.NormailizedTime = normailizedTime;
        }

        if (GUILayout.Button("播放镜像特效"))
        {
            if (testSpePool != null)
            {
                testSpePool.Stop();

                testSpePool.Play();
            }
        } 

        if (GUILayout.Button("停止镜像特效"))
        {
            if (testSpePool != null)
            {
                testSpePool.Stop();
            }
        }
#endif
    }
}

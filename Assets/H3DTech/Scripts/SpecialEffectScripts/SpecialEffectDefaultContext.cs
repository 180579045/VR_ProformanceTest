using UnityEngine;
using System.Collections;

public class SpecialEffectDefaultContext : MonoBehaviour , ISpecialEffectContext
{
    AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    public void SetSpeedScale(GameObject go, AudioClip clip, float speedScale)
    {
        audioSource.pitch = speedScale;
    }

    public void Play(GameObject go, AudioClip clip, float delaySecs)
    {
        audioSource.clip = clip;
        audioSource.PlayDelayed(delaySecs);
    }

    public void Stop(GameObject go, AudioClip clip)
    {
        audioSource.Stop();
    }

    public void Pause(GameObject go, AudioClip clip)
    {
        audioSource.Pause();
    }
}

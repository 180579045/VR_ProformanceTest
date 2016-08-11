using UnityEngine;
using System.Collections;

public class PlayCtrl : EditorControl 
{
    public float PlayTime
    {
        get { return playTime; }
        set { 
            if(playTime != value)
            {
                playTime = value;
                if (playTime > totalTime)
                    playTime = totalTime;
                IsForceUpdate = true;
            }
        }
    }

    public float TotalTime
    {
        get { return totalTime; }
        set { 
            totalTime = value;
            if (totalTime < 0f)
                totalTime = 0f;
            if (playTime > totalTime)
                playTime = totalTime;
        }
    }

    public bool IsPlaying
    {
        get { return playing; }
    }

    public void Play()
    {
        playing = true;
    }

    public void Pause()
    {
        playing = false;
    }

    public void Stop()
    {
        playTime = 0.0f;
        playing = false;
        IsForceUpdate = true;
    }

    override public bool Enable
    {
        get { return enable; }
        set
        {
            if (enable != value)
            {
                if(!value)
                {
                    Stop();
                }

                RequestRepaint();
            }

            enable = value;
        }
    }

    public Rect TotalRect
    {
        get { return totalRect; }
        set
        {
            totalRect = value;
        }
    }

    public float SpeedScale
    {
        get { return speedScale; }
        set
        {
            speedScale = value;
        }
    }

    public bool IsLoop
    {
        set
        {
            isLoop = value;
        }

        get
        {
            return isLoop;
        }
    }

    private Rect totalRect = new Rect();

    private float playTime = 0.0f;
    private float totalTime = 0.0f;
    private bool  playing = false;
    private float speedScale = 1f;
    private bool isLoop = true;

}

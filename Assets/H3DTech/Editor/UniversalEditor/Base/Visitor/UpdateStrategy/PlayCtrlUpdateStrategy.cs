using UnityEngine;
using System.Collections;

public class PlayCtrlUpdateStrategy : EditorUpdateStrategy 
{
    public override void Update(EditorControl c , float deltaTime )
    {
        currCtrl = c as PlayCtrl;
        if (currCtrl == null)
            return;

        if (currCtrl.IsPlaying == false)
            return;

        c.RequestRepaint();
        currCtrl.PlayTime += deltaTime * currCtrl.SpeedScale;
        if (currCtrl.PlayTime >= currCtrl.TotalTime)
        {
            if(currCtrl.IsLoop)
            {
                currCtrl.PlayTime = 0f;
            }
            else
            {
                currCtrl.PlayTime = currCtrl.TotalTime;
                currCtrl.Pause();
            }
        }

        c.frameTriggerInfo.isValueChanged = true;
       
    }

    PlayCtrl currCtrl = null;
}

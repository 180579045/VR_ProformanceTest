using UnityEngine;
using System.Collections;

public class MainViewDestroyStrategy : EditorDestroyStrategy 
{
    public override void Destroy(EditorControl c) 
    {
        currCtrl = c as MainViewCtrl;

        if( null == currCtrl )
        {
            return;
        }

        if( currCtrl.mainViewRoot != null )
        {
            GameObject.DestroyImmediate(currCtrl.mainViewRoot);
            GameObject.DestroyImmediate(currCtrl.assistantCamObj);
            currCtrl.mainViewRoot = null;
            currCtrl.assistantCamObj = null;
        }
        
    }

    MainViewCtrl currCtrl = null;
}

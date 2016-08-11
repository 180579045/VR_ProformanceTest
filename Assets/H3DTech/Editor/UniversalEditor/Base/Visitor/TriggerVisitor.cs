using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerVisitor : EditorCtrlVisitor 
{

    public List<EditorControl> triggerControls = new List<EditorControl>();

    public override void Visit(EditorControl c)
    {

        if( _IsNeedTrigger(c) )
        {
            triggerControls.Add(c);
        } 
    }

    public void Clear()
    {
        triggerControls.Clear();
    }

    private bool _IsNeedTrigger( EditorControl c )
    {
        if(c.frameTriggerInfo.isCtrlBehaveChange)
        {
            return true;
        }

        if (c.frameTriggerInfo.isClick)
        {
            return true;
        }

        if (c.frameTriggerInfo.isHover)
        {
            return true;
        }

        if (c.frameTriggerInfo.isValueChanged)
        {
            return true;
        }

        if (c.frameTriggerInfo.lastSelectItem != -1)
        {
            return true;
        }

        //add by liteng for atlas start
        if (c.frameTriggerInfo.lastSelectItemR != -1)
        {
            return true;
        }

        if (c.frameTriggerInfo.lastSelectItemRU != -1)
        {
            return true;
        }
        //add by liteng end
        if (c.frameTriggerInfo.isScroll)
        {
            return true;
        }


        if (c.frameTriggerInfo.isDraggingObjs)
        {
            return true;
        }

        if (c.frameTriggerInfo.isDropObjs)
        {
            return true;
        }

        //Add by liteng for MoveAtlas At 2014/1/4 Start 
        if (c.frameTriggerInfo.isCustomDragAccept)
        {
            return true;
        }

        if (c.frameTriggerInfo.isCustomDragAcceptCtrl)
        {
            return true; 
        }

        if (c.frameTriggerInfo.isCtrlSelectItem)
        {
            return true; 
        }

        if(c.frameTriggerInfo.isDoubleClick)
        {
            return true;
        }

        if(c.frameTriggerInfo.isOnPress)
        {
            return true;
        }

        if (c.frameTriggerInfo.isPressDown)
        {
            return true;
        }

        if (c.frameTriggerInfo.isPressUp)
        {
            return true;
        }

        if(c.frameTriggerInfo.isRPressDown)
        {
            return true;
        }

        if(c.frameTriggerInfo.isRPressUP)
        {
            return true;
        }

        if(c.frameTriggerInfo.isPlay)
        {
            return true;
        }

        if(c.frameTriggerInfo.isPause)
        {
            return true;
        }

        if(c.frameTriggerInfo.isStop)
        {
            return true;
        }

        return false;
    }
	 
}

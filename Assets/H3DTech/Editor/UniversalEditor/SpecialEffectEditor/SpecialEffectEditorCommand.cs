using UnityEngine;
using System.Collections;
using System.Collections.Generic;

  
public class SpeItemChangeCmd : IEditorCommand
{
    public static List<SpeEditorTimeLineItem> tmpItems = new List<SpeEditorTimeLineItem>();
    public static void ClearTmpItems()
    {
        tmpItems = new List<SpeEditorTimeLineItem>();
    }

    public List<TimeLineViewCtrl.ItemSelectInfo> indices = null;
    public List<SpeEditorTimeLineItem> oldTimeLineItems = new List<SpeEditorTimeLineItem>();
    public List<SpeEditorTimeLineItem> newTimeLineItems = new List<SpeEditorTimeLineItem>();

    public string Name { get { return "SpecialEffect Element Time Changed"; } }

    public bool DontSaved { get { return false; } }

    public void Execute()
    { 
        if( !SpecialEffectEditorModel.GetInstance().HasEditTarget() )
        {
            return;
        }
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();

        int indx = 0;
        foreach( var item in newTimeLineItems )
        {
            //int itemIndx = indices[i];
            //spe.SetItemTimeInfo(itemIndx, item.startTime, item.length);

            if (indices[indx].side == TimeLineViewCtrl.SIDE_LEFT)
            {
                spe.SetItemStartTime(item.BlindSpeItem, item.RealTimeLineItem.startTime);
            }
            else if (indices[indx].side == TimeLineViewCtrl.SIDE_RIGHT)
            {
                spe.SetItemPlayTime(item.BlindSpeItem, item.RealTimeLineItem.length);
            }
            else if (indices[indx].side == TimeLineViewCtrl.SIDE_MID)
            {
                spe.SetItemDelayTime(item.BlindSpeItem, item.RealTimeLineItem.startTime);
            }
            //spe.SetItemTimeInfo(item.BlindSpeItem, item.RealTimeLineItem.startTime, item.RealTimeLineItem.length);
            indx++;
        }
         
          
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();
        //值变动时刷新Inspector
        SpecialEffectEditorModel.GetInstance().NotifySelectItemChanged(); 
    }

    public void UnExecute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }
        SpecialEffectEditProxy spe = SpecialEffectEditorModel.GetInstance().GetEditTarget();

        int indx = 0;
        foreach (var item in oldTimeLineItems)
        {
            //int itemIndx = indices[i];
            //spe.SetItemTimeInfo(itemIndx, item.startTime, item.length);
            //spe.SetItemTimeInfo(item.BlindSpeItem, item.RealTimeLineItem.startTime, item.RealTimeLineItem.length);
            if (indices[indx].side == TimeLineViewCtrl.SIDE_LEFT)
            {
                spe.SetItemStartTime(item.BlindSpeItem, item.RealTimeLineItem.startTime);
            }
            else if (indices[indx].side == TimeLineViewCtrl.SIDE_RIGHT)
            {
                spe.SetItemPlayTime(item.BlindSpeItem, item.RealTimeLineItem.length);
            }
            else if (indices[indx].side == TimeLineViewCtrl.SIDE_MID)
            {
                spe.SetItemDelayTime(item.BlindSpeItem, item.RealTimeLineItem.startTime);
            }
            indx++;
        }
         
          
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();
        //值变动时刷新Inspector
        SpecialEffectEditorModel.GetInstance().NotifySelectItemChanged();
    }
}

public class SpeItemSelectChangeCmd : IEditorCommand
{
    public int oldSelection = -1;
    public int newSelection = -1;

    public string Name { get { return "SpecialEffect Select Item Change"; } }
    public bool DontSaved { get { return false; } }

    public void Execute()
    {  
        SpecialEffectEditorModel.GetInstance().SetItemSelectIndx(newSelection);
    }

    public void UnExecute()
    { 
        SpecialEffectEditorModel.GetInstance().SetItemSelectIndx(oldSelection);
    }
}

public class SpeInspectorValueChangeCmd : IEditorCommand
{
    public SpeInspectorTarget oldValue;
    public SpeInspectorTarget newValue;

    public string Name { get { return "SpecialEffect Inspector Change"; } }
    public bool DontSaved { get { return false; } }

    public void Execute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }

        SpecialEffectEditorModel.GetInstance().GetEditTarget().Style = newValue.playStyle;
        SpecialEffectEditorModel.GetInstance().GetEditTarget().PlayOnAwake = newValue.playOnAwake;
        SpecialEffectEditorModel.GetInstance().GetEditTarget().SupportPhysics = newValue.supportPhysics;

        SpecialEffectEditorModel.GetInstance().SetDirty(); 
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();
        SpecialEffectEditorModel.GetInstance().NotifySelectItemChanged();
    }

    public void UnExecute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }

        SpecialEffectEditorModel.GetInstance().GetEditTarget().Style = oldValue.playStyle;
        SpecialEffectEditorModel.GetInstance().GetEditTarget().PlayOnAwake = oldValue.playOnAwake;
        SpecialEffectEditorModel.GetInstance().GetEditTarget().SupportPhysics = oldValue.supportPhysics;

        SpecialEffectEditorModel.GetInstance().SetDirty(); 
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();
        SpecialEffectEditorModel.GetInstance().NotifySelectItemChanged();
    }
}

public class VirtualSceneInspectorValueChangeCmd : IEditorCommand
{
    public VirturalSceneInspectorTarget oldValue;
    public VirturalSceneInspectorTarget newValue;

    public string Name { get { return "VirtualScene Inspector Change"; } }
    public bool DontSaved { get { return false; } }

    public void Execute()
    {
        if (null == SpecialEffectEditorModel.GetInstance().GetVirturalScene())
        {
            return;
        }

        SpecialEffectEditorModel.GetInstance().SetVirtualSceneVisiable(newValue.sceneVisiable);
        SpecialEffectEditorModel.GetInstance().SetGridMeshVisiable(newValue.gridMeshVisiable);
        SpecialEffectEditorModel.GetInstance().SetVirturalSceneScale(newValue.scale);
    }

    public void UnExecute()
    {
        if (null == SpecialEffectEditorModel.GetInstance().GetVirturalScene())
        {
            return;
        }

        SpecialEffectEditorModel.GetInstance().SetVirtualSceneVisiable(oldValue.sceneVisiable);
        SpecialEffectEditorModel.GetInstance().SetGridMeshVisiable(oldValue.gridMeshVisiable);
        SpecialEffectEditorModel.GetInstance().SetVirturalSceneScale(oldValue.scale);

    }
}

public class SpeElemInspectorValueChangeCmd : IEditorCommand
{
    public SpeElemInspectorTarget oldValue;
    public SpeElemInspectorTarget newValue;
    public int changeType = 0;
    public string Name { get { return "SpecialEffectElement Inspector Change"; } }
    public bool DontSaved { get { return false; } }

    public void Execute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }


        int ielem = newValue.selectItem;

        if (changeType == SpeElemInspectorTarget.STARTTIME_CHANGE)
        {
            SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemStartTime(ielem, newValue.startTime);
        }
        else if (changeType == SpeElemInspectorTarget.PLAYTIME_CHANGE)
        {
            SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemPlayTime(ielem, newValue.length);
        }
        else if (changeType == SpeElemInspectorTarget.DELAYTIME_CHANGE)
        {
            SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemDelayTime(ielem, newValue.startTime);
        }
        SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemStateInfo(ielem, newValue.isLoop);
        //SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemTimeInfo(ielem, newValue.startTime, newValue.length);

        SpecialEffectEditorModel.GetInstance().SetDirty(); 
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();
        SpecialEffectEditorModel.GetInstance().NotifySelectItemChanged();
    }

    public void UnExecute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }

        int ielem = oldValue.selectItem;
        if (changeType == SpeElemInspectorTarget.STARTTIME_CHANGE)
        {
            SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemStartTime(ielem, oldValue.startTime);
        }
        else if (changeType == SpeElemInspectorTarget.PLAYTIME_CHANGE)
        {
            SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemPlayTime(ielem, oldValue.length);
        }
        else if (changeType == SpeElemInspectorTarget.DELAYTIME_CHANGE)
        {
            SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemDelayTime(ielem, oldValue.delayTime);
        }
        //SpecialEffectEditorModel.GetInstance().GetEditTarget().SetItemStateInfo(ielem, oldValue.isLoop);
  
        SpecialEffectEditorModel.GetInstance().SetDirty(); 
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange();
        SpecialEffectEditorModel.GetInstance().NotifySelectItemChanged();
    }
}

public class SpeBindingTargetChangeCmd : IEditorCommand
{ 
    public string oldPath;
    public string newPath;

    public string Name { get { return "SpecialEffect Bind Target Change"; } }

    public bool DontSaved { get { return false; } }

    public void Execute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }

        SpecialEffectEditorModel.GetInstance().GetEditTarget().BindingTargetPath = newPath; 

        SpecialEffectEditorModel.GetInstance().SetDirty(); 
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange(); 
    }

    public void UnExecute()
    {
        if (!SpecialEffectEditorModel.GetInstance().HasEditTarget())
        {
            return;
        }

        SpecialEffectEditorModel.GetInstance().GetEditTarget().BindingTargetPath = oldPath; 

        SpecialEffectEditorModel.GetInstance().SetDirty(); 
        SpecialEffectEditorModel.GetInstance().NotifyEditTargetValueChange(); 
    }

 
}
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
public class InspectorRenderStrategy : EditorRenderStrategy
{ 

    public override bool PreVisit(EditorControl c) 
    {
        EditorGUILayout.BeginVertical(c.GetStyle(), c.GetOptions()); 
        return true;
    }

    public override void AfterVisit(EditorControl c) 
    {
        EditorGUILayout.EndVertical(); 
    }

    public override void Visit(EditorControl c) 
    { 
        currCtrl = c as InspectorViewCtrl;

        if (
               (null == currCtrl)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        if (currCtrl.onInspector != null)
        {
            currCtrl.onInspector(c, currCtrl.editTarget);
        }

        EditorGUI.EndDisabledGroup();

    }
     
    public override void AfterVisitChildren(EditorControl c)
    {
        currCtrl = c as InspectorViewCtrl;
        if (null == currCtrl)
        {
            return;
        }

        currCtrl.UpdateLastRect();

        HandleMouseInput(currCtrl);
    }

    private void HandleMouseInput(EditorControl c)
    {
        currCtrl = c as InspectorViewCtrl;
        if (
               (null == currCtrl)
            || !currCtrl.IsCurrentCtrlEnable()
            || currCtrl.IsEventTriggered()
            )
        {
            return;
        }

        CheckInputEvent(c);

        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);

        if (
                   FrameInputInfo.GetInstance().leftBtnDoubleClick
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isDoubleClick = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnClick
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isClick = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnOnPress
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isOnPress = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPress
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isPressDown = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
        else if (
                   FrameInputInfo.GetInstance().leftBtnPressUp
                && currCtrl.LastRect.Contains(localMousePos)
            )
        {
            currCtrl.frameTriggerInfo.isPressUp = true;
            currCtrl.ClickObject = null;

            currCtrl.RequestRepaint();
        }
    }

    private InspectorViewCtrl currCtrl = null;
}

public class SpeInspectorTarget
{
    public void Set( SpecialEffectEditProxy spe )
    {
        if( spe == null )
        {
            Reset();
            return;
        }
        name = spe.Name;
        bindTargetPath = spe.BindingTargetPath;
        totalTime = spe.TotalTime;
        elemNum = spe.GetItemCount();
        playStyle = spe.Style;
        playOnAwake = spe.PlayOnAwake;
        supportPhysics = spe.SupportPhysics;
    }

    public void Reset()
    {
        name = "";
        bindTargetPath = "";
        totalTime = 0.0f;
        elemNum = 0;
        playStyle = SpecialEffect.PlayStyle.Once;
        playOnAwake = false;
        supportPhysics = false;
    }

    public SpeInspectorTarget Copy()
    {
        return this.MemberwiseClone() as SpeInspectorTarget;
    }

    public string name;
    public string bindTargetPath;
    public float totalTime;
    public int elemNum;
    public SpecialEffect.PlayStyle playStyle;
    public bool playOnAwake;
    public bool supportPhysics;
}

public class SpeElemInspectorTarget
{
   public void Set( SpecialEffectEditProxy e , int sel)
   {
        if( e == null )
        {
            selectItem = -1;
            return;
        }

        selectItem = sel;
        e.GetItemName(sel, ref name);
        e.GetItemTimeInfo(sel, ref startTime, ref length);
        e.GetItemStateInfo(sel, ref isLoop);
        e.GetItemDelayTime(sel, ref delayTime);
   }

   public void Set(SpecialEffectEditProxy e, SpecialEffectElement elem)
   {
       for (int index = 0; index < e.RealSpe.elems.Count; index++)
       {
           if (UnityEngine.Object.ReferenceEquals(e.RealSpe.elems[index], elem))
           {
               Set(e, index);
               break;
           }
       }
   }
   public SpeElemInspectorTarget Copy()
   {
       return this.MemberwiseClone() as SpeElemInspectorTarget;
   }


   public int selectItem = -1;
   public string name;
   public float startTime;
   public float length;
   public float delayTime;
   public bool isLoop = false;

   public const int STARTTIME_CHANGE = 1;
   public const int DELAYTIME_CHANGE = 2;
   public const int PLAYTIME_CHANGE = 3;
}

public class VirturalSceneInspectorTarget
{
    public void Set(GameObject sceneGO, bool gridVisiable)
    {
        if(null == sceneGO)
        {
            return;
        }

        name = sceneGO.name;
        sceneVisiable = sceneGO.activeSelf;
        scale = new Vector2(sceneGO.transform.localScale.x, sceneGO.transform.localScale.y);
        gridMeshVisiable = gridVisiable;
    }

    public VirturalSceneInspectorTarget Copy()
    {
        return this.MemberwiseClone() as VirturalSceneInspectorTarget;
    }

    public string name = string.Empty;
    public bool sceneVisiable = true;
    public bool gridMeshVisiable = true;
    public Vector2 scale = new Vector2();
}

public class SpecialEffectEditorInspectorRenderDelegate
{
    public static void OnSpeInspector( EditorControl c , object target )
    {
        if (c == null || target == null)
            return;

        SpeInspectorTarget spe = target as SpeInspectorTarget;

        if (spe == null)
            return;


        bool isValueChange = false;

        GUILayout.Space(10f); 
        EditorGUILayout.LabelField("特效名", spe.name);
        EditorGUILayout.LabelField("绑定路径",spe.bindTargetPath);
        EditorGUILayout.LabelField("总时长", spe.totalTime.ToString("f2"));
        EditorGUILayout.LabelField("元素数", spe.elemNum.ToString());
        SpecialEffect.PlayStyle newPlayStyle = 
        (SpecialEffect.PlayStyle)EditorGUILayout.EnumPopup("播放方式", (System.Enum)spe.playStyle ); 
        

        if (newPlayStyle != spe.playStyle)
        {
            spe.playStyle = newPlayStyle;
            isValueChange = true;
        }
        
        bool newPlayOnAwake = EditorGUILayout.Toggle("在唤醒时播放", spe.playOnAwake );
        if( newPlayOnAwake != spe.playOnAwake )
        {
            spe.playOnAwake = newPlayOnAwake;
            isValueChange = true;
        }

        bool newSupplyPhysicalCalc = EditorGUILayout.Toggle("支持物理计算", spe.supportPhysics);
        if (newSupplyPhysicalCalc != spe.supportPhysics)
        {
            spe.supportPhysics = newSupplyPhysicalCalc;
            isValueChange = true;
        }

        GUILayout.Space(10f);

        if( isValueChange )
        {
            c.frameTriggerInfo.isValueChanged = true;
        }
    }

    public static void OnSpeElemInspector( EditorControl c , object target )
    {
        if (c == null || target == null)
            return;

        SpeElemInspectorTarget elem = target as SpeElemInspectorTarget;
        
        if (elem == null)
            return;

        if (elem.selectItem == -1)
            return;
        

        GUILayout.Space(10f); 
        bool isValueChange = false;

        EditorGUILayout.LabelField("特效元素",elem.name );  
        bool isLoop = EditorGUILayout.Toggle("循环", elem.isLoop);

        if( isLoop != elem.isLoop )
        {
            elem.isLoop = isLoop;
            isValueChange = true;
        }

        //EditorGUILayout.LabelField("起始时间", elem.startTime.ToString("f2"));
        //float newStartTime = EditorGUILayout.FloatField("起始时间", (float)Math.Round((double)elem.startTime, 2));
        float newStartTime = EditorGUILayout.FloatField("起始时间", elem.startTime);
        if (newStartTime < 0.01f)
        {
            newStartTime = 0;
        }
        if (newStartTime != elem.startTime)
        {
            elem.startTime = newStartTime;
            isValueChange = true;
            c.CurrValue = SpeElemInspectorTarget.STARTTIME_CHANGE;
        }

        //float newDelayTime = EditorGUILayout.FloatField("延迟时间", (float)Math.Round((double)elem.delayTime, 2));
        float newDelayTime = EditorGUILayout.FloatField("延迟时间", elem.delayTime);
        if (newDelayTime < 0.01f)
        {
            newDelayTime = 0;
        }
        if (newDelayTime != elem.delayTime)
        {
            //elem.delayTime = newDelayTime;
            float tempStartTime = (float)Math.Round((double)(elem.startTime + (newDelayTime - elem.delayTime)), 2);
            elem.startTime = tempStartTime;
            isValueChange = true;
            c.CurrValue = SpeElemInspectorTarget.DELAYTIME_CHANGE;

        }

        //EditorGUILayout.LabelField("播放时长", elem.length.ToString("f2"));
        //float newPlayTime = EditorGUILayout.FloatField("播放时长", (float)Math.Round((double)elem.length, 2));
        float newPlayTime = EditorGUILayout.FloatField("播放时长", elem.length);
        if (newPlayTime < 0.01f)
        {
            newPlayTime = 0;
        }
        if (newPlayTime != elem.length)
        {
            elem.length = newPlayTime;
            isValueChange = true;
            c.CurrValue = SpeElemInspectorTarget.PLAYTIME_CHANGE;

        }

        //EditorGUILayout.LabelField("结束时间", (elem.startTime + elem.length).ToString("f2"));
        //float newEndTime = EditorGUILayout.FloatField("结束时间", (float)Math.Round((double)(elem.startTime + elem.length), 2));
        float oldEndTime = (float)Math.Round((double)(elem.startTime + elem.length), 2);
        float newEndTime = EditorGUILayout.FloatField("结束时间", oldEndTime);
        if (newEndTime < 0.01f)
        {
            newEndTime = 0;
        }           
        newEndTime = (float)Math.Round((double)(newEndTime), 2);
        if (newEndTime != oldEndTime)
        {
            elem.length = (float)Math.Round((double)(newEndTime - elem.startTime), 2);
            isValueChange = true;
            c.CurrValue = SpeElemInspectorTarget.PLAYTIME_CHANGE;
        }

        GUILayout.Space(10f);

        if( isValueChange )
        {
            c.frameTriggerInfo.isValueChanged = true;
        }
          
    }

    public static void OnVirtualSceneInspector(EditorControl c, object target)
    {
        if(
            (null == c)
            )
        {
            return;
        }

        VirturalSceneInspectorTarget scene = target as VirturalSceneInspectorTarget;

        if(null == scene)
        {
            return;
        }

        if(string.IsNullOrEmpty(scene.name))
        {
            return;
        }

        GUILayout.Space(10f);
        bool isValueChange = false;

        EditorGUILayout.LabelField("虚拟场景:", scene.name);
     
        GUILayout.Space(5f);

        bool isSceneVisalbe = EditorGUILayout.Toggle("是否显示场景", scene.sceneVisiable);

        if (isSceneVisalbe != scene.sceneVisiable)
        {
            scene.sceneVisiable = isSceneVisalbe;
            isValueChange = true;
        }
        GUILayout.Space(5f);

        bool isGridMeshVisable = EditorGUILayout.Toggle("是否显示网格", scene.gridMeshVisiable);

        if(isGridMeshVisable != scene.gridMeshVisiable)
        {
            scene.gridMeshVisiable = isGridMeshVisable;
            isValueChange = true;
        }

        GUILayout.Space(5f);

        EditorGUILayout.LabelField("缩放尺寸:");
        //EditorGUILayout.LabelField();
        float xScale = EditorGUILayout.FloatField("X:", scene.scale.x, GUILayout.Width(200));
        if (
                (xScale != scene.scale.x)
                && (xScale > 0)
            )
        {
            scene.scale.x = xScale;
            isValueChange = true;
        }

        //EditorGUILayout.LabelField("Y:");
        float yScale = EditorGUILayout.FloatField("Y:", scene.scale.y, GUILayout.Width(200));
        if (
                (yScale != scene.scale.y)
                && (yScale > 0)
            )
        {
            scene.scale.y = yScale;
            isValueChange = true;
        }

        GUILayout.Space(10f);

        if (isValueChange)
        {
            c.frameTriggerInfo.isValueChanged = true;
        }
    }


}

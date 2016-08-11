using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class PlayCtrlRenderStrategy : FocusRenderStrategy 
{
    public override void Visit(EditorControl c)
    {
        currCtrl = c as PlayCtrl;

        if (
               (null == currCtrl)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        EditorGUILayout.BeginHorizontal();
        float newPlayTime = 0.0f;
        try
        {
            GUI.SetNextControlName(currCtrl.CtrlID);
            newPlayTime =
            EditorGUILayout.Slider(currCtrl.PlayTime, 0.0f, currCtrl.TotalTime,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20f), GUILayout.MinWidth(300f) });

        }catch(Exception e )
        {
            e.GetType();
            //Debug.Log(e.Message);
        }

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);

        //若鼠标在播放条发生点按事件,暂停播放
        if (c.LastRect.Contains(CalcLocalPos(c, FrameInputInfo.GetInstance().currPos))) 
        {
            if( 
                FrameInputInfo.GetInstance().leftButtonDown && 
                FrameInputInfo.GetInstance().leftBtnPress &&
                c.IsCurrentCtrlEnable()
                )
            {
                currCtrl.Pause();
            }
        }

        if( !currCtrl.IsPlaying )
        {
            if( Mathf.Abs( currCtrl.PlayTime - newPlayTime ) > Mathf.Epsilon )
            {
                currCtrl.frameTriggerInfo.isValueChanged = true;
            }
            currCtrl.PlayTime = newPlayTime;
            currCtrl.IsForceUpdate = false;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        GUILayoutOption[] btnOptions = new GUILayoutOption[] { 
            GUILayout.Width(40),GUILayout.Height(20)
        }; 

        if(!currCtrl.IsPlaying)
        {
            if (GUILayout.Button("播放", btnOptions))
            {
                currCtrl.Play();
                currCtrl.frameTriggerInfo.isValueChanged = true;
                currCtrl.frameTriggerInfo.isPlay = true;
            }
        }
        else
        {
            if (GUILayout.Button("暂停", btnOptions))
            {
                currCtrl.Pause();
                currCtrl.frameTriggerInfo.isValueChanged = true;
                currCtrl.frameTriggerInfo.isPause = true;
            }
        }
        //if( GUILayout.Button("播放", btnOptions) )
        //{
        //    currCtrl.Play();
        //    currCtrl.frameTriggerInfo.isValueChanged = true;
        //    currCtrl.frameTriggerInfo.isPlay = true;
        //}
        //if( GUILayout.Button("暂停", btnOptions) )
        //{
        //    currCtrl.Pause();
        //    currCtrl.frameTriggerInfo.isValueChanged = true;
        //    currCtrl.frameTriggerInfo.isPause = true;
        //}
        if( GUILayout.Button("停止", btnOptions) )
        {
            currCtrl.Stop();
            currCtrl.frameTriggerInfo.isValueChanged = true;
            currCtrl.frameTriggerInfo.isStop = true;
        }

        EditorGUILayout.LabelField("Loop", GUILayout.Width(30f));
        currCtrl.IsLoop = EditorGUILayout.Toggle(currCtrl.IsLoop, GUILayout.Width(20f));

        EditorGUI.EndDisabledGroup();


        EditorGUILayout.EndHorizontal();

        SpecialEffectEditorUtility.GetLastRect(ref totalRect);

        CheckInputEvent(c, totalRect);
    }

    public void CheckInputEvent(EditorControl c, Rect totalRect)
    {
        if (
               (null == c)
            )
        {
            return;
        }

        Vector2 localMousePos = CalcLocalPos(c, FrameInputInfo.GetInstance().currPos);

        if (
               FrameInputInfo.GetInstance().hasInput
            && totalRect.Contains(localMousePos)
            )
        {
            c.frameTriggerInfo.isHandleInput = true;
        }
    }

    PlayCtrl currCtrl = null;
    Rect totalRect = new Rect();

}

using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
public class CurveEditorRenderStrategy : EditorRenderStrategy
{

    public override void Visit(EditorControl c)
    {
        currCtrl = c as CurveEditorCtrl;
        if (
               (null == currCtrl)
            || (null == currCtrl.CurrValue)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!currCtrl.Enable);

        if (string.IsNullOrEmpty(currCtrl.Caption))
        {
            currCtrl.CurrValue = EditorGUILayout.CurveField((AnimationCurve)currCtrl.CurrValue, currCtrl.CurveColor, currCtrl.CurveRange, currCtrl.GetOptions());
        }
        else
        {
            currCtrl.CurrValue = EditorGUILayout.CurveField(currCtrl.Caption, (AnimationCurve)currCtrl.CurrValue, currCtrl.CurveColor, currCtrl.CurveRange, currCtrl.GetOptions());
        }

        if (IsCurveChange())
        {
            c.frameTriggerInfo.isValueChanged = true;
        }

        UpdateLastCurve();

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }

    bool IsCurveKeysEqual(Keyframe[] keyFrame1, Keyframe[] keyFrame2)
    {
        bool bRet = true;

        if(
               (null == keyFrame1)
            || (null == keyFrame2)
            || (keyFrame1.Length != keyFrame2.Length)
            )
        {
            return false;
        }

        for (int index = 0; index < keyFrame1.Length; index++)
        {
            object thisResult, thatResult;
            FieldInfo[] thisFields = typeof(Keyframe).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach(var item in thisFields)
            {
                thisResult = item.GetValue(keyFrame1[index]);
                thatResult = item.GetValue(keyFrame2[index]);

                if(!thisResult.Equals(thatResult))
                {
                    bRet = false;
                    break;
                }
            }

            if(false == bRet)
            {
                break;
            }
        }

        return bRet;
    }

    bool IsCurveChange()
    {
        bool bRet = false;


        if(
               (null == lastCurve)
            || (null == currCtrl.CurrValue)
            )
        {
            return false;
        }

        if(
               (lastCurve.postWrapMode != ((AnimationCurve)currCtrl.CurrValue).postWrapMode)
            || (lastCurve.preWrapMode != ((AnimationCurve)currCtrl.CurrValue).preWrapMode)
            )
        {
            return true;
        }

        bRet = !IsCurveKeysEqual(lastCurve.keys, ((AnimationCurve)currCtrl.CurrValue).keys);

        return bRet;
    }

    void UpdateLastCurve()
    {
        object temp = ((AnimationCurve)currCtrl.CurrValue).keys.Clone();
        Keyframe[] lastCurveKey = (Keyframe[])temp;
        
        lastCurve = new AnimationCurve(lastCurveKey);
        lastCurve.postWrapMode = ((AnimationCurve)currCtrl.CurrValue).postWrapMode;
        lastCurve.preWrapMode = ((AnimationCurve)currCtrl.CurrValue).preWrapMode;
    }

    private CurveEditorCtrl currCtrl;
    //private Keyframe[] lastCurveKey = null;
    private AnimationCurve lastCurve = null;
}
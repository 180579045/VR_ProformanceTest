using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class SpecialEffectEditorUtility  
{ 

    public static bool GetLastRect(ref Rect lastRect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            lastRect = GUILayoutUtility.GetLastRect();
            return true;
        } 
        return false; 
    }

    public static void EnableSpecialEffect( SpecialEffect spe ,bool enable = true )
    {
        spe.gameObject.SetActive(enable); 
        foreach (var e in spe.elems)
        {
            e.SetEnable(enable);
        }
    }

    public static void MarkSpecialEffectDirty( SpecialEffect spe )
    {
        if (Application.isPlaying)
            return; 
         
        foreach (var e in spe.elems)
        {
            EditorUtility.SetDirty(e);
        }
        EditorUtility.SetDirty(spe);
    }


    public static void UndoRecordSpecialEffect( IEditorCommand cmd, SpecialEffect spe )
    {
        if (Application.isPlaying)
            return;

        List<UnityEngine.Object> objList = new List<UnityEngine.Object>();

        foreach (var e in spe.elems)
        {
            objList.Add(e);
        }
        objList.Add(spe);

#if UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1
        Undo.RegisterCompleteObjectUndo(objList.ToArray(), cmd.Name);
#else 
        Undo.RecordObjects(objList.ToArray(), cmd.Name);
#endif
    }

    //判断特效脚本属性值是否相等
    //前置条件：脚本所挂接的gameObject除特效之外的其他属性必需相等
    public static bool IsSpecialEffectValueEqual( SpecialEffect lhs , SpecialEffect rhs)
    {
        return lhs.Equals(rhs); 
    }

    //将src脚本值拷贝至dst中
    //前置条件：脚本所挂接的gameObject除特效之外的其他属性必需相等
    public static bool CopySpecialEffectValues( SpecialEffect src , SpecialEffect dst )
    {
        return dst._CopyValues(src);
    }

    //获取当前GameObject所属的特效
    public static SpecialEffect GetBelongSpecialEffect( GameObject o )
    {
        if (o == null)
            return null;

        GameObject go = o;
        while( go != null )
        {
            SpecialEffect spe = go.GetComponent<SpecialEffect>();
            if (spe != null)
                return spe;

            //继续向上查找
            if( null != go.transform.parent )
            {
                go = go.transform.parent.gameObject;
            }
            else
            {
                go = null;
            }
        }
         
        return null;
    }

    

    //刷新特效下的所有特效元素，如果元素没有绑定特定
    //的特效元素脚本会自动绑定，不对的脚本也会自动调整。
    public static void RefreshSpecialEffect(GameObject go)
    {
        if (go == null)
            return;

        SpecialEffect spe = go.GetComponent<SpecialEffect>();
        
        if (spe == null)
            return;

        spe.elems.Clear();

        Queue<Transform> q = new Queue<Transform>();
        q.Enqueue(go.transform);

        while(q.Count > 0)
        {
            Transform t = q.Dequeue();
            for(int i = 0 ; i < t.childCount ; i++)
            {
	            Transform child = t.GetChild(i);
                TryAddSpecialElemComponent(child.gameObject);
                spe.elems.Add(child.gameObject.GetComponent<SpecialEffectElement>());
                q.Enqueue(child);
            }
        }

        //spe.UpdateElemNepotism();
    }

    private static bool TryAddSpecialElemComponent(GameObject go )
    {
        if (go == null)
            return false;

        SpecialEffect spe = go.GetComponent<SpecialEffect>();

        //特效根节点
        if (spe != null)
            return false;

        SpecialEffectElement speElem = go.GetComponent<SpecialEffectElement>();

        if( speElem != null )
        {
            //若绑定脚本非法先卸除，重新绑定
            if( !IsSpecialEffectElementLegal(go) )
            {
                UnityEngine.Object.DestroyImmediate(speElem);
                speElem = null;
            }
        }
       
        if (speElem == null)
        {
            //ParticleSystem p = go.GetComponent<ParticleSystem>();

            if (go.GetComponent<ParticleSystem>() != null )
            {
                go.AddComponent<SpecialEffectParticleSys>();
            }
            else if (go.GetComponent<Animation>() != null )
            {
                go.AddComponent<SpecialEffectAnimation>();
            }
            else if(go.GetComponent<Animator>() != null )
            {
                go.AddComponent<SpecialEffectAnimator>();
            }
            else if(go.GetComponent<H3DTrailRender>() != null )
            {
                go.AddComponent<SpecialEffectTrailRenderer>();
            }
            else
            {
                go.AddComponent<SpecialEffectElement>();
            } 
        }

#if UNITY_5_0 || UNITY_5_1
        Animation anim = go.GetComponent<Animation>(); 
        if (
               (anim != null)
            && (anim.clip != null)
            )
        {
            anim.clip.legacy = true;
        }
#endif

        return true;
    }

    private static bool IsSpecialEffectElementLegal( GameObject go )
    {
        if (go == null)
            return false;

        SpecialEffect spe = go.GetComponent<SpecialEffect>();

        //特效根节点
        if (spe != null)
            return false;

        SpecialEffectElement speElem = go.GetComponent<SpecialEffectElement>();
        Type elemType = speElem.GetType();

        if (elemType == typeof(SpecialEffectParticleSys))
        {
            ParticleSystem particleSys = go.GetComponent<ParticleSystem>();
            if (particleSys == null)
                return false;
        }
        else if (elemType == typeof(SpecialEffectAnimation))
        {
            Animation anim = go.GetComponent<Animation>();
            if (anim == null)
                return false;
        }
        else if(elemType == typeof(SpecialEffectAnimator))
        {
            Animator anim = go.GetComponent<Animator>();
            if (anim == null)
                return false;
        }
        else if (elemType == typeof(SpecialEffectTrailRenderer))
        {
            H3DTrailRender trail = go.GetComponent<H3DTrailRender>();
            if (trail == null)
                return false;
        }
        else
        {
            ParticleSystem particleSys = go.GetComponent<ParticleSystem>();
            if (particleSys != null)
                return false;

            Animation animation = go.GetComponent<Animation>();
            if (animation != null)
                return false;

            Animator animator = go.GetComponent<Animator>();
            if (animator != null)
                return false;
        }

        return true;
    }

    public static bool IsSPELegal(GameObject go, out string errorMsg)
    {
        bool bRet = true;
        errorMsg = string.Empty;

        if(null == go)
        {
            errorMsg = "特效有误";
            return false;
        }

        SpecialEffect spe = go.GetComponent<SpecialEffect>();
        if (null == spe)
        {
            errorMsg = "特效有误";
            return false;
        }

        foreach (var item in spe.elems)
        {
            bRet = IsSPEElementLegal(item, out errorMsg);
            if(!bRet)
            {
                break;
            }
        }

        return bRet;
    }

    public static bool IsSPEElementLegal(SpecialEffectElement element, out string errorMsg)
    {
        bool bRet = true;
        errorMsg = string.Empty;

        if (null == element)
        {
            errorMsg = "特效元素为Null";
            return false;
        }

        Type elemType = element.GetType();

        if (elemType == typeof(SpecialEffectAnimation))
        {
            Animation anim = element.gameObject.GetComponent<Animation>();

            if (
                   (null == anim)
                || (null == anim.clip)  
                )
            {   
                errorMsg = element.gameObject.name + "中的animation脚本没有clip动画或者此动画是animator的动画，animation的动画必须是Legacy的属性";
                bRet = false;
                return bRet;
            }
            
#if UNITY_5_0 || UNITY_5_1
            if(!anim.clip.legacy)
            {
                bRet = false;
                return bRet;               
            }
#else 
            SerializedObject serializedClip = new SerializedObject(anim.clip);
            SerializedProperty animationType = serializedClip.FindProperty("m_AnimationType");
            if (animationType == null)
            {
                bRet = false;
                return bRet;
            }

            if ( animationType.intValue != (int)ModelImporterAnimationType.Legacy )
            {
                errorMsg = element.gameObject.name + "中的animation脚本没有clip动画或者此动画是animator的动画，animation的动画必须是Legacy的属性";
                bRet = false;
                return bRet; 
            } 
            
#endif
            if (null == anim.GetComponent<Animation>()[anim.GetComponent<Animation>().clip.name])
            {
                anim.AddClip(anim.clip, anim.GetComponent<Animation>().clip.name);
            }
        }
 
        return bRet;
    }

    public static void ReplacePrefab( GameObject go , UnityEngine.Object targetPrefab )
    {
        PrefabUtility.ReplacePrefab(go, targetPrefab, ReplacePrefabOptions.ReplaceNameBased); 
    }

    public static bool IsPrefab( UnityEngine.Object obj )
    {
        return PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab;
    }

    public static bool IsSpecialEffectPrefab( UnityEngine.Object prefab )
    {
        if (!IsPrefab(prefab))
            return false;

        GameObject go = prefab as GameObject;
        return go.GetComponent<SpecialEffect>() != null;
    }

    public static bool IsAudioClip( UnityEngine.Object obj )
    {
        AudioClip audio =  obj as AudioClip;
        return audio != null;
    }

    private static void UpdateAnimationClip(Animation anim)
    {
        if(null == anim)
        {
            return;
        }

        List<string> elementList = new List<string>();

        foreach (AnimationState state in anim)
        {
            if (
                 (state != null)
                 && (state.clip != null)
                )
            {
                elementList.Add(state.clip.name);
            }
        }

        if (anim.clip != null)
        {
            if (!elementList.Contains(anim.clip.name))
            {
                if (elementList.Count != 0)
                {
                    anim.clip = anim[elementList[0]].clip;
                }
                else
                {
                    anim.clip = null;
                }
            }
        }
        else
        {
            if (elementList.Count != 0)
            {
                anim.clip = anim[elementList[0]].clip;
            }
            else
            {
                anim.clip = null;
            }
        }
    }


    public static void SetGameObjectHideFlagsRecursively(GameObject go, HideFlags flags)
    {
        Stack<Transform> S = new Stack<Transform>();
        S.Push(go.transform);

        while( S.Count > 0 )
        {
            var curr = S.Pop();
            curr.gameObject.hideFlags = flags;
            for( int i = 0 ; i < curr.childCount ; i++)
            {
                S.Push(curr.GetChild(i));
            }
        } 
    }

    public static SpecialEffectElement GetParentElem(SpecialEffectElement currElem)
    {
        if(null == currElem)
        {
            return null;
        }

        return currElem.gameObject.transform.parent.gameObject.GetComponent<SpecialEffectElement>();
    }

    public static List<SpecialEffectElement> GetChildrenElem(SpecialEffectElement currElem)
    {
        List<SpecialEffectElement> children = new List<SpecialEffectElement>();
        if (null == currElem)
        {
            return null;
        }

        for (int index = 0; index < currElem.gameObject.transform.childCount; index++)
        {
            SpecialEffectElement child = currElem.gameObject.transform.GetChild(index).gameObject.GetComponent<SpecialEffectElement>();
            if (child != null)
            {
                children.Add(child);
            }
        }

        return children;
    }

    public static float GetElemDelayTime(SpecialEffectElement currElem)
    {
        if (null == currElem)
        {
            return -1f;
        }

        SpecialEffectElement parent = GetParentElem(currElem);

        if (parent != null)
        {
            return (float)Math.Round(((double)currElem.startTime - (double)parent.startTime), 2);
        }
        else
        {
            return (float)Math.Round((double)currElem.startTime, 2);
        }

    }

    public static bool IsSpeValueVaild(SpecialEffect spe)
    {
        bool bRet = true;
        if(null == spe)
        {
            return false;
        }

        foreach(var elemItem in spe.elems)
        {
            if(elemItem != null)
            {
                if(
                    (elemItem.startTime < 0)
                    || (elemItem.playTime < 0)
                    )
                {
                    bRet = false;
                    break;
                }
                foreach(var child in GetChildrenElem(elemItem))
                {
                    if(child != null)
                    {
                        if(elemItem.startTime > child.startTime)
                        {
                            bRet = false;
                            break;
                        }

                        float parentEndTime = (float)Math.Round((double)(elemItem.startTime + elemItem.playTime), 2);
                        float childEndTime = (float)Math.Round((double)(child.startTime + child.playTime), 2);

                        if (parentEndTime < childEndTime)
                        {
                            bRet = false;
                            break;
                        }
                    }
                    else
                    {
                        bRet = false;
                        break;
                    }
                }
            }
            else
            {
                bRet = false;
                break;
            }

            if(!bRet)
            {
                break;
            }
        }
        return bRet;
    }

    public static void FixElemStartTime(SpecialEffectElement currElem, float value)
    {
        if(null == currElem)
        {
            return;
        }

        value = (float)Math.Round((double)value, 2);

        if (value < 0)
        {
            return;
        }

        SpecialEffectElement parent = GetParentElem(currElem);

        if (
               (parent != null)
            && (value < parent.startTime)
            )
        {
            value = parent.startTime;
        }

        float detalTime = value - currElem.startTime;

        if (!CheckChildrenPlayTime(currElem.playTime - detalTime, currElem))
        {
            return;
        }

        if (!FixChildrenElemStartTime(currElem, GetChildrenElem(currElem), value))
        {
            return;
        }


        currElem.startTime = value;

        FixElemPlayTime(currElem, currElem.playTime - detalTime);
    }

    public static void FixElemPlayTime(SpecialEffectElement currElem, float value)
    {
        if(null == currElem)
        {
            return;
        }

        value = (float)Math.Round((double)value, 2);
        if (value < 0)
        {
            return;
        }


        if (!FixChildrenPlayTime(GetChildrenElem(currElem), value, currElem))
        {
            return;
        }

        if (!FixParentPlayTime(GetParentElem(currElem), value, currElem))
        {
            return;
        }

        currElem.playTime = value;
    }
    public static void FixElemDelayTime(SpecialEffectElement currElem, float value)
    {
        if(null == currElem)
        {
            return;
        }

        value = (float)Math.Round((double)value, 2);
        if (value < 0)
        {
            return;
        }

        SpecialEffectElement parent = GetParentElem(currElem);
        if (
               (parent != null)
            && (value > parent.playTime)
            )
        {
            return;
        }

        CalcElemStartTimeForDelayTimeChange(currElem, value);
        FixParentPlayTime(parent, currElem.playTime, currElem);
    }

    private static bool FixChildrenElemStartTime(SpecialEffectElement currElem, List<SpecialEffectElement> childrenElem, float currStartTime)
    {
        bool bRet = true;

        if(
            (null == currElem)
            || (null == childrenElem)
            )
        {
            return false;
        }

        currStartTime = (float)Math.Round((double)currStartTime, 2);

        if (currStartTime < 0)
        {
            return false;
        }

        if (currElem == null)
        {
            return false;
        }

        foreach (var item in childrenElem)
        {
            if (item != null)
            {
                float childStartTime = 0f;
                if (currStartTime >= item.startTime)
                {
                    childStartTime = currStartTime;
                }
                else
                {
                    childStartTime = (float)Math.Round((double)(currStartTime + GetElemDelayTime(item)), 2);
                }

                bRet = FixChildrenElemStartTime(item, GetChildrenElem(item), childStartTime);
                if (!bRet)
                {
                    break;
                }
                item.startTime = childStartTime;
            }
        }

        return bRet;
    }


    private static bool FixChildrenPlayTime(List<SpecialEffectElement> childrenElem, float currPlayTime, SpecialEffectElement currElem)
    {
        bool bRet = true;

        if(
            (null == childrenElem)
            || (null == currElem)
            )
        {
            return false;
        }
        currPlayTime = (float)Math.Round((double)currPlayTime, 2);

        if (currPlayTime < 0)
        {
            return false;
        }

        if (!CheckChildrenPlayTime(currPlayTime, currElem))
        {
            return false;
        }

        foreach (var item in childrenElem)
        {
            if (item != null)
            {
                float parentEndTime = (float)Math.Round((double)(currElem.startTime + currPlayTime), 2);
                float childEndTime = (float)Math.Round((double)(item.startTime + item.playTime), 2);

                if (parentEndTime <= childEndTime)
                {
                    float itemPlayTime = (float)Math.Round((double)(currPlayTime - GetElemDelayTime(item)), 2);

                    bRet = FixChildrenPlayTime(GetChildrenElem(item), itemPlayTime, item);
                    if (!bRet)
                    {
                        break;
                    }
                    item.playTime = itemPlayTime;
                }
            }
        }

        return bRet;
    }
    private static bool FixParentPlayTime(SpecialEffectElement parentElem, float currPlayTime, SpecialEffectElement currElem)
    {
        bool bRet = true;

        if(
            (null == currElem)
            )
        {
            return false;
        }

        if (currPlayTime < 0)
        {
            return false;
        }

        if (
            (parentElem != null)
            && ((currElem.startTime + currPlayTime) > (parentElem.startTime + parentElem.playTime))
            )
        {
            float parentPlayTime = (float)Math.Round((double)(GetElemDelayTime(currElem) + currPlayTime), 2);

            bRet = FixParentPlayTime(GetParentElem(parentElem), parentPlayTime, parentElem);
            if (bRet)
            {
                parentElem.playTime = parentPlayTime;
            }
        }

        return bRet;
    }



    private static void CalcElemStartTimeForDelayTimeChange(SpecialEffectElement currElem, float currDelayTime)
    {
        float tempTime = 0f;

        if(null == currElem)
        {
            return;
        }

        currDelayTime = (float)Math.Round((double)currDelayTime, 2);

        SpecialEffectElement parent = GetParentElem(currElem);
        if (parent != null)
        {
            tempTime = (float)Math.Round((double)(parent.startTime + currDelayTime), 2);
        }
        else
        {
            tempTime = currDelayTime;
        }

        if (!FixChildrenElemStartTime(currElem, GetChildrenElem(currElem), tempTime))
        {
            return;
        }

        currElem.startTime = tempTime;
    }
    private static bool CheckChildrenPlayTime(float currPlayTime, SpecialEffectElement currElem)
    {
        bool bRet = true;

        if(null == currElem)
        {
            return false;
        }

        currPlayTime = (float)Math.Round((double)currPlayTime, 2);

        if (currPlayTime < 0)
        {
            return false;
        }

        if (
            (currPlayTime <= currElem.playTime)
            )
        {
            foreach (var item in GetChildrenElem(currElem))
            {
                float parentEndTime = (float)Math.Round((double)(currElem.startTime + currPlayTime), 2);
                float childEndTime = (float)Math.Round((double)(item.startTime + item.playTime), 2);

                if (parentEndTime <= childEndTime)
                {
                    float itemPlayTime = (float)Math.Round((double)(currPlayTime - GetElemDelayTime(item)), 2);
                    if (itemPlayTime < 0f)
                    {
                        bRet = false;
                        break;
                    }

                    bRet = CheckChildrenPlayTime(itemPlayTime, item);
                    if (!bRet)
                    {
                        break;
                    }
                }

            }
        }

        return bRet;

    }

}

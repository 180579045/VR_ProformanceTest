using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class SpecialEffectAnimClipProxy 
{ 
    public SpecialEffectAnimClipProxy( string name )
    {
        mClipGo = new GameObject(name);
        mClip = mClipGo.AddComponent<SpecialEffectAnimationClip>();
        mIsDirty = true;
    }

    public SpecialEffectAnimClipProxy( UnityEngine.Object prefab )
    {
        mClipPrefab = prefab;
        mClipGo = GameObject.Instantiate(mClipPrefab) as GameObject;
        mClip = mClipGo.GetComponent<SpecialEffectAnimationClip>();
        mClip.Init_Editor();
        //准备预览动画
        mPreviewAnimClip = AssetDatabase.LoadAssetAtPath(mClip.previewAnimClipPath, typeof(AnimationClip)) as AnimationClip;

        mIsDirty = false;
    }

    //当前播放时间
    public float CurrentPlayTime
    {
        get { return mClip.CurrPlayTime; }
        set
        {
            mClip.CurrPlayTime = value;
        }
    }

    public float TotalTime
    {
        get { return mClip.TotalTime; }
    }

    //当前特效动画是否被修改
    public bool IsDirty
    {
        get { return mIsDirty; }
        set { mIsDirty = value; }
    }

    public SpecialEffectAnimationClip Clip
    {
        get { return mClip; } 
    }

    public UnityEngine.Object ClipPrefab
    {
        get { return mClipPrefab; }
    }

    public GameObject Go
    {
        get { return mClipGo; }
    }

    public AnimationClip PreviewAnimClip
    {
        get { return mPreviewAnimClip; }
        set
        {  
            mPreviewAnimClip = value;
            if( value == null )
            {
                mClip.previewAnimClipPath = "";
                return;
            }
            mClip.previewAnimClipPath = AssetDatabase.GetAssetPath(value);
        }
    }

    public ISpecialEffectContext Context
    {
        get { return mClip.Context; }
        set { mClip.Context = value; }
    }

    public void Init()
    {
        mClip.Init_Editor();
    }

    public void Destory()
    {
        mClip.Detach();
        GameObject.DestroyImmediate(mClipGo);
        mClipGo = null;
        mClip = null;
        mClipPrefab = null;
        mIsDirty = false;
    }

    public int AddItem( UnityEngine.Object obj )
    { 
        mIsDirty = true;
        return mClip.AddItem(obj); 
    }

    public void InsertItem( UnityEngine.Object obj , int i )
    {
        mClip.InsertItem(obj, i);
    }

    public void InsertItem( SpecialEffectAnimClipItem item , int i)
    {
        mClip.InsertItem(item, i);
    }

    public void RemoveItem( int i )
    {
        mClip.DeleteItem(i); 
        mIsDirty = true;
    }

    public void SetItemTimeLine( int i , float startTime , float length )
    { 
        var item = QueryItem(i); 
        if( item != null )
        {
            item.startTime = startTime;
            item.length = length; 
            mIsDirty = true;
        } 
    }

    public bool GetItemTimeLine( int i , out float startTime , out float length )
    {
        startTime = 0.0f;
        length = 0.0f; 
        var item = QueryItem(i); 
        if (item != null)
        {
            startTime = item.startTime;
            length = item.length;
            return true;
        }
        return false;
    }

    public void SetItemBindingPath( int i , string bindPath )
    {
        var item = QueryItem(i);
        if (item != null)
        {
            item.bindingTargetPath = bindPath; 
            mIsDirty = true;
        } 
    }

    public string GetItemBindingPath( int i )
    {
        var item = QueryItem(i);
        if (item != null)
        {
            return item.bindingTargetPath; 
        }
        return "";
    }

    public void SetItemDeathType( int i , int deathType )
    {
        var item = QueryItem(i);
        if (item != null)
        {
            item.deathType = deathType;
            mIsDirty = true;
        } 
    }

    public int GetItemDeathType( int i )
    {
        var item = QueryItem(i);
        if (item != null)
        {
            return item.deathType; 
        }
        return 0;
    }

    public UnityEngine.Object QueryItemObj(int i)
    {
        return mClip.GetItemObj(i); 
    }


    public void Attach( GameObject go )
    {
        mClip.Attach(go);
    }

    public void Detach()
    {
        mClip.Detach();
    }

    public bool TrySave()
    {
        if (mClipPrefab == null)
        {//当前没有保存目标Prefab
            string prefabPath = EditorUtility.SaveFilePanel("保存动画片段", Application.dataPath, "NewSpeAnimClip", "prefab");
            if (prefabPath != "")
            {
                _UpdateAnimationClipItemTransform(mClipGo.GetComponent<SpecialEffectAnimationClip>());
                GameObject saveGo = _GetSaveAnimationClipGameObject(mClipGo);
                //保存Prefab
                mClipPrefab = PrefabUtility.CreatePrefab(prefabPath.Substring(prefabPath.LastIndexOf("Assets/")), saveGo, ReplacePrefabOptions.ReplaceNameBased);
                GameObject.DestroyImmediate(saveGo);
            }
            else
            {
                return false;
            }
        }
        else
        {//当前存在保存目标 
            _UpdateAnimationClipItemTransform(mClipGo.GetComponent<SpecialEffectAnimationClip>());
            GameObject saveGo = _GetSaveAnimationClipGameObject(mClipGo);
            SpecialEffectEditorUtility.ReplacePrefab(saveGo, mClipPrefab);
            GameObject.DestroyImmediate(saveGo);
        }
        AssetDatabase.Refresh();
        mIsDirty = false;
        return true;
    }
    
    public SpecialEffectAnimClipItem QueryItem( int i )
    { 
        return mClip.QueryItem(i);
    }

    void _UpdateAnimationClipItemTransform( SpecialEffectAnimationClip clip )
    {
        foreach( var item in clip.itemList )
        {
            var effItem = item as SpecialEffectAnimClipEffectItem;
            if( effItem != null )
            {
                effItem.localOffsetPos = effItem.effInst.transform.localPosition;
                effItem.localRotation = effItem.effInst.transform.localRotation;
                effItem.localScale = effItem.effInst.transform.localScale;
            }
        }
    }
     
    GameObject _GetSaveAnimationClipGameObject(GameObject go)
    {
        GameObject saveGo = GameObject.Instantiate(go) as GameObject;
         
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < saveGo.transform.childCount; i++)
        {
            children.Add(saveGo.transform.GetChild(i));
        }
        foreach (var child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        children.Clear();
        return saveGo;
    }


     GameObject mClipGo = null; 
     SpecialEffectAnimationClip mClip = null; 
     UnityEngine.Object mClipPrefab = null; 
     AnimationClip mPreviewAnimClip = null; 
     bool mIsDirty = false;
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class SpecialEffectAnimClipEditorModel 
{

    public delegate void NotifyDelegate();

    public NotifyDelegate onSpeAnimClipItemNumChange;

    public NotifyDelegate onSpeAnimClipNew;

    public NotifyDelegate onSpeAnimClipOpen;

    public NotifyDelegate onSpeAnimClipValueChange;

    public NotifyDelegate onSpeAnimClipItemSelect;

    public NotifyDelegate onSpeAnimClipSetNewAction;

    public NotifyDelegate onSpeAnimClipPreviewModelChange;

    public bool isClipItemNumChange = false;

    public bool isClipNew = false;

    public bool isClipOpen = false;

    public bool isClipValueChange = false;

    public bool isClipItemSelect = false;

    public bool isSetNewAction = false;

    public bool isPreviewModelChange = false;

    public void UpdateNotify()
    {
        if( isClipItemNumChange )
        {
           if( onSpeAnimClipItemNumChange != null )
           {
               onSpeAnimClipItemNumChange();
           }
           isClipItemNumChange = false;
        }


        if (isClipNew)
        {
            if (onSpeAnimClipNew != null)
            {
                onSpeAnimClipNew();
            }
            isClipNew = false;
        }

        if (isClipOpen)
        {
            if (onSpeAnimClipOpen != null)
            {
                onSpeAnimClipOpen();
            }
            isClipOpen = false;
        }

        if (isClipValueChange)
        {
            if (onSpeAnimClipValueChange != null)
            {
                onSpeAnimClipValueChange();
            }
            isClipValueChange = false;
        }

        if (isClipItemSelect)
        {
            if (onSpeAnimClipItemSelect != null)
            {
                onSpeAnimClipItemSelect();
            }
            isClipItemSelect = false;
        }

        if (isSetNewAction)
        {
            if (onSpeAnimClipSetNewAction != null)
            {
                onSpeAnimClipSetNewAction();
            }
            isSetNewAction = false;
        }

        if (isPreviewModelChange)
        {
            if (onSpeAnimClipPreviewModelChange != null)
            {
                onSpeAnimClipPreviewModelChange();
            }
            isPreviewModelChange = false;
        }
    }

    public void Init()
    {
        mEditorAttachPoint = new GameObject("_SpecialEffectAnimClipEditorAttachPoint");
        mEditorAttachPoint.transform.localPosition = Vector3.zero;
        mEditorAttachPoint.transform.localRotation = Quaternion.identity;
        mEditorAttachPoint.transform.localScale = Vector3.one;
        if (EditorHelper.IsDebugMode())
        {  
            mEditorAttachPoint.hideFlags = HideFlags.DontSave;
        }
        else
        {//Release版隐藏
            mEditorAttachPoint.hideFlags = HideFlags.HideAndDontSave;
        }

        mEditorSpeAnimClipContext = new GameObject("_SpecialEffectAnimClipContext");
        mEditorSpeAnimClipContext.AddComponent<AudioSource>();
        var defaultContext = mEditorSpeAnimClipContext.AddComponent<SpecialEffectDefaultContext>();
        defaultContext.Start();
        _Attach(mEditorSpeAnimClipContext.transform);

        //GameObject dirLight = new GameObject("_Directional Light");
        //dirLight.transform.parent = mEditorAttachPoint.transform;
        //dirLight.transform.localPosition = new Vector3(100.0f, 100.0f, 100.0f);
        //dirLight.transform.localRotation = Quaternion.identity;
        //dirLight.transform.localScale = Vector3.one;
        //Light light = dirLight.AddComponent<Light>();
        //light.type = LightType.Directional;
        
    }

    public void Destory()
    {
        if (mCurrentClip != null)
        {//若当前已经打开了片段，询问是否保存并清理
            _QuerySaveClip();
            _ClearCurrentClip();
        }

        onSpeAnimClipNew = null;
        onSpeAnimClipOpen = null;
        onSpeAnimClipValueChange = null;
        onSpeAnimClipItemNumChange = null;
        onSpeAnimClipItemSelect = null;
        onSpeAnimClipSetNewAction = null;

        mIsBindingMode = false;
        mCurrSelect = -1;

        mActionMgr.Clear();
        mCmdMgr.Clear();

        _ClearCurrentClip();

        mRefModelBoneRoot = null;
        GameObject.DestroyImmediate(mEditorAttachPoint);
        mEditorAttachPoint = null;
    }
    
    

    public void NewClip()
    {
        NewClip("NewSpecialEffectAnimClip");
    }

    //创建动画片段
    public void NewClip(string name)
    {
        if (mCurrentClip != null)
        {//若当前已经打开了片段，询问是否保存并清理
            _QuerySaveClip();
            _ClearCurrentClip();
        }

        //创建动画片段
        mCurrentClip = new SpecialEffectAnimClipProxy(name);
        mCurrentClip.Context = mEditorSpeAnimClipContext.GetComponent<SpecialEffectDefaultContext>();
        //将新建的动画片段挂接至统一挂接点
        _Attach(mCurrentClip.Go.transform); 
        mCmdMgr.Clear();
         
        //清空动作
        mActionMgr.Clear();

        //尝试绑定特效动画至模型
        _TryAttachSpeAnim();

        CurrentPlayTime = 0.0f; 
        CurrentSelect = -1; 
        isClipNew = true;
        UpdateNotify();

    }

    //打开已存在动画片段
    public void OpenClip(UnityEngine.Object prefab )
    {
        if (mCurrentClip != null)
        {//若当前已经打开了片段，询问是否保存并清理
            _QuerySaveClip();
            _ClearCurrentClip();
        }


        mCurrentClip = new SpecialEffectAnimClipProxy(prefab);
        mCurrentClip.Context = mEditorSpeAnimClipContext.GetComponent<SpecialEffectDefaultContext>();
        //将新建的动画片段挂接至统一挂接点
        _Attach(mCurrentClip.Go.transform);   
        mCmdMgr.Clear(); 

        //为打开的动画片段准备预览动作
        SetPreviewModelAction(mCurrentClip.PreviewAnimClip);

        //尝试绑定特效动画至模型
        _TryAttachSpeAnim();

        CurrentPlayTime = 0.0f; 
        CurrentSelect = -1;
        isClipOpen = true; 
        UpdateNotify(); 
    }

    //保存动画片段
    public void SaveClip()
    {
        if (mCurrentClip != null)
        { 
            if (mCurrentClip.TrySave())
            {
                EditorUtility.DisplayDialog("", "保存动画片段成功！", "确定"); 
                mCmdMgr.Clear();
                isClipValueChange = true;
                UpdateNotify();
            }
        }
    }
 

    public void AddItem( UnityEngine.Object obj )
    {
        if (mCurrentClip == null)
            return;

        if (!IsLegalItem(obj))
            return;

        SpeAnimClipAddItemCmd cmd = new SpeAnimClipAddItemCmd();
        cmd.clip = mCurrentClip;
        cmd.obj = obj;
        mCmdMgr.Execute(cmd);

        _RefreshSpeAttach();

        CurrentSelect = -1; 
        SyncCurrPlayTime();
    }

    public void RemoveItem( int i )
    {
        if (mCurrentClip == null)
            return;

        if (i >= 0)
        {
            SpeAnimClipRemoveItemCmd cmd = new SpeAnimClipRemoveItemCmd();
            cmd.clip = mCurrentClip;
            cmd.i = i;
            mCmdMgr.Execute(cmd);
        }

        _RefreshSpeAttach();
        CurrentSelect = -1;

        SyncCurrPlayTime();
    }
    
    public void SetSpeAnimItemTimeLine( int i , float startTime , float length )
    {
        if (mCurrentClip == null)
            return;

        if (i >= 0)
        {
            SpeAnimClipSetItemTimeLineCmd cmd = new SpeAnimClipSetItemTimeLineCmd();
            cmd.clip = mCurrentClip;
            cmd.i = i;
            cmd.newStartTime = startTime;
            cmd.newLength = length;
            mCmdMgr.Execute(cmd);
        }
        SyncCurrPlayTime(); 
        isClipValueChange = true;
        UpdateNotify();  
    }
     
    public void SetSpeAnimSelectItemBindingPath( int i , string bindPath )
    {
        if (mCurrentClip == null)
            return;

        if (i >= 0)
        {
            SpeAnimClipSetItemBindPathCmd cmd = new SpeAnimClipSetItemBindPathCmd();
            cmd.clip = mCurrentClip;
            cmd.i = i;
            cmd.newBindPath = bindPath;
            mCmdMgr.Execute(cmd);
        }

        //重新挂接特效动画
        _RefreshSpeAttach();

        isClipItemSelect = true;
        isClipValueChange = true;
        UpdateNotify();  
    }

    public void SetSpeAnimItemDeathType(int i, int deathType)
    {
        if (mCurrentClip == null)
            return;

        if (i >= 0)
        {
            SpeAnimClipSetItemDeathTypeCmd cmd = new SpeAnimClipSetItemDeathTypeCmd();
            cmd.clip = mCurrentClip;
            cmd.i = i;
            cmd.newDeathType = deathType;
            mCmdMgr.Execute(cmd);
        }

        SyncCurrPlayTime();


        isClipValueChange = true;
        UpdateNotify();   
    }

    public List<SpeEditorActionManager.TransformInfo> GetTPoseTransformInfos()
    {
        return mActionMgr.GetDefaultBindPose();
    }

  

    //设置预览模型
    public void SetPreviewModel( UnityEngine.Object prefab )
    { 
        //释放旧预览模型
        if (mRefModel != null)
        {
            _TryDetachSpeAnim();
            GameObject.DestroyImmediate(mRefModel);
            mRefModel = null;
        }

        if( prefab == null )
        {
            return;
        } 
        
        mRefModel =  GameObject.Instantiate(prefab) as GameObject;

        //禁止在SceneView中编辑参考模型 
        SpecialEffectEditorUtility.SetGameObjectHideFlagsRecursively(mRefModel, HideFlags.NotEditable);

        _Attach(mRefModel.transform);
        mActionMgr.RegisterDefaultBindPose(mRefModel);
        //创建骨骼预览球
        _CreateRefModelBoneSpheres();
        //隐藏骨骼预览球
        ShowRefModelBoneSpheres(false);
        _TryAttachSpeAnim();

        SyncCurrPlayTime();

        isClipItemSelect = true;
        isPreviewModelChange = true;
        UpdateNotify();
    }

    //设置预览模型动作
    public void SetPreviewModelAction( AnimationClip clip )
    {
        if (mCurrentClip == null)
            return;

        mCurrentClip.PreviewAnimClip = clip;
        mActionMgr.Clear();
        if (clip != null)
        {
            mActionMgr.AddAction(clip);
        }

        isSetNewAction = true; 
        UpdateNotify(); 
    }

    //将预览模型显示为TPose
    public void ShowPreviewModelTPose()
    {
        if ( mRefModel != null )
        {
            mActionMgr.ShowDefaultBindPose(mRefModel);
        }
    }

    public SpeEditorAction GetPreviewModelAction()
    {
        if( mActionMgr.ActionList.Count > 0 )
        {
            return mActionMgr.ActionList[0];
        }
        return null;
    }

    public void ShowRefModelBoneSpheres( bool show )
    {
        if( mRefModelBoneRoot != null )
        {
            mRefModelBoneRoot.SetActive(show);
        }
    }

    public void Undo()
    {
        mCmdMgr.Undo();
    }

    public void Redo()
    {
        mCmdMgr.Redo();
    }


    public bool CanUndo()
    {
        return mCmdMgr.CanUndo();
    }

    public bool CanRedo()
    {
        return mCmdMgr.CanRedo();
    }

    public bool IsLegalItem(UnityEngine.Object obj)
    {
        AudioClip audio = obj as AudioClip;
        if (audio != null)
        {//支持AudioClip
            return true;
        }

        GameObject go = obj as GameObject;
        if (go != null)
        {
            if (go.GetComponent<SpecialEffect>() != null)
            {//支持SpecialEffect
                return true;
            }
        }
        return false;
    }

    public void SyncCurrPlayTime()
    {
        if( CurrentClip != null )
        {
            if( mRefModel != null )
            {
                mActionMgr.SetCurrPlayTime(mRefModel, mCurrPlayTime);
            }

            //更新Clip需在更新动作之后，否则会没有粒子效果
            CurrentClip.CurrentPlayTime = mCurrPlayTime;
        }
    } 
    
    //清除当前正在编辑器的动画片段
    void _ClearCurrentClip()
    {
        if (mCurrentClip != null)
        {
            mCurrentClip.Destory();
            mCurrentClip = null; 
        } 
    }

    bool _QuerySaveClip()
    {
        if (mCurrentClip != null)
        {//若已存在正在编辑的动画片段
            if (mCurrentClip.IsDirty)
            {//当前动画片段被修改，可能需要被保存
                bool saveNeeded = EditorUtility.DisplayDialog("新建动画片段", "当前编辑器片段有修改！是否保存？", "保存", "不保存"); 
                if (saveNeeded)
                {
                    return mCurrentClip.TrySave();
                } 
            } 
        } 
        return false;
    }

 


    Transform _GetAttachPoint()
    {
        return mEditorAttachPoint.transform;
    }

    Transform _Attach( Transform trans )
    {
        return _Attach(trans, Vector3.zero, Quaternion.identity, Vector3.one);
    }

    Transform _Attach( Transform trans , Vector3 localPos , Quaternion localRot , Vector3 localScale  )
    {
       trans.parent = _GetAttachPoint();
       trans.localPosition = localPos;
       trans.localRotation = localRot;
       trans.localScale = localScale;
       return trans;
    }

    void _RefreshSpeAttach()
    {
        _TryDetachSpeAnim();
        _TryAttachSpeAnim();
    }

    void _TryAttachSpeAnim()
    {
        if (mCurrentClip != null && mRefModel != null)
        {
            mCurrentClip.Attach(mRefModel);
        }
    }

    void _TryDetachSpeAnim()
    {
        if (mCurrentClip != null)
        {
            mCurrentClip.Detach();
        }
    }

    void _CreateRefModelBoneSpheres()
    {
        if( mRefModel != null )
        {
            if( mRefModelBoneRoot != null )
            {
                GameObject.DestroyImmediate(mRefModelBoneRoot);
                mRefModelBoneRoot = null;
            }

            mRefModelBoneRoot = new GameObject("_RefModelBones");
            mRefModelBoneRoot.hideFlags = HideFlags.NotEditable;
            _Attach(mRefModelBoneRoot.transform);
             

            var transList = GetTPoseTransformInfos();
            foreach( var trans in transList )
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = trans.path.Substring(trans.path.LastIndexOf('/') + 1);
                go.hideFlags = HideFlags.NotEditable ;
                go.transform.parent = mRefModelBoneRoot.transform;
                go.transform.position = trans.pos;
                go.transform.rotation = trans.rotation;
                go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }


    public SpecialEffectAnimClipProxy CurrentClip
    {
        get { return mCurrentClip; } 
    }
 

    public int CurrentSelect
    {
        get { return mCurrSelect; }
        set 
        { 
            mCurrSelect = value;
            isClipItemSelect = true;
            UpdateNotify();
        }
    }

    public float CurrentPlayTime
    {
        get { return mCurrPlayTime; }
        set
        {
            mCurrPlayTime = value;
            SyncCurrPlayTime();
        }
    }

    //预览长度
    public float PreviewLength
    {
        get
        {
            float len = 0.0f;
            if( mCurrentClip != null )
            {
                len = mCurrentClip.TotalTime;
            }
            SpeEditorAction act = GetPreviewModelAction();
            if( act != null )
            {
                len = Mathf.Max(len, act.Length);
            }
            return len;
        }  
    }

    public bool IsBindingMode
    {
        get { return mIsBindingMode; }
        set { mIsBindingMode = value; }
    }
    
    //编辑器总挂接点
    GameObject mEditorAttachPoint;

    //编辑器动画片段上下文
    GameObject mEditorSpeAnimClipContext;
      
    SpecialEffectAnimClipProxy mCurrentClip;

    //参考模型
    GameObject mRefModel;

    //参考模型骨骼球根节点
    GameObject mRefModelBoneRoot;

    
    //当前播放时间
    float mCurrPlayTime = 0.0f;

    //命令管理器
    SpecialEffectAnimClipEditorCommandManager mCmdMgr = new SpecialEffectAnimClipEditorCommandManager();

    //动作管理器
    SpeEditorActionManager mActionMgr = new SpeEditorActionManager();


    //是否为绑定模式
    bool mIsBindingMode = false;

    //当前选项
    int mCurrSelect = -1;

    public static SpecialEffectAnimClipEditorModel GetInstance()
    {
        if( _instance == null )
        {
            _instance = new SpecialEffectAnimClipEditorModel();
        }
        return _instance;
    }

    static SpecialEffectAnimClipEditorModel _instance;
}

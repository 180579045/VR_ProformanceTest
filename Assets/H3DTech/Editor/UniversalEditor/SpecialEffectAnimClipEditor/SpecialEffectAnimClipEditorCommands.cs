using UnityEngine;
using System.Collections;


public class SpeAnimClipEditorCommandBase : IEditorCommand
{
    public virtual string Name
    {
        get { return "特效动画编辑器空命令"; }
    }

    //指明此命令只执行不记录，没有UndoRedo功能
    public bool DontSaved { get { return false; } }

    public virtual void Execute()
    {

    }

    public virtual void UnExecute()
    {

    }


    public SpecialEffectAnimClipProxy clip;
}

 
public class SpeAnimClipAddItemCmd : SpeAnimClipEditorCommandBase
{
    public override string Name 
    {
        get { return "特效动画添加项"; } 
    }
     

    public override void Execute()
    {
        i = clip.AddItem(obj);
        SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        SpecialEffectAnimClipEditorModel.GetInstance().isClipItemNumChange = true;
        SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify(); 
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = -1; 
    }

    public override void UnExecute()
    {
        clip.RemoveItem(i);
        SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        SpecialEffectAnimClipEditorModel.GetInstance().isClipItemNumChange = true;
        SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify();
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = -1; 
    }

    public UnityEngine.Object obj; 
    int i = -1; 
}

public class SpeAnimClipRemoveItemCmd : SpeAnimClipEditorCommandBase
{
    public override string Name
    {
        get { return "特效动画删除项"; }
    }

    public override void Execute()
    { 
        item = clip.QueryItem(i);
        clip.RemoveItem(i);
        SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        SpecialEffectAnimClipEditorModel.GetInstance().isClipItemNumChange = true;
        SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify();
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = -1;
    }

    public override void UnExecute()
    {
        clip.InsertItem(item, i); 
        SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        SpecialEffectAnimClipEditorModel.GetInstance().isClipItemNumChange = true;
        SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify(); 
        SpecialEffectAnimClipEditorModel.GetInstance().CurrentSelect = -1;
    }
     
    public SpecialEffectAnimClipItem item;
    public int i; 
}

public class SpeAnimClipSetItemTimeLineCmd : SpeAnimClipEditorCommandBase
{
    public override string Name
    {
        get { return "特效动画设置时间线"; }
    }

    public override void Execute()
    { 
        clip.GetItemTimeLine(i, out oldStartTime, out oldLength);
        clip.SetItemTimeLine(i, newStartTime, oldLength);

        SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        SpecialEffectAnimClipEditorModel.GetInstance().isClipValueChange = true;
        SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify();
         
    }

    public override void UnExecute()
    {
        clip.SetItemTimeLine(i, oldStartTime, oldLength);
        SpecialEffectAnimClipEditorModel.GetInstance().SyncCurrPlayTime();
        SpecialEffectAnimClipEditorModel.GetInstance().isClipValueChange = true;
        SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify();
         
    } 

    public int i; 

    public float newStartTime;

    public float oldStartTime;

    public float newLength;

    public float oldLength;
}


public class SpeAnimClipSetItemBindPathCmd : SpeAnimClipEditorCommandBase
{
    public override string Name
    {
        get { return "特效动画设置绑定位置"; }
    }

    public override void Execute()
    { 
        oldBindPath = clip.GetItemBindingPath(i);
        clip.SetItemBindingPath(i, newBindPath); 

		
		//Add by HouXiaoGang for 代码改善 Start
		SpecialEffectAnimClipEditorModel.GetInstance().isClipItemSelect  = true;
		SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify();
		//Add by HouXiaoGang  End
    }

    public override void UnExecute()
    { 
        clip.SetItemBindingPath(i, oldBindPath);

		//Add by HouXiaoGang for 代码改善 Start
		SpecialEffectAnimClipEditorModel.GetInstance().isClipItemSelect = true;
		SpecialEffectAnimClipEditorModel.GetInstance().UpdateNotify();
		//Add by HouXiaoGang  End
    }

    public int i; 

    public string newBindPath;

    public string oldBindPath;
}


public class SpeAnimClipSetItemDeathTypeCmd : SpeAnimClipEditorCommandBase
{
    public override string Name
    {
        get { return "特效动画设置死亡类型"; }
    }

    public override void Execute()
    {
        oldDeathType = clip.GetItemDeathType(i);
        clip.SetItemDeathType(i, newDeathType); 
    }

    public override void UnExecute()
    {
        clip.SetItemDeathType(i, oldDeathType);  
    }

    public int i; 

    public int newDeathType;

    public int oldDeathType;
}
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AtlasLoadProjectCommand : IEditorCommand
{//载入Atlas命令

    public string m_ProjectPath = string.Empty;

    public string Name { get { return "Load AtlasProject"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        string dispStr = "加载" + Path.GetFileNameWithoutExtension(m_ProjectPath);

        EditorUtility.DisplayProgressBar("工程加载中", dispStr, 0f);

        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        errorType = UIAdjustAtlasEditorModel.GetInstance().AddAtlasProject(m_ProjectPath);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("加载工程：" + m_ProjectPath);
        }
    }
    public void UnExecute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = UIAdjustAtlasEditorModel.GetInstance().RemoveAtlasProject(m_ProjectPath);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("撤销加载工程：" + m_ProjectPath);
        }
    }
}

public class AtlasRemoveProjectCommand : IEditorCommand
{
    public string m_ProjectPath = string.Empty;

    public string Name { get { return "Remove AtlasProject"; } }
    public bool DontSaved { get { return true; } }
   
    public void Execute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = UIAdjustAtlasEditorModel.GetInstance().RemoveAtlasProject(m_ProjectPath);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("移除工程：" + m_ProjectPath);
        }
    }

    public void UnExecute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = UIAdjustAtlasEditorModel.GetInstance().AddAtlasProject(m_ProjectPath);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("撤销移除工程：" + m_ProjectPath);
        }
    }
}

public class AdjustAtlas_MoveSpriteCommand : IEditorCommand
{
    public List<UIAdjust_SpriteOperateInfo> m_sourceInfoTable = new List<UIAdjust_SpriteOperateInfo>();

    public string m_destProjectPath = string.Empty;


    private List<UIAdjust_ModifyRefInfo> m_ModifyRefTable = null;
    public string Name { get { return "MoveSprite"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        foreach (var item in m_sourceInfoTable)
        {
            operateAtlasList.Add(item.SourceProjectPath);
        }
        operateAtlasList.Add(m_destProjectPath);

        errorType = UIAdjustAtlasEditorModel.GetInstance().AddOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().MoveSprite(m_sourceInfoTable, m_destProjectPath, out m_ModifyRefTable);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("移动小图");
        }
    }

    public void UnExecute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        foreach (var item in m_sourceInfoTable)
        {
            operateAtlasList.Add(item.SourceProjectPath);
        }
        operateAtlasList.Add(m_destProjectPath);

        errorType = UIAdjustAtlasEditorModel.GetInstance().ReduceOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().UnDoMoveSprite(m_sourceInfoTable, m_destProjectPath, m_ModifyRefTable);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("撤销移动");
        }
    }
}

public class AdjustAtlas_CopySpriteCommand : IEditorCommand
{
    public List<UIAdjust_SpriteOperateInfo> m_sourceInfoTable = new List<UIAdjust_SpriteOperateInfo>();

    public string m_destProjectPath = string.Empty;

    public string Name { get { return "CopySprite"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        operateAtlasList.Add(m_destProjectPath);

        errorType = UIAdjustAtlasEditorModel.GetInstance().AddOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().CopySprites(m_sourceInfoTable, m_destProjectPath);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("复制小图");
        }
    }

    public void UnExecute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        operateAtlasList.Add(m_destProjectPath);
        
        List<UIAdjust_SpriteOperateInfo> newOperateInfoList = new List<UIAdjust_SpriteOperateInfo>();
     
        UIAdjust_SpriteOperateInfo newInfo = new UIAdjust_SpriteOperateInfo();
        newInfo.SourceProjectPath = m_destProjectPath;
        foreach (var item in m_sourceInfoTable)
        {
            newInfo.OperateSpriteTable.AddRange(item.OperateSpriteTable);
        }
        newOperateInfoList.Add(newInfo);

        errorType = UIAdjustAtlasEditorModel.GetInstance().ReduceOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().RemoveSprite(newOperateInfoList);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("撤销复制");
        }
    }
}

public class AdjustAtlas_RemoveSpriteCommand : IEditorCommand
{
    public List<UIAdjust_SpriteOperateInfo> m_sourceInfoTable = new List<UIAdjust_SpriteOperateInfo>();

    public string Name { get { return "RemoveSprite"; } }
    public bool DontSaved { get { return true; } }

    private List<UIAdjust_SpriteOperateInfoForUndoCommand> m_sourceInfoTableForUndo = new List<UIAdjust_SpriteOperateInfoForUndoCommand>();

    public void Execute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        foreach (var item in m_sourceInfoTable)
        {
            operateAtlasList.Add(item.SourceProjectPath);
        }
        UpdateInfoTableForUndo();
        errorType = UIAdjustAtlasEditorModel.GetInstance().AddOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().RemoveSprite(m_sourceInfoTable);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("删除小图");
        }
    }
    public void UnExecute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        foreach (var item in m_sourceInfoTable)
        {
            operateAtlasList.Add(item.SourceProjectPath);
        }

        errorType = UIAdjustAtlasEditorModel.GetInstance().ReduceOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().AddSprite(m_sourceInfoTableForUndo);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("撤销删除小图");
        }
    }

    private void UpdateInfoTableForUndo()
    {
        m_sourceInfoTableForUndo.Clear();

        foreach(var item in m_sourceInfoTable)
        {
            UIAdjust_SpriteOperateInfoForUndoCommand newInfo = new UIAdjust_SpriteOperateInfoForUndoCommand();
            newInfo.SourceProjectPath = item.SourceProjectPath;
            List<AtlasSpriteImage> spriteImage = null;
            UIAdjustAtlasEditorModel.GetInstance().GetSpriteImage(item.SourceProjectPath, item.OperateSpriteTable.ToArray(), out spriteImage);
            newInfo.SpriteImageTable = spriteImage;

            m_sourceInfoTableForUndo.Add(newInfo);
        }
    }
}

public class AdjustAtlas_ImageZoomCommand : IEditorCommand
{//小图缩放命令

    public float m_oldScaleFactor = 0f;
    public float m_newScaleFactor = 0f;
    public string m_SpritePath = string.Empty;
    public string m_ProjectPath = string.Empty;
    public string Name { get { return "Atlas SpriteImage Zoom Change"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        operateAtlasList.Add(m_ProjectPath);
        errorType = UIAdjustAtlasEditorModel.GetInstance().AddOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().ZoomSprite(m_ProjectPath, m_SpritePath, m_newScaleFactor);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("缩放小图");
        }
    }

    public void UnExecute()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        List<string> operateAtlasList = new List<string>();
        operateAtlasList.Add(m_ProjectPath);
        errorType = UIAdjustAtlasEditorModel.GetInstance().ReduceOperateCount(operateAtlasList.ToArray());
        errorType = UIAdjustAtlasEditorModel.GetInstance().ZoomSprite(m_ProjectPath, m_SpritePath, m_oldScaleFactor);
        if (UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE == errorType)
        {
            UniversalEditorLog.DebugLog("撤销缩放小图");
        }
    }


}

public class AdjustAtlas_AddToOperateArea : IEditorCommand
{
    public string m_ProjectPath = string.Empty;
    public int m_AreaIndex = -1;

    public string Name { get { return "Add Project To OperateArea"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        UIAdjustAtlasEditor.UpdateOperateArea(m_ProjectPath, m_AreaIndex);
    }

    public void UnExecute()
    {
        UIAdjustAtlasEditor.ClearOperateArea(m_AreaIndex);
    }
}

public class AdjustAtlas_ClearOperateArea : IEditorCommand
{
    public string m_ProjectPath = string.Empty;
    public int m_AreaIndex = -1;

    public string Name { get { return "Clear OperateArea"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        UIAdjustAtlasEditor.ClearOperateArea(m_AreaIndex);
    }

    public void UnExecute()
    {
        UIAdjustAtlasEditor.UpdateOperateArea(m_ProjectPath, m_AreaIndex);
    }
}

public class AdjustAtlas_ClearAllOperateArea : IEditorCommand
{
    public string[] m_ProjectPaths = new string[UIAdjustAtlasEditor.GetOperateAreaNum()];

    public string Name { get { return "Clear All OperateArea"; } }
    public bool DontSaved { get { return true; } }

    public void Execute()
    {
        UIAdjustAtlasEditor.ClearAllOperateArea();
    }

    public void UnExecute()
    {
        for (int index = 0; index < UIAdjustAtlasEditor.GetOperateAreaNum(); index++)
        {
            if (!string.IsNullOrEmpty(m_ProjectPaths[index]))
            {
                UIAdjustAtlasEditor.UpdateOperateArea(m_ProjectPaths[index], index);
            }
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public enum UIADJUSTATLAS_ERROR_TYPE
{//编辑器错误类型
    UIADJUSTATLAS_ERROR_UNKNOWN = 0,                              //未知错误
    UIADJUSTATLAS_ERROR_ERRORPATH,                           //目标Project路径错误 
    UIADJUSTATLAS_ERROR_PROJECTPATH_NOEXIST,                  //目标工程不存在      
    UIADJUSTATLAS_ERROR_REMOVEPROJECT_MODIFY,                 //移除修改project
    UIADJUSTATLAS_ERROR_ADD_SAMEPROJECT,                      //添加重复工程

    UIADJUSTATLAS_ERROR_LOAD_ERRORPROJECTFILE,                    //工程文件错误
   
    UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR,                //操作sprite时，源Atlas异常
    UIADJUSTATLAS_ERROR_OPERATESPRITE_DESTERROR,                //操作sprite时，目标Atlas异常
    UIADJUSTATLAS_ERROR_MOVESPRITE_ALREADYEXIST,                //移动sprtie时，已存在于目标工程

    UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH,                       //配置文件路径错误

    UIADJUSTATLAS_ERROR_REMOVESPRITE_LEAVENOSPRITE,              //小图全部删除
    UIADJUSTATLAS_ERROR_REBUILD_WITHNONEMODIFY,                 //未变更任何工程时，重新build

    UIADJUSTATLAS_ERROR_UPDATEASSETS_WITHMODIFY,                //有工程变更时刷新Assets

    UIADJUSTATLAS_ERROR_LOADSPRITE_TEXTUREERROR,                //LoadSPrite纹理失败

    UIADJUSTATLAS_ERROR_LOADPROJECT_CORRELATIONFILE_NOEXIST,    //Atlas关联文件不存在(prefab、png、mat)

    UIADJUSTATLAS_ERROR_PROJECTDIC_ERROR,                       //Atlas工程目录错误

    UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_ERROR,     //检查一致性时input信息错误
    UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR,  //检查一致性时input信息中Prefab路径错误
    UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR,    //检查一致性时input信息中SourceAB信息错误
    UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR,      //检查一致性时，结果目录错误
    UIADJUSTATLAS_ERROR_ATLAS_UNCONSISTENT_ERROR,                 //Atlas不一致

    UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR,        //分析引用关系时、关联Prefab路径错误
    UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR,        //分析引用关系时、关联Scene路径错误
    UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_WITH_UNCONSISTPROJECT_ERROR,        //分析引用关系时，存在不一致工程
    UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_RESULTPATH_ERROR,        //分析引用关系时，结果路径错误  
    UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_CONFIGFILE_PATHERROR,    //打开配置文件时，路径错误

    UIADJUSTATLAS_ERROR_OPERATE_WITH_NONEPROJECT,

    UIADJUSTATLAS_ERROR_NONE = -0xFFFE,                            //默认值

    UIADJUSTATLAS_WARNING_UNKNOWN,
    UIADJUSTATLAS_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR,
    UIADJUSTATLAS_WARNING_REFERENCEINFO_NONE,
}

public class UIAdjust_ProjectLoadProgress
{
    public string m_DispMsg = string.Empty;
    public int m_CurrentSpriteCount = 0;
    public int m_Total = 0;
}

public class UIAdjust_RebuildProgress
{
    public string m_DispMsg = string.Empty;
    public int m_CurrentAtlasCount = 0;
    public int m_Total = 0;
}

public class UIAdjust_SpriteOperateInfo
{
    string m_SourceProjectPath = string.Empty;
    List<string> m_OperateSpriteTable = new List<string>();

    public string SourceProjectPath
    {
        set { m_SourceProjectPath = value; }
        get { return m_SourceProjectPath; }
    }

    public List<string> OperateSpriteTable
    {
        set { m_OperateSpriteTable = value; }
        get { return m_OperateSpriteTable; }
    }

}

public class UIAdjust_SpriteOperateInfoForUndoCommand
{
    string m_SourceProjectPath = string.Empty;
    List<AtlasSpriteImage> m_SpriteImageTable = new List<AtlasSpriteImage>();

    public string SourceProjectPath
    {
        set { m_SourceProjectPath = value; }
        get { return m_SourceProjectPath; }
    }

    public List<AtlasSpriteImage> SpriteImageTable
    {
        set { m_SpriteImageTable = value; }
        get
        {
            return m_SpriteImageTable;
        }
    }

}
public class UIAdjust_ModifyRefInfo
{
    private string m_RefAssetFilePath = string.Empty;
    private string m_SpriteName = string.Empty;
    private string m_SourceAtlasGUID = string.Empty;
    private string m_DestAtlasGUID = string.Empty;

    public string RefAssetFilePath
    {
        set { m_RefAssetFilePath = value; }
        get { return m_RefAssetFilePath; }
    }
    public string SpriteName
    {
        set { m_SpriteName = value; }
        get { return m_SpriteName; }
    }
    public string SourceAtlasGUID
    {
        set { m_SourceAtlasGUID = value; }
        get { return m_SourceAtlasGUID; }
    }

    public string DestAtlasGUID
    {
        set { m_DestAtlasGUID = value; }
        get { return m_DestAtlasGUID; }
    }
}


public class UIAdjustAtlasEditorModel
{//Atlas调整工具Model

    public UIADJUSTATLAS_ERROR_TYPE AddAtlasProject(string projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if(string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo atlasItemInfo = null;
        AtlasConsistencyAnalyseResult analyseResult = null;

        do
        {
            foreach (var item in m_AtlasProjectTable)
            {
                if (item.Project.Path == projectPath)
                {
                    errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ADD_SAMEPROJECT;
                }
            }
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }


            StartLoadProjectProgress();

            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().LoadAtlasProject(projectPath, ref atlasItemInfo, out analyseResult));
            if (IsUIAdjustFailed(errorType))
            {    
                break;
            }

            if (ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT == analyseResult.AnalyseResult)
            {
                m_ProjectConsistencyInfoTbl.AddRange(analyseResult.ConsistencyInfo);
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ATLAS_UNCONSISTENT_ERROR;
                break;
            }

            if (ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING == analyseResult.AnalyseResult)
            {
                m_ProjectConsistencyInfoTbl.AddRange(analyseResult.ConsistencyInfo);
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR;
            }

            m_AtlasProjectTable.Add(atlasItemInfo);

        } while (false);

        if (onLoadProject != null)
        {
            string atlasOutputPath = string.Empty;
            if (
                   (atlasItemInfo != null)
                && (atlasItemInfo.Project != null)
                )
            {
                atlasOutputPath = atlasItemInfo.Project.AtlasSavePath;
            }

            onLoadProject(projectPath, atlasOutputPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE RemoveAtlasProject(string projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(projectPath))
        {
            errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
            return errorType;
        }

        do
        {
            bool isModify = false;

            foreach (var item in m_AtlasProjectTable)
            {
                if (item.Project.Path == projectPath)
                {
                    IsProjectModify(projectPath, out isModify);
                    if (isModify)
                    {
                        errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REMOVEPROJECT_MODIFY;
                        break;
                    }

                    errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().UnLoadAtlasProject(item));
                    m_AtlasProjectTable.Remove(item);
                    break;
                }
            }
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }

        } while (false);

        if(onRemoveProject != null)
        {
            onRemoveProject(projectPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReloadProjectTexture()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        foreach(var item in m_AtlasProjectTable)
        {
            Texture2D tex = null;
            UIAtlasOperateUtility.GetInstance().MakeCurrentAtlasTexture(item.Project, out tex);
            item.AtlasTexture = tex;
            item.AtlasTextureBak = tex;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE AddSprite(List<UIAdjust_SpriteOperateInfo> sourceInfo)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == sourceInfo)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        AtlasItemInfo sourceItem = null;

        foreach (var item in sourceInfo)
        {
            if (FindAtlasProject(item.SourceProjectPath, out sourceItem))
            {
                errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().AddSpriteToProject(item.OperateSpriteTable.ToArray(), ref sourceItem));
                if (IsUIAdjustFailed(errorType))
                {
                    break;
                }
            }
        }

        if (onAddSprite != null)
        {
            onAddSprite(sourceInfo, errorType);
        }
        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE AddSprite(List<UIAdjust_SpriteOperateInfoForUndoCommand> sourceInfoForUndo)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == sourceInfoForUndo)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        AtlasItemInfo sourceItem = null;
        List<UIAdjust_SpriteOperateInfo> sourceInfo = new List<UIAdjust_SpriteOperateInfo>();

        foreach (var srouceInfoItem in sourceInfoForUndo)
        {
            UIAdjust_SpriteOperateInfo newInfo = new UIAdjust_SpriteOperateInfo();
            newInfo.SourceProjectPath = srouceInfoItem.SourceProjectPath;
            foreach (var spriteItem in srouceInfoItem.SpriteImageTable)
            {
                newInfo.OperateSpriteTable.Add(spriteItem.Path);
            }

            sourceInfo.Add(newInfo);
        }


        foreach (var item in sourceInfoForUndo)
        {
            if(FindAtlasProject(item.SourceProjectPath, out sourceItem))
            {
                errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().AddSpriteToProject(item.SpriteImageTable.ToArray(), ref sourceItem));
                if (IsUIAdjustFailed(errorType))
                {
                    break;
                }
            }
        }

        if (onAddSprite != null)
        {
            onAddSprite(sourceInfo, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE RemoveSprite(List<UIAdjust_SpriteOperateInfo> sourceInfo)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == sourceInfo)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        AtlasItemInfo sourceItem = null;

        foreach(var item in sourceInfo)
        {
            if(FindAtlasProject(item.SourceProjectPath, out sourceItem))
            {
                if (!CanRemoveSprite(item.OperateSpriteTable.Count, sourceItem.Project))
                {
                    errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REMOVESPRITE_LEAVENOSPRITE;
                    break;
                }
                errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().RemoveSpriteFromProject(item.OperateSpriteTable.ToArray(), ref sourceItem));
                if(IsUIAdjustFailed(errorType))
                {
                    break;
                }
            }
        }

        if (onRemoveSprite != null)
        {
            onRemoveSprite(sourceInfo, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE MoveSprite(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath, out List<UIAdjust_ModifyRefInfo> modifyRefTable)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        modifyRefTable = null;
        List<OperateItemInfo> sourceOperateInfoTable = new List<OperateItemInfo>();

        AtlasItemInfo destProjectItem = null;
        List<AtlasItemInfo> sourceProjectItems = new List<AtlasItemInfo>();

        if (null == sourceInfo)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        if (string.IsNullOrEmpty(destProjectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        do
        {
            //设定操作源信息
            foreach (var sourceInfoItem in sourceInfo)
            {
                AtlasItemInfo projectItem = null;

                if (FindAtlasProject(sourceInfoItem.SourceProjectPath, out projectItem))
                {
                    if(!CanRemoveSprite(sourceInfoItem.OperateSpriteTable.Count, projectItem.Project))
                    {
                        errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REMOVESPRITE_LEAVENOSPRITE;
                        break;
                    }

                    OperateItemInfo newInfo = new OperateItemInfo();
                    newInfo.Project = projectItem.Project;
                    newInfo.SpritePathTable = sourceInfoItem.OperateSpriteTable;

                    sourceOperateInfoTable.Add(newInfo);
                    sourceProjectItems.Add(projectItem);
                }
            }
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }
            if (0 == sourceOperateInfoTable.Count)
            {
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
                break;
            }

            //设定操作目标信息
            if (!FindAtlasProject(destProjectPath, out destProjectItem))
            {
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_DESTERROR;
                break;
            }
            AtlasItemInfo tempDestItem = destProjectItem as AtlasItemInfo;

            //移动sprite
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().MoveSpriteAmongProjects(sourceOperateInfoTable, 
                                                                                                     destProjectItem.Project, 
                                                                                                     ref sourceProjectItems,
                                                                                                     ref tempDestItem,
                                                                                                     out modifyRefTable));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }

            m_ModifyRefTable.AddRange(modifyRefTable);

            m_MoveCount++;

        } while (false);

        if(onMoveSpriteCommand != null)
        {
            onMoveSpriteCommand(sourceInfo, destProjectPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE CopySprites(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == sourceInfo)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        if (string.IsNullOrEmpty(destProjectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        List<OperateItemInfo> sourceOperateInfoTable = new List<OperateItemInfo>();

        AtlasItemInfo destProjectItem = null;

        do
        {
            //设定操作源信息
            foreach (var sourceInfoItem in sourceInfo)
            {
                foreach (var projectItem in m_AtlasProjectTable)
                {
                    if (projectItem.Project.Path == sourceInfoItem.SourceProjectPath)
                    {
                        OperateItemInfo newInfo = new OperateItemInfo();
                        newInfo.Project = projectItem.Project;
                        newInfo.SpritePathTable = sourceInfoItem.OperateSpriteTable;

                        sourceOperateInfoTable.Add(newInfo);
                        break;
                    }
                }
            }
            if (0 == sourceOperateInfoTable.Count)
            {
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
                break;
            }

            //设定操作目标信息
            if(!FindAtlasProject(destProjectPath, out destProjectItem))
            {
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTPATH_NOEXIST;
                break;
            }

            //移动sprite
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().CopySpriteAmongProjects(sourceOperateInfoTable, ref destProjectItem));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }

        } while (false);

        if (onCopySpriteCommand != null)
        {
            onCopySpriteCommand(sourceInfo, destProjectPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE UnDoMoveSprite(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath, List<UIAdjust_ModifyRefInfo> modifyRefTable)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == sourceInfo)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        if (string.IsNullOrEmpty(destProjectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        AtlasItemInfo newSourceItem = null;
        List<AtlasItemInfo> newDestItems = new List<AtlasItemInfo>();
        List<OperateItemInfo> newDestOperateInfoList = new List<OperateItemInfo>();
        List<UIAdjust_ModifyRefInfo> newModifyRefTable = null;

        do
        {
            if (!FindAtlasProject(destProjectPath, out newSourceItem))
            {
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
                break;
            }
            AtlasItemInfo tempSourceItem = newSourceItem as AtlasItemInfo;

            foreach(var infoItem in sourceInfo)
            {
                AtlasItemInfo destAtlasItem = null;
                if (!FindAtlasProject(infoItem.SourceProjectPath, out destAtlasItem))
                {
                    break;
                }

                OperateItemInfo newDestOperateInfo = new OperateItemInfo();
                newDestOperateInfo.Project = destAtlasItem.Project;
                newDestOperateInfo.SpritePathTable = infoItem.OperateSpriteTable;

                newDestOperateInfoList.Add(newDestOperateInfo);
                newDestItems.Add(destAtlasItem);
            }

            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().MoveSpriteAmongProjects(newSourceItem.Project, 
                                                                                                     newDestOperateInfoList,
                                                                                                     ref tempSourceItem, 
                                                                                                     ref newDestItems, 
                                                                                                     out newModifyRefTable));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }

            if(
                   (m_ModifyRefTable != null)
                && (modifyRefTable != null)
                )
            {
                m_ModifyRefTable.AddRange(newModifyRefTable);

                //m_ModifyRefTable.RemoveRange(m_ModifyRefTable.Count - modifyRefTable.Count, modifyRefTable.Count);
            }
           
        } while (false);

        if (onMoveSpriteCommand != null)
        {
            onMoveSpriteCommand(sourceInfo, destProjectPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ZoomSprite(string projectPath, string spritePath, float scaleFactor)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (
               string.IsNullOrEmpty(spritePath)
            || string.IsNullOrEmpty(projectPath) 
            || ((-0.000001f < scaleFactor) && (scaleFactor < 0.000001f))
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATESPRITE_SOURCEERROR;
        }

        AtlasItemInfo atlasItem = null;

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            UIAtlasOperateUtility.GetInstance().ZoomSpriteFromProject(spritePath, scaleFactor, ref atlasItem);
        }

        if(onZoomSprite != null)
        {
            onZoomSprite(projectPath, spritePath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE RebuildModifyProjects()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        List<string> modifyProjectPath = new List<string>();
        bool isModifyRefFile = false;
        do
        {
            StartRebuildProgress();

            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().RebuildProject(m_AtlasProjectTable.ToArray(), m_IsForceBuild));
            if(IsUIAdjustFailed(errorType))
            {
                break;
            }

            if (m_ModifyRefTable.Count != 0)
            {
                errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ChangeReferenceAsset(m_ModifyRefTable));
                if (IsUIAdjustFailed(errorType))
                {
                    break;
                }
                m_ModifyRefTable.Clear();
                isModifyRefFile = true;
            }

            modifyProjectPath = FindModifyProjectPath();
            if (!m_IsForceBuild && (0 == modifyProjectPath.Count))
            {
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_REBUILD_WITHNONEMODIFY;
            }

            SaveOperateCount();

        } while (false);

        if(onRebuildModifyProject != null)
        {
            onRebuildModifyProject(modifyProjectPath.ToArray(), isModifyRefFile, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE UpdateAssets()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        do
        {
            foreach (var item in m_AtlasProjectTable)
            {
                if (item.AtlasSaveCounter != item.AtlasOperateCounter)
                {
                    errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UPDATEASSETS_WITHMODIFY;
                    break;
                }
            }
            if(IsUIAdjustFailed(errorType))
            {
                break;
            }

            foreach(var item in m_AtlasProjectTable)
            {
                errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().UpdateAtlasReference(item.Project));
            }
        } while (false);

        if (onUpdateAssetsCommand != null)
        {
            onUpdateAssetsCommand(errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE CheckAtlasConsistency()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        ANALYSERESULT_TYPE resultType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;
        List<AtlasProject> projcetTbl = null;
        string resultPath = string.Empty;

        do
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().CheckAndExportConsistency(true, out projcetTbl, out resultType, out resultPath));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }
        } while (false);

        if(onCheckConsistency != null)
        {
            onCheckConsistency(resultType, resultPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ExportConsistencyInfoOnLoadProject()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        string resultPath = string.Empty;
        ANALYSERESULT_TYPE analyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;

        do
        {
            analyseResult = FixAnalyseResurltOnLoadProject();

            if (analyseResult != ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT)
            {
                errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ExportConsistencyInfo(m_ProjectConsistencyInfoTbl, out resultPath));
                if (IsUIAdjustFailed(errorType))
                {
                    break;
                }
            }

            m_ProjectConsistencyInfoTbl.Clear();
        } while (false);


        if (onExportConsistencyOnLoadProject != null)
        {
            onExportConsistencyOnLoadProject(analyseResult, resultPath);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ExportDependency()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        string resultDir = string.Empty;

        do
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ExportDependency(out resultDir));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }

        } while (false);


        if(onExportReference != null)
        {
            onExportReference(resultDir, string.Empty, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ExportReverseDependency()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        string resultDir = string.Empty;
        string consistencyPath = string.Empty;

        do
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ExportReverseDependency(out resultDir, out consistencyPath));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }

        } while (false);

        if(onExportReference != null)
        {
            onExportReference(resultDir, consistencyPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ExportNoneDependency()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        string resultDir = string.Empty;
        string consistencyPath = string.Empty;

        do
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ExportNoneDependency(out resultDir, out consistencyPath));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }   
        } while (false);

        if (onExportReference != null)
        {
            onExportReference(resultDir, consistencyPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ExportAllDependency()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        string resultDir = string.Empty;
        string consistencyPath = string.Empty;

        do
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ExportAllDependency(out resultDir, out consistencyPath));
            if (IsUIAdjustFailed(errorType))
            {
                break;
            }            
        } while (false);

        if (onExportReference != null)
        {
            onExportReference(resultDir, consistencyPath, errorType);
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ConfigDependencyFilter()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ConfigDependencyFilter());

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE OpenExportFile(string filePath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
 
        if(string.IsNullOrEmpty(filePath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().OpenExportFile(filePath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteProjectPathConfig(string projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteProjectPathConfig(projectPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadProjectPathConfig(out string projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadProjectPathConfig(out projectPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteImageBasePathConfig(string imageBasePath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(imageBasePath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteImageBasePathConfig(imageBasePath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadImageBasePathConfig(out string imageBasePath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadImageBasePathConfig(out imageBasePath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteConsistencyResultPathConfig(string resultPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(resultPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteConsistencyResultPathConfig(resultPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadConsistencyResultPathConfig(out string resultPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadConsistencyResultPathConfig(out resultPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteConsistencyPrefabPathConfig(string prefabPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(prefabPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteConsistencyPrefabPathConfig(prefabPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadConsistencyPrefabPathConfig(out string prefabPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadConsistencyPrefabPathConfig(out prefabPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteReferencePrefabPathConfig(string prefabPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(prefabPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteReferencePrefabPathConfig(prefabPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadReferencePrefabPathConfig(out string prefabPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadReferencePrefabPathConfig(out prefabPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteReferenceScenePathConfig(string scenePath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(scenePath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteReferenceScenePathConfig(scenePath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadReferenceScenePathConfig(out string scenePath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadReferenceScenePathConfig(out scenePath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE WriteReferenceResultPathConfig(string resultPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (string.IsNullOrEmpty(resultPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_CONFIG_ERRORPATH;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().WriteReferenceResultPathConfig(resultPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReadReferenceResultPathConfig(out string resultPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().ReadReferenceResultPathConfig(out resultPath));

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetAtlasRefTable(string projectPath, out List<string> refTable)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        AtlasItemInfo atlasItem = null;
        refTable = null;

        if(string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetAtlasRefTable(atlasItem.Project, out refTable)); 
        }
        else
        {
            errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTPATH_NOEXIST;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetAtlasRefCount(string projectPath, out int refCount)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        refCount = 0;

        if (string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        List<string> refTable = null;

        errorType = GetAtlasRefTable(projectPath, out refTable);
        if(!IsUIAdjustFailed(errorType))
        {
            refCount = refTable.Count;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetSpriteRefTable(string projectPath, string spritePath, out List<string> refTable)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        AtlasItemInfo atlasItem = null;
        refTable = null;
        
        if(
            string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
        }

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetSpriteRefTable(atlasItem.Project, spritePath, out refTable));
        }
        else
        {
            errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTPATH_NOEXIST;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetSpriteRefCount(string projectPath, string spritePath, out int refCount)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        refCount = 0;

        if (
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        List<string> refTable = null;

        errorType = GetSpriteRefTable(projectPath, spritePath, out refTable);
        if (!IsUIAdjustFailed(errorType))
        {
            refCount = refTable.Count;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE IsProjectModify(string projectPath, out bool isModify)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        isModify = false;

        if (string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo atlasInfo = null;

        if (FindAtlasProject(projectPath, out atlasInfo))
        {
            //isModify = atlasInfo.Project.IsModify;
            if (atlasInfo.AtlasOperateCounter != atlasInfo.AtlasSaveCounter)
            {
                isModify = true;
            }
        }
        else
        {
            errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTPATH_NOEXIST;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE AddOperateCount(string[] projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == projectPath)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo newInfo = null;

        foreach(var item in projectPath)
        {
            if (FindAtlasProject(item, out newInfo))
            {
                newInfo.AtlasOperateCounter++;
                continue;
            }
        }
        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ReduceOperateCount(string[] projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == projectPath)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo newInfo = null;

        foreach (var item in projectPath)
        {
            if (FindAtlasProject(item, out newInfo))
            {
                if (newInfo.AtlasOperateCounter > 0)
                {
                    newInfo.AtlasOperateCounter--;
                }
                continue;
            }
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE ClearSaveOperateCount(string[] projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        if (null == projectPath)
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo newInfo = null;

        foreach (var item in projectPath)
        {
            if (FindAtlasProject(item, out newInfo))
            {
                newInfo.AtlasSaveCounter = -1;
                continue;
            }
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE InitAllOperateCount()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        foreach(var item in m_AtlasProjectTable)
        {
            item.AtlasOperateCounter = 0;
            item.AtlasSaveCounter = 0;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE InitAllOperateCount(string projectPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        if (string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo projectItem = null;
        
        if(FindAtlasProject(projectPath, out projectItem))
        {
            projectItem.AtlasOperateCounter = 0;
            projectItem.AtlasSaveCounter = 0;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE SaveOperateCount()
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        foreach (var item in m_AtlasProjectTable)
        {
            item.AtlasSaveCounter = item.AtlasOperateCounter;
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetSpriteImageZoom(string spritePath, string projectPath, out float zoomScale)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        zoomScale = 0f;

        if (
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
        }

        AtlasItemInfo atlasItem = null;

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetSpriteImageZoom(spritePath, atlasItem.Project, out zoomScale));
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetSpriteImageZoomTexture2D(string spritePath, string projectPath, out Texture2D outputTexture)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        outputTexture = null;

        if (
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetSpriteImageZoomTexture2D(spritePath, projectPath, out outputTexture));
        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetSpriteImageTexture2D(string spritePath, string projectPath, out Texture2D outputTexture)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        outputTexture = null;

        if(
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
        }

        errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetSpriteImageTexture2D(spritePath, projectPath, out outputTexture));
        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE SetAtlasOutputPath(string projectPath, string outputPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        
        if(
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(outputPath)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
        }

        AtlasItemInfo atlasItem = null;

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().SetAtlasOutputPath(atlasItem.Project, outputPath));
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetAtlasOutputPath(string projectPath, out string outputPath)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        outputPath = string.Empty;

        if (string.IsNullOrEmpty(projectPath))
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
        }

        AtlasItemInfo atlasItem = null;

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetAtlasOutputPath(atlasItem.Project, out outputPath));
        }

        return errorType;
    }

    public UIADJUSTATLAS_ERROR_TYPE GetSpriteImage(string projectPath, string[] spritePaths, out List<AtlasSpriteImage> spriteImages)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
        spriteImages = new List<AtlasSpriteImage>();
        if(
            string.IsNullOrEmpty(projectPath)
            || (null == spritePaths)
            )
        {
            return UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
        }

        AtlasItemInfo atlasItem = null;

        if (FindAtlasProject(projectPath, out atlasItem))
        {
            errorType = CheckAtlasOpreateError(UIAtlasOperateUtility.GetInstance().GetSpriteImage(spritePaths, atlasItem.Project, out spriteImages));
        }

        return errorType;
    }
   
    public List<SpriteItemInfo> GetSpriteItems(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            return null;
        }

        List<SpriteItemInfo> spriteItemInfo = null;

        foreach (var item in m_AtlasProjectTable)
        {
            if (item.Project.Path == projectPath)
            {
                spriteItemInfo = UIAtlasOperateUtility.GetInstance().GetSpriteItems(item.Project);
            }
        }

        return spriteItemInfo;
    }

    public Texture2D GetAtlasTexture(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            return null;
        }

        Texture2D tex = null;

        foreach(var item in m_AtlasProjectTable)
        {
            if (item.Project.Path == projectPath)
            {
                tex = item.AtlasTexture;
            }
        }

        return tex;
    }

    public Texture2D GetAtlasTextureBak(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            return null;
        }

        Texture2D tex = null;

        foreach (var item in m_AtlasProjectTable)
        {
            if (item.Project.Path == projectPath)
            {
                tex = item.AtlasTextureBak;
            }
        }

        return tex;
    }

    public string GetProjectPathConfig()
    {
        return UIAtlasOperateUtility.GetInstance().GetProjectPathConfig();
    }

    public string GetImageBasePathConfig()
    {
        return UIAtlasOperateUtility.GetInstance().GetImageBasePathConfig();
    }

    public bool CheckReadOnlyFile(out string firstReadOnlyPath)
    {
        firstReadOnlyPath = string.Empty;
        return UIAtlasOperateUtility.GetInstance().CheckReadOnlyFile(out firstReadOnlyPath);
    }

    public bool HaveModifyProject(out string projectPath)
    {
        bool isModify = false;
        projectPath = string.Empty;

        foreach(var item in m_AtlasProjectTable)
        {
            IsProjectModify(item.Project.Path, out isModify);
            if (isModify)
            {
                projectPath = item.Project.Path;
                break;
            }
        }

        return isModify;
    }

    private void StartLoadProjectProgress()
    {
        m_ProjectLoadProgress = null;
        m_ProjectLoadProgress = new UIAdjust_ProjectLoadProgress();
        UIAtlasOperateUtility.GetInstance().onSpriteImageLoad = OnSpriteLoad;
    }
    private void StartRebuildProgress()
    {
        m_RebuildProgress = null;
        m_RebuildProgress = new UIAdjust_RebuildProgress();
        UIAtlasOperateUtility.GetInstance().onRebuildAtlas = OnRebuild;
    }

    private void OnSpriteLoad(string spritePath, AtlasProject project)
    {
        if (
            (null == m_ProjectLoadProgress)
            || (null == project)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return;
        }

        List<SpriteItemInfo> allSpriteItem = null;

        allSpriteItem = UIAtlasOperateUtility.GetInstance().GetSpriteItems(project);
        m_ProjectLoadProgress.m_CurrentSpriteCount++;
        m_ProjectLoadProgress.m_Total = allSpriteItem.Count;
        m_ProjectLoadProgress.m_DispMsg = "加载" + Path.GetFileNameWithoutExtension(project.Path);

        if(onProjectLoadProgress != null)
        {
            onProjectLoadProgress(m_ProjectLoadProgress);
        }
    }

    private void OnRebuild(string projectPath)
    {
        if (
            (null == m_RebuildProgress)
            || string.IsNullOrEmpty(projectPath)
            )
        {
            return;
        }

        m_RebuildProgress.m_CurrentAtlasCount++;
        m_RebuildProgress.m_Total = 0;
        if(m_IsForceBuild)
        {
            foreach(var item in m_AtlasProjectTable)
            {
                if(item.AtlasOperateCounter != item.AtlasSaveCounter)
                {
                    m_RebuildProgress.m_Total++;
                }
            }
        }
        else
        {
            m_RebuildProgress.m_Total = m_AtlasProjectTable.Count;
        }

        m_RebuildProgress.m_DispMsg = "生成" + Path.GetFileNameWithoutExtension(projectPath);

        if (onRebuildProgress != null)
        {
            onRebuildProgress(m_RebuildProgress);
        }
    }

    private bool IsUIAdjustFailed(UIADJUSTATLAS_ERROR_TYPE errorType)
    {
        bool bRet = false;

        if (errorType != UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE)
        {//失败

            bRet = true;
        }
        else 
        {//成功

            bRet = false;  
        }

        return bRet;
    }

    private bool CanRemoveSprite(int removeCount, AtlasProject project)
    {
        bool bRet = false;

        if(
               (null == project)
            || (removeCount <= 0)
            )
        {
            return false;
        }

        if (removeCount < project.GetAllSprites().Count)
        {
            bRet = true;
        }

        return bRet;
    }

    private bool FindAtlasProject(string projectPath, out AtlasItemInfo projectItem)
    {
        bool bRet = false;
        projectItem = null;

        if (string.IsNullOrEmpty(projectPath))
        {
            return false;
        }

        foreach(var item in m_AtlasProjectTable)
        {
            if (projectPath == item.Project.Path)
            {
                projectItem = item;
                bRet = true;
                break;
            }
        }

        return bRet;
    }

    private List<string> FindModifyProjectPath()
    {
        List<string> modifyProjectPath = new List<string>();

        foreach(var item in m_AtlasProjectTable)
        {
            if(item.AtlasOperateCounter != item.AtlasSaveCounter)
            {
                modifyProjectPath.Add(item.Project.Path);
            }
        }

        return modifyProjectPath;
    }

    private ANALYSERESULT_TYPE FixAnalyseResurltOnLoadProject()
    {
        ANALYSERESULT_TYPE analyseType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;

        foreach (var item in m_ProjectConsistencyInfoTbl)
        {
            if (ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT == item.ConsistencyType)
            {
                continue;
            }
            else if (ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING == item.ConsistencyType)
            {
                analyseType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING;
            }
            else
            {
                analyseType = ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT;
            }
        }

        return analyseType;
    }

    private UIADJUSTATLAS_ERROR_TYPE CheckAtlasOpreateError(ATLASOPERATE_ERROR_TYPE atlasOperateErrorType)
    {
        UIADJUSTATLAS_ERROR_TYPE errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;

        switch (atlasOperateErrorType)
        {
            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ERRORPATH;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_LOAD_ERRORPROJECTFILE;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_SPRITEALREADYEXIST:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_MOVESPRITE_ALREADYEXIST;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOADSPRITE_TEXTUREERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_LOADSPRITE_TEXTUREERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOADPROJECT_CORRELATIONFILE_NOEXIST:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_LOADPROJECT_CORRELATIONFILE_NOEXIST;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_ERROR;
                break;
                
            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_TARGETPROJECT_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_LOAD_ERRORPROJECTFILE;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ATLAS_UNCONSISTENT_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ATLAS_UNCONSISTENT_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_UNKNOWN:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_WARNING_UNKNOWN;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_CONFIGFILE_DIRPATHERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_CONFIGFILE_PATHERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_RESULTPATH_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_RESULTPATH_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_PROJECTDIC_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_PROJECTDIC_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_UNCONSISTENT_ERROR:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_ANALYSEREFERENCE_WITH_UNCONSISTPROJECT_ERROR;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_REFERENCEINFO_NONE:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_WARNING_REFERENCEINFO_NONE;
                break;

            case ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_OPERATE_WITH_NONEPROJECT;
                break;

            default:
                errorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_UNKNOWN;
                break;
        }

        return errorType;
    }

#region 成员变量
    public delegate void LoadProjectCommand(string projectPath, string atlasOutputPath, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void RemoveProjectCommand(string projectPath, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void AddSpriteCommand(List<UIAdjust_SpriteOperateInfo> sourceInfo, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void RemoveSpriteCommand(List<UIAdjust_SpriteOperateInfo> sourceInfo, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void ZoomSpriteCommand(string projectPath, string spritePath, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void MoveSpriteCommand(List<UIAdjust_SpriteOperateInfo> sourceInfo, string destProjectPath, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void RebuildModifyProjectCommand(string[] projectPaths, bool isModifyRefFile, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void UpdateAssetsCommand(UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void ProjectLoadProgressCommand(UIAdjust_ProjectLoadProgress progressInfo);
    public delegate void RebuildProgressCommand(UIAdjust_RebuildProgress progressInfo);
    public delegate void CheckConsistencyCommand(ANALYSERESULT_TYPE analyseType, string resultPath, UIADJUSTATLAS_ERROR_TYPE errorType);
    public delegate void ExportConsistencyOnLoadProject(ANALYSERESULT_TYPE analyseResult, string resultPath);
    public delegate void ExportReferenceCommand(string resultDir, string consistencyPath, UIADJUSTATLAS_ERROR_TYPE errorType);

    public LoadProjectCommand onLoadProject;
    public RemoveProjectCommand onRemoveProject;
    public AddSpriteCommand onAddSprite;
    public RemoveSpriteCommand onRemoveSprite;
    public ZoomSpriteCommand onZoomSprite;
    public MoveSpriteCommand onMoveSpriteCommand;
    public MoveSpriteCommand onCopySpriteCommand;
    public RebuildModifyProjectCommand onRebuildModifyProject;
    public UpdateAssetsCommand onUpdateAssetsCommand;
    public ProjectLoadProgressCommand onProjectLoadProgress;
    public RebuildProgressCommand onRebuildProgress;
    public CheckConsistencyCommand onCheckConsistency;
    public ExportConsistencyOnLoadProject onExportConsistencyOnLoadProject;
    public ExportReferenceCommand onExportReference;

    public UIADJUSTATLAS_ERROR_TYPE ErrorType
    {
        get { return m_ErrorType; }
    }
    public bool IsForceBuild { get { return m_IsForceBuild; } set { m_IsForceBuild = value; } }         

    private List<AtlasItemInfo> m_AtlasProjectTable = new List<AtlasItemInfo>();
    private List<UIAdjust_ModifyRefInfo> m_ModifyRefTable = new List<UIAdjust_ModifyRefInfo>();
    private List<AtlasConsistencyInfo> m_ProjectConsistencyInfoTbl = new List<AtlasConsistencyInfo>();

    private UIADJUSTATLAS_ERROR_TYPE m_ErrorType = UIADJUSTATLAS_ERROR_TYPE.UIADJUSTATLAS_ERROR_NONE;
    private bool m_IsForceBuild = false;
    private UIAdjust_ProjectLoadProgress m_ProjectLoadProgress = null;
    private UIAdjust_RebuildProgress m_RebuildProgress = null;

    private int m_MoveCount = 0;

    static private UIAdjustAtlasEditorModel m_Instance = null;
   
    public static UIAdjustAtlasEditorModel GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new UIAdjustAtlasEditorModel();
        }
        return m_Instance;
    }
    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            UIAdjustAtlasEditorModel.DestoryInstance();
        }
    }
#endregion
}
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public enum ATLASOPERATE_ERROR_TYPE
{//编辑器错误类型
    ATLASOPERATE_ERROR_UNKNOWN = 0,                              //未知错误
    ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT,                 //未存在工程时操作
    ATLASOPERATE_ERROR_LOAD_ERRORPATH,                           //LoadProject路径错误
    ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE,                    //工程文件错误
    ATLASOPERATE_ERROR_GETATLASTEXUTRE_WITH_NONESPRITE,          //不存在小图时生成Atlas纹理
    ATLASOPERATE_ERROR_ADDSPRITE_ERRORPATH,                      //添加小图时，小图路径错误
    ATLASOPERATE_ERROR_REMOVESPRITE_ERRORPATH,                   //删除小图时，小图路径错误
    ATLASOPERATE_ERROR_REFERENCEFILE_ERROR,                      //获取引用列表时，引用列表文件异常
    ATLASOPERATE_ERROR_REFERENCEATLAS_ERRORPATH,                 //获取引用列表时，目标Atlas路径异常
    ATLASOPERATE_ERROR_MOVETO_ERRORSOURCE,                       //移动时，移动源异常
    ATLASOPERATE_ERROR_MOVETO_ERRORDEST,                         //移动时，移动目标异常  
    ATLASOPERATE_ERROR_MOVETO_SPRITEALREADYEXIST,                //移动时，小图已存在  
    ATLASOPERATE_ERROR_MODIFYREF_FILEERROR,                      //修改反依赖文件时，文件异常
    ATLASOPERATE_ERROR_REBUILD_ERRORPROJECT,                      //重新build工程时，工程信息异常
    ATLASOPERATE_ERROR_MAKEPNG_ERRORPROJECT,                     //生成png时，Atlas异常
    ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH,                       //配置文件路径错误
    ATLASOPERATE_ERROR_LOADSPRITE_TEXTUREERROR,                 //load SPrite纹理失败
    ATLASOPERATE_ERROR_LOADPROJECT_CORRELATIONFILE_NOEXIST,      //Atlas关联文件不存在(prefab、png、mat)
    ATLASOPERATE_ERROR_PROJECTDIC_ERROR,                         //Project根目录错误
    
    ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_ERROR,       //检查一致性时input信息错误
    ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR,    //检查一致性时input信息中SourceAB信息错误
    ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR,  //检查一致性时input信息中Prefab路径错误
    ATLASOPERATE_ERROR_ANALYSECONSISTENCY_TARGETPROJECT_ERROR,   //检查一致性时目标工程错误   
    ATLASOPERATE_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR,      //检查一致性时，Result目录错误
    ATLASOPERATE_ERROR_ATLAS_UNCONSISTENT_ERROR,                 //Atlas不一致

    ATLASOPERATE_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR,        //分析引用关系时、关联Prefab路径错误
    ATLASOPERATE_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR,        //分析引用关系时、关联Scene路径错误
    ATLASOPERATE_ERROR_ANALYSEREFERENCE_RESULTPATH_ERROR,       //分析引用关系时，结果保存路径错误  
    ATLASOPERATE_ERROR_ANALYSEREFERENCE_UNCONSISTENT_ERROR,       //分析引用关系时，工程不一致
    ATLASOPERATE_ERROR_ANALYSEREFERENCE_CONFIGFILE_DIRPATHERROR,         //打开配置文件时，路径错误

    ATLASOPERATE_ERROR_NONE = -0xFFFE,                           //默认值
   
    ATLASOPERATE_WARNING_UNKNOWN,                                //未知警告
    ATLASOPERATE_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR, //Atlas一致，但有Prefab不在检索路径中的警告
    ATLASOPERATE_WARNING_REFERENCEINFO_NONE,                     //引用关系为空
        
}


public class RefModifyInfo
{
    private string m_SpriteName = string.Empty;
    private string m_SourceAtlasGUID = string.Empty;
    private string m_DestAtlasGUID = string.Empty;

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

public class AtlasItemInfo
{
    private AtlasProject m_AtlasProject = new AtlasProject();
    private Texture2D m_AtlasTexture = null;
    private Texture2D m_AtlasTextureBak = null;
    private int m_OperateCounter = 0;
    private int m_SavedCounter = 0;

    public AtlasProject Project
    {
        set { m_AtlasProject = value; }
        get { return m_AtlasProject; }
    }
    public Texture2D AtlasTexture
    {
        set { m_AtlasTexture = value; }
        get { return m_AtlasTexture; }
    }

    public Texture2D AtlasTextureBak
    {
        set { m_AtlasTextureBak = value; }
        get { return m_AtlasTextureBak; }
    }

    public int AtlasOperateCounter
    {
        set { m_OperateCounter = value; }
        get { return m_OperateCounter; }
    }

    public int AtlasSaveCounter
    {
        set { m_SavedCounter = value; }
        get { return m_SavedCounter; }
    }
}

public class SpriteItemInfo
{
    private string m_Name = string.Empty;
    private string m_Path = string.Empty;
    private Texture m_Image = null;

    public string Name
    {
        set { m_Name = value; }
        get { return m_Name; }
    }

    public string Path
    {
        set { m_Path = value; }
        get { return m_Path; }
    }

    public Texture Image
    {
        set { m_Image = value; }
        get { return m_Image; }
    }
}

public class OperateItemInfo
{
    private List<string> m_SpritePathTable;
    private AtlasProject m_Project;

    public List<string> SpritePathTable
    {
        set { m_SpritePathTable = value; }
        get { return m_SpritePathTable; }
    }

    public AtlasProject Project
    {
        set { m_Project = value; }
        get { return m_Project; }
    }
}

public class UIAtlasOperateUtility 
{//Atlas相关操作

    public ATLASOPERATE_ERROR_TYPE LoadAtlasProject(string projectPath, ref AtlasItemInfo atlasItemInfo, out AtlasConsistencyAnalyseResult analyseResult)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        analyseResult = null;
        Texture2D atlasTexture = null;

        if (string.IsNullOrEmpty(projectPath))
        {
            errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
            return errorType;
        }

        if(null == atlasItemInfo)
        {
            atlasItemInfo = new AtlasItemInfo();
        }

        do
        {
            errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

            if (!IsProjectPathValid(projectPath))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
                break;
            }

            ClearProject(atlasItemInfo.Project);

            atlasItemInfo.Project = new AtlasProject();
            if (null == atlasItemInfo.Project)
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;
            }

            errorType = CheckProjectErrorType(atlasItemInfo.Project.Load(projectPath));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckSingleAtlasConsistency(atlasItemInfo.Project, out analyseResult);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (analyseResult.AnalyseResult == ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT)
            {
                //errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ATLAS_UNCONSISTENT_ERROR;
                break;
            }
            //else if (analyseResult.AnalyseResult == ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING)
            //{
            //    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHDIR;
            //}

            errorType = UpdateAtlasReference(atlasItemInfo.Project);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            //载入工程中全部小图
            errorType = LoadAllSpriteImage(atlasItemInfo.Project);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            atlasItemInfo.AtlasTexture = atlasTexture;
            atlasItemInfo.AtlasTextureBak = atlasTexture;

        } while (false);


        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CreateExistProjectInstance(string projectPath, out AtlasProject project)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        project = null;

        if (string.IsNullOrEmpty(projectPath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
        }

        do
        {
            if (!IsProjectPathValid(projectPath))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
                break;
            }

            project = new AtlasProject();

            errorType = CheckProjectErrorType(project.Load(projectPath));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

        } while (false);

        return errorType;
    }

    public void CreateAllExistProjectInstance(string dirPath, out List<AtlasProject> projectTbl)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        projectTbl = new List<AtlasProject>();

        if (string.IsNullOrEmpty(dirPath))
        {
            return;
        }

        List<string> projectPathTbl = UniversalEditorUtility.GetAllFilePath(dirPath);

        foreach (var item in projectPathTbl)
        {
            AtlasProject newProject = null;
            errorType = CreateExistProjectInstance(item, out newProject);
            if (IsAtlasOperateFailed(errorType))
            {
                continue;
            }

            projectTbl.Add(newProject);
        }

        return;
    }

    public ATLASOPERATE_ERROR_TYPE UnLoadAtlasProject(AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        //string projectPath = atlasItemInfo.Project.Path;

        if (atlasItemInfo != null)
        {
            ClearProject(atlasItemInfo.Project);
            atlasItemInfo.AtlasTexture = null;
            atlasItemInfo.AtlasTextureBak = null;
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE AddSpriteToProject(string spritePath, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;
        bool bRet = false;

        if (string.IsNullOrEmpty(spritePath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ADDSPRITE_ERRORPATH;
        }

        if (
               (null == atlasItemInfo) 
            || (null == atlasItemInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            bRet = atlasItemInfo.Project.AddSpriteImage(spritePath);
            if(!bRet)
            {
                errorType = CheckProjectErrorType(atlasItemInfo.Project);
                break;
            }

            LoadSpriteImage(spritePath, atlasItemInfo.Project);

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            atlasItemInfo.AtlasTexture = atlasTexture;

            //errorType = UpdateAtlasReference(atlasItemInfo.Project);
            //if (IsAtlasOperateFailed(errorType))
            //{
            //    break;
            //}
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE AddSpriteToProject(string[] spritePathList, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;
        bool bRet = false;

        if (null == spritePathList)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ADDSPRITE_ERRORPATH;
        }

        if (
               (null == atlasItemInfo)
            || (null == atlasItemInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            foreach (var spritePath in spritePathList)
            {
                bRet = atlasItemInfo.Project.AddSpriteImage(spritePath);
                if (!bRet)
                {
                    errorType = CheckProjectErrorType(atlasItemInfo.Project);
                    break;
                }

                LoadSpriteImage(spritePath, atlasItemInfo.Project);
            }
            if(IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            atlasItemInfo.AtlasTexture = atlasTexture;

            //errorType = UpdateAtlasReference(atlasItemInfo.Project);
            //if (IsAtlasOperateFailed(errorType))
            //{
            //    break;
            //}
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE AddSpriteToProject(AtlasSpriteImage spriteImage, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;

        if (null == spriteImage)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        if (
               (null == atlasItemInfo)
            || (null == atlasItemInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            AtlasSpriteImage[] spriteImageTable = new AtlasSpriteImage[1];
            spriteImageTable[0] = spriteImage;
  
            errorType = CheckProjectErrorType(atlasItemInfo.Project.AddSpriteImage(spriteImage));
            if(IsAtlasOperateFailed(errorType))
            {
                break;
            }
            LoadSpriteImage(spriteImage.Path, atlasItemInfo.Project);
            if (spriteImage.ZoomScale != 1.0f)
            {
                errorType = ZoomSpriteFromProject(spriteImage.Path, spriteImage.ZoomScale, ref atlasItemInfo);
                if(IsAtlasOperateFailed(errorType))
                {
                    break;
                }
            }

            atlasItemInfo.Project.ModifyRefTableAfterAddSprite(spriteImageTable);

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            atlasItemInfo.AtlasTexture = atlasTexture;

            //errorType = UpdateAtlasReference(atlasItemInfo.Project);
            //if (IsAtlasOperateFailed(errorType))
            //{
            //    break;
            //}

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE AddSpriteToProject(AtlasSpriteImage[] spriteImages, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;

        if (null == spriteImages)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ADDSPRITE_ERRORPATH;
        }

        if (
               (null == atlasItemInfo)
            || (null == atlasItemInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            foreach (var item in spriteImages)
            {
                errorType = CheckProjectErrorType(atlasItemInfo.Project.AddSpriteImage(item));
                if (IsAtlasOperateFailed(errorType))
                {
                    break;
                }
                LoadSpriteImage(item.Path, atlasItemInfo.Project);
                if (item.ZoomScale != 1.0f)
                {
                    errorType = ZoomSpriteFromProject(item.Path, item.ZoomScale, ref atlasItemInfo);
                    if (IsAtlasOperateFailed(errorType))
                    {
                        break;
                    }
                }
            }
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            atlasItemInfo.Project.ModifyRefTableAfterAddSprite(spriteImages);

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            atlasItemInfo.AtlasTexture = atlasTexture;

            //errorType = UpdateAtlasReference(atlasItemInfo.Project);
            //if (IsAtlasOperateFailed(errorType))
            //{
            //    break;
            //}

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE RemoveSpriteFromProject(string spritePath, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;
        bool bRet = false;

        if (string.IsNullOrEmpty(spritePath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_REMOVESPRITE_ERRORPATH;
        }

        if (
               (null == atlasItemInfo)
            || (null == atlasItemInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            UnloadSpriteImage(spritePath, atlasItemInfo.Project.Path);

            bRet = atlasItemInfo.Project.RemoveSpriteImage(spritePath);
            if(!bRet)
            {
                errorType = CheckProjectErrorType(atlasItemInfo.Project);
                break;
            }

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            atlasItemInfo.AtlasTexture = atlasTexture;
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE RemoveSpriteFromProject(string[] spritePathList, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;
        bool bRet = false;

        if (null == spritePathList)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_REMOVESPRITE_ERRORPATH;
        }

        if (
               (null == atlasItemInfo)
            || (null == atlasItemInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            foreach (var spritePath in spritePathList)
            {
                UnloadSpriteImage(spritePath, atlasItemInfo.Project.Path);

                bRet = atlasItemInfo.Project.RemoveSpriteImage(spritePath);
                if (!bRet)
                {
                    errorType = CheckProjectErrorType(atlasItemInfo.Project);
                    break;
                }
            }

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            atlasItemInfo.AtlasTexture = atlasTexture;
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ZoomSpriteFromProject(string spritePath, float scaleFactor, ref AtlasItemInfo atlasItemInfo)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        Texture2D atlasTexture = null;

        if (
               string.IsNullOrEmpty(spritePath)
            || ((-0.000001f < scaleFactor) && (scaleFactor < 0.000001f))
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_REMOVESPRITE_ERRORPATH;
        }

        do
        {
            UIAtlasTempTextureManager.GetInstance().ZoomTexture(spritePath, atlasItemInfo.Project.Path, scaleFactor);
            atlasItemInfo.Project.SetSpriteImageZoom(spritePath, scaleFactor);

            errorType = MakeCurrentAtlasTexture(atlasItemInfo.Project, out atlasTexture);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            atlasItemInfo.AtlasTexture = atlasTexture;

        } while (false);


        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE MoveSpriteAmongProjects(List<OperateItemInfo> soruceInfo, AtlasProject destProject, ref List<AtlasItemInfo> sourceAtlasOutput, ref AtlasItemInfo destAtlasOutput, out List<UIAdjust_ModifyRefInfo> modifyRefTable)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        modifyRefTable = null;

        if (
               (null == soruceInfo)
            || (null == sourceAtlasOutput)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_ERRORSOURCE;
        }

        if (
               (null == destProject)
            || (null == destAtlasOutput)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_ERRORDEST;
        }

        AtlasItemInfo destAtlasInfo = new AtlasItemInfo();
        List<AtlasItemInfo> sourceAtlasInfo = new List<AtlasItemInfo>();
        List<AtlasSpriteImage> allSpriteImageTable = new List<AtlasSpriteImage>();
        List<string> allSpritePathList = new List<string>();
        //string sourceGUID = string.Empty;
        //string destGUID = string.Empty;

        modifyRefTable = new List<UIAdjust_ModifyRefInfo>();

        do
        {
            destAtlasInfo.Project = destProject;

            foreach (var item in soruceInfo)
            {
                AtlasItemInfo info = new AtlasItemInfo();
                info.Project = item.Project;
                List<UIAdjust_ModifyRefInfo> tempRefList = null;
                List<AtlasSpriteImage> tempSpriteList = null;
                errorType = PrepareRefInfoForMoveSprite(item.SpritePathTable, info.Project, destAtlasInfo.Project, ref tempRefList);
                if (!IsAtlasOperateFailed(errorType))
                {
                    modifyRefTable.AddRange(tempRefList);
                }

                GetSpriteImage(item.SpritePathTable.ToArray(), info.Project, out tempSpriteList);
                sourceAtlasInfo.Add(info);
                allSpritePathList.AddRange(item.SpritePathTable);
                allSpriteImageTable.AddRange(tempSpriteList);
            }

            if (CheckSpriteExistInTartgetProject(allSpritePathList.ToArray(), destProject))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_SPRITEALREADYEXIST;
                break;
            }

            //处理目标工程
            //errorType = AddSpriteToProject(allSpritePathList.ToArray(), ref destAtlasOutput);
            errorType = AddSpriteToProject(allSpriteImageTable.ToArray(), ref destAtlasOutput);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            //destAtlasInfo.Project.ModifyRefTableAfterAddSprite(allSpriteImageTable.ToArray());

            //处理源工程
            for (int index = 0; index < sourceAtlasInfo.Count; index++)
            {
                AtlasItemInfo tempItem = sourceAtlasOutput[index];
                errorType = RemoveSpriteFromProject(soruceInfo[index].SpritePathTable.ToArray(), ref tempItem);
                if (IsAtlasOperateFailed(errorType))
                {
                    break;
                }
                tempItem.Project.ModifyRefTableAfterRemoveSprite(soruceInfo[index].SpritePathTable.ToArray());
                //sourceAtlasOutput[index] = tempItem;
            }
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

        } while (false);


        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE MoveSpriteAmongProjects(AtlasProject sourceProject, List<OperateItemInfo> destInfos, ref AtlasItemInfo sourceAtlasOutput, ref List<AtlasItemInfo> destAtlasOutputs, out List<UIAdjust_ModifyRefInfo> modifyRefTable)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        modifyRefTable = null;

        if (
               (null == sourceProject)
            || (null == sourceAtlasOutput)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_ERRORSOURCE;
        }

        if (
               (null == destInfos)
            || (null == destAtlasOutputs)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_ERRORDEST;
        }


        modifyRefTable = new List<UIAdjust_ModifyRefInfo>();
       
        List<AtlasSpriteImage> allSpriteImageTable = new List<AtlasSpriteImage>();
        List<string> allSpritePathList = new List<string>();
        //string sourceGUID = string.Empty;
        //string destGUID = string.Empty;

        do
        {
            foreach (var operateInfo in destInfos)
            {
                AtlasItemInfo info = new AtlasItemInfo();
                info.Project = operateInfo.Project;
                List<UIAdjust_ModifyRefInfo> tempRefList = null;
                List<AtlasSpriteImage> tempSpriteList = null;

                errorType = PrepareRefInfoForMoveSprite(operateInfo.SpritePathTable, sourceProject, info.Project, ref tempRefList);
                if (!IsAtlasOperateFailed(errorType))
                {
                    modifyRefTable.AddRange(tempRefList);
                }
                GetSpriteImage(operateInfo.SpritePathTable.ToArray(), sourceAtlasOutput.Project, out tempSpriteList);
                allSpritePathList.AddRange(operateInfo.SpritePathTable);
                allSpriteImageTable.AddRange(tempSpriteList);

                if (CheckSpriteExistInTartgetProject(operateInfo.SpritePathTable.ToArray(), operateInfo.Project))
                {
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_SPRITEALREADYEXIST;
                    break;
                }
            }
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            //处理目标工程
            for (int index = 0; index < destInfos.Count; index++)
            {
                AtlasItemInfo tempItem = destAtlasOutputs[index];
                List<AtlasSpriteImage> spriteImageList = null;
                GetSpriteImage(destInfos[index].SpritePathTable.ToArray(), sourceAtlasOutput.Project, out spriteImageList);
                //errorType = AddSpriteToProject(destInfos[index].SpritePathTable.ToArray(), ref tempItem);
                errorType = AddSpriteToProject(spriteImageList.ToArray(), ref tempItem);
                if (IsAtlasOperateFailed(errorType))
                {
                    break;
                }

                List<AtlasSpriteImage> addSpriteList = null;
                GetSpriteImage(destInfos[index].SpritePathTable.ToArray(), tempItem.Project, out addSpriteList);
                //tempItem.Project.ModifyRefTableAfterAddSprite(addSpriteList.ToArray());
            }

            errorType = RemoveSpriteFromProject(allSpritePathList.ToArray(), ref sourceAtlasOutput);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            sourceAtlasOutput.Project.ModifyRefTableAfterRemoveSprite(allSpritePathList.ToArray());

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CopySpriteAmongProjects(List<OperateItemInfo> soruceInfo, ref AtlasItemInfo destAtlasOutput)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (null == soruceInfo)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_ERRORSOURCE;
        }

        if (null == destAtlasOutput)    
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_ERRORDEST;
        }

        List<string> allSpritePathList = new List<string>();

        do
        {
            foreach (var item in soruceInfo)
            {
                allSpritePathList.AddRange(item.SpritePathTable);
            }

            if (CheckSpriteExistInTartgetProject(allSpritePathList.ToArray(), destAtlasOutput.Project))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MOVETO_SPRITEALREADYEXIST;
                break;
            }

            //处理目标工程
            errorType = AddSpriteToProject(allSpritePathList.ToArray(), ref destAtlasOutput);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteProjectPathConfig(string projectPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if(string.IsNullOrEmpty(projectPath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteProjectPath(projectPath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadProjectPathConfig(out string projectPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        projectPath = UIAtlasEditorConfig.ReadProjectPath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteImageBasePathConfig(string imageBasePath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (string.IsNullOrEmpty(imageBasePath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteImageBasePath(imageBasePath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadImageBasePathConfig(out string imageBasePath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        imageBasePath = UIAtlasEditorConfig.ReadImageBasePath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteConsistencyResultPathConfig(string resultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (string.IsNullOrEmpty(resultPath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteConsistencyResultPath(resultPath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadConsistencyResultPathConfig(out string resultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        resultPath = UIAtlasEditorConfig.ReadConsistencyResultPath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteConsistencyPrefabPathConfig(string prefabPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (string.IsNullOrEmpty(prefabPath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteConsistencyPrefabPath(prefabPath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadConsistencyPrefabPathConfig(out string prefabPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        prefabPath = UIAtlasEditorConfig.ReadConsistencyPrefabPath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteReferencePrefabPathConfig(string prefabPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (string.IsNullOrEmpty(prefabPath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteReferencePrefabPath(prefabPath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadReferencePrefabPathConfig(out string prefabPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        prefabPath = UIAtlasEditorConfig.ReadReferencePrefabPath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteReferenceScenePathConfig(string scenePath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (string.IsNullOrEmpty(scenePath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteReferenceScenePath(scenePath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadReferenceScenePathConfig(out string scenePath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        scenePath = UIAtlasEditorConfig.ReadReferenceScenePath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE WriteReferenceResultPathConfig(string resultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (string.IsNullOrEmpty(resultPath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_CONFIGG_ERRORPPATH;
        }

        UIAtlasEditorConfig.WriteReferenceResultPath(resultPath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ReadReferenceResultPathConfig(out string resultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        resultPath = UIAtlasEditorConfig.ReadReferenceResultPath();

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE UpdateAtlasReference(AtlasProject project)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (null == project)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        do
        {
            string atlasPath = project.AtlasSavePath + project.Name + ".prefab";
            List<ReferenceInfo> tempTable = new List<ReferenceInfo>();

            errorType = PrepareAtlasReferenceTable(atlasPath, ref tempTable);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckProjectErrorType(project.UpdateAtlasReferenceTable(tempTable));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

        } while (false);


        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE UpdateAtlasReference(List<AtlasProject> projectTbl)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
     
        if (null == projectTbl)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }


        UpdateReferenceProgresser.GetInstance().InitProgresser(projectTbl.Count, "引用关系分析中");
        int count = 0;

        foreach(var item in projectTbl)
        {
            UpdateAtlasReference(item);
            UpdateReferenceProgresser.GetInstance().UpdateProgress(count++);
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE RebuildProject(ref AtlasItemInfo rebuildAtlasInfo, bool IsForceBuild)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if(
               (null == rebuildAtlasInfo)
            || (null == rebuildAtlasInfo.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_REBUILD_ERRORPROJECT;
        }

        if (
               (rebuildAtlasInfo.AtlasOperateCounter != rebuildAtlasInfo.AtlasSaveCounter) 
            || IsForceBuild
            )
        {
            do
            {
                //生成AtlasPrefab
                errorType = MakeCurrentAtlasPrefab(rebuildAtlasInfo);
                if (IsAtlasOperateFailed(errorType))
                {
                    break;
                }

                //生成AtlasPNG
                errorType = MakeCurrentAtlasPng(rebuildAtlasInfo);
                if (IsAtlasOperateFailed(errorType))
                {
                    break;
                }

                //保存工程
                errorType = CheckProjectErrorType(rebuildAtlasInfo.Project.Save(false));
                if(IsAtlasOperateFailed(errorType))
                {
                    break;
                }

                rebuildAtlasInfo.AtlasTextureBak = rebuildAtlasInfo.AtlasTexture;

                UniversalEditorLog.DebugLog("Atlas生成结束：" + rebuildAtlasInfo.Project.Path);
            } while (false);

            if(onRebuildAtlas != null)
            {
                onRebuildAtlas(rebuildAtlasInfo.Project.Path);
            }
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE RebuildProject(AtlasItemInfo[] rebuildAtlasInfo, bool IsForceBuild)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (null == rebuildAtlasInfo)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_REBUILD_ERRORPROJECT;
        }

        foreach (var projectItem in rebuildAtlasInfo)
        {
            AtlasItemInfo temp = projectItem;
            errorType = RebuildProject(ref temp, IsForceBuild);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CheckAtlasConsistency(out AtlasConsistencyAnalyseResult analyseResult)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        CONSISTENCY_ANALYSE_ERROR_TYPE analyserErrorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;
        analyseResult = new AtlasConsistencyAnalyseResult();

        do
        {
            string atlasProjectDir = GetProjectPathConfig();
            if (string.IsNullOrEmpty(atlasProjectDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_PROJECTDIC_ERROR;
                break;
            }

            string atlasPrefabDir = GetConsistencyPrefabPathConfig();
            if (string.IsNullOrEmpty(atlasPrefabDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR;
                break;
            }

            string sourceABDir = GetImageBasePathConfig();
            if (string.IsNullOrEmpty(sourceABDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR;
                break;
            }

            List<AtlasProject> projectTbl = null;
            CreateAllExistProjectInstance(atlasProjectDir, out projectTbl);
            if(
                   (null == projectTbl)
                || (0 == projectTbl.Count)
                )
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
                break;
            }
            
            AtlasConsistencyInputInfo inputInfo = new AtlasConsistencyInputInfo();         
            inputInfo.AtlasProjectDir = atlasProjectDir;
            inputInfo.AtlasPrefabDir = atlasPrefabDir;
            inputInfo.SourceSpriteDir = sourceABDir;
            inputInfo.ProjectTable = projectTbl;

            AtlasConsistencyAnalyser analyser = new AtlasConsistencyAnalyser(inputInfo, out analyserErrorType);
            errorType = CheckAnalyserError(analyserErrorType);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckAnalyserError(analyser.AnalyseAtlasConsistent(true, out analyseResult));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CheckSingleAtlasConsistency(AtlasProject project, out AtlasConsistencyAnalyseResult analyseResult)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        CONSISTENCY_ANALYSE_ERROR_TYPE analyserErrorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;
        analyseResult = null;

        do
        {
            string atlasProjectDir = GetProjectPathConfig();
            if (string.IsNullOrEmpty(atlasProjectDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_PROJECTDIC_ERROR;
                break;
            }

            string sourceABDir = GetImageBasePathConfig();
            if (string.IsNullOrEmpty(sourceABDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR;
                break;
            }

            string prefabPath = GetConsistencyPrefabPathConfig();

            AtlasConsistencyInputInfo inputInfo = new AtlasConsistencyInputInfo();
            inputInfo.AtlasProjectDir = atlasProjectDir;
            inputInfo.SourceSpriteDir = sourceABDir;
            inputInfo.AtlasPrefabDir = prefabPath;

            AtlasConsistencyAnalyser analyser = new AtlasConsistencyAnalyser(inputInfo, out analyserErrorType);
            errorType = CheckAnalyserError(analyserErrorType);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckAnalyserError(analyser.AnalyseSingleAtlasConsistent(project, out analyseResult));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ExportConsistencyInfo(List<AtlasConsistencyInfo> consistencyInfo, out string resultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        resultPath = string.Empty;

        do
        {
            AtlasCosistencyExporter exporter = new AtlasCosistencyExporter();

            string csvPath = UIAtlasOperateUtility.GetInstance().GetConsistencyResultPathConfig();
            if (string.IsNullOrEmpty(csvPath))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR;
                break;
            }
            string fixFilePath = string.Empty;

            exporter.Export(consistencyInfo, csvPath, out fixFilePath);

            resultPath = fixFilePath;

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CheckAndExportConsistency(bool isNeedChecePrefab, out List<AtlasProject> projectTbl, out ANALYSERESULT_TYPE resultType, out string resultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        CONSISTENCY_ANALYSE_ERROR_TYPE analyserErrorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;

        resultType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;
        resultPath = string.Empty;
        projectTbl = null;

        do
        {
            AtlasConsistencyAnalyseResult analyseResult = new AtlasConsistencyAnalyseResult();

            string atlasProjectDir = string.Empty;
            string atlasPrefabDir = string.Empty;
            string sourceABDir = string.Empty;
            string csvPath = string.Empty;

            errorType = CheckAnalyseConsistencyConfig(isNeedChecePrefab, out atlasProjectDir, out atlasPrefabDir, out sourceABDir, out csvPath);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            CreateAllExistProjectInstance(atlasProjectDir, out projectTbl);
            if(
                   (null == projectTbl)
                || (0 == projectTbl.Count)
                )
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
                break;
            }

            AtlasConsistencyInputInfo inputInfo = new AtlasConsistencyInputInfo();
            inputInfo.AtlasProjectDir = atlasProjectDir;
            inputInfo.AtlasPrefabDir = atlasPrefabDir;
            inputInfo.SourceSpriteDir = sourceABDir;
            inputInfo.ProjectTable = projectTbl;

            AtlasConsistencyAnalyser analyser = new AtlasConsistencyAnalyser(inputInfo, out analyserErrorType);
            errorType = CheckAnalyserError(analyserErrorType);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckAnalyserError(analyser.AnalyseAtlasConsistent(isNeedChecePrefab, out analyseResult));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            resultType = analyseResult.AnalyseResult;
            if (resultType != ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT)
            {
                AtlasCosistencyExporter exporter = new AtlasCosistencyExporter();

                string fixFilePath = string.Empty;

                exporter.Export(analyseResult.ConsistencyInfo, csvPath, out fixFilePath);

                resultPath = fixFilePath;
            }

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CheckAnalyseConsistencyConfig(bool isCheckPrefab, out string atlasProjectDir, out string atlasPrefabDir, out string sourceABDir, out string csvPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        atlasProjectDir = string.Empty;
        atlasPrefabDir = string.Empty;
        sourceABDir = string.Empty;
        csvPath = string.Empty;

        do
        {
            atlasProjectDir = GetProjectPathConfig();
            if (string.IsNullOrEmpty(atlasProjectDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_PROJECTDIC_ERROR;
                break;
            }

            if (isCheckPrefab)
            {
                atlasPrefabDir = GetConsistencyPrefabPathConfig();
                if (string.IsNullOrEmpty(atlasPrefabDir))
                {
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR;
                    break;
                }
            }

            sourceABDir = GetImageBasePathConfig();
            if (string.IsNullOrEmpty(sourceABDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR;
                break;
            }

            csvPath = UIAtlasOperateUtility.GetInstance().GetConsistencyResultPathConfig();
            if (string.IsNullOrEmpty(csvPath))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_RESULTPATH_ERROR;
                break;
            }
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ExportDependency(out string resultDidr)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        resultDidr = string.Empty;

        do
        {
            DependencyInfo dependencyInfo = null;
            DependencInputInfo inputInfo = new DependencInputInfo();

            string prefabPath = string.Empty;
            string scenePath = string.Empty;
            string exportDir = string.Empty;

            errorType = CheckExportReferenceConfig(out prefabPath, out scenePath, out exportDir);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            inputInfo.PrefebPath = prefabPath;
            inputInfo.ScenePath = scenePath;

            AtlasReferenceAnalyser analyser = new AtlasReferenceAnalyser(inputInfo);

            errorType = CheckReferenceAnalyserError(analyser.AnalyseDependency(out dependencyInfo));
            if(IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if(
                (null == dependencyInfo)
                || (null == dependencyInfo.DependencyInfoTbl)
                || (0 == dependencyInfo.DependencyInfoTbl.Count)
                )
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_REFERENCEINFO_NONE;
                break;
            }

            AtlasReferenceExporter exporter = new AtlasReferenceExporter();

            exporter.ExportDependency(exportDir, dependencyInfo);

            resultDidr = exportDir;
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ExportReverseDependency(out string dependencyResultDidr, out string consistentResultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        ANALYSERESULT_TYPE consistenyType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;
       
        consistentResultPath = string.Empty;
        dependencyResultDidr = string.Empty;

        List<AtlasProject> projectTbl = null;

        do
        {
            ReverseDependencyInfo reverseDependencyInfo = null;
            DependencInputInfo inputInfo = new DependencInputInfo();
        
            string prefabPath = string.Empty;
            string scenePath = string.Empty;
            string exportDir = string.Empty;

            errorType = CheckExportReferenceConfig(out prefabPath, out scenePath, out exportDir);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            } 
            errorType = CheckAndExportConsistency(false, out projectTbl, out consistenyType, out consistentResultPath);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT == consistenyType)
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_UNCONSISTENT_ERROR;
                break;
            }

            errorType = UpdateAtlasReference(projectTbl);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }
            inputInfo.ProjectTbl = projectTbl;

            AtlasReferenceAnalyser analyser = new AtlasReferenceAnalyser(inputInfo);

            errorType = CheckReferenceAnalyserError(analyser.AnalyseReverseDependency(out reverseDependencyInfo));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (
                   (null == reverseDependencyInfo)
                || (null == reverseDependencyInfo.ReverseDependencyInfoTbl)
                || (0 == reverseDependencyInfo.ReverseDependencyInfoTbl.Count)
                )
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_REFERENCEINFO_NONE;
                break;
            }

            AtlasReferenceExporter exporter = new AtlasReferenceExporter();

            exporter.ExportReverseDependency(exportDir, reverseDependencyInfo);

            dependencyResultDidr = exportDir;

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ExportNoneDependency(out string dependencyResultDidr, out string consistentResultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        ANALYSERESULT_TYPE consistenyType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;

        consistentResultPath = string.Empty;
        dependencyResultDidr = string.Empty;

        List<AtlasProject> projectTbl = null;

        do
        {
            NoneDependencyInfo noneDependencyInfo = null;
            DependencInputInfo inputInfo = new DependencInputInfo();

            string prefabPath = string.Empty;
            string scenePath = string.Empty;
            string exportDir = string.Empty;

            errorType = CheckExportReferenceConfig(out prefabPath, out scenePath, out exportDir);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            } 

            errorType = CheckAndExportConsistency(false, out projectTbl, out consistenyType, out consistentResultPath);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT == consistenyType)
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_UNCONSISTENT_ERROR;
                break;
            }

            errorType = UpdateAtlasReference(projectTbl);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            inputInfo.PrefebPath = prefabPath;
            inputInfo.ScenePath = scenePath;
            inputInfo.ProjectTbl = projectTbl;

            AtlasReferenceAnalyser analyser = new AtlasReferenceAnalyser(inputInfo);

            errorType = CheckReferenceAnalyserError(analyser.AnalyseNoneDependency(out noneDependencyInfo));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (
                   (null == noneDependencyInfo)
                || (null == noneDependencyInfo.NoneDependencyInfoTbl)
                || (0 == noneDependencyInfo.NoneDependencyInfoTbl.Count)
                )
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_REFERENCEINFO_NONE;
                break;
            }

            AtlasReferenceExporter exporter = new AtlasReferenceExporter();

            exporter.ExportNoneDependency(exportDir, noneDependencyInfo);

            dependencyResultDidr = exportDir;

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ExportAllDependency(out string dependencyResultDidr, out string consistentResultPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        ANALYSERESULT_TYPE consistenyType = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;

        consistentResultPath = string.Empty;
        dependencyResultDidr = string.Empty;

        List<AtlasProject> projectTbl = null;

        do
        {
            DependencInputInfo inputInfo = new DependencInputInfo();

            DependencyInfo dependencyInfo = null;
            ReverseDependencyInfo reverseDependencyInfo = null;
            NoneDependencyInfo noneDependencyInfo = null;

            string prefabPath = string.Empty;
            string scenePath = string.Empty;
            string exportDir = string.Empty;

            errorType = CheckExportReferenceConfig(out prefabPath, out scenePath, out exportDir);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }         

            errorType = CheckAndExportConsistency(false, out projectTbl, out consistenyType, out consistentResultPath);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT == consistenyType)
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_UNCONSISTENT_ERROR;
                break;
            }

            errorType = UpdateAtlasReference(projectTbl);
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            inputInfo.PrefebPath = prefabPath;
            inputInfo.ScenePath = scenePath;
            inputInfo.ProjectTbl = projectTbl;

            AtlasReferenceAnalyser analyser = new AtlasReferenceAnalyser(inputInfo);
 
            errorType = CheckReferenceAnalyserError(analyser.AnalyseDependency(out dependencyInfo));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckReferenceAnalyserError(analyser.AnalyseReverseDependency(out reverseDependencyInfo));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            errorType = CheckReferenceAnalyserError(analyser.AnalyseNoneDependency(out noneDependencyInfo));
            if (IsAtlasOperateFailed(errorType))
            {
                break;
            }

            if (
                   ((null == noneDependencyInfo)
                    || (null == noneDependencyInfo.NoneDependencyInfoTbl)
                    || (0 == noneDependencyInfo.NoneDependencyInfoTbl.Count))
                && ((null == reverseDependencyInfo)
                    || (null == reverseDependencyInfo.ReverseDependencyInfoTbl)
                    || (0 == reverseDependencyInfo.ReverseDependencyInfoTbl.Count))
                && ((null == noneDependencyInfo)
                    || (null == noneDependencyInfo.NoneDependencyInfoTbl)
                    || (0 == noneDependencyInfo.NoneDependencyInfoTbl.Count))
                )
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_WARNING_REFERENCEINFO_NONE;
                break;
            }

            AtlasReferenceExporter exporter = new AtlasReferenceExporter();

            exporter.ExportDependency(exportDir, dependencyInfo);
            exporter.ExportReverseDependency(exportDir, reverseDependencyInfo);
            exporter.ExportNoneDependency(exportDir, noneDependencyInfo);

            dependencyResultDidr = exportDir;
        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE CheckExportReferenceConfig(out string prefabPath, out string scenePath, out string exportDir)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        prefabPath = string.Empty;
        scenePath = string.Empty;
        exportDir = string.Empty;

        do
        {
            prefabPath = GetReferencePrefabPathConfig();
            if (string.IsNullOrEmpty(prefabPath))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR;
                break;
            }

            scenePath = GetReferenceScenePathConfig();
            if (string.IsNullOrEmpty(scenePath))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR;
                break;
            }

            exportDir = GetReferenceResultPathConfig();
            if (string.IsNullOrEmpty(exportDir))
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_RESULTPATH_ERROR;
                break;
            }

        } while (false);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ConfigDependencyFilter()
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        AtlasReferenceAnalyser analyser = new AtlasReferenceAnalyser();

        string configDir = GetReferenceResultPathConfig();

        errorType = CheckReferenceAnalyserError(analyser.OpenAnalyseConfigFile(configDir));           

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE OpenExportFile(string filePath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        if (string.IsNullOrEmpty(filePath))
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
        }

        CSVOperator.OpenFile(filePath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE PrepareAtlasReferenceTable(string atlasPath, ref  List<ReferenceInfo> refTable)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if(string.IsNullOrEmpty(atlasPath))
        {
            refTable = null;
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_REFERENCEATLAS_ERRORPATH;
        }

        string[] allAssetsPath = AssetDatabase.GetAllAssetPaths();

        foreach (var item in allAssetsPath)
        {
            if (IsReferenceFileType(item))
            {
                if (IsReferenceFile(atlasPath, item))
                {
                    ReferenceInfo newInfo = new ReferenceInfo();
                    newInfo.ReferenceFilePath = item;
                    newInfo.ReferencingSprite = GetReferencingSprite(atlasPath, item);

                    refTable.Add(newInfo);
                }
            }
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE GetSpriteImageZoom(string spritePath, AtlasProject project, out float zoomScale)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        zoomScale = 0f;

        if(
            string.IsNullOrEmpty(spritePath)
            || (null == project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        project.GetSpriteImageZoom(spritePath, out zoomScale);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE GetSpriteImageZoomTexture2D(string spritePath, string projectPath, out Texture2D spiteTexture)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        spiteTexture = null;

        if (
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        spiteTexture = UIAtlasTempTextureManager.GetInstance().GetSpriteZoomTexture(spritePath, projectPath);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE GetSpriteImageTexture2D(string spritePath, string projectPath, out Texture2D spriteIamgeTexTure2D)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        spriteIamgeTexTure2D = null;

        if (
               string.IsNullOrEmpty(projectPath)
            || string.IsNullOrEmpty(spritePath)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        spriteIamgeTexTure2D = UIAtlasTempTextureManager.GetInstance().GetSpriteTexture(spritePath, projectPath);

        return errorType;
    }

    public List<SpriteItemInfo> GetSpriteItems(AtlasProject project)
    {
        if (null == project)
        {
            return null;
        }

        List<SpriteItemInfo> itemInfo = new List<SpriteItemInfo>();
        Texture2D spriteImage = null;

        List<AtlasSpriteImage> sprites = project.GetAllSprites();
        foreach (var sprite in sprites)
        {
            SpriteItemInfo newItem = new SpriteItemInfo();
            spriteImage = UIAtlasTempTextureManager.GetInstance().GetSpriteZoomTexture(sprite.Path, project.Path);

            newItem.Name = sprite.Name;
            newItem.Path = sprite.Path;
            newItem.Image = spriteImage;

            itemInfo.Add(newItem);
        }

        return itemInfo;
    }

    public ATLASOPERATE_ERROR_TYPE MakeCurrentAtlasTexture(AtlasProject project, out Texture2D atlasTexture)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        atlasTexture = null;

        if (null == project)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_OPERATE_WITH_NONEPROJECT;
        }

        List<Texture2D> texCaches = UIAtlasTempTextureManager.GetInstance().GetAllSpriteZoomTexture2D(project.Path);
        texCaches.Sort(Compare);
        List<Texture2D> NGUITextures = UIAtlasAPIForNGUI.FixNGUISpriteTextures(texCaches);
        if (null != texCaches)
        {//存在纹理资源

            //打包纹理
            PackageTexture(NGUITextures, out atlasTexture);
        }
        else
        {//不存在

            //设定失败类型为小图不存在
            errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_GETATLASTEXUTRE_WITH_NONESPRITE;
        }

        return errorType;
    }
  
    public ATLASOPERATE_ERROR_TYPE MakeCurrentAtlasPng(AtlasItemInfo atlasItem)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if(
            (null == atlasItem)
            || (null == atlasItem.Project)
            || (null == atlasItem.AtlasTexture)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MAKEPNG_ERRORPROJECT;
        }

        //创建png文件
        byte[] bytes = atlasItem.AtlasTexture.EncodeToPNG();
        string newPath = atlasItem.Project.ImagePath;

        UniversalEditorUtility.MakeFileWriteable(newPath);
        System.IO.File.WriteAllBytes(newPath, bytes);

        bytes = null;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Texture2D tex = NGUIEditorTools.ImportTexture(newPath, false, true, !NGUISettings.atlas.premultipliedAlpha);

        NGUISettings.atlas.spriteMaterial.mainTexture = tex;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE MakeCurrentAtlasPrefab(AtlasItemInfo atlasItem)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        string outputPath = string.Empty;

        if (
            (null == atlasItem)
            || (null == atlasItem.Project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MAKEPNG_ERRORPROJECT;
        }

        Texture2D tex = null;
        outputPath = atlasItem.Project.DescribePath;

        List<Texture> spriteTexTbl = GetSpriteTexture(atlasItem.Project);
        spriteTexTbl.Sort(Compare);
        UIAtlasAPIForNGUI.MakeAtlasPrefab(outputPath, spriteTexTbl, out tex);

        atlasItem.AtlasTexture = tex;
        atlasItem.AtlasTextureBak = atlasItem.AtlasTexture;

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE ChangeReferenceAsset(List<UIAdjust_ModifyRefInfo> modifyRefAsset)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if (null == modifyRefAsset)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_MODIFYREF_FILEERROR;
        }

        Dictionary<string, List<RefModifyInfo>> refModifyInfoTable = new Dictionary<string, List<RefModifyInfo>>();

        for (int index = modifyRefAsset.Count - 1; index >= 0; index--)
        {
            for (int subIndex = index - 1; subIndex >= 0; subIndex--)
            {
                if (
                       (modifyRefAsset[subIndex].RefAssetFilePath == modifyRefAsset[index].RefAssetFilePath)
                    && (modifyRefAsset[subIndex].SpriteName == modifyRefAsset[index].SpriteName)
                    && (modifyRefAsset[subIndex].DestAtlasGUID == modifyRefAsset[index].SourceAtlasGUID)
                    )
                {
                    modifyRefAsset[subIndex].DestAtlasGUID = modifyRefAsset[index].DestAtlasGUID;
                    modifyRefAsset.RemoveAt(index);
                    break;
                }
            }
        }

        foreach (var item in modifyRefAsset)
        {
            RefModifyInfo newInfo = new RefModifyInfo();
            newInfo.SpriteName = item.SpriteName;
            newInfo.SourceAtlasGUID = item.SourceAtlasGUID;
            newInfo.DestAtlasGUID = item.DestAtlasGUID;

            if (refModifyInfoTable.ContainsKey(item.RefAssetFilePath))
            {
                List<RefModifyInfo> newInfoList = null;
                if (refModifyInfoTable.TryGetValue(item.RefAssetFilePath, out newInfoList))
                {
                    newInfoList.Add(newInfo);
                }
            }
            else
            {
                List<RefModifyInfo> newInfoList = new List<RefModifyInfo>();
                newInfoList.Add(newInfo);

                refModifyInfoTable.Add(item.RefAssetFilePath, newInfoList);
            }
        }

        foreach (var item in refModifyInfoTable)
        {
            ChangeRefAssetFile(item.Key, item.Value);
        }

        AssetDatabase.Refresh();
        return errorType;
    }
   
    public ATLASOPERATE_ERROR_TYPE GetAtlasRefTable(AtlasProject project , out List<string> refTable)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        refTable = new List<string>();

        if (null == project)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE;
        }

        foreach (var refItem in project.ReferenceTable)
        {
            refTable.Add(refItem.ReferenceFilePath);
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE GetSpriteRefTable(AtlasProject project, string spritePath, out List<string> refTable)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        refTable = new List<string>();

        if(
            (null == project)
            || (string.IsNullOrEmpty(spritePath))
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE;
        }
        AtlasSpriteImage spriteImage = null;

        if(project.GetSpriteImage(spritePath, out spriteImage))
        {
            refTable.AddRange(spriteImage.ReferenceTable);
        }

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE SetAtlasOutputPath(AtlasProject project, string atalsOutputPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        if(
               (null == project)
            || (string.IsNullOrEmpty(atalsOutputPath))
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }
        string projectName = Path.GetFileNameWithoutExtension(project.Path);
        if (!atalsOutputPath.EndsWith("/") && !atalsOutputPath.EndsWith("\\"))
        {
            atalsOutputPath = atalsOutputPath + "/";
        }

        project.AtlasSavePath = atalsOutputPath;
        project.ImagePath = atalsOutputPath + projectName + ".png";
        project.DescribePath = atalsOutputPath + projectName + ".prefab";

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE GetAtlasOutputPath(AtlasProject project, out string atalsOutputPath)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        atalsOutputPath = string.Empty;

        if(null == project)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        atalsOutputPath = project.AtlasSavePath;

        return errorType;
    }

    public ATLASOPERATE_ERROR_TYPE GetSpriteImage(string[] spritePathTable, AtlasProject project, out List<AtlasSpriteImage> outputSpriteImages)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        outputSpriteImages = new List<AtlasSpriteImage>();
        
        if (
               (null == spritePathTable)
            || (null == project)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        AtlasSpriteImage spriteImage = null;

        foreach (var spritePathItem in spritePathTable)
        {
            if (!string.IsNullOrEmpty(spritePathItem))
            {
                if (project.GetSpriteImage(spritePathItem, out spriteImage))
                {
                    outputSpriteImages.Add(spriteImage);
                }
            }
        }

        return errorType;
    }

    public bool CheckReadOnlyFile(out string firstReadOnlyPath)
    {
        bool bRet = false;
        firstReadOnlyPath = string.Empty;
        string[] allAssetsPath = AssetDatabase.GetAllAssetPaths();

        foreach (var item in allAssetsPath)
        {
            if (!IsReferenceFileType(item))
            {
                continue;
            }

            FileInfo newInfo = new FileInfo(item);

            if (FileAttributes.ReadOnly == (newInfo.Attributes & FileAttributes.ReadOnly))
            {
                bRet = true;
                firstReadOnlyPath = item;
                break;
            }
        }

        return bRet;
    }

    public string GetProjectPathConfig()
    {
        return UIAtlasEditorConfig.ProjectPath;
    }

    public string GetImageBasePathConfig()
    {
        return UIAtlasEditorConfig.ImageBasePath;
    }

    public string GetConsistencyResultPathConfig()
    {
        return UIAtlasEditorConfig.ConsistencyResultPath;
    }

    public string GetConsistencyPrefabPathConfig()
    {
        return UIAtlasEditorConfig.ConsistencyPreafabPath;
    }

    public string GetReferencePrefabPathConfig()
    {
        return UIAtlasEditorConfig.ReferencePrefabPath;
    }

    public string GetReferenceScenePathConfig()
    {
        return UIAtlasEditorConfig.ReferenceScenePath;
    }

    public string GetReferenceResultPathConfig()
    {
        return UIAtlasEditorConfig.ReferenceResultPath;
    }

    private bool CheckSpriteExistInTartgetProject(string[] spritePath, AtlasProject destProject)
    {
        bool bRet = false;
        if(
               (null == spritePath)
            || (null == destProject)
            )
        {
            return false;
        }

        foreach (var item in spritePath)
        {
            AtlasSpriteImage spriteImage = null;
            if (destProject.GetSpriteImage(item, out spriteImage))
            {
                bRet = true;
                break;
            }
        }
        return bRet;
    }

    private bool IsProjectPathValid(string projectPath)
    {
        bool bRet = false;

        projectPath = projectPath.Replace(@"\", @"/");
        projectPath = projectPath.Replace(@"/", @"/");

        if(string.IsNullOrEmpty(projectPath))
        {
            return false;
        }

        if (!projectPath.EndsWith(".atlasproj"))
        {
            return false;
        }

        if (projectPath.Contains(GetProjectPathConfig()))
        {
            bRet = true;
        }

        return bRet;
    }

    private void ChangeRefAssetFile(string refFileName, List<RefModifyInfo>refModifyInfo)
    {
        if(
            (string.IsNullOrEmpty(refFileName))
            || (null == refModifyInfo)
            )
        {
            return;
        }

        bool isNeedWriteFile = false;

        string[] lines = File.ReadAllLines(refFileName);

        for(int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            int atlasLine = lineIndex;
            int spriteLine = lineIndex + 1;

            if(
                   (!lines[atlasLine].StartsWith(m_AtlasTagStr))
                || (!lines[spriteLine].StartsWith(m_SpriteTagStr))
                )
            {
                continue;
            }

            lineIndex++;

            string guid = string.Empty;
            string[] atlasStr = lines[atlasLine].Split(' ');
            for (int index = 0; index < atlasStr.Length - 1; index++)
            {
                if (atlasStr[index] != m_GuidTagStr)
                {
                    continue;
                }

                guid = atlasStr[index + 1].TrimEnd(',');
                break;
            }
            if (string.IsNullOrEmpty(guid))
            {
                continue;
            }

            string sprite = string.Empty;
            string[] spriteStr = lines[spriteLine].Split(' ');
            sprite = spriteStr[3].TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(sprite))
            {
                continue;
            }

            foreach (var item in refModifyInfo)
            {
                if (
                       (sprite == item.SpriteName)
                    && (guid == item.SourceAtlasGUID)
                    )
                {
                    lines[atlasLine] = lines[atlasLine].Replace(guid, item.DestAtlasGUID);
                    isNeedWriteFile = true;
                    break;
                }
            }

        }

        if(isNeedWriteFile)
        {
            UniversalEditorUtility.MakeFileWriteable(refFileName);
            File.WriteAllLines(refFileName, lines);
            UniversalEditorLog.DebugLog("文件引用关系变更：" + refFileName);
        }
    }

    private ATLASOPERATE_ERROR_TYPE PrepareRefInfoForMoveSprite(List<string> spriteList, AtlasProject soruceProject, AtlasProject destProject, ref List<UIAdjust_ModifyRefInfo> modifyRefTable)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        modifyRefTable = new List<UIAdjust_ModifyRefInfo>();

        if (
               (null == spriteList)
            || (null == soruceProject)
            || (null == destProject)
            )
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        foreach(var spriteItem in spriteList)
        {//遍历移动sprite

            List<string> refAsset = null;

            errorType = CheckProjectErrorType(soruceProject.GetSpriteRefTable(spriteItem, ref refAsset));
            if (!IsAtlasOperateFailed(errorType))
            {
                foreach (var assetItem in refAsset)
                {//遍历sprite全部引用文件

                    UIAdjust_ModifyRefInfo record = new UIAdjust_ModifyRefInfo();
                    record.RefAssetFilePath = assetItem;
                    record.SpriteName = Path.GetFileNameWithoutExtension(spriteItem);
                    record.SourceAtlasGUID = AssetDatabase.AssetPathToGUID(soruceProject.AtlasSavePath + soruceProject.Name + ".prefab");
                    record.DestAtlasGUID = AssetDatabase.AssetPathToGUID(destProject.AtlasSavePath + destProject.Name + ".prefab");
                
                    modifyRefTable.Add(record);
                }
            }
        }

        return errorType;
    }
   
    private Texture2D LoadSpriteImage(string spritePath, AtlasProject project)
    {//载入小图资源

        if (
               string.IsNullOrEmpty(spritePath) 
            || (null == project)
            )
        {
            return null;
        }

        string[] spriteTable = new string[1];
        spriteTable[0] = spritePath;
        float zoomScale = 1.0f;

        GetSpriteImageZoom(spritePath, project, out zoomScale);

        Texture2D retTex = UIAtlasTempTextureManager.GetInstance().LoadTexture(spritePath, project.Path, zoomScale);

        return retTex;
    }

    private ATLASOPERATE_ERROR_TYPE LoadAllSpriteImage(AtlasProject project)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
        if(null == project)
        {
            return ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
        }

        Texture2D spriteTexture = null;
        //依次载入工程中全部小图
        List<AtlasSpriteImage> sprites = project.GetAllSprites();
        foreach (var sprite in sprites)
        {
            spriteTexture = LoadSpriteImage(sprite.Path, project);
            if (onSpriteImageLoad != null)
            {
                onSpriteImageLoad(sprite.Path, project);
            }

            if(null == spriteTexture)
            {
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOADSPRITE_TEXTUREERROR;
                break;
            }
        }

        return errorType;
    }

    private bool UnloadSpriteImage(string spritePath, string atlasPath)
    {
        return UIAtlasTempTextureManager.GetInstance().UnloadTexture(spritePath, atlasPath);
    }

    public AtlasProject ClearProject(AtlasProject project)
    {
        if (project != null)
        {
            //清除临时目录中的资源
            UIAtlasTempTextureManager.GetInstance().Clear(project.Path);

            //清除工程中小图信息
            project.ClearSpriteImage();
            project = null;

            if (onClearCurrentProject != null)
            {
                onClearCurrentProject();
            }
        }

        return project;
    }
   
    private bool IsCorrelationFileExist(AtlasProject project)
    {
        bool isExist = false;

        if (null == project)
        {
            return false;
        }

        string pngFile = project.AtlasSavePath + project.Name + ".png";
        string prefabFile = project.AtlasSavePath + project.Name + ".prefab";
        string matFile = project.AtlasSavePath + project.Name + ".mat";

        if(
               (File.Exists(pngFile))
            && (File.Exists(prefabFile))
            && (File.Exists(matFile))
            )
        {
            isExist = true;
        }

        return isExist;
    }

    private bool IsReferenceFileType(string filePath)
    {
        bool bRet = false;

        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        if (
               filePath.EndsWith(".unity")
            || filePath.EndsWith(".prefab")
            )
        {
            bRet = true;
        }
        else
        {
            bRet = false;
        }

        return bRet;
    }

    private bool IsReferenceFile(string atlasPath, string assetPath)
    {
        bool bRet = false;

        if (
               string.IsNullOrEmpty(atlasPath)
            || string.IsNullOrEmpty(assetPath)
            )
        {
            return false;
        }

        string[] referenceFile = AssetDatabase.GetDependencies(new string[] { assetPath });
        foreach(var item in referenceFile)
        {
            if ((atlasPath == item) && (assetPath != atlasPath))
            {
                bRet = true;
                break;
            }     
        }


        return bRet;
    }

    private List<string> GetReferencingSprite(string atlasPath, string assetPath)
    {
        List<string> spriteList = new List<string>();

        if (
               string.IsNullOrEmpty(atlasPath)
            || string.IsNullOrEmpty(assetPath)
            )
        {
            return null;
        }

        string atlasGUID = AssetDatabase.AssetPathToGUID(atlasPath);
        if(string.IsNullOrEmpty(atlasGUID))
        {
            return null;
        }

        string[] lines = File.ReadAllLines(assetPath);
        for (int lineIndex = 0; lineIndex < lines.Length - 1; lineIndex++ )
        {
            int lineAtlas = lineIndex;
            int lineSprite = lineIndex + 1;
            string tempGUID = string.Empty;
            string spriteName = string.Empty;

            if (
                    !lines[lineAtlas].StartsWith("  mAtlas:")
                || !lines[lineSprite].StartsWith("  mSpriteName:")
                )
            {
                continue;
            }

            lineIndex++;

            string[] atlasStr = lines[lineAtlas].Split(' ');
            for (int index = 0; index < atlasStr.Length; index++ )
            {
                if (atlasStr[index] == "guid:")
                {
                    tempGUID = atlasStr[index + 1].TrimEnd(',');
                    break;
                }
            }

            if( string.IsNullOrEmpty(tempGUID) || (tempGUID != atlasGUID))
            {
                continue;
            }

            string[] spriteStr = lines[lineSprite].Split(' ');
            spriteName = spriteStr[3].TrimStart().TrimEnd();

            if(string.IsNullOrEmpty(spriteName))
            {
                continue;
            }

            if (!spriteList.Contains(spriteName))
            {
                spriteList.Add(spriteName);
            }
        }

        return spriteList;
    }

    private List<Texture> GetSpriteTexture(AtlasProject project)
    {
        List<Texture> spriteTextureTable = new List<Texture>();

        if(null == project)
        {
            return null;
        }

        spriteTextureTable = UIAtlasTempTextureManager.GetInstance().GetAllSpriteZoomTexture(project.Path);

        return spriteTextureTable;
    }

    private List<Texture2D> GetSpriteTexture2D(AtlasProject project)
    {
        List<Texture2D> spriteTextureTable = new List<Texture2D>();

        if (null == project)
        {
            return null;
        }

        spriteTextureTable = UIAtlasTempTextureManager.GetInstance().GetAllSpriteZoomTexture2D(project.Path);

        return spriteTextureTable;
    }

    private bool PackageTexture(List<Texture2D> textures, out Texture2D outTexture)
    {
        bool bRet = false;
        outTexture = new Texture2D(1, 1);;

        if (null == textures)
        {
            return false;
        }

        ITexturePackagingStrategy maker = null;
        
        if (NGUISettings.unityPacking)
        {
            maker = new DefaultTexturePackagingStrategy();
        }
        else
        {
            maker = new NGUITexturePackagingStrategy();
        }
        //打包纹理
        maker.Pack(outTexture, textures.ToArray(), NGUISettings.atlasPadding, null);

        return bRet;
    }
    private bool IsAtlasOperateFailed(ATLASOPERATE_ERROR_TYPE errorType)
    {
        bool bRet = false;

        if ((int)errorType >= 0)
        {
            bRet = true;
        }
        else
        {
            bRet = false;
        }

        return bRet;
    }

    private string FixCSVFilePath(string dirPath)
    {
        string csvPath = string.Empty;

        if (string.IsNullOrEmpty(dirPath))
        {
            return csvPath;
        }

        csvPath = dirPath + "Atlas一致性检查结果" + DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss");

        return csvPath;
    }


    static int Compare(Texture a, Texture b)
    {
        if(
            (null == a)
            && (null == b)
            )
        {
            return 0;
        }
        // A is null b is not b is greater so put it at the front of the list
        if (a == null && b != null) return 1;

        // A is not null b is null a is greater so put it at the front of the list
        if (a != null && b == null) return -1;

        // Get the total pixels used for each sprite
        int aPixels = a.width * a.height;
        int bPixels = b.width * b.height;

        if (aPixels > bPixels) return -1;
        else if (aPixels < bPixels) return 1;
        return 0;

        //return String.Compare(a.name, b.name);
    }

    private ATLASOPERATE_ERROR_TYPE CheckProjectErrorType(AtlasProject project)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;


        if(project != null)
        {
            switch (project.ProjectFailedType)
            {
                case PROJECT_ERROR_TYPE.PROJECT_ERROR_UNKNOWN:
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                    break;
                case PROJECT_ERROR_TYPE.PROJECT_ERROR_LOAD_ERRORPATH:
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
                    break;
                case PROJECT_ERROR_TYPE.PROJECT_ERROR_PROJECTFILE_ERROR:
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE;
                    break;
                case PROJECT_ERROR_TYPE.PROJECT_ERROR_REFERENCEFILE_ERROR:
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE;
                    break;
                default:
                    errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
                    break;
            }
        }

        return errorType;
    }

    private ATLASOPERATE_ERROR_TYPE CheckProjectErrorType(PROJECT_ERROR_TYPE projcetErrorType)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        switch (projcetErrorType)
        {
            case PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
                break;
            case PROJECT_ERROR_TYPE.PROJECT_ERROR_UNKNOWN:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;
            case PROJECT_ERROR_TYPE.PROJECT_ERROR_LOAD_ERRORPATH:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPATH;
                break;
            case PROJECT_ERROR_TYPE.PROJECT_ERROR_PROJECTFILE_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_LOAD_ERRORPROJECTFILE;
                break;

            default:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;
        }

        return errorType;
    }

    private ATLASOPERATE_ERROR_TYPE CheckAnalyserError(CONSISTENCY_ANALYSE_ERROR_TYPE analyserErrorType)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        switch (analyserErrorType)
        {
            case CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_ERROR;
                break;

            case CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_SOURCEAB_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_SOURCEAB_ERROR;
                break;

            case CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_TARGETPROJECT_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_TARGETPROJECT_ERROR;
                break;

            case CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_PREFABPATH_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSECONSISTENCY_INPUTINFO_PREFABPATH_ERROR;
                break;

            case CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
                break;

            case CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_UNKNOWN:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;

            default:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;
        }

        return errorType;
    }

    private ATLASOPERATE_ERROR_TYPE CheckReferenceAnalyserError(REFERENCE_ANALYSE_ERROR_TYPE analyserErrorType)
    {
        ATLASOPERATE_ERROR_TYPE errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;

        switch (analyserErrorType)
        {
            case REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_UNKNOWN:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;

            case REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_INPUTINFO_PREFABPATH_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_PREFABPATH_ERROR;
                break;

            case REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_INPUTINFO_SCENEPATH_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_SCENEPATH_ERROR;
                break;

            case REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
                break;

            case REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_CONFIGFILE_DIRPATH_ERROR:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_ANALYSEREFERENCE_CONFIGFILE_DIRPATHERROR;
                break;

            default:
                errorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_UNKNOWN;
                break;
        }

        return errorType;
    }
    #region 成员变量

    public delegate void SpriteImageLoadNotify(string spritePath, AtlasProject project);
    public delegate void ClearCurrentProjectNotify();
    public delegate void RebuildAtlasNotify(string projectPath);

    public SpriteImageLoadNotify onSpriteImageLoad;
    public ClearCurrentProjectNotify onClearCurrentProject;
    public RebuildAtlasNotify onRebuildAtlas;
    public ATLASOPERATE_ERROR_TYPE ErrorType
    {
        get { return m_ErrorType; }
    }

    private ATLASOPERATE_ERROR_TYPE m_ErrorType = ATLASOPERATE_ERROR_TYPE.ATLASOPERATE_ERROR_NONE;
  
    private string m_AtlasTagStr = "  mAtlas:";
    private string m_SpriteTagStr = "  mSpriteName:";
    private string m_GuidTagStr = "guid:";

    static private UIAtlasOperateUtility m_Instance = null;
    public static UIAtlasOperateUtility GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new UIAtlasOperateUtility();
        }
        return m_Instance;
    }
    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
        }
    }
    #endregion
}
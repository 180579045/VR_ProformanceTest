using System.Collections.Generic;
using System.IO;
using System;

public enum CONSISTENCY_ANALYSE_ERROR_TYPE
{
    CONSISTENCY_ANALYSE_ERROR_UNKNOWN = 0,
    CONSISTENCY_ANALYSE_ERROR_INPUTINFO_ERROR,
    CONSISTENCY_ANALYSE_ERROR_INPUTINFO_SOURCEAB_ERROR,
    CONSISTENCY_ANALYSE_ERROR_INPUTINFO_PREFABPATH_ERROR,

    CONSISTENCY_ANALYSE_ERROR_TARGETPROJECT_ERROR,

    CONSISTENCY_ANALYSE_ERROR_NONE = -1,
}

public enum PREFAB_EXIST_INFO
{
    PREFAB_FILE_NOT_EXIST = 0,
    PREFAB_FILE_EXIST_NOTIN_SEARCHDIR,
    PREFAB_FILE_EXIST_IN_SEARCHDIR,
}

public enum ATLASCONSISTENCY_TYPE
{
    ATLAS_CONSISTENT = 0x00,
    ATLAS_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING = 0x01,

    ATLAS_UNCONSISTENT_FOR_PREFAB_NOT_EXIST = 0x10,
    ATLAS_UNCONSISTENT_FOR_PROJECT_NOT_EXIST = 0x11,
    ATLAS_UNCONSISTENT_FOR_SPRITE_NOT_SAME = 0x12,
    ATLAS_UNCONSISTENT_FOR_SPRITE_NOT_SAME_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING = 0x13,
}

public enum ANALYSERESULT_TYPE
{
    ANALYSERESULT_CONSISTENT = 0,
    ANALYSERESULT_CONSISTENT_WITH_WARNING,
    ANALYSERESULT_UNCONSISTENT,
}

public class AtlasConsistencyAnalyseResult
{
    private ANALYSERESULT_TYPE m_analyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;
    private List<AtlasConsistencyInfo> m_consistencyInfo = new List<AtlasConsistencyInfo>();

    public ANALYSERESULT_TYPE AnalyseResult { get { return m_analyseResult; } set { m_analyseResult = value; } }
    public List<AtlasConsistencyInfo> ConsistencyInfo { get { return m_consistencyInfo; } set { m_consistencyInfo = value; } }
}

public class AtlasConsistencyInputInfo
{
    private string m_atlasProjectDir = string.Empty;
    private string m_atlasPrefabDir = string.Empty;
    private string m_sourceSpriteDir = string.Empty;
    private List<AtlasProject> m_projectTable = new List<AtlasProject>();

    public string AtlasProjectDir { get { return m_atlasProjectDir; } set { m_atlasProjectDir = value; } }
    public string AtlasPrefabDir { get { return m_atlasPrefabDir; } set { m_atlasPrefabDir = value; } }
    public string SourceSpriteDir { get { return m_sourceSpriteDir; } set { m_sourceSpriteDir = value; } }
    public List<AtlasProject> ProjectTable { get { return m_projectTable; } set { m_projectTable = value; } }
}

public class SpriteConsistencyInfo
{
    private string m_spriteName = string.Empty;
    private string m_projectPath = string.Empty;
    private bool m_isConsistent = true;
    private bool m_existInProject = false;
    private bool m_existInPrefab = false;
    private bool m_existInSourceAB = false;

    public string SpriteName { get { return m_spriteName; } set { m_spriteName = value; } }
    public string ProjectPath { get { return m_projectPath; } set { m_projectPath = value; } }
    public bool IsConsistent { get { return m_isConsistent; } set { m_isConsistent = value; } }
    public bool ExistInProject { get { return m_existInProject; } set { m_existInProject = value; } }
    public bool ExistInPrefab { get { return m_existInPrefab; } set { m_existInPrefab = value; } }
    public bool ExistInSourceAB { get { return m_existInSourceAB; } set { m_existInSourceAB = value; } }

}

public class AtlasConsistencyInfo
{
    private string m_atlasFilePath = string.Empty;
    private ATLASCONSISTENCY_TYPE m_consistencyType = ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT;
    private List<SpriteConsistencyInfo> m_spriteConsistencyInfoTbl = new List<SpriteConsistencyInfo>();

    public string AtlasFilePath { get { return m_atlasFilePath; } set { m_atlasFilePath = value; } }
    public ATLASCONSISTENCY_TYPE ConsistencyType { get { return m_consistencyType; } set { m_consistencyType = value; } }
    public List<SpriteConsistencyInfo> SpriteConsistencyInfoTbl { get { return m_spriteConsistencyInfoTbl; } set { m_spriteConsistencyInfoTbl = value; } }
}

public class AtlasConsistencyAnalyser
{
    public AtlasConsistencyAnalyser(AtlasConsistencyInputInfo inputInfo, out CONSISTENCY_ANALYSE_ERROR_TYPE errorType)
    {
        errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;

        do
        {
            errorType = CheckInputInfo(inputInfo);
            if (IsAnalserFailed(errorType))
            {
                break;
            }

            m_inputInfo = inputInfo;

            m_projectTable = inputInfo.ProjectTable;
        } while (false);

    }

    public CONSISTENCY_ANALYSE_ERROR_TYPE AnalyseAtlasConsistent(bool isNeedCheckPrefab, out AtlasConsistencyAnalyseResult analyseResult)
    {
        CONSISTENCY_ANALYSE_ERROR_TYPE errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;
        analyseResult = new AtlasConsistencyAnalyseResult();

        InitProgresser(isNeedCheckPrefab);
        int count = 1;

        foreach(var item in m_projectTable)
        {
            AtlasConsistencyAnalyseResult singleAnalyseResult = null;
            AnalyseConsistencyProgresser.GetInstance().UpdateProgress(count++);

            errorType = CheckProjectConsistencyInfo(item, out singleAnalyseResult);
            if (IsAnalserFailed(errorType))
            {
                continue;
            }

            if (singleAnalyseResult.AnalyseResult != ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT)
            {
                analyseResult.ConsistencyInfo.AddRange(singleAnalyseResult.ConsistencyInfo);
            }
        }

        if(isNeedCheckPrefab)
        {
            bool isPrefabConsistent = false;
            List<AtlasConsistencyInfo> prefabWithoutProjectInfo = null;

            errorType = CheckPrefabConsistencyInfo(out isPrefabConsistent, out prefabWithoutProjectInfo);
            if(!IsAnalserFailed(errorType))
            {
                if (!isPrefabConsistent)
                {
                    analyseResult.ConsistencyInfo.AddRange(prefabWithoutProjectInfo);      
                }
            }
        }

        foreach (var item in analyseResult.ConsistencyInfo)
        {
            if(ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT == item.ConsistencyType)
            {
                continue;
            }
            else if (ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING == item.ConsistencyType)
            {
                analyseResult.AnalyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING;
            }
            else
            {
                analyseResult.AnalyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT;
                break;
            }
        }

        AnalyseConsistencyProgresser.GetInstance().UpdateProgress(count++);

        return errorType;
    }

    public CONSISTENCY_ANALYSE_ERROR_TYPE AnalyseSingleAtlasConsistent(AtlasProject project, out AtlasConsistencyAnalyseResult analyseResult)
    {
        CONSISTENCY_ANALYSE_ERROR_TYPE errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;
        analyseResult = null;

        if(null == project)
        {
            return CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_TARGETPROJECT_ERROR;
        }

        errorType = CheckProjectConsistencyInfo(project, out analyseResult);

        return errorType;
    }

    private CONSISTENCY_ANALYSE_ERROR_TYPE CheckProjectConsistencyInfo(AtlasProject project, out AtlasConsistencyAnalyseResult analyseResult)
    {
        CONSISTENCY_ANALYSE_ERROR_TYPE errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;
        analyseResult = new AtlasConsistencyAnalyseResult();

        if(null == project)
        {
            return CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_TARGETPROJECT_ERROR;
        }

        PREFAB_EXIST_INFO prefabInfo = PREFAB_EXIST_INFO.PREFAB_FILE_EXIST_IN_SEARCHDIR;
        prefabInfo = CheckProjectWithoutPrefab(project);
      
        if (PREFAB_EXIST_INFO.PREFAB_FILE_NOT_EXIST == prefabInfo)
        {//Prefab不存在

            AtlasConsistencyInfo consistencyInfo = new AtlasConsistencyInfo();

            consistencyInfo.ConsistencyType = ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_PREFAB_NOT_EXIST;
            consistencyInfo.AtlasFilePath = project.Path;

            analyseResult.AnalyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT;
            analyseResult.ConsistencyInfo.Add(consistencyInfo);
        }
        else
        {//prefab存在

            bool isSpriteConsistent = true;
            List<SpriteConsistencyInfo> spriteConsistencyInfoTbl = null;

            CheckUnConsistentSpriteInProject(project, out isSpriteConsistent, out spriteConsistencyInfoTbl);
            if (isSpriteConsistent)
            {//sprite一致

                if (PREFAB_EXIST_INFO.PREFAB_FILE_EXIST_NOTIN_SEARCHDIR == prefabInfo)
                {//Prefab不在查找目录内

                    AtlasConsistencyInfo consistencyInfo = new AtlasConsistencyInfo();

                    consistencyInfo.ConsistencyType = ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING;
                    consistencyInfo.AtlasFilePath = project.Path;

                    analyseResult.AnalyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT_WITH_WARNING;
                    analyseResult.ConsistencyInfo.Add(consistencyInfo);

                }
                else
                {
                    analyseResult.AnalyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_CONSISTENT;
                }
            }
            else
            {//Sprite不一致

                AtlasConsistencyInfo consistencyInfo = new AtlasConsistencyInfo();

                if (PREFAB_EXIST_INFO.PREFAB_FILE_EXIST_NOTIN_SEARCHDIR == prefabInfo)
                {//Prefab不在查找目录内

                    consistencyInfo.ConsistencyType = ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_SPRITE_NOT_SAME_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING;
                }
                else
                {//Prefab在查找目录内

                    consistencyInfo.ConsistencyType = ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_SPRITE_NOT_SAME;
                }
                consistencyInfo.AtlasFilePath = project.Path;
                consistencyInfo.SpriteConsistencyInfoTbl = spriteConsistencyInfoTbl;

                analyseResult.AnalyseResult = ANALYSERESULT_TYPE.ANALYSERESULT_UNCONSISTENT;
                analyseResult.ConsistencyInfo.Add(consistencyInfo);

            }
        }

        return errorType;
    }

    private CONSISTENCY_ANALYSE_ERROR_TYPE CheckPrefabConsistencyInfo(out bool isConsistent, out List<AtlasConsistencyInfo> consistencyInfoTbl)
    {
        CONSISTENCY_ANALYSE_ERROR_TYPE errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;
      
        isConsistent = false;
        consistencyInfoTbl = new List<AtlasConsistencyInfo>();

        if (null == m_inputInfo)
        {
            return CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_ERROR;
        }

        if (string.IsNullOrEmpty(m_inputInfo.AtlasPrefabDir))
        {
            return CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_PREFABPATH_ERROR;
        }

        List<string> prefabPathTbl = GetAllAtlasPrefabFilePath(m_inputInfo.AtlasPrefabDir);

        foreach(var item in m_projectTable)
        {
            string fullPath = Path.GetFullPath(item.DescribePath);
            fullPath = fullPath.Replace(@"\", @"/");

            prefabPathTbl.Remove(fullPath);
        }

        if (prefabPathTbl.Count != 0)
        {
            foreach (var item in prefabPathTbl)
            {
                AtlasConsistencyInfo newConsistencyInfo = new AtlasConsistencyInfo();

                newConsistencyInfo.ConsistencyType = ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_PROJECT_NOT_EXIST;
                newConsistencyInfo.AtlasFilePath = item;

                consistencyInfoTbl.Add(newConsistencyInfo);
            }

            if (0 == consistencyInfoTbl.Count)
            {
                isConsistent = true;
            }
        }

        return errorType;
    }

    private PREFAB_EXIST_INFO CheckProjectWithoutPrefab(AtlasProject project)
    {
        PREFAB_EXIST_INFO prefabInfo = PREFAB_EXIST_INFO.PREFAB_FILE_EXIST_IN_SEARCHDIR;

        if (null == project)
        {
            return prefabInfo;
        }

        if (!File.Exists(project.DescribePath))
        {
            prefabInfo = PREFAB_EXIST_INFO.PREFAB_FILE_NOT_EXIST;
        }
        else
        {
            if (!project.DescribePath.Contains(m_inputInfo.AtlasPrefabDir))
            {
                prefabInfo = PREFAB_EXIST_INFO.PREFAB_FILE_EXIST_NOTIN_SEARCHDIR;
            }
            else
            {
                prefabInfo = PREFAB_EXIST_INFO.PREFAB_FILE_EXIST_IN_SEARCHDIR;
            }
        }

        return prefabInfo;
    }

    private void InitProgresser(bool isNeedCheckPrefab)
    {        
        int projectNum = m_projectTable.Count;

        if (isNeedCheckPrefab)
        {
            AnalyseConsistencyProgresser.GetInstance().InitProgresser(projectNum + 1, "一致性分析中");
        }
        else
        {
            AnalyseConsistencyProgresser.GetInstance().InitProgresser(projectNum , "一致性分析中");
        }
    }

    private void CheckUnConsistentSpriteInProject(AtlasProject project, out bool isAtlasConsistent, out List<SpriteConsistencyInfo> consistencyInfoTbl)
    {
        consistencyInfoTbl = new List<SpriteConsistencyInfo>();
        isAtlasConsistent = true;

        if(null == project)
        {
            return;
        }

        List<string> spriteNameTblInProject = GetAllSpritePathInProject(project);
        List<string> spriteNameTblInPrefab = GetAllSpriteNameInPrefab(project.DescribePath);

        //查找project
        foreach (var item in spriteNameTblInProject)
        {
            SpriteConsistencyInfo newConsistencyInfo = new SpriteConsistencyInfo();
            string spriteNameWithoutExt = Path.GetFileNameWithoutExtension(item);

            if (spriteNameTblInPrefab.Contains(spriteNameWithoutExt))
            {
                if (!File.Exists(@item))
                {
                    newConsistencyInfo.SpriteName = spriteNameWithoutExt;
                    newConsistencyInfo.ProjectPath = project.Path;
                    newConsistencyInfo.ExistInProject = true;
                    newConsistencyInfo.ExistInPrefab = true;
                    newConsistencyInfo.ExistInSourceAB = false;
                    newConsistencyInfo.IsConsistent = false;

                    consistencyInfoTbl.Add(newConsistencyInfo);
                }

                //从prefab中移除该Sprite
                spriteNameTblInPrefab.Remove(spriteNameWithoutExt);
            }
            else
            {
                if (!File.Exists(item))
                {
                    newConsistencyInfo.ExistInSourceAB = false;
                }
                else {
                    newConsistencyInfo.ExistInSourceAB = true;
                }

                newConsistencyInfo.SpriteName = spriteNameWithoutExt;
                newConsistencyInfo.ProjectPath = project.Path;
                newConsistencyInfo.ExistInProject = true;
                newConsistencyInfo.ExistInPrefab = false;
                newConsistencyInfo.IsConsistent = false;

                consistencyInfoTbl.Add(newConsistencyInfo);
            }
        }

        //查找prefab
        if(spriteNameTblInPrefab.Count != 0)
        {
            List<string> sourcePicFilePath = UniversalEditorUtility.GetAllFileNameWithoutExtension(m_inputInfo.SourceSpriteDir);
            
            foreach (var item in spriteNameTblInPrefab)
            {
                SpriteConsistencyInfo newConsistencyInfo = new SpriteConsistencyInfo();
                if (!sourcePicFilePath.Contains(item))
                {
                    newConsistencyInfo.ExistInSourceAB = false;
                }
                else
                {
                    newConsistencyInfo.ExistInSourceAB = true;
                }

                newConsistencyInfo.SpriteName = item;
                newConsistencyInfo.ProjectPath = project.Path;
                newConsistencyInfo.ExistInProject = false;
                newConsistencyInfo.ExistInPrefab = true;
                newConsistencyInfo.IsConsistent = false;

                consistencyInfoTbl.Add(newConsistencyInfo);
            }
        }


        if(0 == consistencyInfoTbl.Count)
        {
            isAtlasConsistent = true;
        }
        else
        {
            isAtlasConsistent = false;
        }
    }

    private List<string> GetAllSpritePathInProject(AtlasProject project)
    {
        List<string> spriteNameTbl = new List<string>();

        if (null == project)
        {
            return spriteNameTbl;
        }

        List<AtlasSpriteImage> spriteTable = project.GetAllSprites();

        foreach (var item in spriteTable)
        {
            spriteNameTbl.Add(item.Path);
        }

        return spriteNameTbl;
    }

    private List<string> GetAllSpriteNameInPrefab(string prefabPath)
    {
        List<string> spriteNameTbl = new List<string>();

        if (string.IsNullOrEmpty(prefabPath))
        {
            return spriteNameTbl;
        }

        YAMLAnalyser.AnalyseSpriteTblInPrefab(prefabPath, out spriteNameTbl);

        return spriteNameTbl;
    }

    private CONSISTENCY_ANALYSE_ERROR_TYPE CheckInputInfo(AtlasConsistencyInputInfo inputInfo)
    {
        CONSISTENCY_ANALYSE_ERROR_TYPE errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE;

        do
        {
            if(null == inputInfo)
            {
                errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_ERROR;
                break;
            }

            if(string.IsNullOrEmpty(inputInfo.SourceSpriteDir))
            {
                errorType = CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_INPUTINFO_SOURCEAB_ERROR;
                break;
            }

        }while(false);


        return errorType;
    }

    private List<string> GetAllAtlasPrefabFilePath(string dir)
    {
        List<string> atlasPrefabPath = new List<string>();

        if(string.IsNullOrEmpty(dir))
        {
            return atlasPrefabPath;
        }

        List<string> filePath = UniversalEditorUtility.GetAllFilePath(dir);

        foreach (var item in filePath)
        {
            if(YAMLAnalyser.IsAtlasPrefab(item))
            {
                atlasPrefabPath.Add(item);
            }
        }

        return atlasPrefabPath;
    }

    private bool IsAnalserFailed(CONSISTENCY_ANALYSE_ERROR_TYPE errorType)
    {
        bool bRet = false;

        if(errorType != CONSISTENCY_ANALYSE_ERROR_TYPE.CONSISTENCY_ANALYSE_ERROR_NONE)
        {
            bRet = true;
        }

        return bRet;
    }

    private List<AtlasProject> m_projectTable = null;

    private AtlasConsistencyInputInfo m_inputInfo = null;
}

using System.Collections.Generic;
using System.IO;

public enum REFERENCE_ANALYSE_ERROR_TYPE
{
    REFERENCE_ANALYSE_ERROR_UNKNOWN = 0,
    REFERENCE_ANALYSE_ERROR_INPUTINFO_PREFABPATH_ERROR,
    REFERENCE_ANALYSE_ERROR_INPUTINFO_SCENEPATH_ERROR,

    REFERENCE_ANALYSE_ERROR_CONFIGFILE_DIRPATH_ERROR,

    REFERENCE_ANALYSE_ERROR_NONE = -1,
}

public class AllDependencyInfo
{
    private DependencyInfo m_dependencyInfo = new DependencyInfo();
    private ReverseDependencyInfo m_reverseDependencyInfo = new ReverseDependencyInfo();
    private NoneDependencyInfo m_noneDependencyInfo = new NoneDependencyInfo();

    public DependencyInfo AtlasDependencyInfo { get { return m_dependencyInfo; } set { m_dependencyInfo = value; } }
    public ReverseDependencyInfo AtlasrRverseDependencyInfo { get { return m_reverseDependencyInfo; } set { m_reverseDependencyInfo = value; } }
    public NoneDependencyInfo AtlasrNoneDependencyInfo { get { return m_noneDependencyInfo; } set { m_noneDependencyInfo = value; } }
}

public class DependencyInfo
{
    private Dictionary<string, AtlasReferenceInfo> m_dependencyInfoTbl = new Dictionary<string, AtlasReferenceInfo>();
    public Dictionary<string, AtlasReferenceInfo> DependencyInfoTbl { get { return m_dependencyInfoTbl; } set { m_dependencyInfoTbl = value; } }
}

public class ReverseDependencyInfo
{
    private Dictionary<string, SpriteReverseRefInfo> m_reverseDependencyInfoTbl = new Dictionary<string, SpriteReverseRefInfo>();
    public Dictionary<string, SpriteReverseRefInfo> ReverseDependencyInfoTbl { get { return m_reverseDependencyInfoTbl; } set { m_reverseDependencyInfoTbl = value; } }
}

public class NoneDependencyInfo
{
    private Dictionary<string, SpriteNoneRefInfo> m_noneDependencyInfoTbl = new Dictionary<string, SpriteNoneRefInfo>();
  
    public Dictionary<string, SpriteNoneRefInfo> NoneDependencyInfoTbl { get { return m_noneDependencyInfoTbl; } set { m_noneDependencyInfoTbl = value; } }
}

public class SpriteReverseRefInfo
{
    private Dictionary<string, List<string>> m_spriteRefTbl = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> SpriteRefTbl { get { return m_spriteRefTbl; } set { m_spriteRefTbl = value; } }
}

public class SpriteNoneRefInfo
{
    private bool m_isAllUnUse = false;
    private List<string> m_noneUseSpriteTbl = new List<string>();

    public bool IsAllUnUse { get { return m_isAllUnUse; } set { m_isAllUnUse = value; } }
    public List<string> NoneUseSpriteTbl { get { return m_noneUseSpriteTbl; } set { m_noneUseSpriteTbl = value; } }
}

public class DependencInputInfo
{
    private string m_prefabPath = string.Empty;
    private string m_scenePath = string.Empty;
    private List<AtlasProject> m_projectTbl = new List<AtlasProject>();

    public string PrefebPath { get { return m_prefabPath; } set { m_prefabPath = value; } }
    public string ScenePath { get { return m_scenePath; } set { m_scenePath = value; } }
    public List<AtlasProject> ProjectTbl { get { return m_projectTbl; } set { m_projectTbl = value; } }
}

public class AtlasReferenceAnalyser
{
    private DependencInputInfo m_inputInfo = new DependencInputInfo();

    public AtlasReferenceAnalyser()
    {

    }

    public AtlasReferenceAnalyser(DependencInputInfo inputInfo)
    {
        m_inputInfo = inputInfo;
    }

    public REFERENCE_ANALYSE_ERROR_TYPE AnalyseAllDependency(out AllDependencyInfo allDependencyInfo)
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;
     
        allDependencyInfo = new AllDependencyInfo();
        DependencyInfo dependencyInfo = null;
        ReverseDependencyInfo reverseDependencyInfo = null;
        NoneDependencyInfo noneDependencyInfo = null;

        do
        {
            errorType = AnalyseDependency(out dependencyInfo);
            if(IsAnalserFailed(errorType))
            {
                break;
            }

            errorType = AnalyseReverseDependency(out reverseDependencyInfo);
            if(IsAnalserFailed(errorType))
            {
                break;
            }
            
            errorType = AnalyseNoneDependency(out noneDependencyInfo);
            if(IsAnalserFailed(errorType))
            {
                break;
            }

            allDependencyInfo.AtlasDependencyInfo = dependencyInfo;
            allDependencyInfo.AtlasrRverseDependencyInfo = reverseDependencyInfo;
            allDependencyInfo.AtlasrNoneDependencyInfo = noneDependencyInfo;

        }while(false);

        return errorType;
    }

    public REFERENCE_ANALYSE_ERROR_TYPE AnalyseDependency(out DependencyInfo dependencyInfo)
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;
        dependencyInfo = new DependencyInfo();

        do
        {

            errorType = CheckDependencyInput();
            if(IsAnalserFailed(errorType))
            {
                break;
            }

            string configDir = UIAtlasEditorConfig.ReferenceResultPath;

            AtlasReferenceFilter filter = new AtlasReferenceFilter(configDir);

            List<string> assetPathTbl = GetAllAssetFilePath();

            AnnalyseReferenceProgresser.GetInstance().InitProgresser(assetPathTbl.Count, "正向引用导出中");
            int count = 1;

            foreach(var item in assetPathTbl)
            {
                AtlasReferenceInfo newInfo = null;

                if (
                     //YAMLAnalyser.IsAtlasPrefab(item)
                    !filter.CheckDependencyFilter(item)
                    )
                {
                    YAMLAnalyser.AnalyseAtlasReferenceInfo(item, out newInfo);
                    if (newInfo != null)
                    {
                        dependencyInfo.DependencyInfoTbl.Add(item, newInfo);
                    }
                }

                AnnalyseReferenceProgresser.GetInstance().UpdateProgress(count++);
            }

        } while (false);

        return errorType;
    }

    public REFERENCE_ANALYSE_ERROR_TYPE AnalyseReverseDependency(out ReverseDependencyInfo reverseDependencyInfo)
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;
        reverseDependencyInfo = new ReverseDependencyInfo();

        do
        {
            errorType = CheckReverseDependencyInput();
            if(IsAnalserFailed(errorType))
            {
                break;
            }

            string configDir = UIAtlasEditorConfig.ReferenceResultPath;

            AtlasReferenceFilter filter = new AtlasReferenceFilter(configDir);

            AnnalyseReferenceProgresser.GetInstance().InitProgresser(m_inputInfo.ProjectTbl.Count, "反向引用导出中");
            int count = 1;

            foreach(var item in m_inputInfo.ProjectTbl)
            {
                if (filter.CheckReverseDependencyAtlasFilter(item.Path))
                {
                    continue;
                }

                SpriteReverseRefInfo newSpriteInfo = new SpriteReverseRefInfo();
                List<AtlasSpriteImage> spriteTbl = item.GetAllSprites();

                foreach(var spriteItem in spriteTbl)
                {
                    string spriteName = Path.GetFileNameWithoutExtension(spriteItem.Path);

                    if (filter.CheckReverseDependencySpriteFilter(item.Path, spriteName))
                    {
                        continue;
                    }

                    if(spriteItem.ReferenceTable.Count > 0)
                    {
                        newSpriteInfo.SpriteRefTbl.Add(spriteName, spriteItem.ReferenceTable);
                    }
                }

                if (newSpriteInfo.SpriteRefTbl.Count > 0)
                {
                    reverseDependencyInfo.ReverseDependencyInfoTbl.Add(item.Path, newSpriteInfo);
                }

                AnnalyseReferenceProgresser.GetInstance().UpdateProgress(count++);
            }

        }while(false);


        return errorType;
    }

    public REFERENCE_ANALYSE_ERROR_TYPE AnalyseNoneDependency(out NoneDependencyInfo noneDependencyInfo)
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;
        noneDependencyInfo = new NoneDependencyInfo();

        do{
            errorType = CheckNoneDependencyInput();
            if(IsAnalserFailed(errorType))
            {
                break;
            }

            string configDir = UIAtlasEditorConfig.ReferenceResultPath;

            AtlasReferenceFilter filter = new AtlasReferenceFilter(configDir);

            AnnalyseReferenceProgresser.GetInstance().InitProgresser(m_inputInfo.ProjectTbl.Count, "无引用导出中");
            int count = 1;

            foreach(var item in m_inputInfo.ProjectTbl)
            {
                if (filter.CheckNoneDependencyAtlasFilter(item.Path))
                {
                    continue;
                }

                SpriteNoneRefInfo spriteInfo = new SpriteNoneRefInfo();
                List<AtlasSpriteImage> spriteTbl = item.GetAllSprites();

                foreach(var spriteItem in spriteTbl)
                {
                    string spriteName = Path.GetFileNameWithoutExtension(spriteItem.Path);

                    if (filter.CheckNoneDependencySpriteFilter(item.Path, spriteName))
                    {
                        continue;
                    }

                    if(spriteItem.ReferenceTable.Count == 0)
                    {
                        spriteInfo.NoneUseSpriteTbl.Add(spriteName);
                    }
                }

                if(spriteInfo.NoneUseSpriteTbl.Count == spriteTbl.Count)
                {
                    spriteInfo.IsAllUnUse = true;
                }

                noneDependencyInfo.NoneDependencyInfoTbl.Add(item.Path, spriteInfo);

                AnnalyseReferenceProgresser.GetInstance().UpdateProgress(count++);
            }

        }while(false);

        return errorType;
    }

    public REFERENCE_ANALYSE_ERROR_TYPE OpenAnalyseConfigFile(string configDir)
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;

        if (string.IsNullOrEmpty(configDir))
        {
            return REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_CONFIGFILE_DIRPATH_ERROR;
        }

        AtlasReferenceFilter filter = new AtlasReferenceFilter(configDir);

        filter.OpenFilterConfigFile();

        return errorType;
    }

    private List<string> GetAllAssetFilePath()
    {
        List<string> filePathTbl = UniversalEditorUtility.GetAllFilePath(m_inputInfo.PrefebPath);
        filePathTbl.AddRange(UniversalEditorUtility.GetAllFilePath(m_inputInfo.ScenePath));

        List<string> prefabPathTbl = new List<string>();

        foreach (var item in filePathTbl)
        {
            if (
                   item.EndsWith(".prefab") 
                || item.EndsWith(".unity")
                )
            {
                if (!prefabPathTbl.Contains(item))
                {
                    prefabPathTbl.Add(item);
                }
            }
        }

        return prefabPathTbl;
    }

    private REFERENCE_ANALYSE_ERROR_TYPE CheckDependencyInput()
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;
       
        do{
            if(null == m_inputInfo)
            {
                errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_UNKNOWN;
                break;
            }

            if (string.IsNullOrEmpty(m_inputInfo.PrefebPath))
            {
                errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_INPUTINFO_PREFABPATH_ERROR;
                break;
            }

            if(string.IsNullOrEmpty(m_inputInfo.ScenePath))
            {
                errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_INPUTINFO_SCENEPATH_ERROR;
                break;
            }

        }while(false);


        return errorType;
    }

    private REFERENCE_ANALYSE_ERROR_TYPE CheckReverseDependencyInput()
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;

        if(null == m_inputInfo.ProjectTbl)
        {
            errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_UNKNOWN;
        }
        
        return errorType;
    }

    private REFERENCE_ANALYSE_ERROR_TYPE CheckNoneDependencyInput()
    {
        REFERENCE_ANALYSE_ERROR_TYPE errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE;

        if(null == m_inputInfo.ProjectTbl)
        {
            errorType = REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_UNKNOWN;
        }
        
        return errorType;
    }

    private bool IsAnalserFailed(REFERENCE_ANALYSE_ERROR_TYPE errorType)
    {
        bool bRet = false;

        if (errorType != REFERENCE_ANALYSE_ERROR_TYPE.REFERENCE_ANALYSE_ERROR_NONE)
        {
            bRet = true;
        }

        return bRet;
    }
}
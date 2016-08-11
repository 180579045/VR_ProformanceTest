
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AtlasFiltrateInfo
{
    //private string m_atlasProjectPath = string.Empty;
    private bool m_isWholeFiltrate = false;
    private List<string> m_spriteTbl = new List<string>();

    //public string AtlasProjectPath { get { return m_atlasProjectPath; } set { m_atlasProjectPath = value; } }
    public bool IsWholeFiltrate { get { return m_isWholeFiltrate; } set { m_isWholeFiltrate = value; } }
    public List<string> SpriteTbl { get { return m_spriteTbl; } set { m_spriteTbl = value; } }
}

public class AtlasReferenceFilter
{
    private string m_configPath = string.Empty;

    private string m_dependencyTag = "正向引用过滤:";
    private string m_reverseDependencyTag = "反向引用过滤:";
    private string m_noneDependencyTag = "无引用过滤:";

    private List<string> m_dependencyFilter = new List<string>();
    private Dictionary<string, AtlasFiltrateInfo> m_reverseDependencyFilter = new Dictionary<string, AtlasFiltrateInfo>();
    private Dictionary<string, AtlasFiltrateInfo> m_noneDependencyFilter = new Dictionary<string, AtlasFiltrateInfo>();

    public List<string> DependencyFilter { get { return m_dependencyFilter; } set { m_dependencyFilter = value; } }
    public Dictionary<string, AtlasFiltrateInfo> ReverseDependencyFilter { get { return m_reverseDependencyFilter; } set { m_reverseDependencyFilter = value; } }
    public Dictionary<string, AtlasFiltrateInfo> NoneDependencyFilter { get { return m_noneDependencyFilter; } set { m_noneDependencyFilter = value; } }

    public AtlasReferenceFilter(string dirPath)
    {
        m_configPath = dirPath + "引用关系过滤文件.txt";

        FixReferenceFilter();
    }

    public bool CheckDependencyFilter(string filePath)
    {
        bool needFiltrate = false;

        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        if(m_dependencyFilter.Contains(filePath))
        {
            needFiltrate = true;
        }
        else
        {
            needFiltrate = false;
        }

        return needFiltrate;
    }

    public bool CheckReverseDependencyAtlasFilter(string atlasPrefab)
    {
        bool needFiltrate = false;

        if (string.IsNullOrEmpty(atlasPrefab))
        {
            return false;
        }

        AtlasFiltrateInfo info = null;

        if (m_reverseDependencyFilter.TryGetValue(atlasPrefab, out info))
        {
            if(info.IsWholeFiltrate)
            {
                needFiltrate = true;
            }
            else
            {
                needFiltrate = false;
            }
        }
        else
        {
            needFiltrate = false;
        }

        return needFiltrate;
    }

    public bool CheckReverseDependencySpriteFilter(string atlasPrefab, string spriteName)
    {
        bool needFiltrate = false;

        if(
               string.IsNullOrEmpty(atlasPrefab)
            || string.IsNullOrEmpty(spriteName)
            )
        {
            return false;
        }

        AtlasFiltrateInfo info = null;

        if (m_reverseDependencyFilter.TryGetValue(atlasPrefab, out info))
        {
            if(info.IsWholeFiltrate)
            {
                needFiltrate = true;
            }
            else
            {
                if(info.SpriteTbl.Contains(spriteName))
                {
                    needFiltrate = true;
                }
                else
                {
                    needFiltrate = false;
                }
            }
        }
        else
        {
            needFiltrate = false;
        }

        return needFiltrate;
    }

    public bool CheckNoneDependencyAtlasFilter(string atlasPrefab)
    {
        bool needFiltrate = false;

        if (string.IsNullOrEmpty(atlasPrefab))
        {
            return false;
        }

        AtlasFiltrateInfo info = null;

        if (m_noneDependencyFilter.TryGetValue(atlasPrefab, out info))
        {
            if (info.IsWholeFiltrate)
            {
                needFiltrate = true;
            }
            else
            {
                needFiltrate = false;
            }
        }
        else
        {
            needFiltrate = false;
        }

        return needFiltrate;
    }

    public bool CheckNoneDependencySpriteFilter(string atlasPrefab, string spriteName)
    {
        bool needFiltrate = false;

        if (
               string.IsNullOrEmpty(atlasPrefab)
            || string.IsNullOrEmpty(spriteName)
            )
        {
            return false;
        }

        AtlasFiltrateInfo info = null;

        if (m_noneDependencyFilter.TryGetValue(atlasPrefab, out info))
        {
            if (info.IsWholeFiltrate)
            {
                needFiltrate = true;
            }
            else
            {
                if (info.SpriteTbl.Contains(spriteName))
                {
                    needFiltrate = true;
                }
                else
                {
                    needFiltrate = false;
                }
            }
        }
        else
        {
            needFiltrate = false;
        }

        return needFiltrate;
    }

    public void OpenFilterConfigFile()
    {
        List<string> refTag = new List<string>();

        if(!File.Exists(m_configPath))
        {
            StreamWriter fileWriter = File.CreateText(m_configPath);
            UniversalEditorUtility.MakeFileWriteable(m_configPath);

            refTag.Add("正向引用过滤:" + Environment.NewLine);
            refTag.Add("反向引用过滤:" + Environment.NewLine);
            refTag.Add("无引用过滤:" + Environment.NewLine);

            foreach(var item in refTag)
            {
                fileWriter.WriteLine(item);
            }

            fileWriter.Close();
        }

        //System.Diagnostics.Process.Start(m_configPath);
        //UniversalEditorUtility.MakeFileWriteable(m_configPath);

        System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
        myProcess.StartInfo.FileName = @m_configPath;
        myProcess.StartInfo.LoadUserProfile = true;
        myProcess.StartInfo.Verb = "open";
        myProcess.Start();
        myProcess.WaitForExit(1);
        myProcess.Close();
        //myProcess = System.Diagnostics.Process.Start(@m_configPath);
    }
    
    public void FixReferenceFilter()
    {
        if (File.Exists(m_configPath))
        {
            StreamReader reader = File.OpenText(m_configPath);
            if (reader != null)
            {
                string readStr = string.Empty;

                while (!IsFilterTag(readStr) && !reader.EndOfStream)
                {
                    readStr = reader.ReadLine();
                }

                UpdateFilter(reader, readStr);
            }
        }
    }

    private void UpdateFilter(StreamReader reader, string currentStr)
    {
        if (reader.EndOfStream)
        {
            return;
        }

        if(!IsFilterTag(currentStr))
        {
            return;
        }

        while (!reader.EndOfStream)
        {
            string readStr = reader.ReadLine();

            if (string.IsNullOrEmpty(readStr))
            {
                continue;
            }

            if (IsFilterTag(readStr))
            {
                UpdateFilter(reader, readStr);
                break;
            }
            else if (currentStr == m_dependencyTag)
            {
                UpdateDependencyFilter(readStr);
            }
            else if (currentStr == m_reverseDependencyTag)
            {
                UpdateAtlasFilter(readStr, ref m_reverseDependencyFilter);
            }
            else
            {
                UpdateAtlasFilter(readStr, ref m_noneDependencyFilter);
            }
        }
    }

    private void UpdateDependencyFilter(string readStr)
    {
        //string dependencyInfo = string.Empty;

        if(
               string.IsNullOrEmpty(readStr)
            || (!readStr.EndsWith(".prefab") && !readStr.EndsWith(".unity"))
            )
        {
            return;
        }

        m_dependencyFilter.Add(readStr);
    }

    private void UpdateAtlasFilter(string readStr, ref Dictionary<string, AtlasFiltrateInfo> atlasFilter)
    {
        if(string.IsNullOrEmpty(readStr))
        {
            return;
        }

        if(null == atlasFilter)
        {
            atlasFilter = new Dictionary<string, AtlasFiltrateInfo>();
        }

        KeyValuePair<string, AtlasFiltrateInfo> filtrateInfo = FixAtlasFiltrateInfo(readStr);

        if (!string.IsNullOrEmpty(filtrateInfo.Key))
        {
            AtlasFiltrateInfo existInfo = null;

            if (atlasFilter.TryGetValue(filtrateInfo.Key, out existInfo))
            {
                if(
                       (false == existInfo.IsWholeFiltrate)
                    && (true == filtrateInfo.Value.IsWholeFiltrate)
                    )
                {
                    atlasFilter.Remove(filtrateInfo.Key);
                    atlasFilter.Add(filtrateInfo.Key, filtrateInfo.Value);
                }
                else if(
                       (false == existInfo.IsWholeFiltrate)
                    && (false == filtrateInfo.Value.IsWholeFiltrate)
                    )
                {
                    foreach(var item in filtrateInfo.Value.SpriteTbl)
                    {
                        if(!existInfo.SpriteTbl.Contains(item))
                        {
                            existInfo.SpriteTbl.Add(item);
                        }
                    }
                }
            }
            else
            {
                atlasFilter.Add(filtrateInfo.Key, filtrateInfo.Value);          
            }
        }
    }

    private KeyValuePair<string, AtlasFiltrateInfo> FixAtlasFiltrateInfo(string readStr)
    {
        KeyValuePair<string, AtlasFiltrateInfo> filtrateInfo = new KeyValuePair<string,AtlasFiltrateInfo>(string.Empty, null); 

        if(string.IsNullOrEmpty(readStr))
        {
            return filtrateInfo;
        }

        if (readStr.EndsWith(".atlasproj"))
        {
            AtlasFiltrateInfo newInfo = new AtlasFiltrateInfo();

            newInfo.IsWholeFiltrate = true;

            filtrateInfo = new KeyValuePair<string, AtlasFiltrateInfo>(readStr, newInfo);
         }
        else
        {
            int spaceIndex = readStr.LastIndexOf(' ');
            if (spaceIndex > 0)
            {
                AtlasFiltrateInfo newInfo = new AtlasFiltrateInfo();

                string atlasPath = readStr.Substring(0, spaceIndex);
                newInfo.IsWholeFiltrate = false;
                newInfo.SpriteTbl.Add(readStr.Substring(spaceIndex + 1, readStr.Length - spaceIndex - 1));

                filtrateInfo = new KeyValuePair<string, AtlasFiltrateInfo>(atlasPath, newInfo);
            }
        }

        return filtrateInfo;
    }

    private bool IsFilterTag(string readStr)
    {
        bool isFilterTag = false;

        if(string.IsNullOrEmpty(readStr))
        {
            return false;
        }

        if(
               (readStr == m_dependencyTag)
            || (readStr == m_reverseDependencyTag)
            || (readStr == m_noneDependencyTag)
            )
        {
            isFilterTag = true;
        }

        return isFilterTag;
    }
}
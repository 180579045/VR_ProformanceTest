using YamlDotNet.Serialization;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AtlasReferenceInfo
{
    private Dictionary<string, List<string>> m_refAtlasTbl = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> RefAtlasTbl { get { return m_refAtlasTbl; } set { m_refAtlasTbl = value; } }
}

public class YAMLAnalyser
{
    static public void AnalyseSpriteTblInPrefab(string prefabPath, out List<string> spriteNameTbl)
    {
        spriteNameTbl = new List<string>();
        Dictionary<object, object> dataTbl = null;

        AnalysePrefabData(prefabPath, out dataTbl);
   
        AnalyseSpriteNameFromPrefab(dataTbl, out spriteNameTbl);
    }

    static public void AnalyseAtlasReferenceInfo(string filePath, out AtlasReferenceInfo referenceInfo)
    {
        referenceInfo = null;

        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }

        Dictionary<object, object> dataTbl = null;

        AnalysePrefabData(filePath, out dataTbl);

        AnalyseAtlasReferenceFromAssets(dataTbl, out referenceInfo);
    }

    static public bool IsAtlasPrefab(string filePath)
    {
        bool isAtlasPrefab = false;
        Dictionary<object, object> dataTbl = null;

        if (
               (string.IsNullOrEmpty(filePath))
            || !filePath.EndsWith(".prefab")
            )
        {
            return false;
        }

        AnalysePrefabData(filePath, out dataTbl);

        isAtlasPrefab = AnalyseWhetherAtlasPrefab(dataTbl);

        return isAtlasPrefab;
    }

    static private void AnalyseSpriteNameFromPrefab(Dictionary<object, object> prefabData, out List<string> spriteNameTbl)
    {
        spriteNameTbl = new List<string>();
        if(null == prefabData)
        {
            return;
        }

        foreach(var item in prefabData)
        {
            string keyStr = item.Key as string;
            if(string.IsNullOrEmpty(keyStr))
            {
                continue;
            }

            if (keyStr.StartsWith("MonoBehaviour"))
            {
                Dictionary<object, object> MonoData = item.Value as Dictionary<object, object>;
                if (null == MonoData)
                {
                    continue;
                }

                AnalyseSpriteInfoFromMono(MonoData, out spriteNameTbl);

                break;
            }
        }
    }

    static private void AnalyseSpriteInfoFromMono(Dictionary<object, object> monoData, out List<string> spriteNameTbl)
    {
        spriteNameTbl = new List<string>();
        if (null == monoData)
        {
            return;
        }

        foreach (var monoItem in monoData)
        {
            string keyStr = monoItem.Key as string;
            if (string.IsNullOrEmpty(keyStr))
            {
                continue;
            }

            if ("mSprites" == keyStr)
            {
                List<object> SpriteValue = monoItem.Value as List<object>;
                if (null == SpriteValue)
                {
                    continue;
                }

                AnalyseSpriteNameFromList(SpriteValue, out spriteNameTbl);

                break;
            }
        }

    }

    static private void AnalyseSpriteNameFromList(List<object> spriteList, out List<string> spriteNameTbl)
    {
        spriteNameTbl = new List<string>();

        if(null == spriteList)
        {
            return;
        }

        foreach (var spriteItem in spriteList)
        {
            Dictionary<object, object> SpriteValueTbl = spriteItem as Dictionary<object, object>;
            if (null == SpriteValueTbl)
            {
                continue;
            }

            foreach (var valueItem in SpriteValueTbl)
            {
                string keyStr = valueItem.Key as string;
                if (string.IsNullOrEmpty(keyStr))
                {
                    continue;
                }

                if ("name" == keyStr)
                {
                    if (!spriteNameTbl.Contains((string)valueItem.Value))
                    {
                        spriteNameTbl.Add((string)valueItem.Value);
                    }
                }
            }
        }
    }

    static private bool AnalyseWhetherAtlasPrefab(Dictionary<object, object> prefabData)
    {
        bool isAtlasPrefab = false;
        if (null == prefabData)
        {
            return false;
        }

        foreach (var item in prefabData)
        {
            string keyStr = item.Key as string;
            if (string.IsNullOrEmpty(keyStr))
            {
                continue;
            }

            if (keyStr.StartsWith("MonoBehaviour"))
            {
                Dictionary<object, object> MonoData = item.Value as Dictionary<object, object>;
                if (null == MonoData)
                {
                    continue;
                }

                foreach (var monoItem in MonoData)
                {
                    string momokeyStr = monoItem.Key as string;
                    if (string.IsNullOrEmpty(momokeyStr))
                    {
                        continue;
                    }

                    if ("mSprites" == momokeyStr)
                    {
                        isAtlasPrefab = true;
                        break;
                    }
                }
                break;
            }
        }

        return isAtlasPrefab;
    }

    static private void AnalyseAtlasReferenceFromAssets(Dictionary<object, object> fileData, out AtlasReferenceInfo referenceInfo)
    {
        referenceInfo = new AtlasReferenceInfo();

        if (null == fileData)
        {
            return;
        }

        foreach (var item in fileData)
        {
            string keyStr = item.Key as string;
            if (string.IsNullOrEmpty(keyStr))
            {
                continue;
            }

            if (keyStr.StartsWith("MonoBehaviour"))
            {
                Dictionary<object, object> MonoData = item.Value as Dictionary<object, object>;
                if (null == MonoData)
                {
                    continue;
                }

                KeyValuePair<string, List<string>> newInfo;

                AnalyseAtlasReferenceFromMono(MonoData, out newInfo);
                if (!string.IsNullOrEmpty(newInfo.Key))
                {
                    List<string> spriteNameTbl = null;

                    if (referenceInfo.RefAtlasTbl.TryGetValue(newInfo.Key, out spriteNameTbl))
                    {
                        foreach (var infoItem in newInfo.Value)
                        {
                            if (!spriteNameTbl.Contains(infoItem))
                            {
                                spriteNameTbl.Add(infoItem);
                            }
                        }
                    }
                    else
                    {
                        referenceInfo.RefAtlasTbl.Add(newInfo.Key, newInfo.Value);
                    }
                }
            }
        }

        if (0 == referenceInfo.RefAtlasTbl.Count)
        {
            referenceInfo = null;
        }
    }

    static private void AnalyseAtlasReferenceFromMono(Dictionary<object, object> monoData, out KeyValuePair<string, List<string>> referenceInfo)
    {
        referenceInfo = new KeyValuePair<string, List<string>>(string.Empty, null);

        if(null == monoData)
        {
            return;
        }

        string atlasPrefabPath = string.Empty;
        List<string> spriteNameTbl = new List<string>();


        do
        {
            object tempData = null;

            if (!monoData.TryGetValue("mAtlas", out tempData))
            {
                break;
            }

            Dictionary<object, object> atlasData = tempData as Dictionary<object, object>;
            if (atlasData == null)
            {
                break;
            }

            if (atlasData.TryGetValue("guid", out tempData))
            {
                string guidData = tempData as string;
                if (string.IsNullOrEmpty(guidData))
                {
                    break;
                }

                atlasPrefabPath = AssetDatabase.GUIDToAssetPath(guidData);
            }

            if (!monoData.TryGetValue("mSpriteName", out tempData))
            {
                break;
            }

            string spriteName = tempData as string;
            if (string.IsNullOrEmpty(spriteName))
            {
                break;
            }
            spriteNameTbl.Add(spriteName);

            referenceInfo = new KeyValuePair<string, List<string>>(atlasPrefabPath, spriteNameTbl);
        }while(false);
    }

    static private void AnalysePrefabData(string prefabPath, out Dictionary<object, object> dataTbl)
    {
        dataTbl = null;

        if (string.IsNullOrEmpty(prefabPath))
        {
            return;
        }

        object obj = null;
        StreamReader yamlReader = null;
        Deserializer yamlDeserializer = new Deserializer();

        string extension = Path.GetExtension(prefabPath);
        string tempFileName = prefabPath.Replace(extension, ".txt");
        string tempstr = string.Empty;
        string classID = string.Empty;

        StreamWriter fileWriter = File.CreateText(tempFileName);
        StreamReader fileReader = File.OpenText(prefabPath);

        while (!fileReader.EndOfStream)
        {
            tempstr = fileReader.ReadLine();

            if (tempstr.StartsWith("---"))
            {
                classID = tempstr.TrimStart(new char[] { '-', ' ' });
            }

            if (!tempstr.StartsWith("%") && !tempstr.StartsWith("---"))
            {
                if (!string.IsNullOrEmpty(classID))
                {
                    tempstr = tempstr.TrimEnd(':');
                    tempstr += classID + ":";
                }
                fileWriter.WriteLine(tempstr);
                classID = string.Empty;
            }
        }

        fileReader.Close();
        fileWriter.Close();

        yamlReader = new StreamReader(tempFileName);

        obj = yamlDeserializer.Deserialize(yamlReader);
        dataTbl = obj as Dictionary<object, object>;

        yamlReader.Close();

        File.Delete(tempFileName);
    }
}
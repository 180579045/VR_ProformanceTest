using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AtlasCosistencyExporter
{
    public void Export(List<AtlasConsistencyInfo> consistencyInfo, string resultPath, out string fixFilePath)
    {
        fixFilePath = string.Empty;

        if (
               (null == consistencyInfo)
            || (string.IsNullOrEmpty(resultPath))
            )
        {
            return;
        }

        List<string> csvDataTbl = null;

        AnalyseConsistencyInfo(consistencyInfo, out csvDataTbl);

        WriteCSVFile(resultPath, csvDataTbl, out fixFilePath);
    }

    private void AnalyseConsistencyInfo(List<AtlasConsistencyInfo> consistencyInfo, out List<string> csvDataTbl)
    {
        csvDataTbl = new List<string>();
        
        if(null == consistencyInfo)
        {
            return;
        }

        foreach(var item in consistencyInfo)
        {
            List<string> csvData = null;

            AnalyseConsistencyInfo(item, out csvData);

            csvData.Add(System.Environment.NewLine);

            csvDataTbl.AddRange(csvData);
        }
    }

    private void AnalyseConsistencyInfo(AtlasConsistencyInfo consistencyInfo, out List<string> csvData)
    {
        csvData = new List<string>();

        if(null == consistencyInfo)
        {
            return;
        }

        do
        {
            string dataLine = string.Empty;

            switch(consistencyInfo.ConsistencyType)
            {
                //Atlas一致
                case ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT:
                    break;

                //Atlas一致，但Prefab不在检索目录内
                case ATLASCONSISTENCY_TYPE.ATLAS_CONSISTENT_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING:
                    dataLine = "警告：" + consistencyInfo.AtlasFilePath + "一致，但对应的Prefab不在检索目录内！";
                    csvData.Add(dataLine);
                    break;

                //Atlas不一致(Prefab不存在)
                case ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_PREFAB_NOT_EXIST:
                    dataLine = consistencyInfo.AtlasFilePath + "没有对应的Prefab文件！";
                    csvData.Add(dataLine);
                    break;

                //Atlas不一致(Project不存在)
                case ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_PROJECT_NOT_EXIST:
                    dataLine = consistencyInfo.AtlasFilePath + "没有对应的Project文件！";
                    csvData.Add(dataLine);
                    break;

                case ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_SPRITE_NOT_SAME:
                    dataLine = consistencyInfo.AtlasFilePath;
                    csvData.Add(dataLine);
                    dataLine = m_spriteInfoTag;
                    csvData.Add(dataLine);
                    
                    foreach(var item in consistencyInfo.SpriteConsistencyInfoTbl)
                    {
                        dataLine = AnalyaseSpriteDetailData(item);
                        csvData.Add(dataLine);
                    }

                    break;

                case ATLASCONSISTENCY_TYPE.ATLAS_UNCONSISTENT_FOR_SPRITE_NOT_SAME_WITH_PREFAB_NOTIN_SEARCHPATH_WARNING:
                    dataLine = "警告：" + consistencyInfo.AtlasFilePath + "不一致，且对应的Prefab不在检索目录内！";
                    csvData.Add(dataLine);
                    dataLine = m_spriteInfoTag;
                    csvData.Add(dataLine);
                    
                    foreach(var item in consistencyInfo.SpriteConsistencyInfoTbl)
                    {
                        dataLine = AnalyaseSpriteDetailData(item);
                        csvData.Add(dataLine);
                    }

                    break;

                default:
                    break;
            }

        }while(false);

    }

    private string AnalyaseSpriteDetailData(SpriteConsistencyInfo spriteInfo)
    {
        string detailData = string.Empty;

        if (null == spriteInfo)
        {
            return detailData;
        }

        string projectPos = string.Empty;
        string prefabPos = string.Empty;
        string sourceAB = string.Empty;

        if (spriteInfo.ExistInProject)
        {
            projectPos = m_spriteExistIcon;
        }
        else
        {
            projectPos = m_spriteUnExistIcon;
        }

        if (spriteInfo.ExistInPrefab)
        {
            prefabPos = m_spriteExistIcon;
        }
        else
        {
            prefabPos = m_spriteUnExistIcon;

        }

        if (spriteInfo.ExistInSourceAB)
        {
            sourceAB = m_spriteExistIcon;
        }
        else{
            sourceAB = m_spriteUnExistIcon;
        }

        detailData = spriteInfo.SpriteName + "," + projectPos + "," + prefabPos + "," + sourceAB;

        return detailData;
    }

    private void WriteCSVFile(string filePath, List<string> csvDataTbl, out string fixFilePath)
    {
        fixFilePath = string.Empty;

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        fixFilePath = filePath + "Atlas一致性检查结果_" + DateTime.Now.ToString(m_dataFormat) + ".csv";

        CSVOperator.WriteFile(fixFilePath, csvDataTbl.ToArray());
    }

    private string m_spriteInfoTag = ",Atlas Project,Atlas Prefab,Sprite图库";
    private string m_spriteExistIcon = "○";
    private string m_spriteUnExistIcon = "×";
    private string m_dataFormat = "yyyy年MM月dd HH时mm分ss秒";

}
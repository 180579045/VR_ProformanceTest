using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AtlasReferenceExporter
{
    public void ExportAllDependency(string resultdir, AllDependencyInfo allDependencyInfo)
    {
        if (
               (null == allDependencyInfo)
            || string.IsNullOrEmpty(resultdir)
           )
        {
            return;
        }

        ExportDependency(resultdir, allDependencyInfo.AtlasDependencyInfo);

        ExportReverseDependency(resultdir, allDependencyInfo.AtlasrRverseDependencyInfo);

        ExportNoneDependency(resultdir, allDependencyInfo.AtlasrNoneDependencyInfo);
    }

    public void ExportDependency(string resultdir, DependencyInfo dependencyInfo)
    {
        if (
               (null == dependencyInfo)
            || string.IsNullOrEmpty(resultdir)
           )
        {
            return;
        }

        List<string> csvData = null;

        resultdir = resultdir + "正向引用_" + DateTime.Now.ToString(m_dataFormat) + @"/";

        foreach (var infoItem in dependencyInfo.DependencyInfoTbl)
        {
            AnalyseDependencyInfo(infoItem, out csvData);

            WriteDependencyCSVFile(resultdir, infoItem.Key, csvData);

            if (csvData != null)
            {
                csvData.Clear();
            }
        }

    }

    public void ExportReverseDependency(string resultdir, ReverseDependencyInfo reverseDependencyInfo)
    {
        if (
               (null == reverseDependencyInfo)
            || string.IsNullOrEmpty(resultdir)
           )
        {
            return;
        }

        List<string> csvData = null;

        AnalyseReverseDependcyInfo(reverseDependencyInfo, out csvData);

        WriteReverseDependencyCSVFile(resultdir, csvData);
    }

    public void ExportNoneDependency(string resultdir, NoneDependencyInfo noneDependencyInfo)
    {
        if (
               (null == noneDependencyInfo)
            || string.IsNullOrEmpty(resultdir)
           )
        {
            return;
        }

        List<string> csvData = null;

        AnalyseNoneDependcyInfo(noneDependencyInfo, out csvData);

        WriteNoneDependencyCSVFile(resultdir, csvData);
    }

    private void AnalyseDependencyInfo(KeyValuePair<string, AtlasReferenceInfo> dependencyInfo, out List<string> csvData)
    {
        csvData = new List<string>();

        csvData.Add(dependencyInfo.Key + Environment.NewLine);

        foreach (var atlasItem in dependencyInfo.Value.RefAtlasTbl)
        {
            string atlasPrefab = "," + atlasItem.Key;
            csvData.Add(atlasPrefab);

            foreach(var spriteItem in atlasItem.Value)
            {
                string spriteName = "," + "," + spriteItem;
                csvData.Add(spriteName);
            }

            csvData.Add("");
        }
    }

    private void AnalyseReverseDependcyInfo(ReverseDependencyInfo reverseDependencyInfo, out List<string> csvData)
    {
        csvData = new List<string>();

        if (null == reverseDependencyInfo)
        {
            return;
        }

        foreach (var infoItem in reverseDependencyInfo.ReverseDependencyInfoTbl)
        {
            csvData.Add(infoItem.Key);

            foreach(var spriteItem in infoItem.Value.SpriteRefTbl)
            {
                csvData.Add("," + spriteItem.Key);
                
                foreach(var assetItem in spriteItem.Value)
                {
                    csvData.Add("," + "," + assetItem);
                }

                csvData.Add("");
            }

            csvData.Add(Environment.NewLine);
        }
    }

    private void AnalyseNoneDependcyInfo(NoneDependencyInfo noneDependencyInfo, out List<string> csvData)
    {
        csvData = new List<string>();

        if(null == noneDependencyInfo)
        {
            return;
        }

        foreach (var infoItem in noneDependencyInfo.NoneDependencyInfoTbl)
        {
            csvData.Add(infoItem.Key);

            if (infoItem.Value.IsAllUnUse)
            {
                csvData.Add("," + "该Atlas中所有Sprite均没有被引用");
            }
            else
            {
                foreach(var spriteItem in infoItem.Value.NoneUseSpriteTbl)
                {
                    csvData.Add("," + spriteItem);
                }
            }
     
            csvData.Add(Environment.NewLine);
        }
    }

    private void WriteDependencyCSVFile(string resultDir, string assetPath, List<string> csvDataTbl)
    {
        if (
               string.IsNullOrEmpty(resultDir)
            || string.IsNullOrEmpty(assetPath)
            )
        {
            return;
        }
        string resultFilePath = string.Empty;
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetExtension = Path.GetExtension(assetPath);

        //resultDir = resultDir + "正向引用_" + DateTime.Now.ToString(m_dataFormat) + @"/";
        if (!Directory.Exists(resultDir))
        {
            Directory.CreateDirectory(resultDir);
        }


        resultFilePath = resultDir + assetName + "(" + assetExtension + ").csv";

        CSVOperator.WriteFile(resultFilePath, csvDataTbl.ToArray());
    }

    private void WriteReverseDependencyCSVFile(string resultDir, List<string> csvDataTbl)
    {
        if (string.IsNullOrEmpty(resultDir))
        {
            return;
        }

        string resultFilePath = resultDir + "反向引用_" + DateTime.Now.ToString(m_dataFormat) + ".csv";

        CSVOperator.WriteFile(resultFilePath, csvDataTbl.ToArray());
    }

    private void WriteNoneDependencyCSVFile(string resultDir, List<string> csvDataTbl)
    {
        if (string.IsNullOrEmpty(resultDir))
        {
            return;
        }

        string resultFilePath = resultDir + "无引用_" + DateTime.Now.ToString(m_dataFormat) + ".csv";

        CSVOperator.WriteFile(resultFilePath, csvDataTbl.ToArray());
    }

    private string m_dataFormat = "yyyy年MM月dd HH时mm分ss秒";
}
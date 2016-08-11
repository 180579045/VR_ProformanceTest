using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CSVOperator
{
    public static void WriteFile(string filePath, string[] fileData)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        int count = 1;
        WriteFileProgresser.GetInstance().InitProgresser(fileData.Length, "CSV文件写入中");

        StreamWriter fileWriter = File.CreateText(filePath);
        if (fileWriter != null)
        {
            UniversalEditorUtility.MakeFileWriteable(filePath);
            foreach (var item in fileData)
            {
                fileWriter.WriteLine(item);
                WriteFileProgresser.GetInstance().UpdateProgress(count++);
            }
            fileWriter.Close();
            //File.WriteAllLines(filePath, fileData);
        }
    }

    public static void OpenFile(string filePath)
    {
        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }

        if(File.Exists(filePath))
        {
            System.Diagnostics.Process.Start(filePath); 
        }
    }

}
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
public class SharpZip
{
    public static bool CompressDirectory(string directoryPath, string outputFileName, bool bIsDelete)
    {
        bool bRet = true;

        if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(outputFileName))
        {
            return false;
        }

        if (!Directory.Exists(directoryPath))
        {
            return false;
        }

        using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(outputFileName)))
        {
            zipoutputstream.SetLevel(9);

            Dictionary<string, DateTime> fileList = UniversalEditorUtility.GetAllFies(directoryPath);
            foreach (KeyValuePair<string, DateTime> item in fileList)
            {
                FileStream fs = File.OpenRead(item.Key.ToString());
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(item.Key.Substring(directoryPath.Length));
                entry.DateTime = item.Value;
                entry.Size = fs.Length;
                fs.Close();

                zipoutputstream.PutNextEntry(entry);
                zipoutputstream.Write(buffer, 0, buffer.Length);
            }
        }

        if(bIsDelete)
        {
            DirectoryInfo info = new DirectoryInfo(directoryPath);
            UniversalEditorUtility.MakeDictionaryWriteable(info);

            Directory.Delete(directoryPath, true);
        }

        return bRet;
    }
}
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

public class UniversalEditorUtility
{
    public static void MakeFileWriteable(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (File.Exists(path))
        {
            File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.Hidden);
            File.SetAttributes(path, File.GetAttributes(path) & ~(FileAttributes.Archive | FileAttributes.ReadOnly));
        }
    }

    public static void MakeDictionaryWriteable(DirectoryInfo info)
    {
        foreach (DirectoryInfo newInfo in info.GetDirectories())
        {
            MakeDictionaryWriteable(newInfo);
        }
        foreach (FileInfo newInfo in info.GetFiles())
        {
            newInfo.Attributes = newInfo.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
        }

        info.Attributes = info.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
    }

    public static Dictionary<string, DateTime> GetAllFies(string dir)
    {
        Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
        DirectoryInfo fileDire = new DirectoryInfo(dir);
        if (!fileDire.Exists)
        {
            throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
        }
        GetAllDirFiles(fileDire, FilesList);
        GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
        return FilesList;
    }

    public static List<string> GetAllFilePath(string dir)
    {
        List<string> filePath = new List<string>();

        if (string.IsNullOrEmpty(dir))
        {
            return filePath;
        }

        Dictionary<string, DateTime> fileInfo = GetAllFies(dir);

        foreach (var item in fileInfo)
        {
            string newPath = item.Key.Replace('\\', '/');
            filePath.Add(newPath);
        }

        return filePath;
    }

    public static List<string> GetAllFileNameWithoutExtension(string dir)
    {
        List<string> fileNameWithoutExtension = new List<string>();

        if (string.IsNullOrEmpty(dir))
        {
            return fileNameWithoutExtension;
        }

        Dictionary<string, DateTime> fileInfo = GetAllFies(dir);

        foreach (var item in fileInfo)
        {
            string newPath = Path.GetFileNameWithoutExtension(item.Key);
            fileNameWithoutExtension.Add(newPath);
        }

        return fileNameWithoutExtension;
    }
    public static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
    {
        foreach (DirectoryInfo dir in dirs)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
            GetAllDirsFiles(dir.GetDirectories(), filesList);
        }
    }
    public static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
    {
        foreach (FileInfo file in dir.GetFiles("*.*"))
        {
            filesList.Add(file.FullName, file.LastWriteTime);
        }
    }

    public static void CopyDirectory(string srcDirectory, string destDirectory, bool isCover)
    {

        if (Directory.Exists(destDirectory))
        {
            if(isCover)
            {
                DirectoryInfo info = new DirectoryInfo(destDirectory);
                MakeDictionaryWriteable(info);

                Directory.Delete(destDirectory);
                Directory.CreateDirectory(destDirectory);
            }
        }
        else
        {
            Directory.CreateDirectory(destDirectory);
        }

        foreach (string sub in Directory.GetDirectories(srcDirectory))
        {
            CopyDirectory(sub + "/", destDirectory + Path.GetFileName(sub) + "/", isCover);
        }

        foreach (string file in Directory.GetFiles(srcDirectory))
        {
            MakeFileWriteable(file);
            File.Copy(file, destDirectory + Path.GetFileName(file), true);
        }
    }

    public static void DeleteFileByDirectory(DirectoryInfo info)
    {
        foreach (DirectoryInfo newInfo in info.GetDirectories())
        {
            DeleteFileByDirectory(newInfo);
        }
        foreach (FileInfo newInfo in info.GetFiles())
        {
            newInfo.Attributes = newInfo.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
            newInfo.Delete();
        }

        info.Attributes = info.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
        info.Delete();
    }

    public static bool SaveEditorLayout(EditorRoot root)
    {
        bool bRet = false;

        if(null == root)
        {
            return false;
        }

        EditorLayoutInfo editorLayout = new EditorLayoutInfo();

        editorLayout.WirteEditorLayoutInfo(LayoutInfoBaseDir, root);
       
        return bRet;
    }

    public static bool LoadEditorLayout(EditorRoot root)
    {
        bool bRet = false;

        EditorLayoutInfo editorLayout = new EditorLayoutInfo();

        editorLayout.ReadEditorLayoutInfo(LayoutInfoBaseDir, root, ref editorLayout);

        if (CheckLayoutInfoValid(root, editorLayout))
        {
            //root.position = new Rect(editorLayout.XPox, editorLayout.YPox, editorLayout.Width, editorLayout.Height);
            //EditorUtility.SetDirty(root);
            //root.position = new Rect(editorLayout.XPox, editorLayout.YPox, editorLayout.Width, editorLayout.Height);
            List<EditorControl> spliterTbl = root.GetSpliterCtrl(root.RootCtrl);

            foreach (var item in spliterTbl)
            {
                foreach(var info in editorLayout.DivisionInfo)
                {
                    if (item.CtrlID == info.SpliterID)
                    {
                        item.layoutConstraint.spliterOffset = info.SplitOffset;
                        item.layoutConstraint.spliterOffsetInv = info.SpliterOffsetInv;
                        break;
                    }
                }
            }
        }

        return bRet;
    }

    private static bool CheckLayoutInfoValid(EditorRoot root, EditorLayoutInfo layoutInfo)
    {
        bool bRet = true;

        if(
               (null == root)
            || (null == layoutInfo)
            )
        {
            return false;
        }

        do
        {
            List<string> spliterTbl = root.GetSpliterCtrlID(root.RootCtrl);

            if (spliterTbl.Count != layoutInfo.DivisionInfo.Count)
            {
                bRet = false;
                break;
            }

            foreach (var item in layoutInfo.DivisionInfo)
            {
                if (!spliterTbl.Contains(item.SpliterID))
                {
                    bRet = false;
                    break;
                }
            }
            if(!bRet)
            {
                break;
            }

        } while (false);

        return bRet;
    }

    public static void DestoryChildren(GameObject parentGO)
    {
        if(null == parentGO)
        {
            return;
        }
        bool isPlaying = Application.isPlaying;

        Transform t = parentGO.transform;

        while (t.childCount != 0)
        {
            Transform child = t.GetChild(0);

            if (isPlaying)
            {
                child.parent = null;
                UnityEngine.Object.Destroy(child.gameObject);
            }
            else UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
    }

    public static string LayoutInfoBaseDir = Application.dataPath.TrimEnd("Assets".ToCharArray()) + "EditorLayout/";
}
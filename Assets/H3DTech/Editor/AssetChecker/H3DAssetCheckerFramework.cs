using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

public class H3DAssetCheckerFramework
{
    //警告日志工具函数，用于子类输出警告
    public void LogWarning( string log )
    {
        Debug.LogWarning(log);
    }

    public void LogWarning(string log,UnityEngine.Object context)
    {
        Debug.LogWarning(log,context);
    }

    //错误日志工具函数，用于子类输出错误信息
    public void LogError( string log )
    {
        Debug.LogError(log);
    }

    public void LogError(string log, UnityEngine.Object context)
    {
        Debug.LogError(log,context);
    }

    //对 "Assets/" 目录下的所有资源进行检查
    public void TotalCheck()
    {
        if( H3DAssetCheckerConfig.GetInstance().isAssetCheckerOff )
        {
            return;
        }


        var pathCheckerList = H3DAssetCheckerFramework.GetInstance().GetAssetPathCheckerList();
        var assetCheckerList = H3DAssetCheckerFramework.GetInstance().GetAssetCheckerList(H3DAssetChecker.ResouceType.ALL);
 

        var assetPaths =  AssetDatabase.GetAllAssetPaths();
        foreach( var path in assetPaths )
        {
             
            if (ResourceManageToolUtility.PathIsFolder(path))
                continue;

            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(object));
            H3DAssetChecker.ResouceType resType = H3DAssetChecker.QueryAssetResType(asset);
            var assetImporter = AssetImporter.GetAtPath(path);
            bool needImport = false; 
            bool firstImport = !HasAssetImportMark(assetImporter); 

            if( firstImport )
            {//若为第一次导入则加入标记
                MarkAssetImporterAsAlreadyImported(assetImporter);
                needImport = true;
            }

           
            foreach( var checker in pathCheckerList )
            { 

                if( H3DAssetCheckerUtil.IsPathInclude(path,checker) )
                {
                    checker.Check(asset, assetImporter, path, firstImport, ref needImport);
                }
            }
             
            foreach( var checker in assetCheckerList )
            {
                if ( 
                    checker.ResType == resType
                    && H3DAssetCheckerUtil.IsPathInclude(path, checker)
                    )
                {//使用符合资源类型的checker进行检查

                    checker.Check(asset, assetImporter, path, firstImport, ref needImport);
                    checker.PostCheck(asset, assetImporter, path, firstImport, ref needImport);
                }
            }  

            if( needImport )
            {
                AssetDatabase.ImportAsset(path);
            }
        }

        AssetDatabase.Refresh();
    }

    //获得所有路径检查器
    public List<H3DAssetPathChecker> GetAssetPathCheckerList()
    {
        List<H3DAssetPathChecker> checkerList = GetBaseTypeInstanceList<H3DAssetPathChecker>();
        checkerList.Sort( new AssetCheckerComp<H3DAssetPathChecker>() ); 
        return checkerList;
    }

    //获取检查某一资源类型的所有检查器
    public List<H3DAssetChecker> GetAssetCheckerList( H3DAssetChecker.ResouceType resType )
    {
        var checkerList = GetBaseTypeInstanceList<H3DAssetChecker>(); 
        List<H3DAssetChecker> finalCheckerList = new List<H3DAssetChecker>();
        foreach( var c in checkerList )
        {
            if( c.ResType == resType || resType == H3DAssetChecker.ResouceType.ALL )
            {
                if (!(c is H3DAssetPathChecker))
                {//过滤掉路径检查器
                    finalCheckerList.Add(c);
                }
            }
        }
        finalCheckerList.Sort(new AssetCheckerComp<H3DAssetChecker>());  
        return finalCheckerList;
    }

    class AssetCheckerComp<T> : IComparer<T> where T : H3DAssetChecker
    {
        public int Compare(T x, T y)
        {
            if( x.Priority > y.Priority )
            {
                return -1;
            }else if( x.Priority == y.Priority )
            {
                return 0; 
            }
            return 1;
        }
    }

    List<T> GetBaseTypeInstanceList<T>() where T : H3DAssetChecker
    {
        List<T> instList = new List<T>();
        List<System.Type> typeList = new List<System.Type>();

        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        { 
            if( type.IsClass && type.IsSubclassOf(typeof(T)))
            {
                typeList.Add(type);
                
            } 
        }

        //只取没有派生类的Checker，实现派生类Checker覆盖父类行为
        List<System.Type> removeTypeList = new List<System.Type>();
        foreach( var type in typeList )
        {
            foreach( var type2 in typeList )
            {
                if( type.IsSubclassOf(type2) )
                {
                    if (!removeTypeList.Contains(type2))
                    {
                        removeTypeList.Add(type2);
                    } 
                }
            }
        }

        foreach( var type in removeTypeList )
        {
            typeList.Remove(type);
        }

        foreach( var type in typeList )
        {
            var inst = Assembly.GetExecutingAssembly().CreateInstance(type.FullName) as T;
            instList.Add(inst);
        }

        return instList; 
    }

    static string alreadyImportMark = "ASSETIMPORT_FRAMEWORK_ALREADY_IMPORT";

    public static void MarkAssetImporterAsAlreadyImported( AssetImporter imp )
    {
        if (imp.userData != null && !imp.userData.Contains(alreadyImportMark))
        {
            imp.userData += alreadyImportMark;
            return;
        }
        imp.userData = alreadyImportMark;
    }

    public static bool HasAssetImportMark( AssetImporter imp )
    {
        if (imp.userData != null && imp.userData.Contains(alreadyImportMark))
        {
            return true;
        }
        return false;
    }

    [MenuItem("H3D/资源检查/资源全面检查")]
    static void CheckTotalAssets()
    {
        H3DAssetCheckerFramework.GetInstance().TotalCheck();
    }

    [MenuItem("H3D/资源检查/生成个人配置文件")]
    static void GeneratePrivateConfig()
    {
        H3DAssetCheckerConfig.GetInstance().GeneratePrivateConfig();
    }

    [MenuItem("H3D/资源检查/生成工程配置文件")]
    static void GenerateProjectConfig()
    {
        H3DAssetCheckerConfig.GetInstance().GenerateProjectConfig();
    }
 
    public static H3DAssetCheckerFramework GetInstance()
    {
        if( _instance == null )
        {
            _instance = new H3DAssetCheckerFramework();
        }
        return _instance;
    }

    static H3DAssetCheckerFramework _instance;
}

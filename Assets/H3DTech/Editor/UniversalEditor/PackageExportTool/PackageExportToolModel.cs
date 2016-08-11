using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

public struct PACKAGE_VERSION
{
   public string MainVers;
   public string UpgradeVer;
   public string P4Ver;

    public PACKAGE_VERSION(string temp)
   {
       MainVers = string.Empty;
       UpgradeVer = string.Empty;
       P4Ver = string.Empty;
   }
}

public enum PACKAGE_FAILED_TYPE
{//发布错误类型
    PACKAGE_FAILED_UNKNOW_ERROR = 0,            //未知错误
    PACKAGE_FAILED_EXPORT_PATH_ERROR,           //导出路径错误
    PACKAGE_FAILED_XMLCONFIG_PATH_ERROR,        //XML config路径错误
    PACKAGE_FAILED_VERSION_NONE_ERROR,          //未设置版本号
    PACKAGE_FAILED_MANUAL_PATH_ERROR,           //手册路径错误
    PACKAGE_FAILED_ADD_ASSETS_ERROR,            //资源添加错误
    PACKAGE_FAILED_REMOVE_ASSETS_ERROR,         //资源移除错误
    PACKAGE_FAILED_NONE_ASSETS_ERROR,           //导出时没有资源
    
    PACKAGE_FAILED_NONEERROR = -1,                //默认值
}

public class PackageExportToolModel
{
    static private PackageExportToolModel m_Instance = null;
    public static PackageExportToolModel GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new PackageExportToolModel();
        }
        return m_Instance;
    }
    public void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
        }
    }

    private bool m_IsWithManual = false;

    private PackageAssets m_Assets = new PackageAssets();

    private PACKAGE_VERSION m_PackageVer = new PACKAGE_VERSION("");

    private PACKAGE_FAILED_TYPE m_ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_NONEERROR;

    public bool IsWithManual { get { return m_IsWithManual; } set { m_IsWithManual = value; } }
 
    public PACKAGE_FAILED_TYPE ErrorType { get { return m_ErrorType; } set { m_ErrorType = value; } }

    public PACKAGE_VERSION PackageVer { get { return m_PackageVer; } set { m_PackageVer = value; } }

    public void ReadExportPath()
    {
        PackageExportConfig.ReadExportPath();
    }

    public void WriteExportPath(string path)
    {
        if(path == null)
        {
            return;
        }

        PackageExportConfig.WriteExportPath(path);
    }

    public void ReadManualPath()
    {
        PackageExportConfig.ReadManualPath();
    }

    public void WriteManualPath(string path)
    {
        if (path == null)
        {
            return;
        }

        PackageExportConfig.WriteManualPath(path);
    }
    public void ReadPublishPath()
    {
        PackageExportConfig.ReadPublishPath();
    }

    public void WritePublishPath(string path)
    {
        if(path == null)
        {
            return;
        }

        PackageExportConfig.WritePublishPath(path);
    }

    public bool AddAssets(string path)
    {
        bool bRet = false;

        if(m_Assets == null)
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_UNKNOW_ERROR;
            return false;
        }

        bRet = m_Assets.AddAssets(path);
        if(!bRet)
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_ADD_ASSETS_ERROR;
        }

        return bRet;
    }

    public bool RemoveAssets(string path)
    {
        bool bRet = false;

        if (m_Assets == null)
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_UNKNOW_ERROR;
            return false;
        }

        bRet = m_Assets.RemoveAssets(path);
        if (!bRet)
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_REMOVE_ASSETS_ERROR;
        }
        return bRet;
    }

    public PackageInfo GetPackageInfo()
    {
        PackageInfo info = null;

        if(m_Assets != null)
        {
            info = m_Assets.PackageInfo;
        }

        return info;
    }

    public bool CheckPackageInfoValid()
    {
        bool bRet = true;

        do
        {
            if (IsPackageInfoNull())
            {
                ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_UNKNOW_ERROR;
                bRet = false;
                break;
            }

            if (string.IsNullOrEmpty(PackageExportConfig.AssetsConfigPath))
            {
                ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_XMLCONFIG_PATH_ERROR;
                bRet = false;
                break;
            }

            if (!IsSubPackageVersionValid())
            {
                ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_VERSION_NONE_ERROR;
                bRet = false;
                break;
            }

            if (m_IsWithManual && !IsPathValid(PackageExportConfig.ManualPath))
            {
                ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_MANUAL_PATH_ERROR;
                bRet = false;
                break;
            }
        } while (false);

        return bRet;
    }
    public bool Export(string exportPath = null)
    {
        bool bRet = false;

        do
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_NONEERROR;

            bRet = CheckPackageInfoValid();
            if (!bRet)
            {
                break;
            }

            bRet = SaveXMLConfig();
            if (!bRet)
            {
                break;
            }

            bRet = ExportPackage(exportPath);
            if (!bRet)
            {
                break;
            }

            bRet = Publish();
            if (!bRet)
            {
                break;
            }

        } while (false);

        DeleteTempFiles();

        if (onExportComplete != null)
        {
            onExportComplete();
        }

        return bRet;
    }

    public bool Load()
    {
        bool bRet = false;

        bRet = LoadXMLConfig();
        

        return bRet;
    }

    public bool Publish()
    {
        bool bRet = true;
        string directory = null;

        do
        {
            directory = PrepareCompressDirectory();
            if (string.IsNullOrEmpty(directory))
            {
                bRet = false;
                break;
            }

            //bRet = CompressPackage(directory);
            //if(!bRet)
            //{
            //    break;
            //}
        } while (false);

        return bRet;
    }
    public bool SaveXMLConfig()
    {
        bool bRet = true;

        if(IsPackageInfoNull())
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_UNKNOW_ERROR;
            return false;
        }

        if (string.IsNullOrEmpty(PackageExportConfig.AssetsConfigPath))
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_XMLCONFIG_PATH_ERROR;
            return false;
        }

        if (!IsSubPackageVersionValid())
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_VERSION_NONE_ERROR;
            return false;
        }

        PackageExportConfig.CreateXMLConifg(PackageExportConfig.AssetsConfigPath);
        UniversalEditorUtility.MakeFileWriteable(PackageExportConfig.AssetsConfigPath);

        XmlDocument docment = new XmlDocument();
        XmlElement root = docment.CreateElement("ExportPackageConfig");
        docment.AppendChild(root);

        XmlElement nodeVersion = docment.CreateElement("Version");
        UpdatePackageVersion();
        nodeVersion.InnerText = m_Assets.PackageInfo.VersionNum;
        root.AppendChild(nodeVersion);

        XmlElement nodePath = docment.CreateElement("Path");
        root.AppendChild(nodePath);
        foreach (string sPath in m_Assets.PackageInfo.ExportAssets)
        {
            XmlElement node = docment.CreateElement("Item");
            node.InnerText = sPath;
            nodePath.AppendChild(node);
        }

        try
        {
            docment.Save(PackageExportConfig.AssetsConfigPath);
        }
        catch (System.Exception e)
        {
            bRet = false;
            Debug.Log("保存打包工具配置文件失败: " + e.Message);
        }

        return bRet;
    }

    private bool LoadXMLConfig()
    {
        if(IsPackageInfoNull())
        {
            return false;
        }

        if (string.IsNullOrEmpty(PackageExportConfig.AssetsConfigPath))
        {
            return false;
        }

        m_Assets.ClearPackage();

        if (File.Exists(PackageExportConfig.AssetsConfigPath))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PackageExportConfig.AssetsConfigPath);
            XmlNode root = doc.SelectSingleNode("ExportPackageConfig");
            if (root != null)
            {
                XmlNode version_node = root.SelectSingleNode("Version");
                if (version_node != null)
                {
                    m_Assets.PackageInfo.VersionNum = version_node.InnerText;
                    AnalysePackageVersion();
                }

                XmlNode path_node = root.SelectSingleNode("Path");
                if (path_node != null)
                {
                    foreach (XmlNode n in path_node.ChildNodes)
                    {
                        if (n.Name == "Item")
                        {
                            string s = n.InnerText;
                            if (s.Length > 0)
                            {
                                m_Assets.PackageInfo.ExportAssets.Add(s);
                            }
                        }
                    }
                }
            }

        }

        if (0 == m_Assets.PackageInfo.ExportAssets.Count)
        {
            m_Assets.PackageInfo.ExportAssets.Add("Assets");
        }

        //if (string.IsNullOrEmpty(m_Assets.PackageInfo.VersionNum))
        //{
        //    m_PackageVer.MainVers = string.Empty;
        //    m_PackageVer.UpgradeVer = string.Empty;
        //    m_PackageVer.P4Ver = string.Empty;
        //}

        return true;
    }

    public bool ExportPackage(string exportPath)
    {
        bool bRet = true;
        string path = null;

        if(IsPackageInfoNull())
        {
            ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_UNKNOW_ERROR;
            return false;
        }

        do
        {
            path = UpdateExportPath(exportPath);

            if (!IsPathValid(path))
            {
                ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_EXPORT_PATH_ERROR;
                bRet = false;
                break;
            }

            if (0 == m_Assets.PackageInfo.ExportAssets.Count)
            {
                ErrorType = PACKAGE_FAILED_TYPE.PACKAGE_FAILED_NONE_ASSETS_ERROR;
                bRet = false;
                break;
            }

            string[] sAssets = m_Assets.PackageInfo.ExportAssets.ToArray();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            AssetDatabase.ExportPackage(sAssets, path + PackageExportConfig.PackageName, ExportPackageOptions.Recurse);

        } while (false);

        return bRet;
    }

    public string GetExportPath()
    {
        return PackageExportConfig.ExportPath;
    }

    public string GetManualPath()
    {
        return PackageExportConfig.ManualPath;
    }

    public string UpdateExportPath(string exportPath)
    {
        if (exportPath != null)
        {
            if (string.IsNullOrEmpty(PackageExportConfig.ExportPath)
                || (exportPath.CompareTo(PackageExportConfig.ExportPath) != 0))
            {
                if (!(exportPath.EndsWith(@"\") || exportPath.EndsWith("/")))
                {
                    exportPath = exportPath + "/";
                }
                PackageExportConfig.WriteExportPath(exportPath);
            }
        }

        return PackageExportConfig.ExportPath;
    }

    public string UpdateManualPath(string manualPath)
    {
        if (!string.IsNullOrEmpty(manualPath))
        {
            if (string.IsNullOrEmpty(PackageExportConfig.ManualPath)
                || (manualPath.CompareTo(PackageExportConfig.ManualPath) != 0))
            {
                if (!(manualPath.EndsWith(@"\") || manualPath.EndsWith("/")))
                {
                    manualPath = manualPath + "/";
                }
                PackageExportConfig.WriteManualPath(manualPath);
            }
        }
        else
        {
            PackageExportConfig.WriteManualPath("");
        }

        return PackageExportConfig.ManualPath;
    }

    public string PrepareCompressDirectory()
    {
        string zipName = null;
        string mainVerDir = null;

        if (IsPackageInfoNull())
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(PackageExportConfig.ExportPath))
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(PackageExportConfig.PackageName))
        {
            return string.Empty;
        }

        if(!IsSubPackageVersionValid())
        {
            return string.Empty;
        }

        if (m_IsWithManual && string.IsNullOrEmpty(PackageExportConfig.ManualPath))
        {
            return string.Empty;
        }

        mainVerDir = PackageExportConfig.ExportPath + "Ver" + m_PackageVer.MainVers + "." + m_PackageVer.UpgradeVer + "/";
        zipName = mainVerDir + Path.GetFileNameWithoutExtension(PackageExportConfig.PackageName) + "_Ver" + m_Assets.PackageInfo.VersionNum;
      
        if (Directory.Exists(zipName))
        {
            DirectoryInfo info = new DirectoryInfo(zipName);
            UniversalEditorUtility.MakeDictionaryWriteable(info);

            UniversalEditorUtility.DeleteFileByDirectory(info);
        }
        
        Directory.CreateDirectory(zipName);

        File.Copy(PackageExportConfig.ExportPath + PackageExportConfig.PackageName, zipName + "/" + PackageExportConfig.PackageName, true);

        if(m_IsWithManual)
        {
            UniversalEditorUtility.CopyDirectory(PackageExportConfig.ManualPath, zipName + "/Manual/", true);
        }

        return zipName;
    }

    public bool CompressPackage(string directory)
    {
        bool bRet = true;

        if(string.IsNullOrEmpty(directory))
        {
            return false;
        }
        directory = directory.Replace("/", "\\");
        bRet = SharpZip.CompressDirectory(directory, directory + ".zip", true);

        return bRet;
    }

    public void SetMainVer(string mainVer)
    {
        if(null == mainVer)
        {
            return;
        }

        m_PackageVer.MainVers = mainVer;

        UpdatePackageVersion();
    }

    public void SetUpgradeVer(string upgrageVer)
    { 
        if(null == upgrageVer)
        {
            return;
        }

        m_PackageVer.UpgradeVer = upgrageVer;

        UpdatePackageVersion();
    }

    public void SetP4Ver(string p4Ver) 
    {
        if(null == p4Ver)
        {
            return;
        }

        m_PackageVer.P4Ver = p4Ver;

        UpdatePackageVersion();
    }

    public void UpdatePackageVersion()
    {
        if (IsPackageInfoNull()) 
        {
            return;
        }

        m_Assets.PackageInfo.VersionNum = m_PackageVer.MainVers + "." + m_PackageVer.UpgradeVer + "." + m_PackageVer.P4Ver;
    }
   
    private bool IsPathValid(string path)
    {
        bool bRet = true;
       
        if (string.IsNullOrEmpty(path)
            || (!path.Contains(":"))
            || (0 == path.IndexOfAny(new char[] { '\\', '/' })))
        {
            bRet = false;
        }

        return bRet;
    }

    private bool IsPackageInfoNull()
    {
        bool bRet = false;

        if((null == m_Assets) || (null == m_Assets.PackageInfo))
        {
            bRet = true;
        }

        return bRet;
    }

    private bool IsSubPackageVersionValid()
    {
        bool bRet = true;

        do{
            if(string.IsNullOrEmpty(m_PackageVer.MainVers)
              ||string.IsNullOrEmpty(m_PackageVer.UpgradeVer)
              ||string.IsNullOrEmpty(m_PackageVer.P4Ver))
            {
                bRet = false;
                break;
            }
        
            try{
                int.Parse(m_PackageVer.MainVers);
                int.Parse(m_PackageVer.UpgradeVer);
                int.Parse(m_PackageVer.P4Ver);
            }
            catch
            {
                bRet = false;
                break;
            }
        }while(false);

        return bRet;
    }

    private bool AnalysePackageVersion()
    {
        bool bRet = true;
        string tempStr = null;
        int mainLen = 0;
        int upgradeLen = 0;

        if (IsPackageInfoNull())
        {
            return false;
        }

        do
        {
            if (string.IsNullOrEmpty(m_Assets.PackageInfo.VersionNum))
            {
                bRet = false;
                break;
            }

            tempStr = m_Assets.PackageInfo.VersionNum;
            if (tempStr.Contains("."))
            {
                tempStr = tempStr.Substring(tempStr.IndexOf('.') + 1);
                mainLen = m_Assets.PackageInfo.VersionNum.Length - tempStr.Length - 1;

                if (tempStr.Contains("."))
                {
                    tempStr = tempStr.Substring(tempStr.IndexOf('.') + 1);
                    upgradeLen = m_Assets.PackageInfo.VersionNum.Length - tempStr.Length - 1 - mainLen - 1;
                }
                else
                {
                    bRet = false;
                    break;
                }
            }
            else
            {
                bRet = false;
                break;
            }

            m_PackageVer.MainVers = m_Assets.PackageInfo.VersionNum.Substring(0, mainLen);
            m_PackageVer.UpgradeVer = m_Assets.PackageInfo.VersionNum.Substring(mainLen + 1, upgradeLen);
            m_PackageVer.P4Ver = tempStr;
        } while (false);

        if(!bRet)
        {
            m_PackageVer.MainVers = string.Empty;
            m_PackageVer.UpgradeVer = string.Empty;
            m_PackageVer.P4Ver = string.Empty;
        }

        return bRet;
    }

    private void DeleteTempFiles()
    {
        do
        {
            if (string.IsNullOrEmpty(PackageExportConfig.ExportPath))
            {
                break;
            }

            if (string.IsNullOrEmpty(PackageExportConfig.PackageName))
            {
                break;
            }

            if (File.Exists(PackageExportConfig.ExportPath + PackageExportConfig.PackageName))
            {
                UniversalEditorUtility.MakeFileWriteable(PackageExportConfig.ExportPath + PackageExportConfig.PackageName);
                File.Delete(PackageExportConfig.ExportPath + PackageExportConfig.PackageName);
            }
        } while (false);

    }
    public delegate void ExportComplete();
    public ExportComplete onExportComplete;
}
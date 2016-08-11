using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PackageInfo
{
    private List<string> m_ExportAssets = null;
    public List<string> ExportAssets { get { return m_ExportAssets; } set { m_ExportAssets = value; } }

    private string m_VersionNum = string.Empty;
    public string VersionNum { get { return m_VersionNum; } set { m_VersionNum = value; } }

}

public class PackageAssets
{
    private PackageInfo m_PackageInfo = null;
    public PackageInfo PackageInfo { get { return m_PackageInfo; } set { m_PackageInfo = value; } }

    public PackageAssets()
    {
        m_PackageInfo = new PackageInfo();
        m_PackageInfo.ExportAssets = new List<string>();
    }

    public bool IsPackageNull()
    {
        bool bRet = false;

        if ((m_PackageInfo == null) || (m_PackageInfo.ExportAssets == null))
        {
            bRet = true;
        }

        return bRet;
    }

    public void ClearPackage()
    {
        if (m_PackageInfo != null)
        {
            m_PackageInfo.ExportAssets.Clear();
            m_PackageInfo.VersionNum = null;
        }
    }

    public bool AddAssets(string path)
    {
        bool bRet = false;

        if ((IsPackageNull()) || (path == null))
        {
            return false;
        }

        m_PackageInfo.ExportAssets.Add(path);
        bRet = true;

        return bRet;
    }

    public bool RemoveAssets(string path)
    {
        bool bRet = false;

        if ((!IsPackageNull()) || (path == null))
        {
            return false;
        }

        m_PackageInfo.ExportAssets.Remove(path);
        bRet = true;

        return bRet;
    }
}
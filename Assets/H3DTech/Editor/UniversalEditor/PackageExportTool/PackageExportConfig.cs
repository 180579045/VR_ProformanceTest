using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class PackageExportConfig
{
    private static string m_ExportPath = string.Empty;
    private static string m_ManualPath = string.Empty;
    private static string m_PublishPath = string.Empty;
    private static string m_PackageName = "H3DTech_U3DEditor.unitypackage";
    private static string m_ExportConfigPath = "Assets/H3DTechConfig/H3DTechPublishEditor/PackageExportInfoConfig.xml";
    private static string m_PublishConfigPath = "Assets/H3DTechConfig/H3DTechPublishEditor/PackageExportTool/Config/PackagePublishConfig.txt";

    private static string m_AssetsConfigPath = "Assets/H3DTechConfig/H3DTechPublishEditor/PackageExportTool.xml";

    public static string ExportPath { get { return m_ExportPath; } set { m_ExportPath = value; } }
    public static string ManualPath { get { return m_ManualPath; } set { m_ManualPath = value; } }
    public static string PublishPath { get { return m_PublishPath; } set { m_PublishPath = value; } }
    public static string AssetsConfigPath { get { return m_AssetsConfigPath; } set { m_AssetsConfigPath = value; } }
    public static string PackageName { get { return m_PackageName; } set { m_PackageName = value; } }

    public static void WriteExportPath(string path)
    {
        if (null == path)
        {
            return;
        }

        if (!File.Exists(m_ExportConfigPath))
        {
            CreateXMLConifg(m_ExportConfigPath);

            UniversalEditorUtility.MakeFileWriteable(m_ExportConfigPath);

            XmlDocument docment = new XmlDocument();   
            XmlElement root = docment.CreateElement("ExportInfoConfig");
            docment.AppendChild(root);

            XmlElement nodeExportPath = docment.CreateElement("ExportPath");
            nodeExportPath.InnerText = path;
            root.AppendChild(nodeExportPath);

            XmlElement nodeManualPath = docment.CreateElement("ManualPath");
            root.AppendChild(nodeManualPath);

            docment.Save(m_ExportConfigPath);
        }
        else
        {
            XmlDocument docment = new XmlDocument();   
            docment.Load(m_ExportConfigPath);
            XmlNode root = docment.SelectSingleNode("ExportInfoConfig");
            if (root != null)
            {
                XmlNode nodeExportPath = root.SelectSingleNode("ExportPath");
                if (nodeExportPath != null)
                {
                    nodeExportPath.InnerText = path;
                }
                else
                {
                    nodeExportPath = docment.CreateElement("ExportPath");
                    nodeExportPath.InnerText = path;
                    root.AppendChild(nodeExportPath);
                }
            }
            else
            {
                root = docment.CreateElement("ExportInfoConfig");
                docment.AppendChild(root);

                XmlNode nodeExportPath = docment.CreateElement("ExportPath");
                nodeExportPath.InnerText = path;
                root.AppendChild(nodeExportPath);
            }
            docment.Save(m_ExportConfigPath);
        }

        ExportPath = path;
    }

    public static string ReadExportPath()
    {
        string exportPath = null;

        if (File.Exists(m_ExportConfigPath))
        {
            UniversalEditorUtility.MakeFileWriteable(m_ExportConfigPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(m_ExportConfigPath);
            XmlNode root = docment.SelectSingleNode("ExportInfoConfig");
            if (root != null)
            {
                XmlNode nodeExportPath = root.SelectSingleNode("ExportPath");
                if (nodeExportPath != null)
                {
                    exportPath = nodeExportPath.InnerText;
                }
            }
        }

        if ((exportPath != null) && (exportPath != ""))
        {
            ExportPath = exportPath;
        }

        return exportPath;
    }

    public static void WriteManualPath(string path)
    {
        if (null == path)
        {
            return;
        }

        if (!File.Exists(m_ExportConfigPath))
        {
            CreateXMLConifg(m_ExportConfigPath);

            UniversalEditorUtility.MakeFileWriteable(m_ExportConfigPath);

            XmlDocument docment = new XmlDocument();
            XmlElement root = docment.CreateElement("ExportInfoConfig");
            docment.AppendChild(root);

            XmlElement nodeExportPath = docment.CreateElement("ExportPath");
            root.AppendChild(nodeExportPath);

            XmlElement nodeManualPath = docment.CreateElement("ManualPath");
            nodeManualPath.InnerText = path;
            root.AppendChild(nodeManualPath);

            docment.Save(m_ExportConfigPath);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(m_ExportConfigPath);
            XmlNode root = docment.SelectSingleNode("ExportInfoConfig");
            if (root != null)
            {
                XmlNode nodeExportPath = root.SelectSingleNode("ManualPath");
                if (nodeExportPath != null)
                {
                    nodeExportPath.InnerText = path;
                }
                else
                {
                    nodeExportPath = docment.CreateElement("ManualPath");
                    nodeExportPath.InnerText = path;
                    root.AppendChild(nodeExportPath);
                }
            }
            else
            {
                root = docment.CreateElement("ExportInfoConfig");
                docment.AppendChild(root);

                XmlNode nodeExportPath = docment.CreateElement("ManualPath");
                nodeExportPath.InnerText = path;
                root.AppendChild(nodeExportPath);
            }

            docment.Save(m_ExportConfigPath);
        }

        ManualPath = path;

    }

    public static string ReadManualPath()
    {
        string manualPath = null;

        if (File.Exists(m_ExportConfigPath))
        {
            UniversalEditorUtility.MakeFileWriteable(m_ExportConfigPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(m_ExportConfigPath);

            XmlNode root = docment.SelectSingleNode("ExportInfoConfig");
            if (root != null)
            {
                XmlNode nodeExportPath = root.SelectSingleNode("ManualPath");
                if (nodeExportPath != null)
                {
                    manualPath = nodeExportPath.InnerText;
                }
            }
        }

        if ((manualPath != null) && (manualPath != ""))
        {
            ManualPath = manualPath;
        }

        return manualPath;
    }
    public static void WritePublishPath(string path)
    {
        FileStream fileStream = null;
        StreamWriter streamW = null;

        fileStream = new FileStream(m_PublishConfigPath, FileMode.Create);
        UniversalEditorUtility.MakeFileWriteable(m_PublishConfigPath);

        streamW = new StreamWriter(fileStream);
        streamW.Write(path);

        streamW.Close();
        fileStream.Close();

        PublishPath = path;
    }

    public static string ReadPublishPath()
    {
        FileStream fileStream = null;
        StreamReader streamR = null;
        string publishPath = null;

        if (File.Exists(m_PublishConfigPath))
        {
            UniversalEditorUtility.MakeFileWriteable(m_PublishConfigPath);
            fileStream = new FileStream(m_PublishConfigPath, FileMode.Open);
            streamR = new StreamReader(fileStream);

            while (!streamR.EndOfStream)
            {
                publishPath += streamR.ReadLine();
            }

            streamR.Close();
            fileStream.Close();
        }

        if ((publishPath != null) && (publishPath != ""))
        {
            PublishPath = publishPath;
        }

        return publishPath;
    }
    
    public static void CreateXMLConifg(string path)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
    }
}
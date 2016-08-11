
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
public static class EditorHelper
{
    //Modify by liteng for 发布工具改善
    public static bool debugMode = GetPublishMode();
    private static string m_ConfigXMLPath = "Assets/H3DTech/Editor/UniversalEditor/Base/Config/PublishMode.xml";

    public static bool IsDebugMode()
    {
        return debugMode;
    }

    public static string GetProjectPath()
    {
        string projPath = Application.dataPath;
        projPath = projPath.Substring(0, projPath.LastIndexOf('/') + 1);
        return projPath;
    }

    //Add by liteng for 发布工具改善 start
    public static void SetPublishMode(bool bIsDebugMode)
    {
        if (!Directory.Exists(Path.GetDirectoryName(m_ConfigXMLPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(m_ConfigXMLPath));
        }

        UniversalEditorUtility.MakeFileWriteable(m_ConfigXMLPath);

        XmlDocument docment = new XmlDocument();
        XmlElement root = docment.CreateElement("PublishModeConfig");
        docment.AppendChild(root);

        XmlElement nodeMode = docment.CreateElement("Mode");
        if(bIsDebugMode)
        {
            nodeMode.InnerText = "Debug";
        }
        else
        {
            nodeMode.InnerText = "Release";
        }
        root.AppendChild(nodeMode);

        docment.Save(m_ConfigXMLPath);

        debugMode = bIsDebugMode;
    }

    public static bool GetPublishMode()
    {
        bool bIsDebugMode = true;

        if (string.IsNullOrEmpty(m_ConfigXMLPath))
        {
            m_ConfigXMLPath = "Assets/H3DTech/Editor/UniversalEditor/Base/Config/PublishMode.xml";
        }

        if (File.Exists(m_ConfigXMLPath))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_ConfigXMLPath);
            XmlNode root = doc.SelectSingleNode("PublishModeConfig");
            if (root != null)
            {
                XmlNode version_node = root.SelectSingleNode("Mode");
                if (version_node != null)
                {
                    if(0 == version_node.InnerText.CompareTo("Release"))
                    {
                        bIsDebugMode = false;
                    }
                }
            }
        }

        return bIsDebugMode;
    }

    //Add by liteng for 发布工具改善 end
}

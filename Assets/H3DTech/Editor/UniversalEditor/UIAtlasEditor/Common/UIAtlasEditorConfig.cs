using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
public class UIAtlasEditorConfig 
{
    //Common
    public static string ImageBasePath { get { return imageBasePath; } set { imageBasePath = value; } }
    public static string ProjectPath { get { return projectPath; } set { projectPath = value; } }

    //Consistency
    public static string ConsistencyPreafabPath { get { return consistencyPrefabPath; } set { consistencyPrefabPath = value; } }
    public static string ConsistencyResultPath { get { return consistencyResultPath; } set { consistencyResultPath = value; } }

    //Reference
    public static string ReferencePrefabPath { get { return referencePrefabPath; } set { referencePrefabPath = value; } }
    public static string ReferenceScenePath { get { return referenceScenePath; } set { referenceScenePath = value; } }
    public static string ReferenceResultPath { get { return referenceResultPath; } set { referenceResultPath = value; } }

    //得到Atlas编辑器的临时路径
    public static string AbsTempPath { get { return EditorHelper.GetProjectPath()+tempPath;} }

    //得到Atlas编辑器的相对路径（相对于工程路径）
    public static string TempPath { get { return tempPath; } set { tempPath = value; } }

    private static string tempPath = "Assets/H3DTech/Editor/UniversalEditor/UIAtlasEditor/_Temp/";

    //Common
    private static string imageBasePath = string.Empty;
    private static string projectPath = string.Empty;

    //Consistency
    private static string consistencyPrefabPath = string.Empty;
    private static string consistencyResultPath = string.Empty;

    //Reference
    private static string referencePrefabPath = string.Empty;
    private static string referenceScenePath = string.Empty;
    private static string referenceResultPath = string.Empty;

    private static string configPath = "Assets/H3DTechConfig/UIAtlasEditor/UIAtlasConfig.xml";

    public delegate void BasePathChangeNotify(string newBaePath);

    static public BasePathChangeNotify onBasePathChange = UIAtlasEditorModel.OnBasePathChange;

    public static void WriteImageBasePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConifg(configPath);

            UniversalEditorUtility.MakeFileWriteable(configPath);

            XmlDocument docment = new XmlDocument();
            XmlElement root = docment.CreateElement("UIAtlasConfig");
            docment.AppendChild(root);

            XmlElement nodeImageBasePath = docment.CreateElement("ImageBasePath");
            nodeImageBasePath.InnerText = path;
            root.AppendChild(nodeImageBasePath);

            XmlElement nodeProjectPath = docment.CreateElement("ProjectPath");
            root.AppendChild(nodeProjectPath);

            docment.Save(configPath);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeImageBasePath = root.SelectSingleNode("ImageBasePath");
                if (nodeImageBasePath != null)
                {
                    nodeImageBasePath.InnerText = path;
                }
                else
                {
                    nodeImageBasePath = docment.CreateElement("ImageBasePath");
                    nodeImageBasePath.InnerText = path;
                    root.AppendChild(nodeImageBasePath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeImageBasePath = docment.CreateElement("ImageBasePath");
                nodeImageBasePath.InnerText = path;
                root.AppendChild(nodeImageBasePath);
            }
            docment.Save(configPath);
        }

        imageBasePath = path;
        onBasePathChange(path);

    }

    public static string ReadImageBasePath()
    {
        string imagePath = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeImageBasePath = root.SelectSingleNode("ImageBasePath");
                if (nodeImageBasePath != null)
                {
                    imagePath = nodeImageBasePath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(imagePath))
        {
            imageBasePath = imagePath;
        }

        return imagePath;
    }

    public static void WriteProjectPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConifg(configPath);

            UniversalEditorUtility.MakeFileWriteable(configPath);

            XmlDocument docment = new XmlDocument();
            XmlElement root = docment.CreateElement("UIAtlasConfig");
            docment.AppendChild(root);

            XmlElement nodeImageBasePath = docment.CreateElement("ImageBasePath");
            root.AppendChild(nodeImageBasePath);

            XmlElement nodeProjectPath = docment.CreateElement("ProjectPath");
            nodeProjectPath.InnerText = path;
            root.AppendChild(nodeProjectPath);

            docment.Save(configPath);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeProjectPath = root.SelectSingleNode("ProjectPath");
                if (nodeProjectPath != null)
                {
                    nodeProjectPath.InnerText = path;
                }
                else
                {
                    nodeProjectPath = docment.CreateElement("ProjectPath");
                    nodeProjectPath.InnerText = path;
                    root.AppendChild(nodeProjectPath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeProjectPath = docment.CreateElement("ProjectPath");
                nodeProjectPath.InnerText = path;
                root.AppendChild(nodeProjectPath);
            }

            docment.Save(configPath);
        }

        projectPath = path;
    }

    public static string ReadProjectPath()
    {
        string project = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeProjectPath = root.SelectSingleNode("ProjectPath");
                if (nodeProjectPath != null)
                {
                    project = nodeProjectPath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(project))
        {
            projectPath = project;
        }

        return project;
    }

    public static void WriteConsistencyResultPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConfigWithNode(configPath, "ConsistencyResultPath", path);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeConsistencyResultPath = root.SelectSingleNode("ConsistencyResultPath");
                if (nodeConsistencyResultPath != null)
                {
                    nodeConsistencyResultPath.InnerText = path;
                }
                else
                {
                    nodeConsistencyResultPath = docment.CreateElement("ConsistencyResultPath");
                    nodeConsistencyResultPath.InnerText = path;
                    root.AppendChild(nodeConsistencyResultPath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeConsistencyResultPath = docment.CreateElement("ConsistencyResultPath");
                nodeConsistencyResultPath.InnerText = path;
                root.AppendChild(nodeConsistencyResultPath);
            }

            docment.Save(configPath);
        }

        consistencyResultPath = path;

    }

    public static string ReadConsistencyResultPath()
    {
        string path = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeConsistencyResultPath = root.SelectSingleNode("ConsistencyResultPath");
                if (nodeConsistencyResultPath != null)
                {
                    path = nodeConsistencyResultPath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(path))
        {
            ConsistencyResultPath = path;
        }

        return path;
    }

    public static void WriteConsistencyPrefabPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConfigWithNode(configPath, "ConsistencyPrefabPath", path);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeConsistencyPrefabPath = root.SelectSingleNode("ConsistencyPrefabPath");
                if (nodeConsistencyPrefabPath != null)
                {
                    nodeConsistencyPrefabPath.InnerText = path;
                }
                else
                {
                    nodeConsistencyPrefabPath = docment.CreateElement("ConsistencyPrefabPath");
                    nodeConsistencyPrefabPath.InnerText = path;
                    root.AppendChild(nodeConsistencyPrefabPath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeConsistencyPrefabPath = docment.CreateElement("ConsistencyPrefabPath");
                nodeConsistencyPrefabPath.InnerText = path;
                root.AppendChild(nodeConsistencyPrefabPath);
            }

            docment.Save(configPath);
        }

        consistencyPrefabPath = path;
    }

    public static string ReadConsistencyPrefabPath()
    {
        string path = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeConsistencyPrefabPath = root.SelectSingleNode("ConsistencyPrefabPath");
                if (nodeConsistencyPrefabPath != null)
                {
                    path = nodeConsistencyPrefabPath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(path))
        {
            ConsistencyPreafabPath = path;
        }

        return path;
    }

    public static void WriteReferencePrefabPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConfigWithNode(configPath, "ReferencePrefabPath", path);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeReferencePrefabPath = root.SelectSingleNode("ReferencePrefabPath");
                if (nodeReferencePrefabPath != null)
                {
                    nodeReferencePrefabPath.InnerText = path;
                }
                else
                {
                    nodeReferencePrefabPath = docment.CreateElement("ReferencePrefabPath");
                    nodeReferencePrefabPath.InnerText = path;
                    root.AppendChild(nodeReferencePrefabPath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeReferencePrefabPath = docment.CreateElement("ReferencePrefabPath");
                nodeReferencePrefabPath.InnerText = path;
                root.AppendChild(nodeReferencePrefabPath);
            }

            docment.Save(configPath);
        }

        referencePrefabPath = path;

    }

    public static string ReadReferencePrefabPath()
    {
        string path = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeReferencePrefabPath = root.SelectSingleNode("ReferencePrefabPath");
                if (nodeReferencePrefabPath != null)
                {
                    path = nodeReferencePrefabPath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(path))
        {
            ReferencePrefabPath = path;
        }

        return path;
    }

    public static void WriteReferenceScenePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConfigWithNode(configPath, "ReferenceScenePath", path);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeReferenceScenePath = root.SelectSingleNode("ReferenceScenePath");
                if (nodeReferenceScenePath != null)
                {
                    nodeReferenceScenePath.InnerText = path;
                }
                else
                {
                    nodeReferenceScenePath = docment.CreateElement("ReferenceScenePath");
                    nodeReferenceScenePath.InnerText = path;
                    root.AppendChild(nodeReferenceScenePath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeReferenceScenePath = docment.CreateElement("ReferenceScenePath");
                nodeReferenceScenePath.InnerText = path;
                root.AppendChild(nodeReferenceScenePath);
            }

            docment.Save(configPath);
        }

        referenceScenePath = path;

    }

    public static string ReadReferenceScenePath()
    {
        string path = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeReferenceScenePath = root.SelectSingleNode("ReferenceScenePath");
                if (nodeReferenceScenePath != null)
                {
                    path = nodeReferenceScenePath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(path))
        {
            ReferenceScenePath = path;
        }

        return path;
    }

    public static void WriteReferenceResultPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(configPath))
        {
            CreateXMLConfigWithNode(configPath, "ReferenceResultPath", path);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeReferenceResultPath = root.SelectSingleNode("ReferenceResultPath");
                if (nodeReferenceResultPath != null)
                {
                    nodeReferenceResultPath.InnerText = path;
                }
                else
                {
                    nodeReferenceResultPath = docment.CreateElement("ReferenceResultPath");
                    nodeReferenceResultPath.InnerText = path;
                    root.AppendChild(nodeReferenceResultPath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasConfig");
                docment.AppendChild(root);

                XmlNode nodeReferenceResultPath = docment.CreateElement("ReferenceResultPath");
                nodeReferenceResultPath.InnerText = path;
                root.AppendChild(nodeReferenceResultPath);
            }

            docment.Save(configPath);
        }

        ReferenceResultPath = path;

    }

    public static string ReadReferenceResultPath()
    {
        string path = string.Empty;

        if (File.Exists(configPath))
        {
            UniversalEditorUtility.MakeFileWriteable(configPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(configPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasConfig");
            if (root != null)
            {
                XmlNode nodeReferenceResultPath = root.SelectSingleNode("ReferenceResultPath");
                if (nodeReferenceResultPath != null)
                {
                    path = nodeReferenceResultPath.InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(path))
        {
            ReferenceResultPath = path;
        }

        return path;
    }

    public static void CreateXMLConifg(string path)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
    }

    public static void CreateXMLConfigWithNode(string path, string NodeType, string NodeValue)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        UniversalEditorUtility.MakeFileWriteable(configPath);

        XmlDocument docment = new XmlDocument();
        XmlElement root = docment.CreateElement("UIAtlasConfig");
        docment.AppendChild(root);

        XmlElement nodeImageBasePath = docment.CreateElement("ImageBasePath");
        root.AppendChild(nodeImageBasePath);

        XmlElement nodeProjectPath = docment.CreateElement("ProjectPath");
        root.AppendChild(nodeProjectPath);

        XmlElement nodeConsistencyResultPath = docment.CreateElement("ConsistencyResultPath");
        root.AppendChild(nodeConsistencyResultPath);

        XmlElement nodeConsistencyPrefabPath = docment.CreateElement("ConsistencyPrefabPath");
        root.AppendChild(nodeConsistencyPrefabPath);

        XmlElement nodeReferencePrefabPath = docment.CreateElement("ReferencePrefabPath");
        root.AppendChild(nodeReferencePrefabPath);

        XmlElement nodeReferenceScenePath = docment.CreateElement("ReferenceScenePath");
        root.AppendChild(nodeReferenceScenePath);

        XmlElement nodeReferenceResultPath = docment.CreateElement("ReferenceResultPath");
        root.AppendChild(nodeReferenceResultPath);

        switch(NodeType)
        {
            case "ImageBasePath":
                nodeImageBasePath.InnerText = NodeValue;
                break;

            case "ProjectPath":
                nodeProjectPath.InnerText = NodeValue;
                break;

            case "ConsistencyResultPath":
                nodeConsistencyResultPath.InnerText = NodeValue;
                break;

            case "ConsistencyPrefabPath":
                nodeConsistencyPrefabPath.InnerText = NodeValue;
                break;

            case "ReferencePrefabPath":
                nodeReferencePrefabPath.InnerText = NodeValue;
                break;

            case "ReferenceScenePath":
                nodeReferenceScenePath.InnerText = NodeValue;
                break;

            case "ReferenceResultPath":
                nodeReferenceResultPath.InnerText = NodeValue;
                break;

            default:
                break;
        }

        docment.Save(configPath);
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public enum UIATLAS_ERROR_TYPE
{//编辑器错误类型
    UIATLAS_ERROR_UNKNOWN = 0,                           //未知错误
    UIATLAS_ERROR_ADDSPRITE_WITHOUT_PROJECT,             //工程不存在时添加小图
   
    UIATLAS_ERROR_NONE = -1,                               //默认值
}

public class UIAtlasEditorModel 
{//Atlas编辑器工程实现

#region 工程相关处理函数
    public bool NewPorject(string projectName)
    {//创建工程

        bool bRet = true;

        //清空当前工程
        ClearCurrentProject();

        //创建工程对象
        if (null == m_Project)
        {
            m_Project = new AtlasProject();
            m_Project.Name = Path.GetFileNameWithoutExtension(projectName);
            m_Project.Path = projectName;
        }

        //执行新建工程回调
        if (onNewProject != null)
        {
            onNewProject();
        }

        //设定工程类型
        m_Project.ProjectType = PROJECT_TYPE.PROJECT_TYPE_NEW;

        return bRet;
     }

    public bool SaveProject(string projectName)
    {//保存工程

        bool bRet = false;

        if(m_Project != null)
        {
            //更新工程路径
            //m_Project.Path = projectName;
            //m_Project.Name = Path.GetFileNameWithoutExtension(projectName);

            //保存工程
            m_Project.Save();
            bRet = true;
        }

        return bRet;
    }

    public bool LoadProject(string projectName)
    {//打开工程
        bool bRet = true;

        do
        {
            //清空当前工程
            ClearCurrentProject();

            //创建新工程
            m_Project = new AtlasProject();

            //读取工程文件
            m_Project.Load(projectName);

            //依次载入工程中全部小图
            List<AtlasSpriteImage> sprites = m_Project.GetAllSprites();
            foreach (var sprite in sprites)
            {
                LoadSpriteImage(sprite.Path);
                if (onSpriteImageLoad != null)
                {
                    onSpriteImageLoad(sprite.Path);
                }
            }

            //设定工程类型
            m_Project.ProjectType = PROJECT_TYPE.PROJECT_TYPE_EXIST;

            WriteRecentOpenProjectPath(m_Project.Path);
        } while (false);


        return bRet;
    }

    public void ClearCurrentProject()
    {//清空当前工程
   
        if (m_Project != null)
        {
            //清除临时目录中的资源
            UIAtlasTempTextureManager.GetInstance().Clear(m_Project.Path);
  
            //清除工程中小图信息
            m_Project.ClearSpriteImage();
            m_Project = null;
        }

        if (onClearCurrentProject != null)
        {
            onClearCurrentProject();
        }
    }

    public string GetAtlasSavePath()
    {//获取Atlas输出路径

        string path = null;

        if (m_Project != null)
        {
            path = m_Project.AtlasSavePath;
        }

        return path;
    }

    public void SetAtlasSavePath(string path)
    {//设定Atlas输出路径

        if (m_Project != null)
        {
            m_Project.AtlasSavePath = path;
            m_Project.ImagePath = m_Project.AtlasSavePath + m_Project.Name + ".png";
            m_Project.DescribePath = m_Project.AtlasSavePath + m_Project.Name + ".prefab";
        }
    }

    public bool AddSpriteImage(string path)
    {//添加小图

        Texture2D tex = null;
        bool bRet = false;
        tex = null;

        if (m_Project == null)
        {
            m_UIAtlasErrorType = UIATLAS_ERROR_TYPE.UIATLAS_ERROR_ADDSPRITE_WITHOUT_PROJECT;       
            bRet = false;
        }
        else
        {
            //向工程中添加小图
            if (m_Project.AddSpriteImage(path))
            {
                //载入小图资源
                tex = LoadSpriteImage(path);
                if (null == tex)
                {
                    bRet = false;
                }
                else
                {
                    bRet = true;
                }
            }
            else
            {
                bRet = false;
            }
        }

        if (onAddSpriteImageCommand != null)
        {
            onAddSpriteImageCommand(bRet, path);
        }

        return bRet;
    }

    public bool RemoveSpriteImage(string path)
    {//删除小图

        bool bRet = false;

        if (m_Project == null)
        {
            m_Project.ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE_PROJECT;
            bRet = false;
        }
        else
        {
            //卸载小图资源
            if (UnloadSpriteImage(path))
            {
                //从工程中移除小图
                bRet = m_Project.RemoveSpriteImage(path);
            }
            else
            {
                bRet = false;
            }
        }

        if (onDeleteSpriteImageCommand != null)
        {
            onDeleteSpriteImageCommand(bRet, path);
        }

        return bRet;
    }

    public Texture2D LoadSpriteImage(string path)
    {//载入小图资源
        if(null == m_Project)
        {
            return null;
        }


        AtlasSpriteImage spriteImage = null;
        m_Project.GetSpriteImage(path, out spriteImage);
        if (null == spriteImage)
        {
            return null;
        }

        Texture2D retTex = UIAtlasTempTextureManager.GetInstance().LoadTexture(path, m_Project.Path, spriteImage.ZoomScale);
        return retTex;
    }

    public bool UnloadSpriteImage(string path)
    {//卸载小图资源

        bool bRet = false;

        bRet = UIAtlasTempTextureManager.GetInstance().UnloadTexture(path, m_Project.Path);

        return bRet;
    }

    public Texture2D GetSpriteImageTexture(string path)
    {
        Texture2D retTex = UIAtlasTempTextureManager.GetInstance().GetSpriteZoomTexture(path, m_Project.Path);
        return retTex;
    }

    public bool GetSpriteImage(string path, out AtlasSpriteImage spriteImage)
    {//获取指定小图

        bool bRet = false;

        spriteImage = null;

        if (m_Project != null)
        {
            bRet = m_Project.GetSpriteImage(path, out spriteImage);
        }

        return bRet;
    }

    public Texture2D GetSpriteTexture(string path)
    {//取得小图纹理

        Texture2D retTex = UIAtlasTempTextureManager.GetInstance().GetSpriteTexture(path, m_Project.Path);
        return retTex;
    }

    public Texture2D GetSpriteZoomTexture(string path)
    {//取得小图纹理

        Texture2D retTex = UIAtlasTempTextureManager.GetInstance().GetSpriteZoomTexture(path, m_Project.Path);
        return retTex;
    }

    public void ClearSpriteImage()
    {//清空所有小图

        if (m_Project != null)
        {
            m_Project.ClearSpriteImage();
        }
    }

    public void UpdateSprite()
    {//更新全部小图资源

        UIAtlasTempTextureManager.GetInstance().Update();
    }

    public bool IsProjectExist()
    {//判断是否存在工程

        bool bRet = true;

        if (m_Project == null)
        {
            bRet = false;
        }

        return bRet;
    }

    public string GetProjectName()
    {//获取工程名

        string name = null;

        if (m_Project != null)
        {
            name = m_Project.Name;
        }

        return name;
    }

    public string GetProjectPath()
    {//获取工程路径

        string path = null;

        if (m_Project != null)
        {
            path = m_Project.Path;
        }

        return path;
    }

    public PROJECT_ERROR_TYPE GetProjectFailedType()
    {//获取工程失败类型

        PROJECT_ERROR_TYPE type = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (m_Project != null)
        {
            type = m_Project.ProjectFailedType;
        }

        return type;
    }


    public PROJECT_TYPE GetProjectType()
    {//获取工程类型

        PROJECT_TYPE type = PROJECT_TYPE.PROJECT_TYPE_NEW;

        if (m_Project != null)
        {
            type = m_Project.ProjectType;
        }

        return type;
    }

    public void SetProjectType(PROJECT_TYPE type)
    {//设定工程类型

        if (m_Project != null)
        {
            m_Project.ProjectType = type;
        }
    }

    public void WriteImagePathConfig(string path)
    {//写配置文件

        UIAtlasEditorConfig.WriteImageBasePath(path);
    }

    public string ReadImagePathConfig()
    {//读配置文件

        return UIAtlasEditorConfig.ReadImageBasePath();
    }

    public void WriteRecentOpenProjectPath(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            return;
        }

        if (!File.Exists(recentOpenProjectConfigPath))
        {
            if (!Directory.Exists(Path.GetDirectoryName(recentOpenProjectConfigPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(recentOpenProjectConfigPath));
            }

            UniversalEditorUtility.MakeFileWriteable(recentOpenProjectConfigPath);

            XmlDocument docment = new XmlDocument();
            XmlElement root = docment.CreateElement("UIAtlasMaker");
            docment.AppendChild(root);

            XmlElement nodeRecentOpenProjectPath = docment.CreateElement("RecentOpenProject");
            nodeRecentOpenProjectPath.InnerText = path;
            root.AppendChild(nodeRecentOpenProjectPath);


            docment.Save(recentOpenProjectConfigPath);
        }
        else
        {
            XmlDocument docment = new XmlDocument();
            docment.Load(recentOpenProjectConfigPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasMaker");
            if (root != null)
            {
                XmlNode nodeRecentOpenProjectPath = root.SelectSingleNode("RecentOpenProject");
                if (nodeRecentOpenProjectPath != null)
                {
                    nodeRecentOpenProjectPath.InnerText = path;
                }
                else
                {
                    nodeRecentOpenProjectPath = docment.CreateElement("RecentOpenProject");
                    nodeRecentOpenProjectPath.InnerText = path;
                    root.AppendChild(nodeRecentOpenProjectPath);
                }
            }
            else
            {
                root = docment.CreateElement("UIAtlasMaker");
                docment.AppendChild(root);

                XmlNode nodeRecentOpenProjectPath = docment.CreateElement("RecentOpenProject");
                nodeRecentOpenProjectPath.InnerText = path;
                root.AppendChild(nodeRecentOpenProjectPath);
            }
            docment.Save(recentOpenProjectConfigPath);
        }
    }

    public string ReadRecentOpenProjectPath()
    {
        string projectPath = string.Empty;

        if (File.Exists(recentOpenProjectConfigPath))
        {
            UniversalEditorUtility.MakeFileWriteable(recentOpenProjectConfigPath);
            XmlDocument docment = new XmlDocument();
            docment.Load(recentOpenProjectConfigPath);
            XmlNode root = docment.SelectSingleNode("UIAtlasMaker");
            if (root != null)
            {
                XmlNode nodeRecentOpenProjectPath = root.SelectSingleNode("RecentOpenProject");
                if (nodeRecentOpenProjectPath != null)
                {
                    projectPath = nodeRecentOpenProjectPath.InnerText;
                }
            }
        }

        return projectPath;
    }
    public static void OnBasePathChange(string newBasePaht)
    {//图库路径变更处理函数

        UIAtlasEditorConfig.ReadImageBasePath();
    }
#endregion

#region Sprite操作函数
    public void ZoomSpriteImage(string path, float scaleFactor)
    {//小图缩放比例变更

        if (m_Project == null)
        {
            return;
        }
        AtlasItemInfo atlasItem = new AtlasItemInfo();
        atlasItem.Project = m_Project;

        //UIAtlasTempTextureManager.GetInstance().ZoomTexture(path, scaleFactor);
        UIAtlasOperateUtility.GetInstance().ZoomSpriteFromProject(path, scaleFactor, ref atlasItem);
        m_Project.SetSpriteImageZoom(path, scaleFactor);
        if (onSpriteZoomChangedCommand != null)
        {
            onSpriteZoomChangedCommand(path);
        }
    }
   
    public bool PreViewAtlas(out Texture2D outTex)
    {//预览Atlas
        bool bRet = false;
        outTex = null;

        if(m_Project == null)
        {
            return false;
        }

        if (m_Project.GetAllSprites().Count != 0)
        {//存在纹理资源

            //预览
            UIAtlasOperateUtility.GetInstance().MakeCurrentAtlasTexture(m_Project, out outTex);
            bRet = true;
        }
        else
        {//不存在

            //设定失败类型为小图不存在
            m_Project.ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE_IMAGE;
            bRet = false;
        }

        return bRet;
    }

    public bool MakeAtlas()
    {//生成Atlas

        bool bRet = true;

        //Texture2D atlasTexture = null;

        if (m_Project == null)
        {
            m_Project.ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE_PROJECT;
            bRet = false;
        }
        else
        {
            if ((m_Project.AtlasSavePath != null) && (m_Project.AtlasSavePath.Contains("Assets/")))
            {//Atlas输出路径合法

                //string outputPath = m_Project.AtlasSavePath + m_Project.Name + ".prefab";
                AtlasItemInfo atlasItem = new AtlasItemInfo();
                atlasItem.Project = m_Project;
                ////生成Atalas
                //UIAtlasOperateUtility.GetInstance().MakeCurrentAtlasTexture(m_Project, out atlasTexture);
                //atlasItem.AtlasTextureBak = atlasItem.AtlasTexture = atlasTexture;
                //m_Project.MakeAtlasTexture(tex, textures);
                //生成prefab

                UIAtlasOperateUtility.GetInstance().MakeCurrentAtlasPrefab(atlasItem);

                UIAtlasOperateUtility.GetInstance().MakeCurrentAtlasPng(atlasItem);

                //m_Project.MakeAtlasPrefab(outputPath);
                bRet = true;
            }
            else
            {//Atlas输出路径不合法
                bRet = false;
                m_Project.ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_ATLASOUTPU_PATH;
            }
        }


        if(onMakeAtlasCommand != null)
        {
            onMakeAtlasCommand(bRet);
        }

        return bRet;
    }

    public void DeleteAtlas()
    {//删除Atlas

        string atlasName = null;
        string prefabName = null;

        if ((m_Project == null) 
            || (m_Project.AtlasSavePath == null)
            || (m_Project.Name == null))
        {
            return; 
        }

        atlasName = m_Project.AtlasSavePath + m_Project.Name + ".png";
        prefabName = m_Project.AtlasSavePath + m_Project.Name + ".prefab";

        //删除Prefab文件
        if (File.Exists(prefabName))
        {
            File.Delete(prefabName);
        }

        //删除atlas文件
        if (File.Exists(atlasName))
        {
            File.Delete(atlasName);
        }
    }
#endregion

#region 成员变量

    public delegate void ModelChangeNotify();
    public delegate void SpriteImageLoadNotify(string spritePath);
    public delegate void ClearCurrentProjectNotify();

    public delegate void SpriteZoomChangedCommand(string spritePath);
    public delegate void AddSpriteImageCommand(bool bResult, string spriteName);
    public delegate void DeleteSpriteImageCommand(bool bResult, string spriteName);
    public delegate void MakeAtlasCommand(bool bResult);

    public ModelChangeNotify onNewProject;
    public SpriteImageLoadNotify onSpriteImageLoad;
    public ClearCurrentProjectNotify onClearCurrentProject;

    public SpriteZoomChangedCommand onSpriteZoomChangedCommand;
    public AddSpriteImageCommand onAddSpriteImageCommand;
    public DeleteSpriteImageCommand onDeleteSpriteImageCommand;
    public MakeAtlasCommand onMakeAtlasCommand;

    public UIATLAS_ERROR_TYPE UIAtlasErrorType
    {
        get { return m_UIAtlasErrorType; }
    }

    private static string recentOpenProjectConfigPath = "Assets/H3DTech/Editor/UniversalEditor/UIAtlasEditor/UIMakeAtlas/RecentOpenProject/RecentOpenProject.xml";
    private AtlasProject m_Project = null;
    private UIATLAS_ERROR_TYPE m_UIAtlasErrorType = UIATLAS_ERROR_TYPE.UIATLAS_ERROR_NONE;

    static private UIAtlasEditorModel m_Instance = null;
    public static UIAtlasEditorModel GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new UIAtlasEditorModel();
        }
        return m_Instance;
    }

    public void DestoryInstance()
    {
        ClearCurrentProject();
        if (m_Instance != null)
        {
            m_Instance = null;
        }
    }
#endregion
}

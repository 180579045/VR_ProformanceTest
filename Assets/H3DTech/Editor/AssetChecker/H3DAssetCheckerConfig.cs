using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;
using System.Text;

public class H3DAssetCheckerConfig 
{

    public bool isAssetCheckerOff = false;

    bool isInit = false;

    const string configDir = "Assets/H3DTechConfigLocal/AssetsChecker/";
    const string privateConfigPath = configDir + "private.xml";
    const string projectConfigPath = configDir + "project.xml";

    public void Update()
    {
        //配置文件只更新一次,此限制是若配置更改需重启Unity
        if (isInit)
            return; 
         
        TouchConfigPath();
          
        if( File.Exists(privateConfigPath) )
        {
            LoadFromConfig(privateConfigPath);
        }
        else if( File.Exists(projectConfigPath) )
        {
            LoadFromConfig(projectConfigPath);
        }

        isInit = true;
    }

    void TouchConfigPath()
    {
        //创建
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
    }


    public void LoadFromConfig( string path )
    {
         XmlDocument xmlCfg = new XmlDocument(); 
         try
         { 
             xmlCfg.Load(path); 
         }catch(Exception e)
         {
             Debug.Log(e.Message); 
             return;
         }

         XmlNode root = xmlCfg.SelectSingleNode("AssetCheckerConfig");
         if (root == null)
             return;

         XmlNode assetCheckerOffNode = root.SelectSingleNode("IsAssetCheckerOff");
         if (assetCheckerOffNode == null)
             return;
         
         bool.TryParse(assetCheckerOffNode.InnerText, out isAssetCheckerOff);
    }

    public void SaveConfig( string path )
    {
        XmlDocument xmlCfg = new XmlDocument();
        var root = xmlCfg.CreateElement("AssetCheckerConfig");
        xmlCfg.AppendChild(root);

        var assetCheckerOffNode = xmlCfg.CreateElement("IsAssetCheckerOff");
        assetCheckerOffNode.InnerText = isAssetCheckerOff.ToString();
        root.AppendChild(assetCheckerOffNode);

        xmlCfg.Save(path); 
    }

    public void GeneratePrivateConfig()
    {
        TouchConfigPath();
        SaveConfig(privateConfigPath);
    }

    public void GenerateProjectConfig()
    {
        TouchConfigPath();
        SaveConfig(projectConfigPath);
    }
     
    public static H3DAssetCheckerConfig GetInstance()
    {
        if( _instance == null )
        {
            _instance = new H3DAssetCheckerConfig();
        }

        _instance.Update();
        return _instance;
    }

    static H3DAssetCheckerConfig _instance = null;
        
}

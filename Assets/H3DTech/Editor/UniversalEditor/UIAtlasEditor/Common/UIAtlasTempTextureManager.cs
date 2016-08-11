using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using System.Text;
using System.Globalization;
public class TempTextureInfo
{//Temp文件夹中资源信息

    private Texture2D m_texture = null;         //纹理
    private Texture2D m_ZoomTexture = null;     //缩放纹理
    private string m_sourcePath = null;         //源文件绝对路径
    private string m_tempPath = null;           //Temp文件路径
    private string m_tempPathZoom = null;
    private float m_zoomScale = 1f;

    public Texture2D Texture { get { return m_texture; } set { m_texture = value; } }
    public Texture2D ZoomTexture { get { return m_ZoomTexture; } set { m_ZoomTexture = value; } }
    public string SourcePath { get { return m_sourcePath; } set { m_sourcePath = value; } }
    public string TempPath { get { return m_tempPath; } set { m_tempPath = value; } }
    public string TempPathZoom { get { return m_tempPathZoom; } set { m_tempPathZoom = value; } }
    public float ZoomScale { get { return m_zoomScale; } set { m_zoomScale = value; } }

}

public class TempAtlasTextureInfo
{
    private string m_tempAtlasDir = string.Empty;
    private Dictionary<string, TempTextureInfo> m_spriteTextureInfo = new Dictionary<string, TempTextureInfo>();


    public string TempAtlasDir { get { return m_tempAtlasDir; } set { m_tempAtlasDir = value; } }
    public Dictionary<string, TempTextureInfo> SpriteTextureInfo
    {
        get { return m_spriteTextureInfo; }
        set { m_spriteTextureInfo = value; }
    }
}

public class UIAtlasTempTextureManager
{//Temp文件夹资源

    private string m_zoomStr = "zoomed";
    public UIAtlasTempTextureManager() { }

    private string GetAtlasTempDir(string atlasPath)
    {
        string tempAtlasDir = string.Empty;
        TempAtlasTextureInfo tempAtlasTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                tempAtlasDir = tempAtlasTexture.TempAtlasDir;
            }
        }

        return tempAtlasDir;
    }

    private string CreateAtlasTempDir(string atlasPath)
    {
        if(string.IsNullOrEmpty(atlasPath))
        {
            return null;
        }
        string tempAtlasDir = string.Empty;
        //string tempAbsAtlasDir = string.Empty;

        tempAtlasDir = GetAtlasTempDir(atlasPath);
        if (string.IsNullOrEmpty(tempAtlasDir))
        {
            TempAtlasTextureInfo tempAtlasTexture = new TempAtlasTextureInfo();
            string atlasName = Path.GetFileNameWithoutExtension(atlasPath);
            string strGUID = Guid.NewGuid().ToString();

            //tempAbsAtlasDir = UIAtlasEditorConfig.AbsTempPath + atlasName + "_" + strGUID + "/";
            tempAtlasDir = UIAtlasEditorConfig.TempPath + atlasName + "_" + strGUID + "/";

            tempAtlasTexture.TempAtlasDir = tempAtlasDir;
            m_AllTextureCache.Add(atlasPath, tempAtlasTexture);
        }

        if (!Directory.Exists(tempAtlasDir))
        {
            Directory.CreateDirectory(tempAtlasDir);
        }

        return tempAtlasDir;
    }

    //private void _TouchTempDir(string atlasPath)
    //{
    //    //查看缓存文件夹是否存在
    //    if (!Directory.Exists(atlasPath))
    //    {
    //        Directory.CreateDirectory(atlasPath);
    //    }
    //}

    private void DeleteUnuseAsset(string spritePath, string atlasPath)
    {
        if(string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return;
        }

        bool isNeedRename = false;

        string fileName = Path.GetFileName(spritePath);
        string extension = Path.GetExtension(spritePath);

        TempAtlasTextureInfo tempAtlasTexture = null;

        if(m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                if(tempAtlasTexture.SpriteTextureInfo.ContainsKey(spritePath))
                {
                    tempAtlasTexture.SpriteTextureInfo.Remove(spritePath);
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
                    EditorUtility.UnloadUnusedAssets();
#else
                    EditorUtility.UnloadUnusedAssetsImmediate();
#endif
                }
            }

            if (IsSpriteInTempFloder(spritePath, atlasPath, out isNeedRename))
            {
                AssetDatabase.DeleteAsset(tempAtlasTexture.TempAtlasDir + fileName);
                AssetDatabase.DeleteAsset(tempAtlasTexture.TempAtlasDir + m_zoomStr + extension);
            }
        }
    }

    private bool IsSpriteInTempFloder(string spritePath, string atlasPath, out bool isNeedRename)
    {
        isNeedRename = false;

        if (string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return false;
        }

        bool bRet = false;
        string fileName = Path.GetFileName(spritePath);
        TempAtlasTextureInfo tempAtlasTexture = null;

        //该Sprite对应Atlas文件夹不存在
        if (!m_AllTextureCache.ContainsKey(atlasPath))
        {
            isNeedRename = false;
            return false;
        }

        if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
        {
            if (File.Exists(tempAtlasTexture.TempAtlasDir + fileName))
            {
                isNeedRename = true;
                bRet = false;
                foreach(var spriteTexture in tempAtlasTexture.SpriteTextureInfo)
                {
                    if (spriteTexture.Key == spritePath)
                    {
                        bRet = true;
                        isNeedRename = false;
                        break;
                    }
                }
            }
            else
            {
                isNeedRename = false;
                bRet = false;
            }
        }

        return bRet;
    }

    private bool IsSouceSpriteFileNoChange(string spriteSourcePath, string spriteTempPath)
    {
        if (string.IsNullOrEmpty(spriteSourcePath) || string.IsNullOrEmpty(spriteTempPath))
        {
            return false;
        }

        string fileName = Path.GetFileName(spriteSourcePath);

        DateTime orginFileWriteTime = File.GetLastWriteTime(spriteSourcePath);
        DateTime assetFileWriteTime = File.GetLastWriteTime(spriteTempPath + fileName);

        return orginFileWriteTime.Equals(assetFileWriteTime);
    }

    private bool IsSpriteInfoInCache(string spritePath, string atlasPath, out TempTextureInfo spriteInfo)
    {
        spriteInfo = null;

        if (string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return false;
        }

        bool bRet = false;
        TempAtlasTextureInfo tempAtlasTexture = null;
        TempTextureInfo tempSpriteTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if(m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                if (tempAtlasTexture.SpriteTextureInfo.ContainsKey(spritePath))
                {
                    if(tempAtlasTexture.SpriteTextureInfo.TryGetValue(spritePath, out tempSpriteTexture))
                    {
                        spriteInfo = tempSpriteTexture;
                    }

                    bRet = true;
                }
            }
        }

        return bRet;
    }

    private void AddSpriteInCache(string spritePath, string atlasPath, TempTextureInfo spriteInfo)
    {
        if (string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return;
        }

        TempAtlasTextureInfo tempAtlasTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                if (!tempAtlasTexture.SpriteTextureInfo.ContainsKey(spritePath))
                {
                    tempAtlasTexture.SpriteTextureInfo.Add(spritePath, spriteInfo);
                }
            }
        }
    }

    public Texture2D LoadTexture(string spritePath, string atlasPath, float scaleFactor)
    {
        if (
            string.IsNullOrEmpty(spritePath) 
            || string.IsNullOrEmpty(atlasPath)
            || ((-0.000001f < scaleFactor) && (scaleFactor < 0.000001f))
            )
        {
            return null;
        }

        _TouchTempDir();

        string tempAtlasDir = CreateAtlasTempDir(atlasPath);
        if (string.IsNullOrEmpty(tempAtlasDir))
        {
            return null;
        }

        TempTextureInfo spriteInfo = null;

        string fileName = Path.GetFileName(spritePath);
        string extension = Path.GetExtension(spritePath);
        string fileNameWithoutext = Path.GetFileNameWithoutExtension(spritePath);
        string zoomedName = string.Empty;

        bool isNeedRename = false;
        bool needCopy = false;

        if (!IsEnableTextureFile(fileName))
        {
            return null;
        }

        if (!File.Exists(spritePath))
        {
            DeleteUnuseAsset(spritePath, atlasPath);
            return null;
        }

        if (IsSpriteInTempFloder(spritePath, atlasPath, out isNeedRename))
        {
            if (!IsSouceSpriteFileNoChange(spritePath, atlasPath))
            {
                needCopy = true;
            }
        }
        else
        {
            needCopy = true;
        }

        if(needCopy)
        {
            if (isNeedRename)
            {
                fileName = fileNameWithoutext + "副本" + extension;
                zoomedName = fileNameWithoutext + "副本" + m_zoomStr + extension;
            }
            else
            {
                zoomedName = fileNameWithoutext + m_zoomStr + extension;
            }

            string absTempAtlasDir = EditorHelper.GetProjectPath() + tempAtlasDir;

            UniversalEditorUtility.MakeFileWriteable(absTempAtlasDir + fileName);
            File.Copy(spritePath, absTempAtlasDir + fileName, true);

            UniversalEditorUtility.MakeFileWriteable(absTempAtlasDir + zoomedName);
            File.Copy(spritePath, absTempAtlasDir + zoomedName, true);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }


        //TempTextureInfo retTexInfo = null;
        if (!IsSpriteInfoInCache(spritePath, atlasPath, out spriteInfo))
        {//还未载入内存
            AssetDatabase.ImportAsset(tempAtlasDir + fileName);
            MakeTextureReadable(tempAtlasDir + fileName, false);
            TempTextureInfo newTexInfo = new TempTextureInfo();
            newTexInfo.SourcePath = spritePath;
            newTexInfo.TempPath = tempAtlasDir + fileName;
            newTexInfo.TempPathZoom = tempAtlasDir + zoomedName;
            newTexInfo.Texture = AssetDatabase.LoadAssetAtPath(newTexInfo.TempPath, typeof(Texture)) as Texture2D;
            newTexInfo.ZoomScale = scaleFactor;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            if (newTexInfo.Texture != null)
            {

                newTexInfo.ZoomTexture = newTexInfo.Texture;
                AddSpriteInCache(spritePath, atlasPath, newTexInfo);
         
                if (scaleFactor != 1.0f)
                {
                    ZoomTexture(spritePath, atlasPath, scaleFactor);
                }
            }

        }


        return GetSpriteTexture(spritePath, atlasPath); 
    }
    //public Texture2D LoadTexture( string path )
    //{//载入纹理（支持载入磁盘任意位置的纹理）

    //    _TouchTempDir();

    //    string fileName = Path.GetFileName(path);
    //    string extension = Path.GetExtension(path);
    //    string fileNameWithoutext = Path.GetFileNameWithoutExtension(path);

    //    string zoomedName = null;

    //    bool isNeedRename = false;
    //    TempTextureInfo retTexInfo = null;

    //    //是纹理文件
    //    if (!IsEnableTextureFile(fileName))
    //    {
    //        return null;
    //    }

    //    //文件必需存在
    //    if (!File.Exists(path))
    //    {
    //        if( textureCache.ContainsKey(path))
    //        {
    //            textureCache.Remove(path);
    //            EditorUtility.UnloadUnusedAssets();
    //        }

    //        if (_IsTextureAssetAlreadyExistsInTempFolder(fileName, out isNeedRename))
    //        {
    //            AssetDatabase.DeleteAsset(UIAtlasEditorConfig.TempPath + fileName);
    //            AssetDatabase.DeleteAsset(UIAtlasEditorConfig.TempPath + m_zoomStr + extension);
    //        }
    //        return null;
    //    }
        
 
    //    bool needCopy = false;
    //    if (_IsTextureAssetAlreadyExistsInTempFolder(path, out isNeedRename))
    //    {//纹理资源已经存在于缓存文件夹中
    //        if(!_IsTextureAssetSameModTime(path))
    //        {
    //            needCopy = true;
    //        }
    //    }
    //    else
    //    {
    //        needCopy = true;
    //    }

    //    if (needCopy)
    //    {
    //        if (isNeedRename)
    //        {
    //            fileName = fileNameWithoutext + "副本" + extension;
    //            zoomedName = fileNameWithoutext + "副本" + m_zoomStr + extension;
    //        }
    //        else
    //        {
    //            zoomedName = fileNameWithoutext + m_zoomStr + extension;
    //        }

    //        UniversalEditorUtility.MakeFileWriteable(UIAtlasEditorConfig.AbsTempPath + fileName);
    //        File.Copy(path, UIAtlasEditorConfig.AbsTempPath + fileName, true);

    //        UniversalEditorUtility.MakeFileWriteable(UIAtlasEditorConfig.AbsTempPath + zoomedName);       
    //        File.Copy(path, UIAtlasEditorConfig.AbsTempPath + zoomedName, true);

    //        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    //    }

    //    if (!textureCache.ContainsKey(path))
    //    {//还未载入内存
    //        AssetDatabase.ImportAsset(UIAtlasEditorConfig.TempPath + fileName);
    //        MakeTextureReadable(UIAtlasEditorConfig.TempPath + fileName, false);
         
    //        TempTextureInfo newTexInfo = new TempTextureInfo();
    //        newTexInfo.SourcePath = path;
    //        newTexInfo.TempPath = UIAtlasEditorConfig.TempPath + fileName;
    //        newTexInfo.TempPathZoom = UIAtlasEditorConfig.TempPath + zoomedName;
    //        newTexInfo.Texture = AssetDatabase.LoadAssetAtPath(newTexInfo.TempPath, typeof(Texture)) as Texture2D;

    //        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

    //        if (newTexInfo.Texture != null)
    //        {
    //            newTexInfo.ZoomTexture = newTexInfo.Texture;
    //            textureCache.Add(path, newTexInfo);
    //        }
            
    //    }

    //    if (textureCache.ContainsKey(path))
    //    {
    //        textureCache.TryGetValue(path, out retTexInfo);
    //    }

    //    return retTexInfo.ZoomTexture;
    //}

    public bool UnloadTexture(string spritePath , string atlasPath)
    {
        bool bRet = true;

        if (string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return false;
        }

        TempAtlasTextureInfo tempAtlasTexture = null;
        TempTextureInfo tempSpriteTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                if (tempAtlasTexture.SpriteTextureInfo.ContainsKey(spritePath))
                {
                    if (tempAtlasTexture.SpriteTextureInfo.TryGetValue(spritePath, out tempSpriteTexture))
                    {
                        AssetDatabase.DeleteAsset(tempSpriteTexture.TempPath);
                        AssetDatabase.DeleteAsset(tempSpriteTexture.TempPathZoom);

                        tempAtlasTexture.SpriteTextureInfo.Remove(spritePath);
                    }
                }
            }
        }

        return bRet;
    }

    //public bool UnloadTexture(string path)
    //{//卸载纹理
    //    bool bRet = true;

    //    if(path == null)
    //    {
    //        return false;
    //    }

    //    foreach (var textureInfo in textureCache)
    //    {
    //        if (textureInfo.Key == path)
    //        {
    //            AssetDatabase.DeleteAsset(textureInfo.Value.TempPath);
    //            AssetDatabase.DeleteAsset(textureInfo.Value.TempPathZoom);
               
    //            textureCache.Remove(path);
    //            bRet = true;
    //            break;
    //        }
    //    }

    //    return bRet;
    //}

    public Texture2D ZoomTexture(string spritePath , string atlasPath, float scaleFactor)
    {//缩放纹理

        Texture2D tex = null;
        TempTextureInfo spriteInfo = null;

        if ((string.IsNullOrEmpty(spritePath)) || (string.IsNullOrEmpty(atlasPath)) ||
            ((-0.000001f < scaleFactor) && (scaleFactor < 0.000001f)))
        {
            return null;
        }

        if (IsSpriteInfoInCache(spritePath, atlasPath, out spriteInfo))
        {
            spriteInfo.ZoomTexture = ScaleTextureBilinear(spriteInfo.Texture, scaleFactor);
            spriteInfo.ZoomScale = scaleFactor;

            byte[] bytes = spriteInfo.ZoomTexture.EncodeToPNG();
            string newPath = Path.GetFileNameWithoutExtension(spriteInfo.TempPath);
            string extension = Path.GetExtension(spriteInfo.TempPath);
            newPath = GetAtlasTempDir(atlasPath) + newPath + m_zoomStr + extension;

            UniversalEditorUtility.MakeFileWriteable(newPath);
            System.IO.File.WriteAllBytes(newPath, bytes);

            bytes = null;

            AssetDatabase.ImportAsset(newPath);
            MakeTextureReadable(newPath, false);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            spriteInfo.ZoomTexture = AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture)) as Texture2D;

            tex = spriteInfo.ZoomTexture;
        }

        return tex;
    }

    //public Texture2D ZoomTexture(string path, float scaleFactor)
    //{//缩放纹理

    //    Texture2D tex = null;

    //    if ((path == null) ||
    //        ((-0.000001f < scaleFactor) && (scaleFactor < 0.000001f)))
    //    {
    //        return null;
    //    }

    //    if (textureCache.ContainsKey(path))
    //    {
    //        TempTextureInfo sourceTexInfo = null;

    //        textureCache.TryGetValue(path, out sourceTexInfo);

    //        sourceTexInfo.ZoomTexture = ScaleTextureBilinear(sourceTexInfo.Texture, scaleFactor);

    //        byte[] bytes = sourceTexInfo.ZoomTexture.EncodeToPNG();
    //        string newPath = Path.GetFileNameWithoutExtension(sourceTexInfo.TempPath);
    //        string extension = Path.GetExtension(sourceTexInfo.TempPath);
    //        newPath = UIAtlasEditorConfig.TempPath + newPath + m_zoomStr + extension;

    //        UniversalEditorUtility.MakeFileWriteable(newPath);
    //        System.IO.File.WriteAllBytes(newPath, bytes);
      
    //        bytes = null;

    //        AssetDatabase.ImportAsset(newPath);
    //        MakeTextureReadable(newPath, false);
    //        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

    //        sourceTexInfo.ZoomTexture = AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture)) as Texture2D;

    //        tex = sourceTexInfo.ZoomTexture;
    //    }
    //    return tex;
    //}

    public Texture2D GetSpriteTexture(string spritePath , string atlasPath)
    {//获取原始纹理

        Texture2D tex = null;

        if (string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return null;
        }

        TempAtlasTextureInfo tempAtlasTexture = null;
        TempTextureInfo tempSpriteTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                if (tempAtlasTexture.SpriteTextureInfo.ContainsKey(spritePath))
                {
                    if (tempAtlasTexture.SpriteTextureInfo.TryGetValue(spritePath, out tempSpriteTexture))
                    {
                        tex = tempSpriteTexture.Texture;
                    }
                }
            }
        }

        return tex;
    }

    //public Texture2D GetSpriteTexture(string path)
    //{//获取原始纹理

    //    Texture2D tex = null;
    //    if (path == null)
    //    {
    //        return null;
    //    }

    //    foreach (var textureInfo in textureCache)
    //    {
    //        if (textureInfo.Key == path)
    //        {
    //            tex = textureInfo.Value.Texture;
    //            break;
    //        }
    //    }

    //    return tex;
    //}

    public Texture2D GetSpriteZoomTexture(string spritePath , string atlasPath)
    {
        Texture2D tex = null;

        if (string.IsNullOrEmpty(spritePath) || string.IsNullOrEmpty(atlasPath))
        {
            return null;
        }

        TempAtlasTextureInfo tempAtlasTexture = null;
        TempTextureInfo tempSpriteTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                if (tempAtlasTexture.SpriteTextureInfo.ContainsKey(spritePath))
                {
                    if (tempAtlasTexture.SpriteTextureInfo.TryGetValue(spritePath, out tempSpriteTexture))
                    {
                        tex = tempSpriteTexture.ZoomTexture;
                    }
                }
            }
        }

        return tex;
    }

    //public Texture2D GetSpriteZoomTexture(string path)
    //{//获取缩放纹理

    //    Texture2D tex = null;
    //    if(path == null)
    //    {
    //        return null;
    //    }

    //    foreach (var textureInfo in textureCache)
    //    {
    //        if (textureInfo.Key == path)
    //        {
    //            tex = textureInfo.Value.ZoomTexture;
    //            break;
    //        }
    //    }

    //    return tex;
    //}

    public List<Texture2D> GetAllSpriteZoomTexture2D(string atlasPath)
    {//获取Atlas的全部Sprite压缩纹理(2D)

        if (string.IsNullOrEmpty(atlasPath))
        {
            return null;
        }

        List<Texture2D> textureList = new List<Texture2D>();
        TempAtlasTextureInfo tempAtlasTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                foreach(KeyValuePair<string, TempTextureInfo> spriteInfo in tempAtlasTexture.SpriteTextureInfo)
                {
                    textureList.Add(spriteInfo.Value.ZoomTexture);
                }
            }
        }

        return textureList;
    }

    public List<Texture> GetAllSpriteZoomTexture(string atlasPath)
    {//获取Atlas的全部Sprite压缩纹理

        if (string.IsNullOrEmpty(atlasPath))
        {
            return null;
        }

        List<Texture> textureList = new List<Texture>();
        TempAtlasTextureInfo tempAtlasTexture = null;

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                foreach (KeyValuePair<string, TempTextureInfo> spriteInfo in tempAtlasTexture.SpriteTextureInfo)
                {
                    Texture tex = spriteInfo.Value.ZoomTexture as Texture;
                    textureList.Add(tex);
                }
            }
        }

        return textureList;
    }

    public List<Texture2D> GetTextureCache()
    {//获取全部纹理资源

        List<Texture2D> textureList = new List<Texture2D>();

        foreach (KeyValuePair<string, TempTextureInfo> texture in textureCache)
        {
            textureList.Add(texture.Value.ZoomTexture);
        }

        return textureList;
    }
    public List<Texture> GetTextureCacheSprite()
    {//获取全部纹理资源

        List<Texture> textureList = new List<Texture>();

        foreach (KeyValuePair<string, TempTextureInfo> texture in textureCache)
        {
            Texture tex = texture.Value.ZoomTexture as Texture;
            textureList.Add(tex);
        }

        return textureList;
    }

    //public void Update()
    //{//更新全部纹理

    //    foreach( var texInfo in textureCache )
    //    {
    //        LoadTexture(texInfo.Key);
    //    }
    //}

    public void Update()
    {
        foreach(var textureItem in m_AllTextureCache)
        {
            TempAtlasTextureInfo textureInfo = null;

            if(m_AllTextureCache.TryGetValue(textureItem.Key, out textureInfo))
            {
                foreach(var spriteItem in textureInfo.SpriteTextureInfo)
                {
                    LoadTexture(spriteItem.Key, textureItem.Key, spriteItem.Value.ZoomScale);
                }
            }          
        }
    }
    public void ClearAll()
    {
        //List<string> assetPaths = new List<string>();

        //foreach(var atlasInfo in m_AllTextureCache)
        //{
        //    foreach(var spriteInfo in atlasInfo.Value.SpriteTextureInfo)
        //    {
        //        string fileName = Path.GetFileName(spriteInfo.Key);
        //        assetPaths.Add(atlasInfo.Value.TempAtlasDir + fileName);
        //    }
        //}

        //foreach (var path in assetPaths)
        //{
        //    AssetDatabase.DeleteAsset(path);
        //}

        m_AllTextureCache.Clear();

        //删除临时文件夹
        if (Directory.Exists(UIAtlasEditorConfig.TempPath))
        {
            DirectoryInfo info = new DirectoryInfo(UIAtlasEditorConfig.TempPath);
            DeleteFileByDirectory(info);
            AssetDatabase.DeleteAsset(UIAtlasEditorConfig.TempPath);
        }
    }

    public void Clear(string atlasPath)
    {
        if (string.IsNullOrEmpty(atlasPath))
        {
            return;
        }
      
        TempAtlasTextureInfo tempAtlasTexture = null;
        //List<string> assetPaths = new List<string>();

        if (m_AllTextureCache.ContainsKey(atlasPath))
        {
            if (m_AllTextureCache.TryGetValue(atlasPath, out tempAtlasTexture))
            {
                //foreach (var spriteInfo in tempAtlasTexture.SpriteTextureInfo)
                //{
                //    string fileName = Path.GetFileName(spriteInfo.Key);
                //    assetPaths.Add(tempAtlasTexture.TempAtlasDir + fileName);
                //}

                //foreach (var path in assetPaths)
                //{
                //    AssetDatabase.DeleteAsset(path);
                //}

                tempAtlasTexture.SpriteTextureInfo.Clear();

                //删除临时文件夹
                if (Directory.Exists(tempAtlasTexture.TempAtlasDir))
                {
                    DirectoryInfo info = new DirectoryInfo(tempAtlasTexture.TempAtlasDir);
                    DeleteFileByDirectory(info);
                    AssetDatabase.DeleteAsset(tempAtlasTexture.TempAtlasDir);
                }
            }
        }
    }

    public void Clear()
    {//清空临时文件夹资源

        List<string> assetPaths = new List<string>();
        foreach( var tex in textureCache )
        {
            string fileName = Path.GetFileName(tex.Key);
            assetPaths.Add(UIAtlasEditorConfig.TempPath + fileName);
        }
        textureCache.Clear();

        foreach( var path in assetPaths )
        {
            AssetDatabase.DeleteAsset(path);
        }

        //删除临时文件夹
        if (Directory.Exists(UIAtlasEditorConfig.TempPath))
        {
            DirectoryInfo info = new DirectoryInfo(UIAtlasEditorConfig.TempPath);
            DeleteFileByDirectory(info);
        }
    }

    public void DeleteFileByDirectory(DirectoryInfo info)
    {//删除临时文件夹

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

    private bool _IsTextureAssetAlreadyExistsInTempFolder(string path,out bool bIsNeedRename)
    {
        bool bRet = false;
        string fileName = Path.GetFileName(path);

        if (File.Exists(UIAtlasEditorConfig.AbsTempPath + fileName))
        {
            bIsNeedRename = true;
            bRet = false;
            foreach (var textureInfo in textureCache)
            {
                if (textureInfo.Key == path)
                {
                    bRet = true;
                    bIsNeedRename = false;
                    break;
                }
            }
        }
        else 
        {
            bIsNeedRename = false;
            bRet = false;
        }

        return bRet;
    }

    private bool _IsTextureAssetSameModTime(string path)
    {
        string fileName = Path.GetFileName(path);
        string assetFilePath = UIAtlasEditorConfig.AbsTempPath + fileName;
        DateTime orginFileWriteTime = File.GetLastWriteTime(path);
        DateTime assetFileWriteTime = File.GetLastWriteTime(assetFilePath);
        return orginFileWriteTime.Equals(assetFileWriteTime);
    }

    private void _TouchTempDir()
    {
        //查看缓存文件夹是否存在
        if (!Directory.Exists(UIAtlasEditorConfig.AbsTempPath))
        {
            Directory.CreateDirectory(UIAtlasEditorConfig.AbsTempPath);
        }
    }

    private bool MakeTextureReadable(string path, bool force)
    {//使纹理可读

        if (string.IsNullOrEmpty(path)) return false;
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return false;

        TextureImporterSettings settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);

        if (force || !settings.readable || settings.npotScale != TextureImporterNPOTScale.None
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
 || settings.alphaIsTransparency
#endif
)
        {
            settings.readable = true;
            settings.textureFormat = TextureImporterFormat.ARGB32;
            settings.npotScale = TextureImporterNPOTScale.None;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
            settings.alphaIsTransparency = false;
#endif
            ti.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }

        return true;
    }

    private Texture2D ScaleTextureBilinear(Texture2D originalTexture, float scaleFactor)
    {//缩放纹理

        if ((originalTexture == null) 
            || ((-0.000001f < scaleFactor) && (scaleFactor < 0.000001f)))
        {
            return null;
        }

        if (scaleFactor == 1.0f)
        {
            return originalTexture;
        }

        Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalTexture.width * scaleFactor), Mathf.CeilToInt(originalTexture.height * scaleFactor));
        float scale = 1.0f / scaleFactor;
        int maxX = originalTexture.width - 1;
        int maxY = originalTexture.height - 1;
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                // Bilinear Interpolation
                float targetX = x * scale;
                float targetY = y * scale;
                int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                int x2 = Mathf.Min(maxX, x1 + 1);
                int y2 = Mathf.Min(maxY, y1 + 1);

                float u = targetX - x1;
                float v = targetY - y1;
                float w1 = (1 - u) * (1 - v);
                float w2 = u * (1 - v);
                float w3 = (1 - u) * v;
                float w4 = u * v;
                Color color1 = originalTexture.GetPixel(x1, y1);
                Color color2 = originalTexture.GetPixel(x2, y1);
                Color color3 = originalTexture.GetPixel(x1, y2);
                Color color4 = originalTexture.GetPixel(x2, y2);
                Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                    Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                    Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                    Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                newTexture.SetPixel(x, y, color);
            }
        }

        return newTexture;
    }

    public bool IsEnableTextureFile(string fileName)
    {//判断目标文件是否是纹理

        bool bRet = false;

        if(fileName == null)
        {
            return false;
        }

        if(fileName.EndsWith(".png") 
           ||fileName.EndsWith(".bmp")
           ||fileName.EndsWith(".jpg")
           ||fileName.EndsWith(".jpeg"))
        {
            bRet = true;
        }

        return bRet;
    }

    private Dictionary<string, TempTextureInfo> textureCache = new Dictionary<string, TempTextureInfo>();
    private Dictionary<string, TempAtlasTextureInfo> m_AllTextureCache = new Dictionary<string, TempAtlasTextureInfo>();

    static public UIAtlasTempTextureManager GetInstance()
    {
        if( s_instance == null )
        {
            s_instance = new UIAtlasTempTextureManager();
        }

        return s_instance;
    }

    static public void DestroyInstance()
    {
        if( s_instance != null )
        {
            s_instance.ClearAll();
            s_instance = null;
        }
    }

    static private UIAtlasTempTextureManager s_instance = null;
}

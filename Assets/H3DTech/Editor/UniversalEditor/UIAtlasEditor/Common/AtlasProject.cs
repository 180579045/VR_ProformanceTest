using System.Collections;
using System.IO;
using System.Collections.Generic;

public enum PROJECT_TYPE
{//工程类型
    PROJECT_TYPE_NEW = 0,   //新建工程
    PROJECT_TYPE_EXIST,     //已有工程
}

public enum PROJECT_ERROR_TYPE
{//工程操作错误类型
    PROJECT_ERROR_UNKNOWN = 0,                              //未知错误
    PROJECT_ERROR_SPRITEIMAGE_PATH,                        //小图路径错误
    PROJECT_ERROR_SPRITE_EXIST,                           //小图已存在
    PROJECT_ERROR_ATLASOUTPU_PATH,                         //Atlas输入路径错误
    PROJECT_ERROR_NONE_IMAGE,                              //未添加小图
    PROJECT_ERROR_NONE_PROJECT,                            //未创建工程
    PROJECT_ERROR_LOAD_ERRORPATH,                         //Load路径错误
    PROJECT_ERROR_PROJECTFILE_ERROR,                      //工程文件异常
    PROJECT_ERROR_PROJECTFILE_PATHERROR,                      //工程文件路径异常
    PROJECT_ERROR_REFERENCEFILE_ERROR,                    //引用信息异常
    PROJECT_ERROR_MAKEPNG_TEXTUREERROR,                   //生成PNG时，纹理异常
    PROJECT_ERROR_MAKEPREFAB_OUTPUTPATHERROR,             //生成Prefab时，输出路径异常

    PROJECT_ERROR_NONE = -1,                //默认值
}

public class SpriteImageInfo
{//小图信息（记录于工程文件中）

    private string m_spritePath = null; //小图路径（相对于配置路径）
    private float m_zoomScale = 0f;     //小图在Atlas中的缩放比例

    public string SpritePath { get { return m_spritePath; } set { m_spritePath = value; } }
    public float ZoomScale { get { return m_zoomScale; } set { m_zoomScale = value; } }

}

public class ReferenceInfo
{
    private string m_referenceFilePath = string.Empty;
    private List<string> m_referencingSprite = new List<string>();

    public string ReferenceFilePath { get { return m_referenceFilePath; } set { m_referenceFilePath = value; } }
    public List<string> ReferencingSprite { get { return m_referencingSprite; } set { m_referencingSprite = value; } }

}

public class AtlasProject
{//Atals工程

    public bool CheckSpriteImagePath(string path)
    {//检查待添加的小图的路径是否正确
        bool bRet = false;

        if ((path == null) || (UIAtlasEditorConfig.ImageBasePath == null))
        {
            bRet = false;
        }
        else
        {
            string dirStr = System.IO.Path.GetDirectoryName(path);
            dirStr = dirStr.Replace(@"/", @"\");
            if (!dirStr.EndsWith("\\"))
            {
                dirStr = dirStr + "\\";
            }

            bRet = dirStr.Contains(UIAtlasEditorConfig.ImageBasePath.Replace(@"/", @"\"));
        }
      
        return bRet;
    }

#region 工程操作函数
    public void Save()
    {//保存工程文件

        if (Path == null)
        {
            return;
        }

        do
        {
            //制作持久化对象
            AtlasSerializeObject obj = GetSerializeObject();
            if (null == obj)
            {
                break;
            }

            obj.SaveAtlasSerializeObject(Path, obj);

            IsModify = false;
        } while (false);

    }

    public PROJECT_ERROR_TYPE Save(bool test)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (string.IsNullOrEmpty(Path))
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_PROJECTFILE_PATHERROR;
        }

        do
        {
            //制作持久化对象
            AtlasSerializeObject obj = GetSerializeObject();
            if (null == obj)
            {
                break;
            }

            obj.SaveAtlasSerializeObject(Path, obj);

            IsModify = false;
        } while (false);

        return errorType;
    }

    //public bool Load(string path)
    //{//读取工程文件

    //    bool bRet = true;

    //    if (string.IsNullOrEmpty(path))
    //    {
    //        errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_LOAD_ERRORPATH;
    //        return false;
    //    }

    //    do
    //    {
    //        AtlasSerializeObject obj = null;

    //        AtlasSerializeObject.LoadAtlasSerializeObject(path, out obj);
    //        if(null == obj)
    //        {
    //            bRet = false;
    //            errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_PROJECTFILE_ERROR;

    //            break;
    //        }
 
    //        //更新小图信息
    //        ApplySerializeObject(obj);

    //        //更新工程名，工程路径
    //        Path = path;
    //        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    //        imagePath = atlasSavePath + name + ".png";
    //        describePath = atlasSavePath + name + ".prefab";

    //        //更新工程类型、是否修改
    //        ProjectType = PROJECT_TYPE.PROJECT_TYPE_EXIST;
           
    //        IsModify = false;

    //    } while (false);

    //    return bRet;
    //}

    public PROJECT_ERROR_TYPE Load (string path)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (string.IsNullOrEmpty(path))
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_LOAD_ERRORPATH;
        }

        do
        {
            AtlasSerializeObject obj = null;

            AtlasSerializeObject.LoadAtlasSerializeObject(path, out obj);
            if (null == obj)
            {
                errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_PROJECTFILE_ERROR;
                break;
            }

            //更新小图信息
            ApplySerializeObject(obj);

            //更新工程名，工程路径
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            imagePath = atlasSavePath + name + ".png";
            describePath = atlasSavePath + name + ".prefab";

            //更新工程类型、是否修改
            ProjectType = PROJECT_TYPE.PROJECT_TYPE_EXIST;

            IsModify = false;

        } while (false);

        return errorType;
    }

    public bool AddSpriteImage(string path)
    {//向工程中添加小图

        bool bRet = true;

        if (string.IsNullOrEmpty(path))
        {
            ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
            return false;
        }

        AtlasSpriteImage spriteImage = new AtlasSpriteImage();

        //首次添加时缩放比例默认是1
        spriteImage.ZoomScale = 1f;

        if (CheckSpriteImagePath(path))
        {//路径合法

            //spriteImage.Path = path.Substring(ImageRelativePath.Length);
            spriteImage.Path = path;
            spriteImage.Name = path.Substring(path.LastIndexOfAny(new char[] { '/', '\\' }) + 1);

            foreach (var sprite in spriteImages)
            {
                if (sprite.Path == spriteImage.Path)
                {
                    //更新错误类型
                    ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITE_EXIST;
                    bRet = false;
                    break;
                }
            }
        }
        else
        {//路径非法

            //更新错误类型
            ProjectFailedType = PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
            bRet = false;
        }

        if (bRet)
        {
            //添加小图
            spriteImages.Add(spriteImage);
            IsModify = true;
            bRet = true;
        }

        return bRet;
    }

    public PROJECT_ERROR_TYPE AddSpriteImage(AtlasSpriteImage spriteImage)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (null == spriteImage)
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_UNKNOWN;
        }

        do
        {
            if (CheckSpriteImagePath(spriteImage.Path))
            {
                foreach (var sprite in spriteImages)
                {
                    if (sprite.Path == spriteImage.Path)
                    {
                        //更新错误类型
                        errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITE_EXIST;
                        break;
                    }
                }
                if (IsProjectFailed(errorType))
                {
                    break;
                }
                AtlasSpriteImage newSpriteImage = spriteImage;
                //添加小图
                spriteImages.Add(newSpriteImage);
                IsModify = true;
            }
            else
            {
                errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
                break;
            }
        } while (false);     

        return errorType;
    }

    public bool RemoveSpriteImage(string path)
    {//从工程中移除小图

        bool bRet = true;

        if (path == null)
        {
            return false;
        }

        //查询待删除的小图
        foreach (var ImageInfo in spriteImages)
        {
            if (ImageInfo.Path == path)
            {
                spriteImages.Remove(ImageInfo);
                IsModify = true;
                break;
            }
        }

        return bRet;
    }

    public PROJECT_ERROR_TYPE AddItemToAtlasReferenceTable(ReferenceInfo refItem)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if(string.IsNullOrEmpty(refItem.ReferenceFilePath))
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_REFERENCEFILE_ERROR;
        }

        ReferenceTable.Add(refItem);

        return errorType;
    }
    public PROJECT_ERROR_TYPE UpdateAtlasReferenceTable(List<ReferenceInfo> refTable)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (null == refTable)
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_REFERENCEFILE_ERROR;
        }

        foreach(var refItem in refTable)
        {//遍历所有反依赖项

            foreach(var refingSpriteItem in refItem.ReferencingSprite)
            {//遍历每个反依赖项中引用的Sprite
                
                foreach(var spriteItem in spriteImages)
                {//遍历Atals中所有的Sprite

                    if (System.IO.Path.GetFileNameWithoutExtension(spriteItem.Path) == refingSpriteItem)
                    {
                        if (!spriteItem.ReferenceTable.Contains(refItem.ReferenceFilePath))
                        {
                            spriteItem.ReferenceTable.Add(refItem.ReferenceFilePath);
                        }
                        break;
                    }
                }
            }
        }

        ReferenceTable = refTable;
        referenceTableBak = ReferenceTable;

        return errorType;
    }
    public PROJECT_ERROR_TYPE AddItemToSpriteReferenceTable(string spritePath, string refItem)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (string.IsNullOrEmpty(spritePath))
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_REFERENCEFILE_ERROR;
        }

        foreach(var item in spriteImages)
        {
            if(item.Path == spritePath)
            {
                item.ReferenceTable.Add(refItem);
                break;
            }
        }

        return errorType;
    }
    public PROJECT_ERROR_TYPE ModifyRefTableAfterRemoveSprite(string[] spritePathTable)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        if (null == spritePathTable)
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
        }

        foreach (var spriteItem in spritePathTable)
        {//遍历全部移动Sprite

            foreach(var refAssetItem in ReferenceTable)
            {//遍历全部反依赖项目,并删除移动Sprite

                refAssetItem.ReferencingSprite.Remove(System.IO.Path.GetFileNameWithoutExtension(spriteItem));
            }
        }


        //删除无用反依赖记录
        for (int i = ReferenceTable.Count - 1; i >= 0; i--)
        {
            if (0 == ReferenceTable[i].ReferencingSprite.Count)
            {
                ReferenceTable.RemoveAt(i);
            }

        }


        return errorType;
    }

    public PROJECT_ERROR_TYPE ModifyRefTableAfterAddSprite(AtlasSpriteImage[] spriteImageTable)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;
        AtlasSpriteImage tempSprite = null;
        ReferenceInfo refInfo = null;

        if (null == spriteImageTable)
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
        }

        foreach (var spriteItem in spriteImageTable)
        {
            if (!FindSprite(spriteItem.Path, ref tempSprite))
            {
                continue;
            }

            //更新Sprite反依赖
            tempSprite.ReferenceTable = spriteItem.ReferenceTable;

            //更新Atlas反依赖
            foreach (var assetItem in spriteItem.ReferenceTable)
            {
                if (!FindRefAssetFile(assetItem, ref refInfo))
                {
                    ReferenceInfo newInfo = new ReferenceInfo();
                    newInfo.ReferenceFilePath = assetItem;
                    newInfo.ReferencingSprite.Add(System.IO.Path.GetFileNameWithoutExtension(spriteItem.Path));

                    ReferenceTable.Add(newInfo);
                }
                else
                {
                    if (!FindRefAssetSprite(assetItem, System.IO.Path.GetFileNameWithoutExtension(spriteItem.Path)))
                    {
                        refInfo.ReferencingSprite.Add(System.IO.Path.GetFileNameWithoutExtension(spriteItem.Path));
                    }
                }
            }
        }

        return errorType;
    }
    
    public PROJECT_ERROR_TYPE GetSpriteRefTable(string spritePath, ref List<string> refAssetList)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;

        refAssetList = new List<string>();

        if (string.IsNullOrEmpty(spritePath))
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
        }

        foreach(var item in spriteImages)
        {
            if (item.Path == spritePath)
            {
                refAssetList = item.ReferenceTable;
                break;
            }
        }

        return errorType;
    }

    public bool GetSpriteImage(string path, out AtlasSpriteImage spriteImage)
    {//获取指定文件名的小图

        bool bRet = false;

        if (path == null)
        {
            spriteImage = null;
            return false;
        }

       // string tempPath = path.Substring(path.IndexOfAny(ImageRelativePath.ToCharArray()) + ImageRelativePath.Length);
        spriteImage = null;

        //查询目标小图
        foreach (var sprite in spriteImages)
        {
            if (sprite.Path == path)
            {
                spriteImage = sprite;
                bRet = true;
                break;
            }
        }

        return bRet;
    }

    public List<AtlasSpriteImage> GetAllSprites()
    {//获取工程中全部小图

        return spriteImages;
    }

    public void ClearSpriteImage()
    {//清空小图

        spriteImages.Clear();
    }

    private AtlasSerializeObject GetSerializeObject()
    {//制作待持久化的对象

        AtlasSerializeObject obj = new AtlasSerializeObject();

        //设定Atals输出路径（相对Until的路径）
        obj.AtlasOutputPath = AtlasSavePath;

        List<KeyValuePair<string, SpriteImageInfo>> spriteImageInfo = new List<KeyValuePair<string, SpriteImageInfo>>();

        //设定小图路径（相对配置路径）、小图缩放比例
        foreach (var ImageInfo in spriteImages)
        {
            SpriteImageInfo newInfo = new SpriteImageInfo();
            newInfo.SpritePath = ImageInfo.Path.Substring(UIAtlasEditorConfig.ImageBasePath.Replace(@"/", @"\").Length);
            newInfo.ZoomScale = ImageInfo.ZoomScale;

            KeyValuePair<string, SpriteImageInfo> spriteInfoKeyPair = new KeyValuePair<string, SpriteImageInfo>
            (ImageInfo.Name, newInfo);
            spriteImageInfo.Add(spriteInfoKeyPair);
        }

        obj.SpriteInfoTable = spriteImageInfo;

        return obj;
    }

    private bool ApplySerializeObject(AtlasSerializeObject obj)
    {//获取持久化对象

        bool bRet = true;

        if (obj == null)
        {
            return false;
        }

        //获取Atlas输出路径
        AtlasSavePath = obj.AtlasOutputPath;

        spriteImages.Clear();


        //获取小图信息
        foreach (var ImageInfo in obj.SpriteInfoTable)
        {
            AtlasSpriteImage image = new AtlasSpriteImage();
            image.Name = ImageInfo.Key;
            image.Path = UIAtlasEditorConfig.ImageBasePath.Replace(@"/", @"\") + ImageInfo.Value.SpritePath;
            image.ZoomScale = ImageInfo.Value.ZoomScale;

            spriteImages.Add(image);
        }

        return bRet;
    }

    private bool FindRefAssetFile(string assetFilePath, ref ReferenceInfo refInfo)
    {
        bool bRet = false;
        refInfo = null;

        if(string.IsNullOrEmpty(assetFilePath))
        {
            return false;
        }

        foreach(var assetItem in ReferenceTable)
        {
            if(assetFilePath == assetItem.ReferenceFilePath)
            {
                bRet = true;
                refInfo = assetItem;
                break;
            }
        }

        return bRet;
    }

    private bool FindRefAssetSprite(string assetFilePath, string spriteName)
    {
        bool bRet = false;
        
        if(
               string.IsNullOrEmpty(assetFilePath)
            || string.IsNullOrEmpty(spriteName)
            )
        {
            return false;
        }

        foreach(var assetItem in ReferenceTable)
        {
            if (assetItem.ReferenceFilePath == assetFilePath)
            {
                foreach(var spriteRefItem in assetItem.ReferencingSprite)
                {
                    if(spriteRefItem == spriteName)
                    {
                        bRet = true;
                        break;
                    }
                }
                if(bRet)
                {
                    break;
                }
            }
        }

        return bRet;
    }
#endregion


#region Sprite操作函数
    public void SetSpriteImageZoom(string path , float scaleFactor)
    {//变更小图在Atlas中的缩放比例

        if(path == null)
        {
            return;
        }

        //查询目标小图
        foreach (var ImageInfo in spriteImages)
        {
            //string spritePath = path.Substring(path.IndexOfAny(UIAtlasEditorConfig.ImageBasePath.Replace(@"/", @"\").ToCharArray()) + UIAtlasEditorConfig.ImageBasePath.Replace(@"/", @"\").Length);
            if (ImageInfo.Path == path)
            {
                ImageInfo.ZoomScale = scaleFactor;
                break;
            }
        }
    }

    public PROJECT_ERROR_TYPE GetSpriteImageZoom(string spritePath, out float scaleFactor)
    {
        PROJECT_ERROR_TYPE errorType = PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE;
        scaleFactor = 0f;
        if (string.IsNullOrEmpty(spritePath))
        {
            return PROJECT_ERROR_TYPE.PROJECT_ERROR_SPRITEIMAGE_PATH;
        }

        AtlasSpriteImage spriteImage = null;

        if(GetSpriteImage(spritePath, out spriteImage))
        {
            scaleFactor = spriteImage.ZoomScale;
        }

        return errorType;
    }

    //public bool PreViewAtlastexture(Texture2D tex, Texture2D[] imgs, out Texture2D outTex)
    //{//获取预览Atals

    //    outTex = null;
    //    DefaultTexturePackagingStrategy maker = new DefaultTexturePackagingStrategy();

    //    //打包纹理
    //    if(maker.Pack(tex, imgs, null))
    //    {
    //        outTex = tex;
    //    }

    //    return true;
    //}

    //public bool MakeAtlasTexture(Texture2D tex, Texture2D[] imgs)
    //{//生成Atlas Png

    //    string newPath = null;
    //    bool bRet = false;

    //    if ((atlasSavePath == null) || (name == null))
    //    {
    //        return false;
    //    }

    //    DefaultTexturePackagingStrategy maker = new DefaultTexturePackagingStrategy();

    //    //打包纹理
    //    if(maker.Pack(tex, imgs, null))
    //    {
    //        if(tex == null)
    //        {
    //            return false;
    //        }

    //        //创建png文件
    //        byte[] bytes = tex.EncodeToPNG();
    //        newPath = atlasSavePath + name + ".png";
    //        UniversalEditorUtility.MakeFileWriteable(newPath);
    //        System.IO.File.WriteAllBytes(newPath, bytes);
    //        bytes = null;
    //        bRet = true;
    //    }

    //    return bRet;
    //}   

    //public void MakeAtlasPrefab(string outputPath)
    //{//生成Atlas prefab
    //    if ((outputPath == null) || (!outputPath.Contains(".prefab")))
    //    {
    //        return;
    //    }

    //    GameObject go = AssetDatabase.LoadAssetAtPath(outputPath, typeof(GameObject)) as GameObject;
    //    string matPath = outputPath.Replace(".prefab", ".mat");

    //    // Try to load the material
    //    Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

    //    // If the material doesn't exist, create it
    //    if (mat == null)
    //    {
    //        Shader shader = Shader.Find(NGUISettings.atlasPMA ? "Unlit/Premultiplied Colored" : "Unlit/Transparent Colored");
    //        mat = new Material(shader);

    //        // Save the material
    //        AssetDatabase.CreateAsset(mat, matPath);
    //        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

    //        // Load the material so it's usable
    //        mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
    //    }

    //    // Create a new prefab for the atlas
    //    Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(outputPath);

    //    if (go == null)
    //    {
    //        // Create a new game object for the atlas
    //        string atlasName = outputPath.Replace(".prefab", "");
    //        atlasName = atlasName.Substring(outputPath.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
    //        go = new GameObject(atlasName);
    //        go.AddComponent<UIAtlas>().spriteMaterial = mat;

    //        // Update the prefab
    //        PrefabUtility.ReplacePrefab(go, prefab);
    //        GameObject.DestroyImmediate(go);
    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    //    }


    //    // Select the atlas
    //    go = AssetDatabase.LoadAssetAtPath(outputPath, typeof(GameObject)) as GameObject;
    //    NGUISettings.atlas = go.GetComponent<UIAtlas>();
    //    Selection.activeGameObject = go;

    //    List<UIAtlasMaker.SpriteEntry> sprites = CreateSprites(UIAtlasTempTextureManager.GetInstance().GetTextureCacheSprite());
    //    UIAtlasMaker.UpdateAtlas(NGUISettings.atlas, sprites); ;
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

    //    NGUIEditorTools.UpgradeTexturesToSprites(NGUISettings.atlas);
    //    NGUIEditorTools.RepaintSprites();
    //}

    //static public List<UIAtlasMaker.SpriteEntry> CreateSprites(List<Texture> textures)
    //{
    //    List<UIAtlasMaker.SpriteEntry> list = new List<UIAtlasMaker.SpriteEntry>();

    //    foreach (Texture tex in textures)
    //    {
    //        Texture2D oldTex = NGUIEditorTools.ImportTexture(tex, true, false, true);
    //        if (oldTex == null) oldTex = tex as Texture2D;
    //        if (oldTex == null) continue;

    //        // If we aren't doing trimming, just use the texture as-is
    //        if (!NGUISettings.atlasTrimming && !NGUISettings.atlasPMA)
    //        {
    //            UIAtlasMaker.SpriteEntry sprite = new UIAtlasMaker.SpriteEntry();
    //            sprite.SetRect(0, 0, oldTex.width, oldTex.height);
    //            sprite.tex = oldTex;
    //            if (oldTex.name.EndsWith("zoomed"))
    //            {
    //                sprite.name = oldTex.name.Substring(0, oldTex.name.Length - "zoomed".Length);
    //            }
    //            else
    //            {
    //                sprite.name = oldTex.name;
    //            }
    //            sprite.temporaryTexture = false;
    //            list.Add(sprite);
    //            continue;
    //        }

    //        // If we want to trim transparent pixels, there is more work to be done
    //        Color32[] pixels = oldTex.GetPixels32();

    //        int xmin = oldTex.width;
    //        int xmax = 0;
    //        int ymin = oldTex.height;
    //        int ymax = 0;
    //        int oldWidth = oldTex.width;
    //        int oldHeight = oldTex.height;

    //        // Find solid pixels
    //        if (NGUISettings.atlasTrimming)
    //        {
    //            for (int y = 0, yw = oldHeight; y < yw; ++y)
    //            {
    //                for (int x = 0, xw = oldWidth; x < xw; ++x)
    //                {
    //                    Color32 c = pixels[y * xw + x];

    //                    if (c.a != 0)
    //                    {
    //                        if (y < ymin) ymin = y;
    //                        if (y > ymax) ymax = y;
    //                        if (x < xmin) xmin = x;
    //                        if (x > xmax) xmax = x;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            xmin = 0;
    //            xmax = oldWidth - 1;
    //            ymin = 0;
    //            ymax = oldHeight - 1;
    //        }

    //        int newWidth = (xmax - xmin) + 1;
    //        int newHeight = (ymax - ymin) + 1;

    //        if (newWidth > 0 && newHeight > 0)
    //        {
    //            UIAtlasMaker.SpriteEntry sprite = new UIAtlasMaker.SpriteEntry();
    //            sprite.x = 0;
    //            sprite.y = 0;
    //            sprite.width = oldTex.width;
    //            sprite.height = oldTex.height;

    //            // If the dimensions match, then nothing was actually trimmed
    //            if (!NGUISettings.atlasPMA && (newWidth == oldWidth && newHeight == oldHeight))
    //            {
    //                sprite.tex = oldTex;
    //                if (oldTex.name.EndsWith("zoomed"))
    //                {
    //                    sprite.name = oldTex.name.Substring(0, oldTex.name.Length - "zoomed".Length);
    //                }
    //                else
    //                {
    //                    sprite.name = oldTex.name;
    //                }
    //                sprite.temporaryTexture = false;
    //            }
    //            else
    //            {
    //                // Copy the non-trimmed texture data into a temporary buffer
    //                Color32[] newPixels = new Color32[newWidth * newHeight];

    //                for (int y = 0; y < newHeight; ++y)
    //                {
    //                    for (int x = 0; x < newWidth; ++x)
    //                    {
    //                        int newIndex = y * newWidth + x;
    //                        int oldIndex = (ymin + y) * oldWidth + (xmin + x);
    //                        if (NGUISettings.atlasPMA) newPixels[newIndex] = NGUITools.ApplyPMA(pixels[oldIndex]);
    //                        else newPixels[newIndex] = pixels[oldIndex];
    //                    }
    //                }

    //                // Create a new texture
    //                sprite.temporaryTexture = true;
    //                if (oldTex.name.EndsWith("zoomed"))
    //                {
    //                    sprite.name = oldTex.name.Substring(0, oldTex.name.Length - "zoomed".Length);
    //                }
    //                else
    //                {
    //                    sprite.name = oldTex.name;
    //                }
    //                sprite.tex = new Texture2D(newWidth, newHeight);
    //                sprite.tex.SetPixels32(newPixels);
    //                sprite.tex.Apply();

    //                // Remember the padding offset
    //                sprite.SetPadding(xmin, ymin, oldWidth - newWidth - xmin, oldHeight - newHeight - ymin);
    //            }
    //            list.Add(sprite);
    //        }
    //    }
    //    return list;
    //}

    private bool FindSprite(string spritePath, ref AtlasSpriteImage spriteImage)
    {
        bool bRet = false;
        spriteImage = null;

        if (string.IsNullOrEmpty(spritePath))
        {
            return false;
        }

        foreach(var spriteItem in spriteImages)
        {
            if(spritePath == spriteItem.Path)
            {
                spriteImage = spriteItem;
                bRet = true;
            }
        }

        return bRet;
    }

    private bool IsProjectFailed(PROJECT_ERROR_TYPE errorType)
    {
        bool bRet = false;

        if (errorType != PROJECT_ERROR_TYPE.PROJECT_ERROR_NONE)
        {
            bRet = true;
        }
 
        return bRet;
    }
#endregion

#region 成员变量
    private string name = null;                         //工程名
    private string path = null;                         //工程路径
    private string imagePath = string.Empty;            //atlas image路径
    private string describePath = string.Empty;         //atlas 描述文件路径
    private string atlasSavePath = null;                //Atals输出路径

    private bool isModify = false;                      //工程是否修改       
    private PROJECT_TYPE projcetType;                   //工程类型
    private PROJECT_ERROR_TYPE errorType;               //失败类型
    
    private List<AtlasSpriteImage> spriteImages = new List<AtlasSpriteImage>(); //存储小图资源
    private List<ReferenceInfo> referenceTable = new List<ReferenceInfo>();     //存储Atlas工程引用资源
    private List<ReferenceInfo> referenceTableBak = new List<ReferenceInfo>();     //存储Atlas工程引用资源（备份）

    public string Name { get { return name; } set { name = value; } }         
    public string Path { get { return path; } set { path = value; } }
    public string ImagePath { get { return imagePath; } set { imagePath = value; } }
    public string DescribePath { get { return describePath; } set { describePath = value; } }

    public bool IsModify { get { return isModify; } set { isModify = value; } }         
    public PROJECT_TYPE ProjectType { get { return projcetType; } set { projcetType = value; } }
    public PROJECT_ERROR_TYPE ProjectFailedType { get { return errorType; } set { errorType = value; } }
    public string AtlasSavePath { get { return atlasSavePath; } set { atlasSavePath = value; } }
    public List<ReferenceInfo> ReferenceTable { get { return referenceTable; } set { referenceTable = value; } }
    public List<ReferenceInfo> ReferenceTableBak { get { return referenceTableBak; } set { referenceTableBak = value; } }     
   
#endregion
}

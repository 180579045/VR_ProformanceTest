using UnityEngine;
using System.Collections;
using UnityEditor;

//当纹理不是tga或png时会报错
//将纹理类型设置为Advanced
//关闭mipmap生成
//在Iphone上将纹理最大大小设置为1024,导入格式为PVRTC_RGB4
//在Android上将纹理最大大小设置为1024，导入格式为ETC_RGB4
public class H3DTextureFormatCheckerForTDemoShooter : H3DAssetChecker
{
    public override ResouceType ResType
    {
        get { return ResouceType.TEXTURE; }
    }

    public override void Check(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    {
        //TextureImporter imp = assetImporter as TextureImporter;
        //if (imp == null)
        //    return; 

        //if( !(assetPath.ToLower().EndsWith(".tga")) &&
        //    !(assetPath.ToLower().EndsWith(".png")) &&
        //    !imp.lightmap //光照图
        //    )
        //{
        //    LogError(assetPath + ":请将纹理转换为tga或png!!!",assetObj);
        //}

        //if( assetPath.ToLower().EndsWith(".tga") && !imp.DoesSourceTextureHaveAlpha() )
        //{
        //    LogWarning(assetPath + ":TGA纹理需包含透明通道！",assetObj);
        //}

        //if( imp.mipmapEnabled && firstImport)
        //{//只第一次导入时关闭mipmap
        //    imp.mipmapEnabled = false;
        //    needImport = true;
        //}

        //if( imp.textureType != TextureImporterType.Advanced )
        //{
        //    imp.textureType = TextureImporterType.Advanced;
        //    needImport = true;
        //}

        //int maxTextureSize = 0;
        //TextureImporterFormat textureFormat;

        //if (!imp.DoesSourceTextureHaveAlpha())
        //{//不含Alpha通道
        //    //IPhone
        //    bool ret = imp.GetPlatformTextureSettings("iPhone", out maxTextureSize, out textureFormat);
        //    if (!ret || maxTextureSize != 1024 || textureFormat != TextureImporterFormat.PVRTC_RGB4)
        //    {
        //        imp.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGB4);
        //        needImport = true;
        //    }

        //    //Android
        //    ret = imp.GetPlatformTextureSettings("Android", out maxTextureSize, out textureFormat);
        //    if (!ret || maxTextureSize != 1024 || textureFormat != TextureImporterFormat.ETC_RGB4)
        //    {
        //        imp.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC_RGB4);
        //        needImport = true;
        //    }
        //}
        //else
        //{//含Alpha通道
        //    //IPhone
        //    bool ret = imp.GetPlatformTextureSettings("iPhone", out maxTextureSize, out textureFormat);
        //    if (!ret || maxTextureSize != 1024 || textureFormat != TextureImporterFormat.PVRTC_RGBA4)
        //    {
        //        imp.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4);
        //        needImport = true;
        //    }

        //    //Android
        //    ret = imp.GetPlatformTextureSettings("Android", out maxTextureSize, out textureFormat);
        //    if (!ret || maxTextureSize != 1024 || textureFormat != TextureImporterFormat.ETC2_RGBA8)
        //    {
        //        imp.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGBA8);
        //        needImport = true;
        //    }
        //}
    }


    public override void PostCheck(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    { 
        //Texture2D tex = assetObj as Texture2D;
        //TextureImporter imp = assetImporter as TextureImporter;
        //if (imp == null || tex == null)
        //    return; 

        //if (assetPath.ToLower().EndsWith(".tga") && imp.DoesSourceTextureHaveAlpha())
        //{//对TGA纹理的透明通道进行筛查  
        //    if (!H3DAssetCheckerUtil.IsTGATextureAlphaChannelLegal(assetPath))
        //    {
        //        LogError(assetPath + ":此TGA纹理的所有像素透明通道都为1或0！",assetObj);
        //    } 
        //}
    }
}

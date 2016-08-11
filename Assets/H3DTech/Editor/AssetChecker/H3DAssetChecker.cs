using UnityEngine;
using System.Collections;
using UnityEditor;

public class H3DAssetChecker
{
    public enum ResouceType
    {
        NONE = -1,
        FBX_MODEL = 0,
        FBX_ANIM,
        AUDIOCLIP,
        TEXTURE,
        FONT,

        MATERIAL,
        SHADER,
        SCRIPT,
        PHYMATERIAL,
        PREFAB,
        ANIMATIONCLIP, 
        ALL
    }

    //当前检查器所检查的资源类型
    public virtual ResouceType ResType
    {
        get { return ResouceType.NONE; } 
    }

    //当前检查器关注的文件路径，包括此路径下的所有子文件夹
    //不复写此属性视同关注Assets目录下全部资源,路径格式以;分割。
    //例如：若对Assets/Resouces以及Assets/Materials路径下的文件
    //感兴趣可以这样写：Assets/Resources;Assets/Materials。若路径
    //存在包含关系以最外层的路径为准
    public virtual string FilterPath
    {
        get { return ""; } 
    }

    //排除目录，路径以;分割。
    //例如：Assets/Resources标识排除Assets/Resources路径
    public virtual string ExcludePath
    {
        get { return ""; }
    }

    //检查器的优先级，值越高越靠前执行
    public virtual int Priority
    {
        get { return 0; }
    }

    //警告日志工具函数，用于子类输出警告
    public void LogWarning( string log )
    {
        H3DAssetCheckerFramework.GetInstance().LogWarning(log);
    }

    public void LogWarning(string log, UnityEngine.Object context)
    {
        H3DAssetCheckerFramework.GetInstance().LogWarning(log, context); 
    }


    //错误日志工具函数，用于子类输出错误信息
    public void LogError( string log )
    {
        H3DAssetCheckerFramework.GetInstance().LogError(log);
    }

    public void LogError(string log, UnityEngine.Object context)
    {
        H3DAssetCheckerFramework.GetInstance().LogError(log, context);
    }

    //刷新指定路径资源，目前功能是重新导入指定路径资源
    public void RefreshAsset( string path )
    {
        AssetDatabase.ImportAsset(path);
    }

    //检查接口，所有检查器都必需复写此函数
    public virtual void Check( UnityEngine.Object assetObj , AssetImporter assetImporter , string assetPath , bool firstImport , ref bool needImport )
    {
      
    }

    public virtual void PostCheck(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    {

    }

    public static ResouceType QueryAssetResType(Object assetObj )
    { 
        if( assetObj is Texture )
        {
            return ResouceType.TEXTURE;
        }else if( assetObj is AudioClip )
        {
            return ResouceType.AUDIOCLIP;
        }
        else if ( assetObj is Material )
        {
            return ResouceType.MATERIAL;
        }
        else if ( assetObj is PhysicMaterial )
        {
            return ResouceType.PHYMATERIAL;
        }else if( assetObj is Font ){
            return ResouceType.FONT;
        }
        else if ( assetObj is Shader )
        {
            return ResouceType.SHADER;
        }else if( assetObj is GameObject )
        {
            string path =  AssetDatabase.GetAssetPath(assetObj); 
            if( path.ToLower().EndsWith(".fbx") )
            {//导入模型资源文件
                return ResouceType.FBX_MODEL;
            }
            else if (PrefabUtility.GetPrefabType(assetObj) == PrefabType.Prefab)
            {
                return ResouceType.PREFAB;
            }
        }
        else if (assetObj is AnimationClip)
        {
            return ResouceType.ANIMATIONCLIP;
        }

        return ResouceType.NONE;
    }
}

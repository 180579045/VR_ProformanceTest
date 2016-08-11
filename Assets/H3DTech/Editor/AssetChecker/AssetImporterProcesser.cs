using UnityEngine;
using System.Collections;
using UnityEditor;

  
public class AssetImporterProcesser : AssetPostprocessor
{ 

    public override int GetPostprocessOrder()
    {
        return 0;
    }

    public override uint GetVersion()
    {
        return 0;
    }

    public void OnPreprocessTexture()
    { 
        CheckResourceByType(H3DAssetChecker.ResouceType.TEXTURE);
    }

    public void OnPreprocessSpeedTree()
    { 
    }

    public void OnPreprocessModel()
    { 
        CheckResourceByType(H3DAssetChecker.ResouceType.FBX_MODEL);
    }

    public void OnPreprocessAudio()
    { 
        CheckResourceByType(H3DAssetChecker.ResouceType.AUDIOCLIP);
    }

    public void OnPreprocessAnimation()
    { 
        CheckResourceByType(H3DAssetChecker.ResouceType.ANIMATIONCLIP);
    }

    public void OnPostprocessTexture(Texture2D texture)
    {
        PostCheckResourceByType(texture, H3DAssetChecker.ResouceType.TEXTURE);
    }

    public void OnPostprocessSpeedTree( GameObject go )
    { 
    }

    public void OnPostprocessModel( GameObject go )
    {
        PostCheckResourceByType(go, H3DAssetChecker.ResouceType.FBX_MODEL);
    }

    public void OnPostprocessAudio( AudioClip clip )
    {
        PostCheckResourceByType(clip, H3DAssetChecker.ResouceType.AUDIOCLIP);
    }

    public void OnPostprocessGameObjectWithUserProperties( GameObject go , string[] propNames , object[] values )
    { 
    }

    public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    { 
    }

    public void CheckResourceByType( H3DAssetChecker.ResouceType resType )
    {
        if (H3DAssetCheckerConfig.GetInstance().isAssetCheckerOff)
            return;

        bool firstImport = !H3DAssetCheckerFramework.HasAssetImportMark(assetImporter);  
        bool needImport = false;
        var checkerList = H3DAssetCheckerFramework.GetInstance().GetAssetCheckerList(resType);
        foreach (var checker in checkerList)
        {
            if (H3DAssetCheckerUtil.IsPathInclude(assetImporter.assetPath, checker))
            {
                checker.Check(null, assetImporter, assetImporter.assetPath, firstImport, ref needImport);
            }
        }
    }
    
    public void PostCheckResourceByType( UnityEngine.Object assetObj , H3DAssetChecker.ResouceType resType )
    {
        if (H3DAssetCheckerConfig.GetInstance().isAssetCheckerOff)
            return;

        bool firstImport = !H3DAssetCheckerFramework.HasAssetImportMark(assetImporter);
        if (firstImport)
        {
            H3DAssetCheckerFramework.MarkAssetImporterAsAlreadyImported(assetImporter);
        }

        bool needImport = false;
        var checkerList = H3DAssetCheckerFramework.GetInstance().GetAssetCheckerList(resType);
        foreach (var checker in checkerList)
        {
            if (H3DAssetCheckerUtil.IsPathInclude(assetImporter.assetPath, checker))
            {
                checker.PostCheck(assetObj, assetImporter, assetImporter.assetPath, firstImport, ref needImport);
            }
        }
    }
}

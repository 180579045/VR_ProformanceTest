using UnityEngine;
using System.Collections;
using UnityEditor;

public class H3DMaterialPathChecker : H3DAssetPathChecker
{

    //指定此路径检测器关注的路径
    public override string FilterPath
    {
        get { return "Assets/Materials/"; }
    }

    public override void Check(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    {  
        if( !assetPath.EndsWith(".mat") )
        {//若在Materials路径下出现非材质文件，报错提示
            LogError(assetPath + " 不是材质！"+FilterPath+"只可存放材质球！", assetObj);
        }
    }
 
}

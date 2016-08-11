using UnityEngine;
using System.Collections;
using UnityEditor;

//将FBX模型的缩放设置为1
public class H3DFBXImportScaleFactorChecker : H3DAssetChecker 
{
    //指定此资源检查器所关注的资源类型为 FBX
    public override ResouceType ResType
    {
        get { return H3DAssetChecker.ResouceType.FBX_MODEL; }
    }

    public override void Check(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    {
        ModelImporter imp = assetImporter as ModelImporter;
        if (imp == null)
            return; 

        if (imp.globalScale != 1.0f)
        {
            imp.globalScale = 1.0f;
            //需要重新导入
            needImport = true; 
        }  
    } 
}

using UnityEngine;
using System.Collections;
using UnityEditor;


//对于不使用H3D/ Shader的材质球报Warning
public class H3DMaterialShaderChecker : H3DAssetChecker
{
    public override ResouceType ResType
    {
        get { return ResouceType.MATERIAL; }
    }
     

    public override void Check(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    {
        Material mat = assetObj as Material;
        if (mat == null)
            return;
         
        if (!mat.shader.name.ToLower().StartsWith("h3d/"))
        {
            LogWarning(assetPath + ":请尽量使用 H3D/... 分类下的Shader!",assetObj);
        }
    }
	 
}
 
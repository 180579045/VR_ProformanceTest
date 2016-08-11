using UnityEngine;
using System.Collections;
using UnityEditor;

//导入的模型材质名以模型材质球名为准
public class H3DFBXImportMaterialImportChecker : H3DAssetChecker
{ 
    public override ResouceType ResType
    {
        get { return H3DAssetChecker.ResouceType.FBX_MODEL; }
    }

    public override void Check(UnityEngine.Object assetObj, AssetImporter assetImporter, string assetPath, bool firstImport, ref bool needImport)
    {
        ModelImporter imp = assetImporter as ModelImporter;
        if (imp == null)
            return;
          
        if (imp.materialName != ModelImporterMaterialName.BasedOnMaterialName)
        {
            imp.materialName = ModelImporterMaterialName.BasedOnMaterialName;
            needImport = true;
        }
    }
 
}

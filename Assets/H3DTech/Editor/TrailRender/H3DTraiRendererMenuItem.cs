using UnityEngine;
using UnityEditor;

public class H3DTraiRendererMenuItem
{
    [MenuItem("H3D/自定义条带/创建条带")]
    static void CreateH3DTrailRenderer()
    {
        GameObject newGO = new GameObject("H3DTrail");
        newGO.AddComponent<H3DTrailRender>();
    }
}
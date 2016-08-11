using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.Collections.Generic;

[ExecuteInEditMode]
public class UIPrefabNode : MonoBehaviour
{
    public string prefabPath = "";
    [HideInInspector]
    public Vector3 prefabLocalPos = Vector3.zero;
    [HideInInspector]
    public bool SavePivot = false;
    [HideInInspector]
    public UIWidget.Pivot origPivot = UIWidget.Pivot.BottomLeft;
    void Awake()
    {
#if !UNITY_EDITOR
        InstancePrefab(this);
#endif
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        
    }

    void OnEnable()
    {
        
    }

    void Update()
    {
        if (EditorApplication.isPlaying)
            return;
    }

    void OnDrawGizmos()
    {
        
    }
    void OnDrawGizmosSelected()
    {
        
    }
   
    [PostProcessScene]
    public static void OnPostprocessScene()
    {
        foreach (UIPrefabNode pi in UnityEngine.Object.FindObjectsOfType(typeof(UIPrefabNode)))
            InstancePrefab(pi);
    }

#endif
    
    public static void InstancePrefab(UIPrefabNode pi)
    {
        if (pi.prefabPath.Length == 0 || !pi.enabled)
            return;

        pi.enabled = false;

        string sPath = CheckPath(pi.prefabPath);
        GameObject goPrefab;
#if UNITY_EDITOR
        goPrefab = AssetDatabase.LoadAssetAtPath(sPath, typeof(GameObject)) as GameObject; 
#else
        goPrefab = Resources.Load(Trans2ResourcesPath(sPath), typeof(GameObject)) as GameObject;
#endif
        GameObject go = GameObject.Instantiate(goPrefab) as GameObject;
        Quaternion rot = go.transform.localRotation;
        Vector3 scale = go.transform.localScale;

        go.transform.parent = pi.transform;

        go.transform.localPosition = pi.prefabLocalPos;
        go.transform.localScale = scale;
        go.transform.localRotation = rot;

        foreach (UIPrefabNode childPi in go.GetComponentsInChildren<UIPrefabNode>())
            InstancePrefab(childPi);
    }

    public static string CheckPath(string sPrefabPath)
    {
        string s = sPrefabPath.Replace('\\', '/');
        string sSuffix = ".prefab";
        if (s.EndsWith(sSuffix))
            return s;

        s += sSuffix;
        return s;
    }

    public static string Trans2ResourcesPath(string path)
    {
        string temp = path.ToLower();
        int i = temp.LastIndexOf("resources");
        temp = temp.Substring(i + "resources".Length + 1);
        int j = temp.LastIndexOf(".");
        return temp.Substring(0, j);

    }

}
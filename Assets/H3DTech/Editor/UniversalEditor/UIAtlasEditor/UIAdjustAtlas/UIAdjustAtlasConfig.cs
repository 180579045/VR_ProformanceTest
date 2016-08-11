using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

public class UIAdjustAtlasConfig : EditorWindow
{
    public string m_strProjectName = "New Project";

    public string m_projectPath = string.Empty;
    public string m_imageBasePath = string.Empty;
    public string m_atlasOutputPath = string.Empty;
    public string m_consistencyPrefabPath = string.Empty;
    public string m_consisitencyResultPath = string.Empty;
    public string m_referencePrefabPath = string.Empty;
    public string m_referenceScenePath = string.Empty;
    public string m_referenceResultPath = string.Empty;

    void OnEnable()
    {
        UIAdjustAtlasEditorModel.GetInstance().ReadProjectPathConfig(out m_projectPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadImageBasePathConfig(out m_imageBasePath);
        UIAdjustAtlasEditorModel.GetInstance().ReadConsistencyResultPathConfig(out m_consisitencyResultPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadConsistencyPrefabPathConfig(out m_consistencyPrefabPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadReferencePrefabPathConfig(out m_referencePrefabPath);
        UIAdjustAtlasEditorModel.GetInstance().ReadReferenceScenePathConfig(out m_referenceScenePath);
        UIAdjustAtlasEditorModel.GetInstance().ReadReferenceResultPathConfig(out m_referenceResultPath);
    }

    void OnGUI()
    {
        bool isForceBuild = UIAdjustAtlasEditorModel.GetInstance().IsForceBuild;
        maxSize = new Vector2(800f, 600f);
        minSize = new Vector2(800f, 600f);

        GUILayout.BeginVertical();

        GUILayout.Space(20f);
        GUILayout.Label("Common设定项:", GUILayout.Width(100f));

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Project路径", GUILayout.Width(80f));
        GUI.SetNextControlName("配置Project路径");
        m_projectPath = GUILayout.TextField(m_projectPath, 150, GUILayout.Width(600f));
        if(GUILayout.Button("配置",GUILayout.Width(76f)))
        {
            ConfigProjectPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Sprite图库路径", GUILayout.Width(80f));
        GUI.SetNextControlName("配置Sprite图库路径");
        m_imageBasePath = GUILayout.TextField(m_imageBasePath, 150, GUILayout.Width(600f));
        if (GUILayout.Button("配置",GUILayout.Width(76f)))
        {
            ConfigImageBasePath();
        } 
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        isForceBuild = GUILayout.Toggle(isForceBuild, "强制生成atlas");
        UIAdjustAtlasEditorModel.GetInstance().IsForceBuild = isForceBuild;

        GUILayout.Space(30f);
        GUILayout.Label("一致性检查设定项:", GUILayout.Width(100f));

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("一致性检查用prefab路径", GUILayout.Width(130f));
        GUI.SetNextControlName("一致性检查用prefab路径");
        m_consistencyPrefabPath = GUILayout.TextField(m_consistencyPrefabPath, 150, GUILayout.Width(560f));
        if (GUILayout.Button("配置", GUILayout.Width(76f)))
        {
            ConfigConsistencyPrefabPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("一致性检查结果路径", GUILayout.Width(110f));
        GUI.SetNextControlName("一致性检查结果路径");
        m_consisitencyResultPath = GUILayout.TextField(m_consisitencyResultPath, 150, GUILayout.Width(580f));
        if (GUILayout.Button("配置", GUILayout.Width(76f)))
        {
            ConfigConsistencyResultPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30f);
        GUILayout.Label("引用关系导出设定项:", GUILayout.Width(110f));

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("关联Prefab路径", GUILayout.Width(110f));
        GUI.SetNextControlName("关联Prefab路径");
        m_referencePrefabPath = GUILayout.TextField(m_referencePrefabPath, 150, GUILayout.Width(580f));
        if (GUILayout.Button("配置", GUILayout.Width(76f)))
        {
            ConfigReferencePrefabPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("关联Scene路径", GUILayout.Width(110f));
        GUI.SetNextControlName("关联Scene路径");
        m_referenceScenePath = GUILayout.TextField(m_referenceScenePath, 150, GUILayout.Width(580f));
        if (GUILayout.Button("配置", GUILayout.Width(76f)))
        {
            ConfigReferenceScenePath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("引用关系导出路径", GUILayout.Width(110f));
        GUI.SetNextControlName("引用关系导出路径");
        m_referenceResultPath = GUILayout.TextField(m_referenceResultPath, 150, GUILayout.Width(580f));
        if (GUILayout.Button("配置", GUILayout.Width(76f)))
        {
            ConfigReferenceResultPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.Used)
        {
            string U3DAssetPath = "Assets/";

            switch(GUI.GetNameOfFocusedControl())
            {
                case "配置Project路径":
                    UIAdjustAtlasEditorModel.GetInstance().WriteProjectPathConfig(m_projectPath);
                    break;

                case "配置Sprite图库路径":
                    UIAdjustAtlasEditorModel.GetInstance().WriteImageBasePathConfig(m_imageBasePath);
                    break;

                case "一致性检查结果路径":
                    UIAdjustAtlasEditorModel.GetInstance().WriteConsistencyResultPathConfig(m_consisitencyResultPath);
                    break;

                case "一致性检查用prefab路径":
                    if (m_consistencyPrefabPath.Contains(U3DAssetPath))
                    {
                        UIAdjustAtlasEditorModel.GetInstance().WriteConsistencyPrefabPathConfig(m_consistencyPrefabPath);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("配置失败", "\n请设置Assets/下的路径", "确认");
                        UIAdjustAtlasEditorModel.GetInstance().ReadConsistencyPrefabPathConfig(out m_consistencyPrefabPath);
                    }
                    break;

                case "关联Prefab路径":
                    if (m_referencePrefabPath.Contains(U3DAssetPath))
                    {
                        UIAdjustAtlasEditorModel.GetInstance().WriteReferencePrefabPathConfig(m_referencePrefabPath);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("配置失败", "\n请设置Assets/下的路径", "确认");
                        UIAdjustAtlasEditorModel.GetInstance().ReadReferencePrefabPathConfig(out m_referencePrefabPath);
                    }
                    break;

                case "关联Scene路径":
                    if (m_referenceScenePath.Contains(U3DAssetPath))
                    {
                        UIAdjustAtlasEditorModel.GetInstance().WriteReferenceScenePathConfig(m_referenceScenePath);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("配置失败", "\n请设置Assets/下的路径", "确认");
                        UIAdjustAtlasEditorModel.GetInstance().ReadReferenceScenePathConfig(out m_referenceScenePath);
                    }

                    break;

                case "引用关系导出路径":
                    UIAdjustAtlasEditorModel.GetInstance().WriteReferenceResultPathConfig(m_referenceResultPath);
                    break;

                default:
                    break;
            }
        }

    }

    private void ConfigProjectPath()
    {
        string projectPath = string.Empty;

        projectPath = EditorUtility.SaveFolderPanel("配置Project路径", UIAtlasEditorConfig.ProjectPath, "");
        if (!string.IsNullOrEmpty(projectPath))
        {
            UIAdjustAtlasEditorModel.GetInstance().WriteProjectPathConfig(projectPath + "/");
            m_projectPath = projectPath + "/";
        }
    }

    private void ConfigImageBasePath()
    {
        string imageBasePath = string.Empty;

        imageBasePath = EditorUtility.SaveFolderPanel("配置Spsrite图库路径", UIAtlasEditorConfig.ImageBasePath, "");
        if (!string.IsNullOrEmpty(imageBasePath))
        {
            UIAdjustAtlasEditorModel.GetInstance().WriteImageBasePathConfig(imageBasePath + "/");
            m_imageBasePath = imageBasePath + "/";
        }
    }

    private void ConfigConsistencyResultPath()
    {
        string consistencyResultPath = string.Empty;

        consistencyResultPath = EditorUtility.SaveFolderPanel("配置一致性检查结果路径", UIAtlasEditorConfig.ConsistencyResultPath, "");
        if (!string.IsNullOrEmpty(consistencyResultPath))
        {
            UIAdjustAtlasEditorModel.GetInstance().WriteConsistencyResultPathConfig(consistencyResultPath + "/");
            m_consisitencyResultPath = consistencyResultPath + "/";
        }
    }

    private void ConfigConsistencyPrefabPath()
    {
        string consistencyPrefabPath = string.Empty;

        consistencyPrefabPath = EditorUtility.SaveFolderPanel("配置一致性检查用Prefab路径", UIAtlasEditorConfig.ConsistencyPreafabPath, "");
        if (!string.IsNullOrEmpty(consistencyPrefabPath))
        {
            string U3DAssetPath = UnityEngine.Application.dataPath;
            if (!consistencyPrefabPath.Contains(U3DAssetPath))
            {
                EditorUtility.DisplayDialog("配置失败", "\n请设置Assets/下的路径", "确认");
                return;
            }

            string relativityPath = consistencyPrefabPath.Substring(U3DAssetPath.Length - "Assets".Length);

            UIAdjustAtlasEditorModel.GetInstance().WriteConsistencyPrefabPathConfig(relativityPath + "/");
            m_consistencyPrefabPath = relativityPath + "/";
        }
    }

    private void ConfigReferencePrefabPath()
    {
        string referencePrefabPath = string.Empty;

        referencePrefabPath = EditorUtility.SaveFolderPanel("配置关联Prefab路径", UIAtlasEditorConfig.ReferencePrefabPath, "");
        if (!string.IsNullOrEmpty(referencePrefabPath))
        {
            string U3DAssetPath = UnityEngine.Application.dataPath;
            if (!referencePrefabPath.Contains(U3DAssetPath))
            {
                EditorUtility.DisplayDialog("配置失败", "\n请设置Assets/下的路径", "确认");
                return;
            }

            string relativityPath = referencePrefabPath.Substring(U3DAssetPath.Length - "Assets".Length);

            UIAdjustAtlasEditorModel.GetInstance().WriteReferencePrefabPathConfig(relativityPath + "/");
            m_referencePrefabPath = relativityPath + "/";
        }
    }

    private void ConfigReferenceScenePath()
    {
        string referenceScenePath = string.Empty;

        referenceScenePath = EditorUtility.SaveFolderPanel("配置一致性检查用Prefab路径", UIAtlasEditorConfig.ReferenceScenePath, "");
        if (!string.IsNullOrEmpty(referenceScenePath))
        {
            string U3DAssetPath = UnityEngine.Application.dataPath;
            if (!referenceScenePath.Contains(U3DAssetPath))
            {
                EditorUtility.DisplayDialog("配置失败", "\n请设置Assets/下的路径", "确认");
                return;
            } 
            
            string relativityPath = referenceScenePath.Substring(U3DAssetPath.Length - "Assets".Length);

            UIAdjustAtlasEditorModel.GetInstance().WriteReferenceScenePathConfig(relativityPath + "/");
            m_referenceScenePath = relativityPath + "/";
        }
    }

    private void ConfigReferenceResultPath()
    {
        string referenceResultPath = string.Empty;

        referenceResultPath = EditorUtility.SaveFolderPanel("配置引用关系导出路径", UIAtlasEditorConfig.ReferenceResultPath, "");
        if (!string.IsNullOrEmpty(referenceResultPath))
        {
            UIAdjustAtlasEditorModel.GetInstance().WriteReferenceResultPathConfig(referenceResultPath + "/");
            m_referenceResultPath = referenceResultPath + "/";
        }
    }
}

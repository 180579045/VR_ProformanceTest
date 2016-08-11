using UnityEngine;
using UnityEditor;
using System.IO;

[CanEditMultipleObjects, CustomEditor(typeof(UIPrefabNode))]
public class H3DUIPrefabNodeInspector : Editor
{
    private SerializedObject m_prefabNode;
    private SerializedProperty m_prefabPath;
    private string m_textFieldName = "UIPrefabNode_PrefabPath";
    private string m_inputStr = string.Empty;
    void OnEnable()
    {
        m_prefabNode = new SerializedObject(target);
        m_prefabPath = m_prefabNode.FindProperty("prefabPath");
        m_inputStr = m_prefabPath.stringValue;
    }

    public override void OnInspectorGUI() 
    {
        GUILayoutOption[] guiLayoutOption = new GUILayoutOption[] { GUILayout.Width(200f), GUILayout.ExpandWidth(true) };
       
        m_prefabNode.Update();

        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();
     
        EditorGUILayout.LabelField("Prefab Path:", GUILayout.Width(80f));
    
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));   
     
        GUI.SetNextControlName(m_textFieldName);
        EditorGUILayout.LabelField(m_inputStr, guiLayoutOption);
        EditorGUILayout.EndHorizontal();
    
        EditorGUILayout.EndHorizontal(); 

     
        GUILayout.Space(10f);
        GUI.color = Color.green;
        if (GUILayout.Button("配置"))
        {
            ConfigPrefabPath();
        }
        GUI.color = Color.white;


        if ((Event.current.keyCode == KeyCode.Return) && (Event.current.type == EventType.Used))
        {
            if (GUI.GetNameOfFocusedControl() == m_textFieldName)
            {
                m_prefabPath.stringValue = m_inputStr;
            }
        }

        m_prefabNode.ApplyModifiedProperties();
    }

    private void ConfigPrefabPath()
    {
        string prefabPath = string.Empty;
        string defaultPath = string.Empty;
        string U3DAssetPath = UnityEngine.Application.dataPath;

        if (string.IsNullOrEmpty(m_prefabPath.stringValue))
        {
            defaultPath = "Assets/";
        }
        else
        {
            defaultPath = Path.GetDirectoryName(m_prefabPath.stringValue);
        }

        prefabPath = EditorUtility.OpenFilePanel("配置Prefab Path", defaultPath, "prefab");
        if(!string.IsNullOrEmpty(prefabPath))
        {
            if (prefabPath.Contains(U3DAssetPath))
            {
                prefabPath = prefabPath.Substring(U3DAssetPath.Length - "Assets".Length);
               
                m_prefabPath.stringValue = prefabPath;
                m_inputStr = prefabPath;
            }
            else
            {
                EditorUtility.DisplayDialog("配置失败",
                                             "请选择Assets/下的文件！",
                                             "确认");
            }
        }

    }
}
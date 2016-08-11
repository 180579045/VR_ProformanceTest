
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(H3DTrailRender))]
public class H3DTrailRenderEditor : Editor
{
    private SerializedObject m_TrailRender;

    private SerializedProperty m_CastShadows;
    private SerializedProperty m_ReceiveShadows;
    private SerializedProperty m_Time;
    public SerializedProperty m_StartWidth;
    public SerializedProperty m_EndWidth;
    public SerializedProperty m_MinVerDis;
    public SerializedProperty m_IsAutoDestruct;
    public SerializedProperty m_IsAlwaysFaceToCam;
    public SerializedProperty m_RotationAngle;
    public SerializedProperty m_TrailMaterial;
    public SerializedProperty m_Colors;
    private H3DTrailRender render = null;
    private void OnEnable()
    {
        render = (H3DTrailRender)target;

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            //render.InitTrailGO();
            EditorApplication.update += update;
        }

#endif

        m_TrailRender = new SerializedObject(target);
        m_CastShadows = m_TrailRender.FindProperty("m_CastShadows");
        m_ReceiveShadows = m_TrailRender.FindProperty("m_ReceiveShadows");
        m_Time = m_TrailRender.FindProperty("m_Time");
        m_StartWidth = m_TrailRender.FindProperty("m_StartWidth");
        m_EndWidth = m_TrailRender.FindProperty("m_EndWidth");
        m_MinVerDis = m_TrailRender.FindProperty("m_MinVerDis");
        m_IsAutoDestruct = m_TrailRender.FindProperty("m_IsAutoDestruct");
        m_IsAlwaysFaceToCam = m_TrailRender.FindProperty("m_IsAlwaysFaceToCam");
        m_RotationAngle = m_TrailRender.FindProperty("m_RotationAngle");
        m_TrailMaterial = m_TrailRender.FindProperty("m_TrailMaterial");
        m_Colors = m_TrailRender.FindProperty("m_Colors");
    }

    protected virtual void OnDestroy()
    {
        if(target == null)
        {
            render.DestructTrail();

        }
    }
    void update()
    {
        if (null == render)
        {
            return;
        }

        if (render.IsCommonError())
        {
            render.Clear();
            return;
        }

        render.CheckCurrLayer();
        render.FixSection();
        render.FixVertex();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        //m_CastShadows.boolValue = EditorGUILayout.Toggle("Cast Shadows", m_CastShadows.boolValue);
        EditorGUILayout.PropertyField(m_CastShadows, true);
        m_ReceiveShadows.boolValue = EditorGUILayout.Toggle("Receive Shadows", m_ReceiveShadows.boolValue);
        m_Time.floatValue = EditorGUILayout.FloatField("Time", m_Time.floatValue);
        m_StartWidth.floatValue = EditorGUILayout.FloatField("Start Width", m_StartWidth.floatValue);
        m_EndWidth.floatValue = EditorGUILayout.FloatField("End Width", m_EndWidth.floatValue);
        m_MinVerDis.floatValue = EditorGUILayout.FloatField("Min Vertex Distance", m_MinVerDis.floatValue);
        m_IsAutoDestruct.boolValue = EditorGUILayout.Toggle("Is Auto Destruct", m_IsAutoDestruct.boolValue);
        m_IsAlwaysFaceToCam.boolValue = EditorGUILayout.Toggle("Is Always FaceTo Camera", m_IsAlwaysFaceToCam.boolValue);
        m_RotationAngle.floatValue = EditorGUILayout.FloatField("RotationAngle", m_RotationAngle.floatValue);
        EditorGUILayout.PropertyField(m_TrailMaterial, true);
        render.UpdateTrailGOMat();
        EditorGUILayout.LabelField("Colors");
        for (int i = 0; i < m_Colors.arraySize; i++)
        {
            SerializedProperty color = m_Colors.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(color);
        }
        if (GUILayout.Button("Clear"))
        {
            render.Clear();
        }

        EditorGUILayout.EndVertical();

        m_TrailRender.ApplyModifiedProperties();
    }
}

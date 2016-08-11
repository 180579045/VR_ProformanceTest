using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(H3DScrollView), true)]
public class H3DScrollViewInspector : Editor 
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty movementTypeSp = serializedObject.FindProperty("mMovementType"); 
        H3DScrollView.H3DScrollViewMovementType moveType = (H3DScrollView.H3DScrollViewMovementType)movementTypeSp.enumValueIndex;      

        H3DInspectorTool.DrawProperty("移动方式", serializedObject, "mMovementType",true);
        H3DInspectorTool.DrawProperty("拖拽效果", serializedObject, "mDragEffect", true);

        

        H3DInspectorTool.DrawProperty("列数限制", serializedObject, "mColumnLimit", true);
        
        SerializedProperty columnLimitSp = serializedObject.FindProperty("mColumnLimit");
        columnLimitSp.intValue = Mathf.Max(1, columnLimitSp.intValue); 

        SerializedProperty dragEffSp = serializedObject.FindProperty("mDragEffect");
        H3DScrollView.H3DScrollViewDragEffect dragEff = (H3DScrollView.H3DScrollViewDragEffect)dragEffSp.enumValueIndex;
        if (
             dragEff == H3DScrollView.H3DScrollViewDragEffect.Momentum ||
             dragEff == H3DScrollView.H3DScrollViewDragEffect.MomentumAndSpring
            )
        {
            H3DInspectorTool.DrawProperty("动量", serializedObject, "mMomentumAmount", true);
        }


        if (
             moveType == H3DScrollView.H3DScrollViewMovementType.Horizontal || 
             moveType == H3DScrollView.H3DScrollViewMovementType.Unrestricted ||
             columnLimitSp.intValue > 1
            )
        {
            H3DInspectorTool.DrawProperty("条目宽度(像素)", serializedObject, "mItemWidth", true);
            SerializedProperty itemWidthSp = serializedObject.FindProperty("mItemWidth");
            itemWidthSp.intValue = Mathf.Max(0, itemWidthSp.intValue); 
        }

        if (
             moveType == H3DScrollView.H3DScrollViewMovementType.Vertical ||
             moveType == H3DScrollView.H3DScrollViewMovementType.Unrestricted ||
             columnLimitSp.intValue > 1
            )
        {
            H3DInspectorTool.DrawProperty("条目高度(像素)", serializedObject, "mItemHeight", true);
            SerializedProperty itemHeightSp = serializedObject.FindProperty("mItemHeight");
            itemHeightSp.intValue = Mathf.Max(0, itemHeightSp.intValue); 
        }

        if (
             moveType == H3DScrollView.H3DScrollViewMovementType.Horizontal ||
             moveType == H3DScrollView.H3DScrollViewMovementType.Unrestricted
            )
        {
            H3DInspectorTool.DrawProperty("水平滚动条", serializedObject, "mHorizonScrollBar", true);
        }

        if (
             moveType == H3DScrollView.H3DScrollViewMovementType.Vertical ||
             moveType == H3DScrollView.H3DScrollViewMovementType.Unrestricted
            )
        {
            H3DInspectorTool.DrawProperty("垂直滚动条", serializedObject, "mVerticalScrollBar", true);
        }


        bool dirty = serializedObject.ApplyModifiedProperties();
        
        if( dirty )
        {
           H3DScrollView scrollView = target as H3DScrollView;
           if (scrollView != null)
           {
               scrollView.RecalcLayout();
           }
        }
    }
     
}

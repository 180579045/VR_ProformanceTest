using UnityEngine;
using System;

public class CurveEditorCtrl : EditorControl
{
    public Rect CurveRange
    {
        get { return curveRange; }
        set { curveRange = value; }
    }

    public Color CurveColor
    {
        get { return curveColor; }
        set { curveColor = value; }
    }

    public CurveEditorCtrl(AnimationCurve value)
    {
        this.CurrValue = value;
    }

    public override object CurrValue
    {
        get { return currValue; }
        set
        {
            currValue = value as AnimationCurve;
            if (null == currValue) 
            {
                currValue = AnimationCurve.Linear(0, 0, 0, 0);
                Debug.Log("为CurveEditorCtrl控件的CurrValue赋值了错误类型数据!");
            }
        }
    }

    public CurveEditorCtrl()
    {
        this.CurrValue = AnimationCurve.Linear(0, 0, 0, 0);
    }

    public override GUILayoutOption[] GetOptions()
    {
        if (layoutConstraint.expandWidth == true)
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(Size.height) };
        }
        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) };
    }

    private Rect curveRange = new Rect(0, 0, 0, 0);
    private Color curveColor = Color.white;
}
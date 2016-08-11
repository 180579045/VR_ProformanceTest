using UnityEngine;
using System;

public class ObjectFieldCtrl : EditorControl
{
    public Type ObjectType
    {
        get { return objectType; }
        set { objectType = value; }
    }

    public bool IsAlowSceneObjects
    {
        get { return isAlowSceneObjects; }
        set { isAlowSceneObjects = value; }
    }
    
    public ObjectFieldCtrl()
    {
        this.CurrValue = null;
        this.Size = new Rect(0f, 0f, 300f, 16f);
    }

    public ObjectFieldCtrl(object value)
    {
        this.CurrValue = value;
        this.Size = new Rect(0f, 0f, 300f, 16f);
    }

    public override GUILayoutOption[] GetOptions()
    {
        if (layoutConstraint.expandWidth == true)
        {
            return new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(Size.height) };
        }
        return new GUILayoutOption[] { GUILayout.Width(Size.width), GUILayout.Height(Size.height) };
    }

    private Type objectType = typeof(object);
    private bool isAlowSceneObjects = false;
}
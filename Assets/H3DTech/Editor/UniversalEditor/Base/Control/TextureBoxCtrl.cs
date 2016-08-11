using UnityEngine;
using System.Collections;

public class TextureBoxCtrl : EditorControl
{
    public Texture Image
    {
        get { return m_Image; }
        set { m_Image = value; }
    }

    private Texture m_Image = null;
    public override GUIStyle GetStyle()
    {
        return SpecialEffectEditorStyle.PanelBox;
    }

    public override GUILayoutOption[] GetOptions()
    {
        return new GUILayoutOption[] {   
            GUILayout.ExpandHeight(true), 
            GUILayout.ExpandWidth(true) };
    }
}
using UnityEngine;
using System.Collections;
using UnityEditor;

public class TextureBoxRenderStrategy : EditorRenderStrategy
{
    public override void Visit(EditorControl c)
    {
        TextureBoxCtrl textureBox = c as TextureBoxCtrl;
        if (
               (null == textureBox)
            )
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(!textureBox.Enable);

        //Texture2D backgroundTex = AssetDatabase.LoadAssetAtPath("Assets/H3DTech/Editor/UniversalEditor/UIAtlasEditor/Data/Resources/background.png", typeof(Texture)) as Texture2D;
        EditorGUILayout.BeginVertical(c.GetStyle(), c.GetOptions());
        if (textureBox.Image != null)
        {
            //GUI.DrawTexture(tempRect, backgroundTex, ScaleMode.StretchToFill, true);
            //NGUIEditorTools.DrawTiledTexture(textureBox.LastRect, NGUIEditorTools.backdropTexture);
            EditorGUI.DrawTextureTransparent(textureBox.LastRect, textureBox.Image, ScaleMode.ScaleToFit);
            //EditorGUI.DrawPreviewTexture(textureBox.LastRect, textureBox.Image);
           // GUI.DrawTexture(textureBox.LastRect, textureBox.Image, ScaleMode.ScaleToFit, true);
        }
        EditorGUILayout.EndVertical();

        EditorGUI.EndDisabledGroup();

        c.UpdateLastRect();

        CheckInputEvent(c);
    }
}
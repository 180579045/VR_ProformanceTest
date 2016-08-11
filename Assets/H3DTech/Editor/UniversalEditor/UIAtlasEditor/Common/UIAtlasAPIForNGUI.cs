using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UIAtlasAPIForNGUI
{
    static public List<UIAtlasMaker.SpriteEntry> CreateSprites(List<Texture> textures)
    {
        List<UIAtlasMaker.SpriteEntry> list = new List<UIAtlasMaker.SpriteEntry>();

        foreach (Texture tex in textures)
        {
            Texture2D oldTex = NGUIEditorTools.ImportTexture(tex, true, false, true);
            if (oldTex == null) oldTex = tex as Texture2D;
            if (oldTex == null) continue;

            // If we aren't doing trimming, just use the texture as-is
            if (!NGUISettings.atlasTrimming && !NGUISettings.atlasPMA)
            {
                UIAtlasMaker.SpriteEntry sprite = new UIAtlasMaker.SpriteEntry();
                sprite.SetRect(0, 0, oldTex.width, oldTex.height);
                sprite.tex = oldTex;
                if (oldTex.name.EndsWith("zoomed"))
                {
                    sprite.name = oldTex.name.Substring(0, oldTex.name.Length - "zoomed".Length);
                }
                else
                {
                    sprite.name = oldTex.name;
                }
                sprite.temporaryTexture = false;
                list.Add(sprite);
                continue;
            }

            #region 屏蔽NGUI设定项

            // If we want to trim transparent pixels, there is more work to be done
            Color32[] pixels = oldTex.GetPixels32();

            int xmin = oldTex.width;
            int xmax = 0;
            int ymin = oldTex.height;
            int ymax = 0;
            int oldWidth = oldTex.width;
            int oldHeight = oldTex.height;

            // Find solid pixels
            if (NGUISettings.atlasTrimming)
            {
                for (int y = 0, yw = oldHeight; y < yw; ++y)
                {
                    for (int x = 0, xw = oldWidth; x < xw; ++x)
                    {
                        Color32 c = pixels[y * xw + x];

                        if (c.a != 0)
                        {
                            if (y < ymin) ymin = y;
                            if (y > ymax) ymax = y;
                            if (x < xmin) xmin = x;
                            if (x > xmax) xmax = x;
                        }
                    }
                }
            }
            else
            {
                xmin = 0;
                xmax = oldWidth - 1;
                ymin = 0;
                ymax = oldHeight - 1;
            }

            int newWidth = (xmax - xmin) + 1;
            int newHeight = (ymax - ymin) + 1;

            if (newWidth > 0 && newHeight > 0)
            {
                UIAtlasMaker.SpriteEntry sprite = new UIAtlasMaker.SpriteEntry();
                sprite.x = 0;
                sprite.y = 0;
                sprite.width = oldTex.width;
                sprite.height = oldTex.height;

                // If the dimensions match, then nothing was actually trimmed
                if (!NGUISettings.atlasPMA && (newWidth == oldWidth && newHeight == oldHeight))
                {
                    sprite.tex = oldTex;
                    if (oldTex.name.EndsWith("zoomed"))
                    {
                        sprite.name = oldTex.name.Substring(0, oldTex.name.Length - "zoomed".Length);
                    }
                    else
                    {
                        sprite.name = oldTex.name;
                    }
                    sprite.temporaryTexture = false;
                }
                else
                {
                    // Copy the non-trimmed texture data into a temporary buffer
                    Color32[] newPixels = new Color32[newWidth * newHeight];

                    for (int y = 0; y < newHeight; ++y)
                    {
                        for (int x = 0; x < newWidth; ++x)
                        {
                            int newIndex = y * newWidth + x;
                            int oldIndex = (ymin + y) * oldWidth + (xmin + x);
                            if (NGUISettings.atlasPMA) newPixels[newIndex] = NGUITools.ApplyPMA(pixels[oldIndex]);
                            else newPixels[newIndex] = pixels[oldIndex];
                        }
                    }

                    // Create a new texture
                    sprite.temporaryTexture = true;
                    if (oldTex.name.EndsWith("zoomed"))
                    {
                        sprite.name = oldTex.name.Substring(0, oldTex.name.Length - "zoomed".Length);
                    }
                    else
                    {
                        sprite.name = oldTex.name;
                    }
                    sprite.tex = new Texture2D(newWidth, newHeight);
                    sprite.tex.SetPixels32(newPixels);
                    sprite.tex.Apply();

                    // Remember the padding offset
                    sprite.SetPadding(xmin, ymin, oldWidth - newWidth - xmin, oldHeight - newHeight - ymin);
                }
                list.Add(sprite);
            }
            #endregion
        }
        return list;
    }

    static public bool MakeAtlasPrefab(string outputPath, List<Texture> spriteTextureTable, out Texture2D atlasTex)
    {
        bool bRet = false;
        atlasTex = null;

        if(
               (string.IsNullOrEmpty(outputPath))
            || (null == spriteTextureTable)
            )
        {
            return false;
        }

        GameObject go = AssetDatabase.LoadAssetAtPath(outputPath, typeof(GameObject)) as GameObject;
        string matPath = outputPath.Replace(".prefab", ".mat");

        // Try to load the material
        Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

        // If the material doesn't exist, create it
        if (mat == null)
        {
            Shader shader = Shader.Find(NGUISettings.atlasPMA ? "Unlit/Premultiplied Colored" : "Unlit/Transparent Colored");
            mat = new Material(shader);

            // Save the material
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Load the material so it's usable
            mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
        }

        // Create a new prefab for the atlas
        Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(outputPath);

        if (go == null)
        {
            // Create a new game object for the atlas
            string atlasName = outputPath.Replace(".prefab", "");
            atlasName = atlasName.Substring(outputPath.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
            go = new GameObject(atlasName);
            go.AddComponent<UIAtlas>().spriteMaterial = mat;

            // Update the prefab
            PrefabUtility.ReplacePrefab(go, prefab);
            GameObject.DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        // Select the atlas
        go = AssetDatabase.LoadAssetAtPath(outputPath, typeof(GameObject)) as GameObject;
        NGUISettings.atlas = go.GetComponent<UIAtlas>();
        Selection.activeGameObject = go;


        List<UIAtlasMaker.SpriteEntry> sprites = UIAtlasAPIForNGUI.CreateSprites(spriteTextureTable);

        UpdateUIAtlas(NGUISettings.atlas, sprites, out atlasTex);

        NGUIEditorTools.UpgradeTexturesToSprites(NGUISettings.atlas);
        NGUIEditorTools.RepaintSprites();

        return bRet;
    }

    static private void UpdateUIAtlas(UIAtlas atlas, List<UIAtlasMaker.SpriteEntry> sprites, out Texture2D atlasTex)
    {
        atlasTex = null;
        if (
               (null == atlas)
            || (null == sprites)
            || (0 == sprites.Count)
            )
        {
            return;
        }

        if (UpdateTexture(atlas, sprites, out atlasTex))
        {
            UIAtlasMaker.ReplaceSprites(atlas, sprites);
        }

        UIAtlasMaker.ReleaseSprites(sprites);

        atlas.MarkAsChanged();
    }

    static private bool UpdateTexture(UIAtlas atlas, List<UIAtlasMaker.SpriteEntry> sprites, out Texture2D atlasTex)
    {
        bool bRet = true;
        atlasTex = null;

        Rect[] rects = null;
        ITexturePackagingStrategy maker = null;

        Texture2D[] textures = new Texture2D[sprites.Count];
        atlasTex = atlas.texture as Texture2D;

        atlasTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);


        if (NGUISettings.unityPacking)
        {
            maker = new DefaultTexturePackagingStrategy();
          
            for (int i = 0; i < sprites.Count; ++i) textures[i] = sprites[i].tex;

            rects = maker.Pack(atlasTex, textures, NGUISettings.atlasPadding, null);
        }
        else
        {
            maker = new NGUITexturePackagingStrategy();

            for (int i = 0; i < sprites.Count; ++i) textures[i] = sprites[i].tex;

            rects = maker.Pack(atlasTex, textures, NGUISettings.atlasPadding, null);
        }

        for (int i = 0; i < sprites.Count; ++i)
        {
            Rect rect = NGUIMath.ConvertToPixels(rects[i], atlasTex.width, atlasTex.height, true);

            // Make sure that we don't shrink the textures
            if (Mathf.RoundToInt(rect.width) != textures[i].width)
            {
                bRet = false;
                break;
            }

            UIAtlasMaker.SpriteEntry se = sprites[i];
            se.x = Mathf.RoundToInt(rect.x);
            se.y = Mathf.RoundToInt(rect.y);
            se.width = Mathf.RoundToInt(rect.width);
            se.height = Mathf.RoundToInt(rect.height);
        }

        return bRet;
    }
    static int Compare(UIAtlasMaker.SpriteEntry a, UIAtlasMaker.SpriteEntry b)
    {
        // A is null b is not b is greater so put it at the front of the list
        if (a == null && b != null) return 1;

        // A is not null b is null a is greater so put it at the front of the list
        if (a != null && b == null) return -1;

        // Get the total pixels used for each sprite
        int aPixels = a.width * a.height;
        int bPixels = b.width * b.height;

        if (aPixels > bPixels) return -1;
        else if (aPixels < bPixels) return 1;
        return 0;
    }

    static public List<Texture2D> FixNGUISpriteTextures(List<Texture2D> orgTextures)
    {
        List<Texture2D> NGUITextures = new List<Texture2D>();

        if (!NGUISettings.atlasTrimming && !NGUISettings.atlasPMA)
        {
            return orgTextures;
        }

        foreach(var item in orgTextures)
        {
            Texture2D oldTex = NGUIEditorTools.ImportTexture(item, true, false, true);
            if (null == oldTex)
            {
                oldTex = item as Texture2D;
            }

            if(null == oldTex)
            {
                continue;
            }

            // If we want to trim transparent pixels, there is more work to be done
            Color32[] pixels = oldTex.GetPixels32();

            int xmin = oldTex.width;
            int xmax = 0;
            int ymin = oldTex.height;
            int ymax = 0;
            int oldWidth = oldTex.width;
            int oldHeight = oldTex.height;

            // Find solid pixels
            if (NGUISettings.atlasTrimming)
            {
                for (int y = 0, yw = oldHeight; y < yw; ++y)
                {
                    for (int x = 0, xw = oldWidth; x < xw; ++x)
                    {
                        Color32 c = pixels[y * xw + x];

                        if (c.a != 0)
                        {
                            if (y < ymin) ymin = y;
                            if (y > ymax) ymax = y;
                            if (x < xmin) xmin = x;
                            if (x > xmax) xmax = x;
                        }
                    }
                }
            }
            else
            {
                xmin = 0;
                xmax = oldWidth - 1;
                ymin = 0;
                ymax = oldHeight - 1;
            }

            int newWidth = (xmax - xmin) + 1;
            int newHeight = (ymax - ymin) + 1;

            if (newWidth > 0 && newHeight > 0)
            {
                Texture2D newTex = null;
                // If the dimensions match, then nothing was actually trimmed
                if (!NGUISettings.atlasPMA && (newWidth == oldWidth && newHeight == oldHeight))
                {
                    newTex = oldTex;
                }
                else
                {
                    // Copy the non-trimmed texture data into a temporary buffer
                    Color32[] newPixels = new Color32[newWidth * newHeight];

                    for (int y = 0; y < newHeight; ++y)
                    {
                        for (int x = 0; x < newWidth; ++x)
                        {
                            int newIndex = y * newWidth + x;
                            int oldIndex = (ymin + y) * oldWidth + (xmin + x);
                            if (NGUISettings.atlasPMA) newPixels[newIndex] = NGUITools.ApplyPMA(pixels[oldIndex]);
                            else newPixels[newIndex] = pixels[oldIndex];
                        }
                    }

                    // Create a new texture
                    newTex = new Texture2D(newWidth, newHeight);
                    newTex.SetPixels32(newPixels);
                    newTex.Apply();
                }

                NGUITextures.Add(newTex);
            }
        }

        return NGUITextures;
    }
}
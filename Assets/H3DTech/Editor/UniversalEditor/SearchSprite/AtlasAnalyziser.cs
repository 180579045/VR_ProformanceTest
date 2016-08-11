using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AtlasInfoForSearchSprite
{
    public string AtlasPath = string.Empty;
    public Texture AtlasTexture = null;
    //public string SpriteName = string.Empty;
    //public Rect SpriteRect = new Rect(0, 0, 0, 0);
    public Dictionary<string, Rect> SpriteInfo = new Dictionary<string, Rect>();
}

public class AtlasAnalyziser
{
    public List<string> GetAllAtlasPath()
    {
        List<string> prefabPathTbl = new List<string>();



        return prefabPathTbl;
    }

    public List<AtlasInfoForSearchSprite> VagueSearchAtlasWithSpecifySprite(string spriteName)
    {
        List<AtlasInfoForSearchSprite> atlasInfoTbl = new List<AtlasInfoForSearchSprite>();
        if(string.IsNullOrEmpty(spriteName))
        {
            return null;
        }

        string[] paths = AssetDatabase.GetAllAssetPaths();

        for (int index = 0; index < paths.Length; index++)
        {
            GameObject atlasObj = null;
            if (UtilityForNGUI.IsAtlasPrefab(paths[index], out atlasObj))
            {
                if(null == atlasObj)
                {
                    continue;
                }

                AtlasInfoForSearchSprite newInfo = new AtlasInfoForSearchSprite();
                newInfo.AtlasPath = paths[index];
                newInfo.AtlasTexture = UtilityForNGUI.GetAtlasTexture(atlasObj);
                //newInfo.SpriteInfo = UtilityForNGUI.GetSpriteInfo(atlasObj);
                newInfo.SpriteInfo = VagueGetSpriteInfoInAtlas(spriteName, atlasObj);

                if(
                    (newInfo.SpriteInfo != null)
                    && (newInfo.SpriteInfo.Count != 0)
                    )
                {
                    atlasInfoTbl.Add(newInfo);
                }
                //if (IsSpriteInAtlas(spriteName, newInfo))
                //{
                //    atlasInfoTbl.Add(newInfo);
                //}
            }
        }

        return atlasInfoTbl;
    }

    public List<AtlasInfoForSearchSprite> SearchAtlasWithSpecifySprite(string spriteName)
    {
        List<AtlasInfoForSearchSprite> atlasInfoTbl = new List<AtlasInfoForSearchSprite>();
        if (string.IsNullOrEmpty(spriteName))
        {
            return null;
        }

        string[] paths = AssetDatabase.GetAllAssetPaths();

        for (int index = 0; index < paths.Length; index++)
        {
            GameObject atlasObj = null;
            if (UtilityForNGUI.IsAtlasPrefab(paths[index], out atlasObj))
            {
                if (null == atlasObj)
                {
                    continue;
                }

                AtlasInfoForSearchSprite newInfo = new AtlasInfoForSearchSprite();
                newInfo.AtlasPath = paths[index];
                newInfo.AtlasTexture = UtilityForNGUI.GetAtlasTexture(atlasObj);
                newInfo.SpriteInfo = GetSpriteInfoInAtlas(spriteName, atlasObj);

                if (
                    (newInfo.SpriteInfo != null)
                    && (newInfo.SpriteInfo.Count != 0)
                    )
                {
                    atlasInfoTbl.Add(newInfo);
                }

            }
        }

        return atlasInfoTbl;
    }

    private bool IsSpriteInAtlas(string spriteName, AtlasInfoForSearchSprite atlasInfo)
    {
        bool bRet = false;

        if(
            string.IsNullOrEmpty(spriteName)
            || (null == atlasInfo)
            )
        {
            return false;
        }

        Rect spriteRect = new Rect();

        if (atlasInfo.SpriteInfo.TryGetValue(spriteName, out spriteRect))
        {
            bRet = true;
        }

        return bRet;
    }

    private Dictionary<string, Rect> VagueGetSpriteInfoInAtlas(string spriteName, GameObject atlasObj)
    {
        Dictionary<string, Rect> newInfo = new Dictionary<string, Rect>();
        if (
            string.IsNullOrEmpty(spriteName)
            || (null == atlasObj)
            )
        {
            return null;
        }

        Dictionary<string, Rect> tempInfo = UtilityForNGUI.GetSpriteInfo(atlasObj);
        if(null == tempInfo)
        {
            return null;
        }

        foreach (KeyValuePair<string, Rect> item in tempInfo)
        {
            if (item.Key.Contains(spriteName))
            {
                newInfo.Add(item.Key, item.Value);
            }
        }

        return newInfo;
    }

    private Dictionary<string, Rect> GetSpriteInfoInAtlas(string spriteName, GameObject atlasObj)
    {
        Dictionary<string, Rect> newInfo = new Dictionary<string, Rect>();
        if (
            string.IsNullOrEmpty(spriteName)
            || (null == atlasObj)
            )
        {
            return null;
        }

        Dictionary<string, Rect> tempInfo = UtilityForNGUI.GetSpriteInfo(atlasObj);
        if (null == tempInfo)
        {
            return null;
        }

        Rect spriteRect;
        if (tempInfo.TryGetValue(spriteName, out spriteRect))
        {
            newInfo.Add(spriteName, spriteRect);
        }

        return newInfo;
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public enum UTILITYFORNGUI_ERROR_TYPE
{
    UTILITYFORNGUI_ERROR_NONE = -1,         //操作未发生错误
    UTILITYFORNGUI_ERROR_UNKNOWN = 0,       //未知错误
    UTILITYFORNGUI_ERROR_ISNOT_UISPRITE,    //设定的对象不是UISprite
    UTILITYFORNGUI_ERROR_ISNOT_ATLAS,       //设定的源资源不是Atlas
}

public class UtilityForNGUI
{
    //判断某个资源是否是Atlas
    static public bool IsAtlasPrefab(string prefabPath, out GameObject altasObj)
    {
        bool bRet = false;
        altasObj = null;

        if (string.IsNullOrEmpty(prefabPath))
        {
            return false;
        }

        Object obj = AssetDatabase.LoadMainAssetAtPath(prefabPath);
        GameObject go = obj as GameObject;
        if (go != null)
        {
            UIAtlas atlas = go.GetComponent<UIAtlas>();
            if(atlas != null)
            {
                altasObj = go;
                bRet = true;
            }
        }

        return bRet;
    }

    //获取指定Atlas的纹理
    static public Texture GetAtlasTexture(GameObject go)
    {
        Texture tex = null;

        if (null == go)
        {
            return null;
        }

        UIAtlas atlas = go.GetComponent<UIAtlas>();
        if(null == atlas)
        {
            return null;
        }


        tex = atlas.texture;

        return tex;
    }

    //获取指定Atlas的Sprite信息
    static public Dictionary<string, Rect> GetSpriteInfo(GameObject go)
    {
        if (null == go)
        {
            return null;
        }

        UIAtlas atlas = go.GetComponent<UIAtlas>();
        if(null == atlas)
        {
            return null;
        }

        Dictionary<string, Rect> spriteInfoTbl = new Dictionary<string, Rect>();

        for (int index = 0; index < atlas.spriteList.Count; index++)
        {
            Rect spriteRect = new Rect();
            spriteRect.x = atlas.spriteList[index].x;
            spriteRect.y = atlas.spriteList[index].y;
            spriteRect.width = atlas.spriteList[index].width;
            spriteRect.height = atlas.spriteList[index].height;

            spriteInfoTbl.Add(atlas.spriteList[index].name, spriteRect);
        }

        return spriteInfoTbl;
    }

    //获取Sprite在Atlas中的位置
    static public Rect ConvertToTexCoords(Rect rect, int width, int height)
    {
        return NGUIMath.ConvertToTexCoords(rect, width, height);
    }

    //将Atlas、Sprite设置给UISprite
    static public UTILITYFORNGUI_ERROR_TYPE SetUISprite(GameObject go, string spriteName, string atlasPath)
    {
        UTILITYFORNGUI_ERROR_TYPE errorType = UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_NONE;
        if(
            (null == go)
            || (string.IsNullOrEmpty(spriteName))
            || (string.IsNullOrEmpty(atlasPath))
            )
        {
            return UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_UNKNOWN;
        }

        UISprite uiSprite = go.GetComponent<UISprite>();
        if (null == uiSprite)
        {
            return UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_ISNOT_UISPRITE;
        }

        GameObject atlasObj = AssetDatabase.LoadMainAssetAtPath(atlasPath) as GameObject;
        if(null == atlasObj)
        {
            return UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_UNKNOWN;
        }

        UIAtlas atlas = atlasObj.GetComponent<UIAtlas>();
        if(null == atlas)
        {
            return UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_ISNOT_ATLAS;
        }

        //设置Atlas
        uiSprite.atlas = atlas;

        //设置Sprite
        SerializedObject serializedSprite = new SerializedObject(uiSprite);
        serializedSprite.Update();
        SerializedProperty spriteDataType = serializedSprite.FindProperty("mSpriteName");
        spriteDataType.stringValue = spriteName;
        serializedSprite.ApplyModifiedProperties();
        UnityEditor.EditorUtility.SetDirty(serializedSprite.targetObject);

        return errorType;
    }
}
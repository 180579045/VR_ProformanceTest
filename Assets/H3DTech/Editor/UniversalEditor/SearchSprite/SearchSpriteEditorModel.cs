using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public enum SEARCHSPRITE_ERROR_TYPE
{
    SEARCHSPRITE_NONE_ERROR = -1,           //操作未发生错误
    SEARCHSPRITE_UNKNOWN = 0,               //未知错误
    SEARCHSPRITE_SEARCH_WITH_EMPTY_NAME,    //搜索的Sprite名称为空
    SEARCHSPRITE_SET_IS_NOT_UISPRITE,       //设定的目标不是UISprite
    SEARCHSPRITE_SET_IS_NOT_ATLAS,          //设定的源资源不是Atlas
}

public class SearchSpriteEidtorModel
{
    public SEARCHSPRITE_ERROR_TYPE VagueSearchSprite(string spriteName, out List<AtlasInfoForSearchSprite> atlasInfoTbl)
    {
        atlasInfoTbl = null;
        SEARCHSPRITE_ERROR_TYPE errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;
      
        if(string.IsNullOrEmpty(spriteName))
        {
            errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_SEARCH_WITH_EMPTY_NAME;
        }

        AtlasAnalyziser analyziser = new AtlasAnalyziser();
        atlasInfoTbl = analyziser.VagueSearchAtlasWithSpecifySprite(spriteName);

        return errorType;
    }

    public SEARCHSPRITE_ERROR_TYPE SearchSprite(string spriteName, out List<AtlasInfoForSearchSprite> atlasInfoTbl)
    {
        atlasInfoTbl = null;
        SEARCHSPRITE_ERROR_TYPE errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;

        if (string.IsNullOrEmpty(spriteName))
        {
            errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_SEARCH_WITH_EMPTY_NAME;
            return errorType;
        }

        AtlasAnalyziser analyziser = new AtlasAnalyziser();
        atlasInfoTbl = analyziser.SearchAtlasWithSpecifySprite(spriteName);

        return errorType;
    }
    public SEARCHSPRITE_ERROR_TYPE SetUISprite(GameObject go, string spriteName, string atlasPath)
    {
        SEARCHSPRITE_ERROR_TYPE errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;

        errorType = CheckUtilityForNGUIError(UtilityForNGUI.SetUISprite(go, spriteName, atlasPath)); 

        return errorType;
    }

    private SEARCHSPRITE_ERROR_TYPE CheckUtilityForNGUIError(UTILITYFORNGUI_ERROR_TYPE utilityForNGUIErrorType)
    {
        SEARCHSPRITE_ERROR_TYPE errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;

        switch (utilityForNGUIErrorType)
        {
            case UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_NONE:
                errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;

                break;

            case UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_UNKNOWN:
                errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_UNKNOWN;

                break;

            case UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_ISNOT_UISPRITE:
                errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_SET_IS_NOT_UISPRITE;

                break;

            case UTILITYFORNGUI_ERROR_TYPE.UTILITYFORNGUI_ERROR_ISNOT_ATLAS:
                errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_SET_IS_NOT_ATLAS;

                break;

            default:
                errorType = SEARCHSPRITE_ERROR_TYPE.SEARCHSPRITE_NONE_ERROR;

                break;
        }

        return errorType;
    }

    static private SearchSpriteEidtorModel m_Instance = null;

    public static SearchSpriteEidtorModel GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new SearchSpriteEidtorModel();
        }
        return m_Instance;
    }
    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            SearchSpriteEidtorModel.DestoryInstance();
        }
    }
}
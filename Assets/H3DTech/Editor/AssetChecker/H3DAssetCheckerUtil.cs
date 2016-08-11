using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FreeImageAPI;
using FreeImageAPI.IO;
using FreeImageAPI.Plugins;

public class H3DAssetCheckerUtil 
{
    
    public static bool IsTGATextureAlphaChannelLegal( string path )
    {
        FREE_IMAGE_FORMAT fif = FREE_IMAGE_FORMAT.FIF_UNKNOWN; ;
        fif = FreeImage.GetFileType(path, 0);

        if (fif == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
        {
            fif = FreeImage.GetFIFFromFilename(path);
        }

        if ((fif == FREE_IMAGE_FORMAT.FIF_UNKNOWN) || (FreeImage.FIFSupportsReading(fif) == false))
            return false;

        if (fif != FREE_IMAGE_FORMAT.FIF_TARGA)
            return false;

        FIBITMAP bmp = FreeImage.Load(fif, path, 0);

        uint w = FreeImage.GetWidth(bmp);
        uint h = FreeImage.GetHeight(bmp);

        bool totalTrans = true;
        bool hasTransPixel = false;

        for (uint y = 0; y < h; y++)
        {
            for (uint x = 0; x < w; x++)
            {
                RGBQUAD rgbquad;
                FreeImage.GetPixelColor(bmp, x, y, out rgbquad);
                if( rgbquad.rgbReserved > 0 )
                {
                    totalTrans = false;
                    break;
                }
            }
        }

        if (totalTrans)
        {
            FreeImage.Unload(bmp);
            return false;
        }

        for (uint y = 0; y < h; y++ )
        {
            for (uint x = 0; x < w; x++)
            {
                RGBQUAD rgbquad;
                FreeImage.GetPixelColor(bmp,x,y,out rgbquad);
                if (rgbquad.rgbReserved < byte.MaxValue)
                {
                    hasTransPixel = true;
                    break;
                }
            }
        } 

        FreeImage.Unload(bmp);

        return hasTransPixel;
    }


    public static string[] GetPathList( string paths )
    { 
        return paths.Split(new char[]{';'});
    }

    public static bool IsPathInclude( string path , H3DAssetChecker checker )
    { 
        if (checker.ExcludePath != "")
        {

            var excludePaths = GetPathList(checker.ExcludePath);

            foreach (var p in excludePaths)
            {
                if (path.StartsWith(p))
                {
                    return false;
                }
            }
        }

        if (checker.FilterPath == "")
            return true;
         
        var includePaths = GetPathList(checker.FilterPath);

        foreach( var p in includePaths )
        {
            if( path.StartsWith(p) )
            {
                return true;
            }
        }

        return false;
    }
}

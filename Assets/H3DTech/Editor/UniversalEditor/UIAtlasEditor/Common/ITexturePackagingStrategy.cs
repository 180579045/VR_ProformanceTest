using UnityEngine;
using System.Collections;

public interface ITexturePackagingStrategy 
{
    Rect[] Pack(Texture2D tex, Texture2D[] imgs, int padding, object config);	
}

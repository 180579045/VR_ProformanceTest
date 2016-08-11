using UnityEngine;
using System.Collections;

public class ColorCtrl : EditorControl
{
    public Color currColor = Color.black;
     
    public ColorCtrl()
    {
        this.Size = new Rect(0, 0, 100, 20);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class H3DGraph {
    static H3DGraph mInstance;



    static Dictionary<string,Texture2D> texCache = new Dictionary<string,Texture2D>();
    static Texture2D mTex;
    public static void DrawRect(Rect rect,Color color) {
        EditorGUI.DrawRect(rect,color);
    }

    public static void ClearCache() {
        List<string> keys = new List<string>();
        foreach(var key in texCache.Keys) {
            keys.Add(key);
        }
        foreach(var key in keys) {
            Texture2D.DestroyImmediate(texCache[key]);
            texCache[key] = null;
        }
        keys.Clear();
        texCache.Clear();
    }

    public static void DrawRoundRect(Rect rect,int radius,Color frameColor,Color backgroundColor,Color radColor) {
        //无边框无背景
        if(frameColor == Color.clear && backgroundColor == Color.clear) return;
        if(radius == 0) {
            //有边框有背景
            if(frameColor != Color.clear && backgroundColor != Color.clear) {
                EditorGUI.DrawRect(new Rect(rect.x-1,rect.y-1,rect.width+2,rect.height+2),frameColor);
                EditorGUI.DrawRect(rect,backgroundColor);
                return;
            }
            //无边框有背景
            if(frameColor == Color.clear && backgroundColor != Color.clear) {
                EditorGUI.DrawRect(rect,backgroundColor);
                return;
            }
        }
        string key = string.Format("RoundRect{0},{1},{2},{3},{4}",rect.size,radius,frameColor,backgroundColor,radColor);
        Texture2D tex = null;
        if(!texCache.ContainsKey(key) || texCache[key] == null) {
            //            Debug.Log(key);
            ////画角
            tex = new Texture2D((int)rect.width,(int)rect.height);
            FillTexture(tex,backgroundColor);
            int r = radius;
            //画边      
            int w = (int)rect.width;
            int h = (int)rect.height;
            line(new Vector2(r,h - 1),new Vector2(w - r - 1,h - 1),frameColor,tex);
            line(new Vector2(r,0),new Vector2(w - r - 1,0),frameColor,tex);
            line(new Vector2(0,r),new Vector2(0,h - r - 1),frameColor,tex);
            line(new Vector2(w - 1,r),new Vector2(w - 1,h - r - 1),frameColor,tex);
            Color fillColor = frameColor;
            for(r = 0;r <= radius;r++) {
                if(r == radius) {
                    fillColor = frameColor;
                } else {
                    fillColor = radColor;
                }
                int p = 1 - r;
                Vector2 circPt = new Vector2(0,r);
                rountRectCirclePlotPoints(r,(int)rect.width,(int)rect.height,circPt,tex,fillColor);
                while(circPt.x < circPt.y) {
                    circPt.x++;
                    if(p < 0) {
                        p += 2 * (int)circPt.x + 1;
                    } else {
                        circPt.y--;
                        p += 2 * (int)(circPt.x - circPt.y) + 1;
                    }
                    rountRectCirclePlotPoints(r,(int)rect.width,(int)rect.height,circPt,tex,fillColor);
                }
            }
            tex.Apply();
            texCache[key] = tex;
        } else {
            tex = texCache[key];
        }
        if(tex != null) {
            GUI.DrawTexture(new Rect(rect.x,rect.y,tex.width,tex.height),tex);
        } else {
            texCache.Remove(key);
        }        
    }

    public static void line(Vector2 p1,Vector2 p2,Color color,Texture2D tex) {
        int x,y,dx,dy,dx2,dy2,xstep,ystep,error,index;
        x = (int)p1.x;
        y = (int)p1.y;
        dx = (int)(p2.x - p1.x);
        dy = (int)(p2.y - p1.y);
        if(dx >= 0) // 从左往右画
	{
            xstep = 1; // x步进正1
        } else // 从右往左画
	{
            xstep = -1; // x步进负1
            dx = -dx; // 取绝对值
        }

        if(dy >= 0) // 从上往下画
	{
            ystep = 1; // y步进正1
        } else // 从下往上画
	{
            ystep = -1; // y步进负1
            dy = -dy; // 取绝对值
        }

        dx2 = dx << 1; // 2 * dx
        dy2 = dy << 1; // 2 * dy

        if(dx > dy) // 近X轴直线
	{
            error = dy2 - dx;
            for(index = 0;index <= dx;++index) {
                tex.SetPixel(x,y,color);
                if(error >= 0) {
                    error -= dx2;
                    y += ystep;
                }
                error += dy2;
                x += xstep;
            }
        } else // 近Y轴直线
	{
            error = dx2 - dy;
            for(index = 0;index <= dy;++index) {
                tex.SetPixel(x,y,color);
                if(error >= 0) {
                    error -= dy2;
                    x += xstep;
                }
                error += dx2;
                y += ystep;
            }
        }
    }

    //public static void DrawCircle(int xc,int yc,int radius,Color frameColor) {
    //    string key = string.Format("Circle{0},{1},{2},{3}",xc,yc,radius,frameColor);
    //    Texture2D tex = null;
    //    frameColor = Color.blue;
    //    if(!texCache.ContainsKey(key)) {
    //        tex = new Texture2D(radius * 2+2,radius * 2+2);
    //        FillTexture(tex,Color.white);
    //        int p = 1 - radius;
    //        Vector2 circPt = new Vector2(0,radius);
    //        circlePlotPoints(radius+1,radius+1,circPt,tex,frameColor);
    //        while(circPt.x < circPt.y) {
    //            circPt.x++;
    //            if(p < 0) {
    //                p += 2 * (int)circPt.x + 1;
    //            } else {
    //                circPt.y--;
    //                p += 2 * (int)(circPt.x - circPt.y) + 1;
    //            }
    //            circlePlotPoints(radius+1,radius+1,circPt,tex,frameColor);
    //        }
    //        tex.Apply();
    //        texCache[key] = tex;
    //    } else {
    //        tex = texCache[key];
    //    }
    //    GUI.DrawTexture(new Rect(xc-1,yc-1,radius*2,radius*2),tex);
    //}
    private static void circlePlotPoints(int xc,int yc,Vector2 circPt,Texture2D tex,Color frameColor) {
        tex.SetPixel((int)(xc + circPt.x),yc + (int)(circPt.y),frameColor); //右上圆弧
        tex.SetPixel((int)(xc - circPt.x),yc + (int)(circPt.y),frameColor);//左上圆弧
        tex.SetPixel((int)(xc + circPt.x),yc - (int)(circPt.y),frameColor);//右下圆弧
        tex.SetPixel((int)(xc - circPt.x),yc - (int)(circPt.y),frameColor);//左下圆弧
        tex.SetPixel((int)(xc + circPt.y),yc + (int)(circPt.x),frameColor);//右上圆弧2
        tex.SetPixel((int)(xc - circPt.y),yc + (int)(circPt.x),frameColor);
        tex.SetPixel((int)(xc + circPt.y),yc - (int)(circPt.x),frameColor);
        tex.SetPixel((int)(xc - circPt.y),yc - (int)(circPt.x),frameColor);
    }
    private static void rountRectCirclePlotPoints(int r,int w,int h,Vector2 circPt,Texture2D tex,Color frameColor) {
        tex.SetPixel((int)(r - circPt.x),-r + (int)(circPt.y),frameColor);//左上圆弧
        tex.SetPixel((int)(r - circPt.y),-r + (int)(circPt.x),frameColor);     
        tex.SetPixel((int)(w-1-r + circPt.x),-r + (int)(circPt.y),frameColor); //右上圆弧
        tex.SetPixel((int)(w-1-r + circPt.y),-r + (int)(circPt.x),frameColor);
        tex.SetPixel((int)(r - circPt.x),h-1 + r - (int)(circPt.y),frameColor);//左下圆弧
        tex.SetPixel((int)(r - circPt.y),h-1 + r - (int)(circPt.x),frameColor);
        tex.SetPixel((int)(w-1-r + circPt.x),h-1+r - (int)(circPt.y),frameColor);//右下圆弧
        tex.SetPixel((int)(w-1-r + circPt.y),h-1+r - (int)(circPt.x),frameColor);
    }

    public static void DrawFillCircle(int xc,int yc,int r,Color fillColor) {
        int x,y;
        int deltax,deltay;
        int d;
        int xi;
        x = 0;
        y = r;
        deltax = 3;
        deltay = 2 - r - r;
        d = 1 - r;
        int x0 = r + 1;
        int y0 = r + 1;
        Texture2D tex = null;
        tex = new Texture2D(r * 2 + 2,r * 2 + 2);
        tex.SetPixel(x + x0,y + y0,fillColor);
        tex.SetPixel(x + x0,-y + y0,fillColor);
        for(xi = -r + x0;xi <= r + x0;xi++)
            tex.SetPixel(xi,y0,fillColor);//水平线填充
        while(x < y) {
            if(d < 0) {
                d += deltax;
                deltax += 2;
                x++;
            } else {
                d += (deltax + deltay);
                deltax += 2;
                deltay += 2;
                x++;
                y--;
            }
            for(xi = -x + x0;xi <= x + x0;xi++) {
                tex.SetPixel(xi,-y + y0,fillColor);
                tex.SetPixel(xi,y + y0,fillColor);//扫描线填充
            }
            for(xi = -y + x0;xi <= y + x0;xi++) {
                tex.SetPixel(xi,-x + y0,fillColor);
                tex.SetPixel(xi,x + y0,fillColor);//扫描线填充其量
            }
        }
        tex.Apply();
        GUI.DrawTexture(new Rect(xc - 1,yc - 1,r * 2+2,r * 2+2),tex);
    }
    private static void FillTexture(Texture2D tex,Color color) {
        for(int x = 0;x < tex.width;x++) {
            for(int y = 0;y < tex.height;y++) {
                tex.SetPixel(x,y,color);
            }
        }
    }
}

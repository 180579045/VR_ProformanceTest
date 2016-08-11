using UnityEngine;
using System.Collections;
using UnityEditor;
namespace EditorUIKit {
    public class UILabel:UIView {
        public string text;
        public Color color = Color.white;
        public int fontSize = 18;
        public FontStyle fontStyle = FontStyle.Normal;
        //public string fontName;
        public TextAnchor alignment = TextAnchor.MiddleCenter;
        GUIStyle s = new GUIStyle();
        public UILabel(Rect frame,string text) : base(frame) {
            this.text = text;
        }
        public override void OnDraw() {
            base.OnDraw();
            s.normal.textColor = color;
            s.fontSize = fontSize;
            s.alignment = alignment;
            s.fontStyle = fontStyle;
            GUI.Label(frame,text,s);
            
        }
    }
}
using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Graphs;
namespace EditorUIKit {
    public class UIButton:UIControlView {
        public UILabel textLabel;
        public Action<UIButton> ClickEvent;
        public Action<UIButton,bool> PressEvent;
        public Action<UIButton,Vector2,Vector2> DragEvent;
        public Action<UIButton,bool> HoverEvent;

        public Color normalColor = new Color32(62,122,246,255);
        public Color hoverColor = new Color32(92,142,248,255);
        public Color pressColor = new Color32(11,87,244,255);
        public Color disableColor = new Color32(126,126,126,255);
        public UIButton(Rect frame) : base(frame) {
        }

        enum ButtonSatus { 
            Normal,
            Hover,
            Press,
            Disable
        }

        private ButtonSatus status =ButtonSatus.Normal; //0:normal 1:hover 2:press 3:disable

        protected override void OnInit() {
            base.OnInit();
            textLabel = new UILabel(new Rect(0,0,frame.width,frame.height),"刷新资源数据库");
            this.AddSubview(textLabel);
            radius = 5;
            textLabel.color = Color.white;
            textLabel.fontSize = 18;   
            //borderColor = Color.blue;
        }

        public override void OnDraw() {
            //EditorGUI.DrawRect(new Rect(frame.x + 2,frame.y + 2,frame.width,frame.height),Color.black);
            Color bgColor = Color.white;
            if(status == ButtonSatus.Normal) {
                bgColor = normalColor;
            } else if(status == ButtonSatus.Hover) {
                bgColor = hoverColor;
            } else if(status == ButtonSatus.Press) {
                bgColor = pressColor;
            } else if(status == ButtonSatus.Disable) {
                bgColor = disableColor;
            }
            backgroundColor = bgColor;
            base.OnDraw();

        }
        protected override void OnMouseUp(int button) {
            base.OnMouseUp(button);
            if(ClickEvent != null) ClickEvent(this);
            if(PressEvent != null && status == ButtonSatus.Press) {
                PressEvent(this,false);
            }
            status = ButtonSatus.Normal;
        }
        protected override void OnMouseDown(int button) {
            base.OnMouseDown(button);
            status = ButtonSatus.Press;
            if(PressEvent != null) PressEvent(this,true);
        }

        protected override void OnMouseMove(Vector2 postion) {
            base.OnMouseMove(postion);
            
        }
        protected override void OnMouseDrag(int button,Vector2 mousePosition,Vector2 delta) {
            base.OnMouseDrag(button,mousePosition,delta);
            if(DragEvent != null) {
                DragEvent(this,mousePosition,delta);
            }
        }

        protected override void OnMouseEnter(Vector2 position) {
            //Debug.Log("Enter");
            base.OnMouseEnter(position);
            if(status != ButtonSatus.Press) {
                status = ButtonSatus.Hover;
                if(HoverEvent != null) {
                    HoverEvent(this,true);
                }
            }
        }

        protected override void OnMouseLeave(Vector2 position) {
            //Debug.Log("Leave");
            base.OnMouseLeave(position);
            if(PressEvent != null && status == ButtonSatus.Press) {
                PressEvent(this,false);
            }
            status = ButtonSatus.Normal;
            if(HoverEvent != null) {
                HoverEvent(this,false);
            }
        }
    }
}
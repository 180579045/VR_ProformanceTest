using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Graphs;
namespace EditorUIKit {
    public class UIInput:UIControlView {
        static UIInput focusText = null;
        private string mValue = "";
        public Action<UIInput> FocusEvent;
        public Action<UIInput> ValueChangeEvent;
        public string Value {
            get {
                return mValue;
            }
            set {
                if(mValue != value) {
                    mValue = value;
                    if(ValueChangeEvent != null) {
                        ValueChangeEvent(this);
                    }
                }
            }
        }
        public UIInput(Rect frame):base(frame){}
        protected override void OnInit() {
            base.OnInit();
            backgroundColor = new Color32(241,241,241,255);
            borderColor = new Color32(62,122,246,255);
            radius = 0;
            style.clipping = TextClipping.Clip;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;
            style.normal.textColor = new Color32(33,33,33,255);
            handleEvent = false;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            focusText = null;
        }

        public override void OnDraw() {
            base.OnDraw();
            if(focusText == this) {
                borderColor = new Color32(92,142,248,255);
            } else {
                borderColor = new Color32(62,122,246,255);
            }

            Value = GUI.TextField(new Rect(frame.x+2,frame.y,frame.width-4,frame.height),mValue,style);
        }

        protected override void OnMouseUp(int button) {
            base.OnMouseUp(button);
            focusText = this;
            if(FocusEvent != null) {
                FocusEvent(this);
            }
        }
    }
}
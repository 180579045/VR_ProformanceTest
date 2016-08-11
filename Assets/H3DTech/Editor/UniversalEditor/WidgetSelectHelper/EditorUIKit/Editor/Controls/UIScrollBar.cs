using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Graphs;
namespace EditorUIKit {
    public class UIScrollBar:UIView {
        float mSize = 1.0f;
        ScrollBarType mType;
        float mValue = 0;
        public Action<UIScrollBar> ValueChangeEvent;
        public float Value {
            get { return mValue; }
            set {
                if(mValue != value) {
                    mValue = Mathf.Clamp01(value);
                    if(mType == ScrollBarType.Horizontal) {                   
                        thumb.Position = new Vector2(mValue * frame.width *(1-mSize),0);
                    } else {
                        thumb.Position = new Vector2(0,mValue*frame.height*(1-mSize));
                    }
                    if(ValueChangeEvent != null) {
                        ValueChangeEvent(this);
                    }
                }
            }
        }

        public float ContentSize { 
            get{
                return mSize;
            }
            set {
                if(mSize != value) {
                    mSize = value;
                    if(mType == ScrollBarType.Horizontal) {
                        thumb.frame = new Rect(0,0,frame.width * mSize,frame.height);
                    } else {
                        thumb.frame = new Rect(0,0,frame.width,frame.height * mSize);
                    }
                }
            }
        }

        public enum ScrollBarType {
            Horizontal,
            Vertical
        }
        UIButton thumb;
        public UIScrollBar(Rect frame,float contentSize,ScrollBarType type) : base(frame) {
            this.mSize = Mathf.Clamp01(contentSize);
            this.mType = type;
            Init();
        }

        public override void OnDraw() {
            base.OnDraw();
        }

        public void Scroll(float delta) {
            if(mType == ScrollBarType.Horizontal) {
                Value += delta / (frame.width*(1-mSize));
            } else {
                Value += delta / (frame.height * (1 - mSize));
            }
        }

        protected void Init() {
            base.OnInit();
            borderColor = new Color32(185,185,185,255);
            radius = 0;
            //radColor = borderColor;
            if(mType == ScrollBarType.Horizontal) {
                thumb = new UIButton(new Rect(0,0,frame.width*mSize,frame.height));
                thumb.textLabel.text = "";
            } else {
                thumb = new UIButton(new Rect(0,0,frame.width,frame.height*mSize));
                thumb.textLabel.text = "";
            }
            thumb.radius = 0;
            thumb.borderColor = Color.clear;         
            thumb.DragEvent = (sender,mousePosition,delta) => {
                if(mType == ScrollBarType.Horizontal) {
                    Scroll(delta.x);
                } else {
                    Scroll(delta.y);
                }
                    
            };
            AddSubview(thumb);
        }
    }
}
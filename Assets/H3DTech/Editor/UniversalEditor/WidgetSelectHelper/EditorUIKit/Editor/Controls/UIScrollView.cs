using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
namespace EditorUIKit {
    public class UIScrollView:UIControlView {
        Vector2 scrollPosition;
        UIView mContentView;
        UIScrollBar mHSB;
        UIScrollBar mVSB;
        int sbWidth = 5;
        public UIView ContentView {
            get {
                return mContentView;
            }
            set {
                if(mContentView != value) {
                    mContentView = value;
                }
            }
        }
        public UIScrollView(Rect frame,UIView contentView) : base(frame) {
            backgroundColor = Color.white;
            mContentView = contentView;
        }
        public override void OnDraw() {
            base.OnDraw();
            if(mHSB == null && frame.width < mContentView.Size.x) {
                mHSB = new UIScrollBar(new Rect(0,frame.height - sbWidth,frame.width,sbWidth),(frame.width) / mContentView.Size.x,UIScrollBar.ScrollBarType.Horizontal);
                mHSB.borderColor = Color.clear;
                mHSB.radius = 5;
                mHSB.ValueChangeEvent = (sender) => {
                    scrollPosition.x = -mContentView.frame.width*sender.Value * (1-sender.ContentSize);
                };
                
                AddSubview(mHSB);
            }
            if(mVSB == null && frame.height < mContentView.Size.y) {
                mVSB = new UIScrollBar(new Rect(frame.width - sbWidth,0,sbWidth,frame.height),(frame.height) / mContentView.Size.y,UIScrollBar.ScrollBarType.Vertical);
                mVSB.borderColor = Color.clear;
                mVSB.radius = 5;
                mVSB.ValueChangeEvent = (sender) => {
                    scrollPosition.y = -mContentView.frame.height * sender.Value * (1 - sender.ContentSize);
                };
                AddSubview(mVSB);
            }

            if(mHSB != null) {
                if(frame.width < mContentView.Size.x) {
                    mHSB.visible = true;
                    mHSB.frame = new Rect(0,frame.height-sbWidth,frame.width,sbWidth);
                    mHSB.ContentSize = (frame.width) / mContentView.Size.x;
                   
                    float d = ((1 - mHSB.ContentSize) * mContentView.frame.width);
                    if(d != 0) {
                        mHSB.Value = -scrollPosition.x / d;
                    } else {
                        mHSB.Value = 0;
                    }
                } else {
                    mHSB.visible = false;
                    mHSB.Value = 0;
                }

            } 
            if(mVSB != null) {
                if(frame.height < mContentView.Size.y) {
                    mVSB.visible = true;
                    mVSB.frame = new Rect(frame.width - sbWidth,0,sbWidth,frame.height);
                    mVSB.ContentSize = (frame.height) / mContentView.Size.y;
                    float d = ((1-mVSB.ContentSize)*mContentView.frame.height);
                    if(d != 0) {
                        mVSB.Value = -scrollPosition.y / d;
                    } else {
                        mVSB.Value = 0;
                    }
                    
                } else {
                    mVSB.visible = false;
                    mVSB.Value = 0;
                }
            }

            GUI.BeginClip(new Rect(frame.x,frame.y,frame.width,frame.height));
            if(mContentView != null && mContentView.visible) {
                mContentView.Position = scrollPosition;
                mContentView.OnDraw();
            }
            GUI.EndClip();

        }

        protected override void OnScrollWheel(float delta) {
            base.OnScrollWheel(delta);
            if(mVSB != null) {
                mVSB.Scroll(delta);
            } else if(mHSB != null) {
                mHSB.Scroll(delta);
            }
        }
        protected override void OnMouseDrag(int button,Vector2 mousePosition,Vector2 delta) {
            base.OnMouseDrag(button,mousePosition,delta);
            //if(mVSB != null && mVSB.visible) {
            //    mVSB.Scroll(-delta.y);
            //}
            //if(mHSB != null && mHSB.visible) {
            //    mHSB.Scroll(-delta.x);
            //}
        }
        public override bool OnEvent(Event e) {
            //Debug.Log(e.type);
            if(base.OnEvent(e))return true;
            var worldRect = GetWorldRect();
            if(mContentView != null && mContentView.visible) {
                if(e.isMouse && worldRect.Contains(e.mousePosition)) {
                    mContentView.Position = frame.position + scrollPosition;
                    mContentView.OnEvent(e);
                    mContentView.Position = scrollPosition;
                    return true;
                } else if(e.isKey) {
                    mContentView.Position = frame.position + scrollPosition;
                    mContentView.OnEvent(e);
                    mContentView.Position = scrollPosition;
                    return true;
                }

            }
            return false;
        }
    }
}
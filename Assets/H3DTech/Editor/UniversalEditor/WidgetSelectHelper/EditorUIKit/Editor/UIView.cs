using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace EditorUIKit {
    public class UIView {
        public string name = "";
        public object userdata;
        protected List<UIView> subviews = new List<UIView>();
        protected UIView parentView = null;
        public Rect frame;
        public bool visible = true;
        public Color backgroundColor = Color.clear;
        public Color borderColor = Color.clear;
        public int borderWidth = 0;
        public int radius = 0;//圆角半径
        public Color radColor = Color.clear;//圆角填充区颜色
        public bool clip = false;
        public bool interceptEvent = true;//拦截消息
        public bool handleEvent = true;//接收消息
        protected GUIStyle style = new GUIStyle();

        public Vector2 Position {
            get {
                return new Vector2(frame.x,frame.y);
            }
            set {
                frame.x = value.x;
                frame.y = value.y;
            }
        }

        public Vector2 Size {
            get {
                return new Vector2(frame.width,frame.height);
            }
            set {
                frame.width = value.x;
                frame.height = value.y;
            }
        }

        public UIView(Rect frame) {
            this.frame = frame;
            OnInit();
        }
        protected virtual void OnInit() {
            style.clipping = TextClipping.Overflow;
        }

        public virtual void OnDestroy() {
            for(int i = 0;i < subviews.Count;i++) {
                subviews[i].OnDestroy();
            }
            subviews.Clear();
            subviews = null;
            parentView = null;
            style = null;
            userdata = null;
        }
        public virtual void OnDraw() {
            if(frame.width <= 0 || frame.height <= 0) return;
            H3DGraph.DrawRoundRect(frame,radius,borderColor,backgroundColor,radColor);
            Rect rect = frame;
            if(!clip) {
                rect = new Rect(frame.x,frame.y,float.MaxValue,float.MaxValue);
            }
            GUI.BeginGroup(rect);
            for(int i = 0;i < subviews.Count;i++) {
                if(subviews[i].visible) {
                    subviews[i].OnDraw();
                } 
            }
            GUI.EndGroup();
        }
       
        UIView handelEventView;
        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="e"></param>
        /// <returns>返回true表示以处理事件,上层无需再处理</returns>
        public virtual bool OnEvent(Event e) {
            //同一个层级只拦截区域覆盖的事件
            for(int i = subviews.Count-1;i >= 0;i--) {
                if(subviews[i].visible && subviews[i].handleEvent) {
                    if(subviews[i].OnEvent(e) && subviews[i].interceptEvent) {
                        if(e.isMouse && e.type != EventType.MouseMove && e.type != EventType.MouseDrag) {
                            return true;
                        }
                    }
                } 
            }
            return false;
        }
        public void AddSubview(UIView view) {
            if(!subviews.Contains(view) && view != this) {
                subviews.Add(view);
                view.parentView = this;
            }

        }
        public void RemoveSubview(UIView view) {
            if(subviews.Contains(view) && view != this) {
                subviews.Remove(view);
                view.parentView = null;
            }
        }
        public void BringSubviewToFront(UIView view) {
            int index = subviews.IndexOf(view);
            if(index < 0 || index == subviews.Count - 1) return;
            var temp = subviews[index + 1];
            subviews[index + 1] = subviews[index];
            subviews[index] = temp;
        }

        protected Vector2 WorldPos2LocalPos(Vector2 wordPos) {
            return wordPos - GetWorldRect().position;
        }

        protected virtual Rect GetWorldRect() {
            Rect rect = new Rect(frame);
            UIView currentView = this.parentView;
            while(currentView != null) {
                rect.x += currentView.frame.x;
                rect.y += currentView.frame.y;
                currentView = currentView.parentView;
            }
            return rect;
        }

       
    }
}
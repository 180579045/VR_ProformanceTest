using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace EditorUIKit {
    public class UIWindow:UIView {

        public UIWindow(Rect frame)
            : base(frame) {

        }

        public override void OnDraw() {
           // GUI.ModalWindow(1,GetWorldRect(),_windowFunc,"",style);
            GUI.Window(100,GetWorldRect(),_windowFunc,"",style);
            GUI.FocusWindow(100);
        }
        public override bool OnEvent(Event e) {
            return false;
        }

        public virtual bool OnWindowEvent(Event e) {
            return base.OnEvent(e);
        }
        public void OnWindowDraw() {
            if(frame.width <= 0 || frame.height <= 0) return;
            H3DGraph.DrawRoundRect(new Rect(0,0,frame.width,frame.height),radius,borderColor,backgroundColor,radColor);
            //Rect rect = frame;
            //if(!clip) {
            //    rect = new Rect(frame.x,frame.y,float.MaxValue,float.MaxValue);
            //}
            //GUI.BeginGroup(rect);
            for(int i = 0;i < subviews.Count;i++) {
                if(subviews[i].visible) {
                    subviews[i].OnDraw();
                }
            }
            //GUI.EndGroup();
        }

        void _windowFunc(int id) {
            if(visible) {
                if(Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint) {
                    Event.current.mousePosition += frame.position;             
                    if(OnWindowEvent(Event.current)) {
                        Event.current.Use();
                    }

                }
            }
            OnWindowDraw();
        }
       
    }
}
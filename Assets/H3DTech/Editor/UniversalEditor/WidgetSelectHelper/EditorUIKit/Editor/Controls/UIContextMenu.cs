using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.Graphs;
namespace EditorUIKit {
    public class UIContextMenu:UIControlView {
        public Action<UIContextMenu,string,object> SelectedEvent;
        public Action<UIContextMenu,string,object> HangingEvent;
        public Action<UIContextMenu> CancelEvent;
        UIView menuBackgroundView;
        public UIContextMenu():base(new Rect(0,0,0,0)) {
            
        }


        protected override void OnInit() {
            base.OnInit();
            backgroundColor = Color.clear;
            visible = false;
            menuBackgroundView = new UIView(new Rect(0,0,100,100));
            menuBackgroundView.backgroundColor = new Color32(241,241,241,255);
            borderColor = new Color32(33,33,33,33);
            AddSubview(menuBackgroundView);
        }

        int maxItemWidth = 0;
        int maxItemHeight = 20;
        List<UIButton> menuItemList = new List<UIButton>();
        public void ClearAllMenuItem() {
            if(CancelEvent != null) CancelEvent(this);
            foreach(var item in menuItemList) {
                menuBackgroundView.RemoveSubview(item);
            }
            menuItemList.Clear();
            AdjustItemPostion();
            maxItemWidth = 0;
        }
        public List<object> GetAllUserData() {
            List<object> UDList = new List<object>();
            foreach(var item in menuItemList) {
                if(item.userdata != null) {
                    UDList.Add(item.userdata);
                }
            }
            return UDList;
        }
        public void AddMenuItem(string title,object ud = null) {
            int width = title.Length * 12 + 24;
            maxItemWidth = width > maxItemWidth ? width : maxItemWidth;
            UIButton item = new UIButton(new Rect(0,0,maxItemWidth,maxItemHeight));
            item.name =  title;
            item.userdata = ud;
            item.textLabel.text ="  " + title;
            item.textLabel.color = Color.black;
            item.radius = 0;
            item.normalColor = backgroundColor;
            item.textLabel.alignment = TextAnchor.MiddleLeft;
            item.HoverEvent = (sender,hover) => {
                if(hover && HangingEvent!=null) {
                    HangingEvent(this,sender.name,sender.userdata);
                }
            };
            item.ClickEvent = (sender) => {
                visible = false;
                if(SelectedEvent != null) {
                    SelectedEvent(this,sender.name,sender.userdata);
                }
            };
            menuItemList.Add(item);
            menuBackgroundView.AddSubview(item);
            AdjustItemPostion();
        }

        
        public void Show(Vector2 position) {
            menuBackgroundView.Position = position;
            visible = true;
        }

        void AdjustItemPostion() {
            menuBackgroundView.Size = new Vector2(maxItemWidth,maxItemHeight * menuItemList.Count);
            for(int i = 0;i < menuItemList.Count;i++) {
                var item = menuItemList[i];
                item.frame.Set(0,i * maxItemHeight,maxItemWidth,maxItemHeight);
            }
        }

        protected override void OnMouseUp(int button) {
            base.OnMouseUp(button);
            if(button == 0 || button == 1) {
                visible = false;
                if(CancelEvent != null) CancelEvent(this);
            }
        }

        protected override void OnMouseDown(int button) {
            base.OnMouseDown(button);

        }

        public override void OnDraw() {
            base.OnDraw();
            if(parentView != null) {
                frame.Set(0,0,parentView.frame.width,parentView.frame.height);
            } else {
                frame.Set(0,0,Screen.width,Screen.height);
            }
            
        }
    }
}
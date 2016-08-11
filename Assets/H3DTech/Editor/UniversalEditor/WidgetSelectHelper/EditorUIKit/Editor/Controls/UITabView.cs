using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace EditorUIKit {
    public class UITabView:UIView {
        List<UIView> tabViews = new List<UIView>();
        List<UIButton> tabButtons = new List<UIButton>();
        public Action<UITabView> TabChangeEvent;
        int currentTabIndex = 0;
        public int CurrentTab {
            get {
                return currentTabIndex;
            }
            set {
                if(value < 0 || value >= tabViews.Count) {
                    return;
                }
                if(value != currentTabIndex) {
                    currentTabIndex = value;
                    update();
                    if(TabChangeEvent != null) {
                        TabChangeEvent(this);
                    }
                }
            }
        }
        public UITabView(Rect frame) : base(frame) { }
        protected override void OnInit() {
            base.OnInit();
        }
        public int AddTab(string tabTitle) {
            UIView view = new UIView(frame);
            tabViews.Add(view);
            AddSubview(view);
            view.backgroundColor = Color.white;
            view.borderColor = new Color32(138,138,138,138);
            view.Position = new Vector2(0,24);

            int index = tabViews.Count - 1;
            UIButton tabBtn = new UIButton(new Rect(0,0,tabTitle.ToCharArray().Length * 20,25));
            tabBtn.textLabel.text = tabTitle;
            tabBtn.radius = 0;
            tabButtons.Add(tabBtn);
            AddSubview(tabBtn);
            update();
            return index;
        }

        public override void OnDraw() {
            base.OnDraw();
            for(int i = 0;i < tabViews.Count;i++) {
                tabViews[i].frame.width = frame.width;
                tabViews[i].frame.height = frame.height;
            }
        }

        public void RemoveTab(int tabIndex) { 
        
        }

        public void update() {
            int x = 0;
            for(int i = 0;i < tabButtons.Count;i++) {
                var btn = tabButtons[i];
                btn.Position = new Vector2(x,0);
                x += (int)btn.Size.x;
                Color bgColor = Color.clear;
                Color textColor = Color.clear;
                if(i == currentTabIndex) {
                    bgColor = new Color32(62,122,246,255);
                    textColor = Color.white;
                    tabViews[i].visible = true;
                } else {
                    bgColor = Color.clear;
                    textColor = new Color32(62,122,246,255);
                    tabViews[i].visible = false;
                }
                btn.normalColor = bgColor;
                btn.hoverColor = bgColor;
                btn.pressColor = bgColor;
                btn.textLabel.color = textColor;
                btn.ClickEvent = (sender) => {
                    int index = tabButtons.IndexOf(sender);
                    CurrentTab = index;
                    update();
                };
            }
        }
        public UIView GetTabView(int tabIndex) {
            if(tabIndex < 0 || tabIndex >= tabViews.Count) return null;
            return tabViews[tabIndex];
        }
    }
}
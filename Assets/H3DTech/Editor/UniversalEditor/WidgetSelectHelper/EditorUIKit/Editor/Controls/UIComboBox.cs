using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Graphs;
namespace EditorUIKit {
    public class UIComboBox:UIView {
        public Action<UIComboBox> ValueChangeEvent;
        public string value {
            get {
                return input.Value;
            }
            set {
                input.Value = value;
            }
        }
        public UIInput input;
        public UIButton button;
        private Dictionary<string,UIButton> itemDic = new Dictionary<string,UIButton>();
        UIView listView;
        public UIComboBox(Rect frame) : base(frame) { }
        protected override void OnInit() {
            base.OnInit();
            input = new UIInput(new Rect(0,0,frame.width - 20,frame.height));
            
            input.ValueChangeEvent = (s) => {
                if(ValueChangeEvent != null) {
                    ValueChangeEvent(this);
                }
            };
            AddSubview(input);
            button = new UIButton(new Rect(frame.width - 20,0,20,frame.height));
            button.radius = 0;
            button.textLabel.text = "▽";
            button.textLabel.fontSize = 16;
            button.textLabel.fontStyle = FontStyle.Normal;
            button.borderColor = input.borderColor;
            button.ClickEvent = onClick;
            AddSubview(button);
            listView = new UIView(new Rect(0,frame.height+1,frame.width,90));
            listView.backgroundColor = new Color32(241,241,241,255);
            listView.borderColor = new Color32(138,138,138,255);
            listView.visible = false;
            AddSubview(listView);
        }

        public void AddItem(string item) {
            if(itemDic.ContainsKey(item)) return;
            UIButton itemBtn = new UIButton(new Rect(1,0,frame.width-2,30));
            itemBtn.textLabel.text = item;
            itemBtn.textLabel.fontSize = 14;
            itemBtn.textLabel.color = new Color32(33,33,33,255);
            itemBtn.radius = 0;
            itemBtn.ClickEvent = onItemClick;
            itemBtn.normalColor = listView.backgroundColor;
            itemBtn.hoverColor = Color.white;
            itemBtn.pressColor = itemBtn.hoverColor;
            listView.AddSubview(itemBtn);
            itemDic[item] = itemBtn;
            relayoutListView();
        }
        public void RemoveItem(string item) {
            if(!itemDic.ContainsKey(item)) return;
            UIButton btn = itemDic[item];
            listView.RemoveSubview(btn);
            btn = null;
            itemDic.Remove(item);
            relayoutListView();
        }
        void relayoutListView() {
            int i = 0;
            foreach(var item in itemDic) {
                item.Value.Position = new Vector2(1,i * 30);
                i++;
            }
            listView.Size = new Vector2(frame.width,i * 30+1);
            frame.size = new Vector2(frame.width,i * 30 + button.Size.y+1);
        }
        private void onClick(UIButton sender) {
            listView.visible = !listView.visible;
        }
        private void onItemClick(UIButton sender) {
            input.Value = sender.textLabel.text;
            listView.visible = false;
        }

    }
}
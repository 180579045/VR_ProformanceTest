using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace EditorUIKit {
    public class UIListView:UIView {
        List<UILabel> mLableList = new List<UILabel>();
        List<string> mItemList = new List<string>();
        float minWidth = 0;
        public UIListView(Rect frame) : base(frame) {
            minWidth = frame.width;
        }

        public void AddItem(string item) {
            mItemList.Add(item);
            UILabel label = new UILabel(new Rect(0,0,frame.width,20),item);
            mLableList.Add(label);
            updatePos();
        }
        public void RemoveItem(string item) {
            mItemList.Remove(item);
            var label = mLableList.Find((l) => {
                return l.text == item;
            });
            mLableList.Remove(label);
            RemoveSubview(label);
            updatePos();
        }
        void updatePos() {
            float maxWidth = 0;
            for(int i = 0;i < mLableList.Count;i++) {
                mLableList[i].frame.y = i * 20;
                if(mLableList[i].frame.width > maxWidth) {
                    maxWidth = mLableList[i].frame.width;
                }
            }
            maxWidth = Mathf.Clamp(maxWidth,minWidth,maxWidth);
            frame.width = maxWidth;
        }
    }
}

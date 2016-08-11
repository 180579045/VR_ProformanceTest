using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Graphs;
namespace EditorUIKit {
    public class UISplitView : UIControlView {
        UIView[,] views = null;
        UIButton[] colButtons = null;
        UIButton[] rowButtons = null;
        int currentDragBtnType = 0;//1:colbtn 2: rowbtn
        UIButton currentDragBtn = null;
        int mCols,mRows;
        public UISplitView(Rect frame,int cols,int rows) : base(frame) {
            mCols = cols;
            mRows = rows;
            views = new UIView[cols,rows];
            float width = frame.width / cols;
            float height = frame.height / rows;
            float btnWidth = 4;
            if(cols > 1) {
                width = (frame.width - btnWidth * (cols - 1)) / cols;
            }
            if(rows > 1) {
                height = (frame.height - btnWidth * (rows - 1)) / rows;
            }
            for(int x = 0;x < cols;x++) {
                for(int y = 0;y < rows;y++) {
                    views[x,y] = new UIView(new Rect(x * width + x * btnWidth,y * height + y * btnWidth,width,height));
                    views[x,y].backgroundColor = new Color32(241,241,241,255);
                    views[x,y].borderColor = new Color32(138,138,138,255);
                    views[x,y].clip = true;
                    AddSubview(views[x,y]);
                }
            }
            if(cols > 1) {               
                colButtons = new UIButton[cols - 1];
                for(int i = 0;i < cols-1;i++) {
                    colButtons[i] = new UIButton(new Rect((width) * (i + 1) + i * btnWidth,0,btnWidth,frame.height));
                    colButtons[i].userdata = i;
                    colButtons[i].normalColor = new Color32(138,138,138,255);
                    colButtons[i].radius = 0;
                    colButtons[i].textLabel.text = "";
                    colButtons[i].interceptEvent = false;
                    colButtons[i].HoverEvent = (sender,hover) => {
                        if(hover && currentDragBtn == null) {
                            currentDragBtnType = 1;
                        } else if(currentDragBtn == null) {
                            currentDragBtnType = 0;
                        }
                    };
                    colButtons[i].PressEvent = (sender,press) => {
                        if(press){
                            currentDragBtn = sender;
                            currentDragBtnType = 1;
                        }
                    };
                    AddSubview(colButtons[i]);
                }
            }
            if(rows > 1) {
                rowButtons = new UIButton[rows - 1];
                for(int i = 0;i < rows - 1;i++) {
                    rowButtons[i] = new UIButton(new Rect(0,(height) * (i + 1) + i * btnWidth,frame.width,btnWidth));
                    rowButtons[i].normalColor = new Color32(138,138,138,255);
                    rowButtons[i].userdata = i;
                    rowButtons[i].radius = 0;
                    rowButtons[i].textLabel.text = "";
                    rowButtons[i].interceptEvent = false;
                    rowButtons[i].HoverEvent = (sender,hover) => {
                        if(hover && currentDragBtn == null) {
                            currentDragBtnType = 2;
                        } else if(currentDragBtn == null) {
                            currentDragBtnType = 0;
                        }
                    };
                    rowButtons[i].PressEvent = (sender,press) => {
                        if(press) {
                            currentDragBtn = sender;
                            currentDragBtnType = 2;
                        }
                    };
                    AddSubview(rowButtons[i]);
                }
            }
           
        }
        public override void OnDraw() {
            base.OnDraw();
            if(currentDragBtnType == 1) {
                EditorGUIUtility.AddCursorRect(frame,MouseCursor.SplitResizeLeftRight);
            }
            if(currentDragBtnType == 2) {
                EditorGUIUtility.AddCursorRect(frame,MouseCursor.SplitResizeUpDown);
            }
        }

        protected override void OnMouseDrag(int button,Vector2 mousePosition,Vector2 delta) {
            base.OnMouseDrag(button,mousePosition,delta);
            if(currentDragBtn != null) {
                if(currentDragBtnType == 1) {
                    int col = (int)currentDragBtn.userdata;
                    MoveVerticalBarPos(col,delta.x);
                }
                if(currentDragBtnType == 2) {
                    int row = (int)currentDragBtn.userdata;
                    MoveHorizontalBarPos(row,delta.y);
                }
            }
        }

        public UIView GetView(int col,int row) {
            if(col < 0 || col >= views.GetLength(0)) return null;
            if(row < 0 || row >= views.Rank) return null;
            return views[col,row];
        }

        protected override void OnMouseUp(int button) {
            base.OnMouseUp(button);
            currentDragBtn = null;
        }
        protected override void OnMouseLeave(Vector2 position) {
            base.OnMouseLeave(position);
            currentDragBtn = null;
            currentDragBtnType = 0;
        }
        public void MoveHorizontalBarPos(int index,float delta) {
            if(index < 0 || index >= rowButtons.Length) return;
            var btn = rowButtons[index];
            btn.Position = btn.Position + new Vector2(0,delta);
            for(int col = 0;col < mCols;col++) {
                //上面改变高度
                var upView = views[col,index];
                upView.Size += new Vector2(0,delta);
                //下面改变位置和高度
                var bottomView = views[col,index + 1];
                bottomView.Position += new Vector2(0,delta);
                bottomView.Size -= new Vector2(0,delta);
            }
        }
        public void MoveVerticalBarPos(int index,float delta) {
            if(index < 0 || index >= colButtons.Length) return;
            var btn = colButtons[index];
            if(btn.Position.x + delta < btn.frame.width) {
                delta = btn.frame.width - btn.Position.x;
            }
            if(btn.Position.x + delta > frame.width - btn.frame.width) {
                delta = frame.width - btn.frame.width - btn.Position.x;
            }
            btn.Position = btn.Position + new Vector2(delta,0);
            for(int row = 0;row < mRows;row++) {
                //左侧改变宽度
                var leftView = views[index,row];
                leftView.Size += new Vector2(delta,0);
                //右侧改变位置和宽度
                var rightView = views[index + 1,row];
                rightView.Position += new Vector2(delta,0);
                rightView.Size -= new Vector2(delta,0);
            }
        }
    }
}

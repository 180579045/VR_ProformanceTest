using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
namespace EditorUIKit {
    public class UITreeNode {

        public enum NodeType {
            Swith,//开关
            Item//项
        }
        public string title;
        public NodeType type = NodeType.Item;
        public List<UITreeNode> children;
        public UITreeNode parent;
        public bool isOpen = false;//是否已展开
        public void InsertNode(UITreeNode node) {
            if(this.children == null) {
                this.children = new List<UITreeNode>();
            }
            children.Add(node);
            node.parent = this;
        }
    }

    public class UITreeView:UIView {
        UITreeNode rootNode;
        public Action<UITreeView,UITreeNode> SelectedEvent;
        Vector2 maxSize = Vector2.zero;
        Vector2 minSize = Vector2.zero;
        public UITreeView(Rect frame,UITreeNode rootNode = null) : base(frame) {
            this.rootNode = rootNode;
            minSize = frame.size;
        }
        public override void OnDraw() {
            base.OnDraw();
            if(rootNode != null) {
                treeIndex = 0;
                maxSize = Vector2.zero;
                DrawFileTree(rootNode,0);
                frame.size = maxSize;
                //Debug.Log("maxSize:" + maxSize);
            }
        }


        public void SetTreeNode(UITreeNode root) {
            this.rootNode = root;
        }

        public UITreeNode currentNode = null;
        int treeIndex = 0;
        void DrawFileTree(UITreeNode node,int level) {
            //Debug.LogFormat("Name:{0},Level:{1},Index:{2}",node.name,level,treeIndex);
            style.normal.background = null;     
            if(currentNode == node) {       
                //style.normal.textColor = new Color32(1,115,158,255);
                style.normal.textColor = new Color32(35,35,35,255);
            } else {
                style.normal.textColor = new Color32(140,140,140,255);
                
            }
            style.fontSize = 12;
            var worldRect = GetWorldRect();
            float offsetX = worldRect.x;
            float offsetY = worldRect.y;
            var rect = new Rect(offsetX + 20 * level,offsetY + 20 * treeIndex,node.title.Length * 9,20);
            if((rect.x-offsetX+rect.width) > maxSize.x) {
                maxSize.x = (rect.x - offsetX + rect.width);
            }
            if((rect.y-offsetY+rect.height) > maxSize.y) {
                maxSize.y = (rect.y-offsetY+rect.height);
            }

            if(maxSize.x < minSize.x) maxSize.x = minSize.x;
            if(maxSize.y < minSize.y) maxSize.y = minSize.y;

            treeIndex += 1;
            if(node.type == UITreeNode.NodeType.Swith) {
                style.normal.textColor = new Color32(62,122,246,255);
                string title = node.isOpen ? "- "+node.title : "+ "+node.title;
                node.isOpen = EditorGUI.Foldout(rect,node.isOpen,new GUIContent(title),true,style);
            } else {
                //GUI.Label(rect,node.name);
                if(GUI.Button(rect,node.title,style)) {
                    // to do...
                    currentNode = node;
                    if(SelectedEvent != null) {
                        SelectedEvent(this,currentNode);
                    }
                    //Debug.Log(node.title);
                }
            }
            if(!node.isOpen || node.children == null) return;
            for(int i = 0;i < node.children.Count;i++) {
                var child = node.children[i];
                DrawFileTree(child,level + 1);
            }
        }
    }
}

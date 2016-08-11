using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace EditorUIKit {

    public class SampleView:UIView {
        public SampleView(Rect frame) : base(frame) { 

        }

        protected override void OnInit() {
            base.OnInit();
            //UIView toolBarView = new UIView(new Rect(100,100,500,500));
            //UILabel label = new UILabel(new Rect(100,100,179,40),"hello world");
            //rootView.backgroundColor = new Color32(33,33,33,255);
            //label.backgroundColor = new Color32(33,33,33,255);
            //label.borderColor = new Color32(180,180,180,255);
            //label.color = new Color32(180,180,180,255);
            //label.radius = 15;

            //toolBarView.AddSubview(label);
            //rootView.AddSubview(toolBarView);
            UIButton button = new UIButton(new Rect(10,10,170,30));
            button.name = "b1";
            button.ClickEvent = (sender) => {
                Debug.Log("click1");
            };
            AddSubview(button);

            UIButton button2 = new UIButton(new Rect(10,40,170,30));
            button2.name = "b2";
            button2.ClickEvent = (sender) => {
                Debug.Log("click2");
            };
            //button2.interceptEvent = false;
            AddSubview(button2);

            //UIInput input = new UIInput(new Rect(10,50,130,30));
            //AddSubview(input);

            UIComboBox combox = new UIComboBox(new Rect(10,100,130,30));
            combox.AddItem("item1");
            combox.AddItem("item2");
            combox.AddItem("item3");
            combox.AddItem("item4");
            combox.AddItem("item5");
            AddSubview(combox);

            //UITabView tabView = new UITabView(new Rect(170,30,400,350));
            //tabView.AddTab("资源依赖项");
            //tabView.AddTab("反向引用");
            //tabView.AddTab("其他");
            //AddSubview(tabView);


            //var rootNode = new UITreeNode();
            //rootNode.title = "Assets";
            //rootNode.type = UITreeNode.NodeType.Swith;
            //var child1 = new UITreeNode();
            //child1.title = "xxx.png";
            //child1.type = UITreeNode.NodeType.Item;
            //rootNode.InsertNode(child1);

            //var child2 = new UITreeNode();
            //child2.title = "Prefabs";
            //child2.type = UITreeNode.NodeType.Swith;
            //rootNode.InsertNode(child2);

            //var child3 = new UITreeNode();
            //child3.title = "test1.prefab";
            //child3.type = UITreeNode.NodeType.Item;
            //child2.InsertNode(child3);




            //UITreeView treeView = new UITreeView(new Rect(10,200,400,350),rootNode);
            //AddSubview(treeView);


            //UIScrollBar hsb = new UIScrollBar(new Rect(10,400,500,16),0.2f,UIScrollBar.ScrollBarType.Horizontal);
            //AddSubview(hsb);
            //UIScrollBar vsb = new UIScrollBar(new Rect(10,420,16,200),0.4f,UIScrollBar.ScrollBarType.Vertical);
            //AddSubview(vsb);
            //UIView contentView = new UIView(new Rect(0,0,400,400));
            //contentView.backgroundColor = Color.blue;
            //contentView.borderColor = Color.black;
            //UIView buttom = new UIView(new Rect(5,5,400 - 10,5));
            //buttom.backgroundColor = Color.red;
            //contentView.AddSubview(buttom);
            //UIScrollView scrollView = new UIScrollView(new Rect(180,100,200,200),contentView);
            //AddSubview(scrollView);

            //UISplitView splitView = new UISplitView(new Rect(250,100,400,400),2,2);
            //AddSubview(splitView);
        }
    }
}
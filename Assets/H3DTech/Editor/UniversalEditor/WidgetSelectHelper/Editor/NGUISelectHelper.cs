using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using EditorUIKit;
public class NGUISelectHelperApp:EditorUIKit.UISceneViewEditorApp {

    #region MenuItem
    static bool IsEnable = false;
    static NGUISelectHelperApp app = null;
    [MenuItem("H3D/UI/辅助选择工具 %#u",false,6)]
    public static void menu() {
        IsEnable = !IsEnable;
        Menu.SetChecked("H3D/UI/辅助选择工具",IsEnable);
        if(IsEnable) {
            if(app == null) {
                app = new NGUISelectHelperApp();
                app.Launch();
            }
            app.Show();
            UnityEditor.Tools.current = Tool.Rect;
        } else {
            app.Hide();
            app.contextMenu.visible = false;
            //app.contextMenu.ClearAllMenuItem();
            //UnityEditor.Tools.current = Tool.Rect;
        }    
    }
    #endregion

    EditorUIKit.UIButton exitBtn;
    public EditorUIKit.UIContextMenu contextMenu;
    GameObject lastActiveObject = null;
    public override void OnDidLaunch() {
        base.OnDidLaunch();
        exitBtn = new EditorUIKit.UIButton(new Rect(Screen.width,30,200,30));
        exitBtn.pressColor = exitBtn.hoverColor;
        exitBtn.textLabel.text = "点我退出辅助选择模式";
        exitBtn.ClickEvent = (sender) => {
            menu();
        };
        root.AddSubview(exitBtn);
        contextMenu = new UIContextMenu();
        //鼠标悬吊在Item上
        contextMenu.HangingEvent = (sender,title,go) => {
            if(go != null) {
                var widgetObject = go as GameObject;
                if(widgetObject != null) {
                    Selection.activeObject = widgetObject;
                }
            }       
        };
        //选择某项Item
        contextMenu.SelectedEvent = (sender,title,go) => {
            if(go != null) {
                var widgetObject = go as GameObject;
                if(widgetObject != null) {
                    UnityEditor.Tools.current = Tool.Rect;
                    Selection.activeObject = widgetObject;
                }
            }
        };
        //取消选择
        contextMenu.CancelEvent = (sender) => {
           Selection.activeGameObject = lastActiveObject;
        };
        root.AddSubview(contextMenu);
        root.BringSubviewToFront(exitBtn);

    }
    protected override bool OnEvent(Event e) {
        base.OnEvent(e);
        if(e.type == EventType.MouseDown) {
            if(e.button == 1) {              
                //var oldCam = Camera.current;
                Camera.SetupCurrent(SceneView.lastActiveSceneView.camera);
                lastActiveObject = Selection.activeGameObject;
                List<UIWidget> widgets = NGUIEditorTools.SceneViewRaycast(e.mousePosition);
                if(widgets.Count > 0) {
                    UnityEditor.Tools.current = Tool.Rect;
                    ShowSpriteSelectionMenu(widgets,e.mousePosition);
                    return true;
                }
               // Camera.SetupCurrent(oldCam);
            }
        }

        return false;
    }

    public override void Update(Event e) {
        base.Update(e);
        exitBtn.frame.Set(Screen.width - 250,30,200,30);
    }
    protected override void OnRepaint() {
        base.OnRepaint();

    }

    class MenuEntry {
        public string name;
        public GameObject go;
        public MenuEntry(string name,GameObject go) { this.name = name; this.go = go; }
    }
    public void ShowSpriteSelectionMenu(List<UIWidget> widgets,Vector2 showPos) {
        contextMenu.ClearAllMenuItem();
        List<UIWidgetContainer> containers = new List<UIWidgetContainer>();
        List<MenuEntry> entries = new List<MenuEntry>();

        bool divider = false;
        UIWidget topWidget = null;
        // Process widgets and their containers in the raycast order
        for(int i = 0;i < widgets.Count;++i) {
            UIWidget w = widgets[i];
            if(topWidget == null) topWidget = w;

            UIWidgetContainer wc = NGUITools.FindInParents<UIWidgetContainer>(w.cachedGameObject);

            // If we get a new container, we should add it to the list
            if(wc != null && !containers.Contains(wc)) {
                containers.Add(wc);

                // Only proceed if there is no widget on the container
                if(wc.gameObject != w.cachedGameObject) {
                    if(!divider) {
                        entries.Add(null);
                        divider = true;
                    }
                    entries.Add(new MenuEntry(wc.name + " (container)",wc.gameObject));
                }
            }

            string name = (i + 1 == widgets.Count) ? (w.name) : w.name;
            entries.Add(new MenuEntry(name,w.gameObject));
            divider = false;
        }
        // Add widgets to the menu in the reverse order so that they are shown with the top-most widget first (on top)
        for(int i = entries.Count;i > 0;) {
            MenuEntry ent = entries[--i];
            if(ent != null) {
                contextMenu.AddMenuItem(ent.name,ent.go);
            }
        }
        if(entries.Count <= 0) return;
        contextMenu.Show(showPos);
    }

}




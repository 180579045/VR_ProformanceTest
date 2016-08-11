using UnityEngine;
using System.Collections;
using UnityEditor;
namespace EditorUIKit {



    public class UIApplication{
        private UIView rootView;
        protected UIView root {
            get {
                if(rootView == null) {
                    rootView = new UIView(new Rect(0,0,Screen.width,Screen.height));
                }
                return rootView;
            }
        }
    }


    #region 基于编辑器窗口的APP

    public class H3DEditorWindow:EditorWindow {
        public UIWindowEditorApplication app;
        void OnGUI() {
            if(app != null) {
                app.Update(Event.current);
                this.Repaint();
            }
        }

        void OnDestroy() {
            if(app != null) {
                app.OnDestroy();
                app = null;
            }
            Resources.UnloadUnusedAssets();
        }
    }
    /// <summary>
    /// 基于编辑器窗口的APP
    /// </summary>
    public class UIWindowEditorApplication : UIApplication {
        protected H3DEditorWindow win;
        private Rect mRect;
        private string mTitle;
        public UIWindowEditorApplication(Rect rect,string title) {
            mRect = rect;
            mTitle = title;
        }
        public virtual void Launch() {
            win = EditorWindow.GetWindowWithRect<H3DEditorWindow>(mRect,false,mTitle);
            win.wantsMouseMove = true;
            win.app = this;
            root.frame = new Rect(0,0,mRect.width,mRect.height);
            root.backgroundColor = new Color32(241,241,241,255);
            OnDidLaunch();
            if(win != null) {
                win.Show();
            }
        }

        public virtual void OnDestroy() {
            H3DGraph.ClearCache();
            root.OnDestroy();
            win = null;
        }
        protected virtual void OnDidLaunch() {

        }
        public virtual void Update(Event e) {
            if(e != null) {
                if(e.type != EventType.Layout && e.type != EventType.Repaint) {
                    root.OnEvent(e);
                }
                root.OnDraw();
            }
        }
    }

    #endregion

    #region 基于SceneView的App
    public class UISceneViewEditorApp:UIApplication {
        public virtual void Launch() {
            root.frame = new Rect(0,0,Screen.width,Screen.height);
            root.backgroundColor = Color.clear;
            OnDidLaunch();
        }
        SceneView sv = null;
        public void Show() {
            SceneView.onSceneGUIDelegate += _sceneViewGUIFunc;
            
            if(SceneView.currentDrawingSceneView != null) {
                sv = SceneView.currentDrawingSceneView;
            } else if(SceneView.sceneViews.Count>0) {
                sv = SceneView.sceneViews[0] as SceneView;
            }
            if(sv != null) {
                sv.wantsMouseMove = true;
            }
           
            SceneView.RepaintAll();
            root.frame = new Rect(0,0,Screen.width,Screen.height);
        }
        public void Hide() {
            SceneView.onSceneGUIDelegate -= _sceneViewGUIFunc;
            if(sv != null) {
                sv.wantsMouseMove = false;
            }         
            SceneView.RepaintAll();
        }

        public virtual void Exit() {
            Hide();
            OnDestroy();
        }

        public virtual void OnDestroy() {
            root.OnDestroy();
        }

        public virtual void OnDidLaunch() {
            
        }

        public virtual void Update(Event e) {
            if(e != null) {
                if(e.type != EventType.Repaint && e.type != EventType.layout) {
                    if(OnEvent(e)) {
                        e.Use();
                    } else {
                        if(root.OnEvent(e)) {
                            e.Use();
                        }
                    }
                }
                Handles.BeginGUI();
                GUI.BeginClip(new Rect(0,0,Screen.width,Screen.height));
                root.frame.Set(0,0,Screen.width,Screen.height);
                root.OnDraw();
                GUI.EndClip();
                Handles.EndGUI();
            } else {
                OnRepaint();
            }
        }

        protected virtual bool OnEvent(Event e) {
            return false;
        }
        protected virtual void OnRepaint() {

        }

        private void _windowFunc(int windowID) {
            Update(Event.current);
        }
        private void _sceneViewGUIFunc(SceneView sc) {
            
            GUI.Window(1,new Rect(0,0,Screen.width,Screen.height),_windowFunc,"",GUIStyle.none);
            //Update(Event.current);
            sc.Repaint();
        }

    }

    #endregion

}

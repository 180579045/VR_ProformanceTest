using UnityEngine;
using System.Collections;

public class UIControl : MonoBehaviour {

}
namespace EditorUIKit {
    public class UIControlView:UIView {
        public UIControlView(Rect frame) : base(frame) {

        }
        public override void OnDraw() {
            base.OnDraw();
        }
        Vector2 lastMousePosition;
        bool isPress = false;
        public override bool OnEvent(Event e) {
            if(base.OnEvent(e)) return true;
            var worldRect = GetWorldRect();
            if(!worldRect.Contains(lastMousePosition) && worldRect.Contains(e.mousePosition)) {
                OnMouseEnter(e.mousePosition);
                lastMousePosition = e.mousePosition;
                return true;
            } else if(!worldRect.Contains(e.mousePosition) && worldRect.Contains(lastMousePosition)) {
                OnMouseLeave(e.mousePosition);
                lastMousePosition = e.mousePosition;
                return false;
            }
            lastMousePosition = e.mousePosition;
            if(e.type == EventType.mouseUp) {
                isPress = false;
            }
            if(worldRect.Contains(e.mousePosition)) {
                //Debug.Log(e.type);
                switch(e.type) {
                    case EventType.mouseDown:
                        isPress = true;
                        OnMouseDown(e.button);
                        break;
                    case EventType.mouseUp:
                        OnMouseUp(e.button);
                        break;
                    case EventType.mouseMove:
                        OnMouseMove(e.mousePosition);                       
                        break;
                    case EventType.scrollWheel:
                        OnScrollWheel(e.delta.y);
                        break;
                    case EventType.keyDown:
                        OnKeyDown(e.keyCode);
                        break;
                    case EventType.keyUp:
                        OnKeyUp(e.keyCode);
                        break;
                    case EventType.mouseDrag:
                        //Debug.Log("内部拖动" + this.GetType().Name);
                        OnMouseDrag(e.button,e.mousePosition,e.delta);
                        break;
                }
                return true;
            } else if(e.type == EventType.mouseDrag && isPress) {
                //Debug.Log("外部拖动" + this.GetType().Name);
                OnMouseDrag(e.button,e.mousePosition,e.delta);            
                return true;
            }
            return false;
        }

        /// <summary>
        /// 鼠标按钮按下时被调用
        /// </summary>
        /// <param name="button">0 左键 1 右键 2 中键</param>
        protected virtual void OnMouseDown(int button) { 
            
        }
        /// <summary>
        /// 鼠标按钮松开时被调用
        /// </summary>
        /// <param name="button">0 左键 1 右键 2 中键</param>
        protected virtual void OnMouseUp(int button) {

        }

        /// <summary>
        /// 鼠标按钮按下并移动时被调用
        /// </summary>
        /// <param name="button">0 左键 1 右键 2 中键</param>
        /// <param name="delta">相对上一帧位置偏移</param>
        protected virtual void OnMouseDrag(int button,Vector2 mousePosition,Vector2 delta) { 
        
        }
        /// <summary>
        /// 滑动鼠标滚轮时被调用
        /// </summary>
        /// <param name="delta">相对上一帧位置偏移</param>
        protected virtual void OnScrollWheel(float delta) { 
        
        }
        /// <summary>
        /// 鼠标移动时被调用
        /// </summary>
        /// <param name="postion"></param>
        protected virtual void OnMouseMove(Vector2 position) { 
            
        }

        protected virtual void OnMouseEnter(Vector2 position) { 
        
        }
        protected virtual void OnMouseLeave(Vector2 position) { 
        
        }

        /// <summary>
        /// 键盘按键被按下时调用
        /// </summary>
        /// <param name="key">键值码</param>
        protected virtual void OnKeyDown(KeyCode key) {

        }
        /// <summary>
        /// 键盘按键被松开时调用
        /// </summary>
        /// <param name="key">键值码</param>
        protected virtual void OnKeyUp(KeyCode key) {

        }

    }
}
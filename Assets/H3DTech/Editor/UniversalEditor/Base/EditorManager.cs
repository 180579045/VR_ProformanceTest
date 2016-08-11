using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

public class EditorManager 
{ 
    private EditorManager()
    {
#if  (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
        Application.RegisterLogCallback(_EditorLogDispatcher);
#else
        Application.logMessageReceived += _EditorLogDispatcher;
#endif
    }

    public delegate void VoidDelegate(EditorRoot root);


    public EditorRoot CreateEditor( string name , bool utility , VoidDelegate initCallback, object userData = null )
    {
        return CreateEditor<EditorRoot>(  name ,  utility , initCallback,  userData );
    }

    public EditorRoot CreateEditor<T>( string name , bool utility , VoidDelegate initCallback, object userData = null ) 
        where T : EditorRoot , new()
    {
        if( roots.ContainsKey(name) )
        {//有重名编辑器
            Debug.LogError("出现重名编辑器"+"\"name\"!");
            return null;
        }

        //EditorRoot newEditor =  EditorWindow.GetWindow<EditorRoot>(name, true);
        //EditorRoot newEditor = EditorWindow.GetWindow<EditorRoot>(false,name,true);
        //EditorRoot newEditor = EditorWindow.GetWindowWithRect<EditorRoot>(new Rect(0, 0, 512, 512), false, name, true);

        EditorRoot newEditor = EditorWindow.CreateInstance<T>();
        

        //初始化编辑器最新的组件
        newEditor.Init();

        //记录初始化回调，用于反射重生
        newEditor.initCallbackRefType = initCallback.Method.ReflectedType.FullName;
        newEditor.initCallback = initCallback.Method.Name;
        newEditor.isUtility = utility;

        newEditor.editorName = name;
#if UNITY_5_1
        newEditor.titleContent = new GUIContent(name);
#else
        newEditor.title = name;
#endif

        newEditor.UserData = userData;

        if (utility)
        {
            newEditor.ShowUtility();
        }
        else
        {
            newEditor.Show();
        }
        newEditor.Focus();

        //初始化控件
        initCallback(newEditor);

        AssignCtrlID(newEditor, newEditor.RootCtrl);

        UniversalEditorUtility.LoadEditorLayout(newEditor);

        if (newEditor.onAwake != null)
        {
            newEditor.onAwake(newEditor);
        }

        if( newEditor.onEnable != null )
        {
            newEditor.onEnable(newEditor);
        }

        roots.Add(name, newEditor);


        return newEditor;
    }

    public EditorRoot FindEditor( string name )
    {
        EditorRoot editor = null;
        roots.TryGetValue(name,out editor);
        return editor;
    }

    public bool RemoveEditor( string name )
    { 
       return roots.Remove(name); 
    }

    //重新创建编辑器
    public void RespawnEditor( EditorRoot e )
    {
        string editorName = e.editorName;
        string initCallbackRefTypeName = e.initCallbackRefType;
        string initCallbackName = e.initCallback;
        bool isUtility = e.isUtility;
        object userData = e.UserData;

        //Debug.Log("Editor Respawn " + e.initCallbackRefType + "." + e.initCallback + "  utility=" + isUtility);
        e.Close(); 

        EditorRoot findEditor = FindEditor(editorName);
        if( findEditor != null )
        {
            RemoveEditor(editorName);
        }

        Type refType = Assembly.GetExecutingAssembly().GetType(initCallbackRefTypeName); 
        MethodInfo initCallbackInfo = refType.GetMethod(initCallbackName,BindingFlags.Public|BindingFlags.Static); 
        if (refType == null || initCallbackInfo == null)
        {
            Debug.Log("编辑器\""+editorName+"\"恢复失败！");
            return;
        }
         
        VoidDelegate initDelegate = Delegate.CreateDelegate(typeof(VoidDelegate), null, initCallbackInfo, false) as VoidDelegate;
        CreateEditor(editorName, isUtility, initDelegate, userData);
    }

    public void InitEidtorFromLayout(EditorRoot e)
    {
        if(null == e)
        {
            return;
        }

        string editorName = e.editorName;
        string initCallbackRefTypeName = e.initCallbackRefType;
        string initCallbackName = e.initCallback;
//        object userData = e.UserData;

        e.Init();
        
        Type refType = Assembly.GetExecutingAssembly().GetType(initCallbackRefTypeName);
        MethodInfo initCallbackInfo = refType.GetMethod(initCallbackName, BindingFlags.Public | BindingFlags.Static);
        if (refType == null || initCallbackInfo == null)
        {
            Debug.Log("编辑器\"" + editorName + "\"恢复失败！");
            return;
        }

        VoidDelegate initDelegate = Delegate.CreateDelegate(typeof(VoidDelegate), null, initCallbackInfo, false) as VoidDelegate;
        initDelegate(e);

        EditorManager.GetInstance().AssignCtrlID(e, e.RootCtrl);

        UniversalEditorUtility.LoadEditorLayout(e);

        if (e.onAwake != null)
        {
            e.onAwake(e);
        }

        if (e.onEnable != null)
        {
            e.onEnable(e);
        }

        if (!roots.ContainsKey(editorName))
        {
            roots.Add(editorName, e);
        }
    }
    public int GetCount()
    {
        return roots.Count;
    }

    public void Clear()
    {
        roots.Clear();
    }

    //用来分发Unity的Log
    private void _EditorLogDispatcher(string condition, string stackTrace, LogType type)
    {
        if( logCallback != null )
        {
            logCallback(condition, stackTrace, type);
        }
    }

    public void AssignCtrlID(EditorRoot root, EditorControl rootCtrl)
    {
        if(
               (null == root)
            || (null == rootCtrl)
            )
        {
            return;
        }

        do
        {
            root.CtrlCounter++;

            rootCtrl.CtrlID = root.editorName + "_" + rootCtrl.GetType() + "_" + (root.CtrlCounter).ToString();

            EditorCtrlComposite rootCtrlComp = rootCtrl as EditorCtrlComposite;
            if (null == rootCtrlComp)
            {
                break;
            }

            foreach(var item in rootCtrlComp.children)
            {
                AssignCtrlID(root, item);
            }

        } while (false);
    }

    public Application.LogCallback logCallback;

    private Dictionary<string, EditorRoot> roots = 
        new Dictionary<string,EditorRoot>();

    private static EditorManager s_instance = null;
    public static EditorManager GetInstance()
    {
        if( s_instance == null )
        {
            s_instance = new EditorManager();
        }
        return s_instance;
    }
}

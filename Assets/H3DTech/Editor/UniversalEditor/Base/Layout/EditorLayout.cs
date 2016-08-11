using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;

public class SplitInfo
{
    public string SpliterID
    {
        set
        {
            m_spliterID = value;
        }
        get 
        {
            return m_spliterID;
        }
    }

    //public Type SpliterType
    //{
    //    set
    //    {
    //        m_spliterType = value;
    //    }

    //    get
    //    {
    //        return m_spliterType;
    //    }
    //}

    public string SpliterPath
    {
        set
        {
            m_spliterPath = value;
        }

        get
        {
            return m_spliterPath;
        }
    }

    public float SplitOffset
    {
        set
        {
            m_splitValue.spliterOffset = value;
        }

        get
        {
            return m_splitValue.spliterOffset;
        }
    }

    public bool SpliterOffsetInv
    {
        set
        {
            m_splitValue.spliterOffsetInv = value;
        }

        get
        {
            return m_splitValue.spliterOffsetInv;
        }
    }
    private string m_spliterID = string.Empty;
    //private Type m_spliterType = typeof(SpliterCtrl);
    private string m_spliterPath = string.Empty;
    private LayoutConstraint m_splitValue = new LayoutConstraint();
}

public class EditorLayoutInfo
{
    public float XPox
    {
        set
        {
            m_position.x = value;
        }

        get
        {
            return m_position.x;
        }
    }
    public float YPox
    {
        set
        {
            m_position.y = value;
        }

        get
        {
            return m_position.y;
        }
    }

    public float Width
    {
        set
        {
            m_position.width = value;
        }

        get
        {
            return m_position.width;
        }
    }

    public float Height
    {
        set
        {
            m_position.height = value;
        }

        get
        {
            return m_position.height;
        }
    }

    //public Rect Position
    //{
    //    set
    //    {
    //        m_position = value;
    //    }

    //    get 
    //    {
    //        return m_position;
    //    }
    //}

    public List<SplitInfo> DivisionInfo
    {
        set
        {
            m_divisionInfo = value;
        }
        get
        {
            return m_divisionInfo;
        }
    }

    private Rect m_position = new Rect();
    private List<SplitInfo> m_divisionInfo = new List<SplitInfo>();

    public void WirteEditorLayoutInfo(string baseDir, EditorRoot root)
    {
        if(
               (null == root)
            || string.IsNullOrEmpty(baseDir)
            )
        {
            return;
        }

        object obj = GetSerializeObject(root);
        if(null == obj)
        {
            return;
        }

        string eidtorLayoutInfoPath = baseDir + root.editorName + "/" + "EditorLayoutInfo.layout";

        if (!Directory.Exists(baseDir + root.editorName))
        {
            Directory.CreateDirectory(baseDir + root.editorName);
        }

        UniversalEditorUtility.MakeFileWriteable(eidtorLayoutInfoPath);

        StreamWriter yamlWriter = File.CreateText(eidtorLayoutInfoPath);
        Serializer yamlSerializer = new Serializer();

        yamlSerializer.Serialize(yamlWriter, obj);

        yamlWriter.Close();
    }

    public void ReadEditorLayoutInfo(string baseDir, EditorRoot root, ref EditorLayoutInfo info)
    {
        if(
               (null == root)
            || string.IsNullOrEmpty(baseDir)
            )
        {
            return;
        }

        string eidtorLayoutInfoPath = baseDir + root.editorName + "/" + "EditorLayoutInfo.layout";

        if (!File.Exists(eidtorLayoutInfoPath))
        {
            return;
        }
        StreamReader yamlReader = File.OpenText(eidtorLayoutInfoPath);
        Deserializer yamlDeserializer = new Deserializer();

        //读取持久化对象
        info = yamlDeserializer.Deserialize<EditorLayoutInfo>(yamlReader);

        yamlReader.Close();
    }

    public object GetSerializeObject(EditorRoot root)
    {
        if(null == root)
        {
            return null;
        }

        EditorLayoutInfo newInfo = new EditorLayoutInfo();
        
        newInfo.XPox = root.position.x;
        newInfo.YPox = root.position.y;
        newInfo.Width = root.position.width;
        newInfo.Height = root.position.height;

        newInfo.DivisionInfo = GetDivisionInfo(root.RootCtrl);

        return newInfo;
    }

    private List<SplitInfo> GetDivisionInfo(EditorControl rootCtrl)
    {
        List<SplitInfo> splitInfoTbl = new List<SplitInfo>();

        if (null == rootCtrl)
        {
            return splitInfoTbl;
        }

        EditorCtrlComposite rootComp = rootCtrl as EditorCtrlComposite;
        if (null == rootComp)
        {
            return splitInfoTbl;
        }

        if (rootCtrl is SpliterCtrl)
        {
            SplitInfo newInfo = new SplitInfo();
            newInfo.SpliterID = rootCtrl.CtrlID;
            newInfo.SpliterPath = rootCtrl.GetCtrlIDPath();
            newInfo.SplitOffset = rootCtrl.layoutConstraint.spliterOffset;
            newInfo.SpliterOffsetInv = rootCtrl.layoutConstraint.spliterOffsetInv;

            splitInfoTbl.Add(newInfo);
        }

        foreach (var item in rootComp.children)
        {
            splitInfoTbl.AddRange(GetDivisionInfo(item));
        }

        return splitInfoTbl;
    }
}
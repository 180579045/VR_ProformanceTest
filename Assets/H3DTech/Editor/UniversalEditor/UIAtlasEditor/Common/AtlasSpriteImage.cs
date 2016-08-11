using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AtlasSpriteImage 
{//小图信息

    private string m_Name = string.Empty;                                 //小图绝对路径
    private string m_Path = string.Empty;                                 //小图相对于配置目录路径
    private float m_ZoomScale = 0.0f;                             //小图缩放比例
    private List<string> m_ReferenceTable = new List<string>();     //引用关系列表

    public string Name { get { return m_Name; } set { m_Name = value; } }
    public string Path { get { return m_Path; } set { m_Path = value; } }
    public float ZoomScale { get { return m_ZoomScale; } set { m_ZoomScale = value; } }
    public List<string> ReferenceTable { get { return m_ReferenceTable; } set { m_ReferenceTable = value; } }
}

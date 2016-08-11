
using YamlDotNet.Serialization;
using System.IO;
using System.Collections.Generic;

public class AtlasSerializeObject
{//Atals序列化对象（持久化使用）

    private string m_atlasOutputPath = null;                                //Atlas输入路径（相对于Unity路径）

    private List<KeyValuePair<string, SpriteImageInfo>> m_spriteInfoTable;  //小图信息

    public string AtlasOutputPath { get { return m_atlasOutputPath; } set { m_atlasOutputPath = value; } }

    public List<KeyValuePair<string, SpriteImageInfo>> SpriteInfoTable
    {
        get { return m_spriteInfoTable; }
        set { m_spriteInfoTable = value; }
    }

    public void SaveAtlasSerializeObject(string filePath, object obj)
    {
        if (
               string.IsNullOrEmpty(filePath)
            || (null == obj)
            )
        {
            return;
        }

        UniversalEditorUtility.MakeFileWriteable(filePath);
        StreamWriter yamlWriter = File.CreateText(filePath);
        Serializer yamlSerializer = new Serializer();

        //将持久化对象写入工程文件
        yamlSerializer.Serialize(yamlWriter, obj);
        yamlWriter.Close();
    }

    static public void LoadAtlasSerializeObject(string filePath, out AtlasSerializeObject obj)
    {
        obj = null;

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        //打开工程文件
        StreamReader yamlReader = File.OpenText(filePath);
        Deserializer yamlDeserializer = new Deserializer();

        //读取持久化对象
        obj = yamlDeserializer.Deserialize<AtlasSerializeObject>(yamlReader);

        yamlReader.Close();
    }

}

using UnityEngine;

public class TextObject : GeometryObject
{
    public TextObject(string objectID)
        : base(objectID)
    {

    }

    public TextObject(string objectID, Quaternion roration, Vector3 pos, Vector3 size, Material mat, int layer)
        : base(objectID, roration, pos, size, mat, layer)
    {

    }

    public override string Text
    {
        get
        {
            return base.Text;
        }
        set
        {
            base.Text = value;

            TextMesh textMesh = m_GameObject.GetComponent<TextMesh>();
            if(textMesh != null)
            {
                textMesh.text = value;
            }
        }
    }

    public override Material DefaultMat
    {
        get
        {
            return base.DefaultMat;
        }
        set
        {
            //TextMesh textMesh = m_GameObject.GetComponent<TextMesh>();
            //if (textMesh != null)
            //{
            //    base.DefaultMat = textMesh.font.material;
            //}
            return;
        }
    }

    public override void SetDispMaterial(Material mat)
    {
        //TextMesh textMesh = m_GameObject.GetComponent<TextMesh>();
        //if (textMesh != null)
        //{
        //    mat = textMesh.font.material;

        //    base.SetDispMaterial(mat);
        //}
    }

    protected override void FixComponent(Material mat)
    {
        if (null == m_GameObject)
        {
            return;
        }

        TextMesh textMesh = m_GameObject.AddComponent<TextMesh>();

        if(
               (null == textMesh)
            )
        {
            return;
        }

        textMesh.text = Text;
        textMesh.fontSize = 25;
        textMesh.characterSize = 0.5f;
        textMesh.fontStyle = FontStyle.Bold;
        MeshRenderer meshRender = m_GameObject.GetComponent<MeshRenderer>();

        Material labelMat = new Material(Shader.Find("Custom/3DTextShader"));
        labelMat.mainTexture = textMesh.font.material.mainTexture;
        meshRender.material = labelMat;
    }
}
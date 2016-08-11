using UnityEngine;
using System.Collections;

public class SpliterCtrl : EditorCtrlComposite
{
    public float MaxOffset
    {
        get { return maxOffset; }
        set { maxOffset = value; }
    }

    public float MinOffset
    {
        get { return minOffset; }
        set { minOffset = value; }
    }

    public bool Dragable
    {
        get { return dragable; }
        set { dragable = value; }
    }

    public bool IsDragging
    {
        get { return isDragging; }
        set { isDragging = value; }
    }

    public override void Traverse(EditorCtrlVisitor v)
    {
        v.PreVisit(this);

        if (children.Count >= 1)
        {
            v.PreVisitChild(this, 0);
            children[0].Traverse(v);
            v.AfterVisitChild(this, 0);
        }

        v.Visit(this);

        if (children.Count > 1)
        {
            v.PreVisitChild(this, 1);
            children[1].Traverse(v);
            v.AfterVisitChild(this, 1);
        }

        v.AfterVisit(this);
    }

    private bool dragable = false;
    private bool isDragging = false;

    protected float maxOffset = float.PositiveInfinity;
    protected float minOffset = 10f;
}

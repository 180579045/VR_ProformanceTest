using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutUpdateVisitor : EditorCtrlVisitor 
{ 
    //栈顶元素为当前控件可用区域
    public Stack<Rect> areaStack = new Stack<Rect>();
    public override void Visit(EditorControl c) 
    {
        Rect currArea = areaStack.Peek();

        if (!c.IsRoot && !(c.Parent is SpliterCtrl))
        {
            //c.Size = new Rect(0, 0, 0, 0);
            return;
        }
         
        c.Size = currArea; 
    }

    public override bool PreVisitChild(EditorControl c, int i) 
    {
        Rect currArea = areaStack.Peek();
        Rect newArea = new Rect();
        LayoutConstraint constraint = c.layoutConstraint;
        float part0Width, part0Height, part1Width, part1Height;

        //Modify by liteng for 代码改善 at 2015/2/26
        if (c is HSpliterCtrl)
        {
           if( !constraint.spliterOffsetInv )
           {
               part0Height = constraint.spliterOffset;
               part1Height = currArea.height - constraint.spliterOffset;
               if (part1Height < 0.0f)
                   part1Height = 1.0f;
           }
           else
           {
               part1Height = constraint.spliterOffset;
               part0Height = currArea.height - constraint.spliterOffset;
               if (part0Height < 0.0f)
                   part0Height = 1.0f;
           }
            
            if( i == 0 )
            {
                newArea.Set(0, 0, currArea.width, part0Height);
            }
            else if (i == 1)
            { 
                newArea.Set(0, 0, currArea.width, part1Height);
            }
            areaStack.Push(newArea);
        }
        //Modify by liteng for 代码改善 at 2015/2/26
        else if (c is VSpliterCtrl)
        {
            if (!constraint.spliterOffsetInv)
            {
                part0Width = constraint.spliterOffset;
                part1Width = currArea.width - constraint.spliterOffset;
                if (part1Width < 0.0f)
                    part1Width = 1.0f;
            }
            else
            {
                part1Width = constraint.spliterOffset;
                part0Width = currArea.width - constraint.spliterOffset;
                if (part0Width < 0.0f)
                    part0Width = 1.0f;

            }

            if (i == 0)
            {
                newArea.Set(0, 0, part0Width, currArea.height);
            }
            else if (i == 1)
            { 
                newArea.Set(0, 0, part1Width, currArea.height);
            }
            areaStack.Push(newArea);
        }
        //Modify by liteng for 代码改善 at 2015/2/26
        else if(c is HBoxCtrl)
        {
          
        }
        //Modify by liteng for 代码改善 at 2015/2/26
        else if (c is VBoxCtrl)
        {

        }

        return true;
    }
    public override void AfterVisitChild(EditorControl c, int i) 
    {
        //Rect currArea = areaStack.Peek();

        //Modify by liteng for 代码改善 at 2015/2/26 Start
        if (c is HSpliterCtrl)
        {
            areaStack.Pop();
        }
        else if (c is VSpliterCtrl)
        {
            areaStack.Pop();
        }
        else if (c is HBoxCtrl)
        { 
        }
        else if (c is VBoxCtrl)
        {  
        }
        //Modify by liteng for 代码改善 End
    }
}

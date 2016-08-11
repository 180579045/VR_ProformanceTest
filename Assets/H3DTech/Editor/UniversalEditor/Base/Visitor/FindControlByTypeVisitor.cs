using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindControlByTypeVisitor<T> : EditorCtrlVisitor 
{ 
   public List<EditorControl> results = new List<EditorControl>();
   public override void Visit(EditorControl c)
   {
       //Modify by liteng for 代码改善 at 2015/2/26 Start
       if (c is T)
       {
           results.Add(c);
       }
       //Modify by liteng for 代码改善 End
   }
	 
}

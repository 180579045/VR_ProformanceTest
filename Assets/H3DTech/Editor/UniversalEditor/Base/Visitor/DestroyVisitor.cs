using UnityEngine;
using System.Collections;

public class DestroyVisitor : EditorCtrlVisitor 
{
    EditorDestroyStrategy mainViewStrategy = new MainViewDestroyStrategy();

    private EditorDestroyStrategy _GetStrategy( EditorControl c )
    {
        //Modify by liteng for 代码改善 at 2015/2/26 Start
        if (c is MainViewCtrl)
        {
            return mainViewStrategy;
        }
        //Modify by liteng for 代码改善 End

        return null;
    }

    public override void Visit(EditorControl c)
    {
        EditorDestroyStrategy strategy = _GetStrategy(c);
        if( strategy != null )
        {
            strategy.Destroy(c);
        }
    }
}

using UnityEngine;
using System.Collections;

public class UpdateVisitor : EditorCtrlVisitor
{
    private EditorUpdateStrategy playCtrlUpdateStrategy = new PlayCtrlUpdateStrategy();


    private EditorUpdateStrategy _GetStrategy(EditorControl c)
    {
        //Modify by liteng for 代码改善 at 2015/2/26 Start
        if(c is PlayCtrl)
        {
            return playCtrlUpdateStrategy;
        }
        //Modify by liteng for 代码改善 End

        return null;
    }

    public override void Visit(EditorControl c)
    {
        EditorUpdateStrategy strategy = _GetStrategy(c);
        if( null != strategy )
        {
            strategy.Update(c,deltaTime);
        }
    }


    public void _InternalUpdate( float dt)
    {
        deltaTime = dt;
    }

    float deltaTime = 0.0f;
}

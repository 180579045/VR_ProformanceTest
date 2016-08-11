using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEffectAnimClipEditorCommandManager 
{

    public void Execute( IEditorCommand cmd )
    {
        if (cmd == null)
            return;

        cmd.Execute();

        if( currCmdIndx == -1 )
        {
            cmdList.Clear();
        }
        else
        {
            cmdList.RemoveRange( currCmdIndx + 1 , cmdList.Count - currCmdIndx - 1);
        }

        cmdList.Add(cmd);
        currCmdIndx++;
    }

    public void Redo()
    {
        if ( CanRedo() )
        {
            cmdList[++currCmdIndx].Execute();
        }
    }

    public void Undo()
    {
        if ( CanUndo() )
        {
            cmdList[currCmdIndx--].UnExecute();
        }
    }

    public bool CanUndo()
    {
        return currCmdIndx != -1;
    }

    public bool CanRedo()
    {
        return currCmdIndx + 1 < cmdList.Count;
    }

    public void Clear()
    {
        cmdList.Clear();
        currCmdIndx = -1;
    }
    
    //指示当前已执行的命令
    int currCmdIndx = -1;

    List<IEditorCommand> cmdList = new List<IEditorCommand>();
}

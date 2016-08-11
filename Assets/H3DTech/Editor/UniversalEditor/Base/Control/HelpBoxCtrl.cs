using UnityEngine;
using UnityEditor;

public class HelpBoxCtrl : EditorControl
{
    public MessageType MsgType
    {
        get { return msgType; }
        set { msgType = value; }
    }

    private MessageType msgType = MessageType.None;
}